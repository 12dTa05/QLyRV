import json
import io  
from enum import EnumMeta
from flask import Flask, redirect, url_for, render_template, request, session, Blueprint, jsonify
from datetime import timedelta
from flask.helpers import flash
from flask_sqlalchemy import SQLAlchemy
from os import path  
import requests
from flask import Flask
from flask import request, jsonify
from flask_cors import CORS, cross_origin
import numpy as np
import cv2
import base64 
from pyzbar.pyzbar import decode
#from deepface import DeepFace
url = 'https://api.fpt.ai/vision/idr/vnm'
 
headers = {
    'api-key': 'ofq2MZLOMqVOVek2G7KvcZc0fFHOVj13'
}


global model  
# Khởi tạo flask app
app = Flask(__name__) 
#app = Flask(__name__)
app.config["SECRET_KEY"] = "haidang"
app.config["SQLALCHEMY_DATABASE_URI"] = "sqlite:///user.db"
app.config["SQLALCHEMY_TRACK_MODIFICATIONS"] = False
app.permanent_session_lifetime = timedelta(minutes=1)
db = SQLAlchemy(app)


class User(db.Model):
    user_id = db.Column(db.Integer, primary_key=True)
    name = db.Column(db.String(100))
    email = db.Column(db.String(100))

    def __init__(self, name, email):
        self.name = name
        self.email = email

@app.route("/home")
@app.route("/")
def home():
    return render_template("home.html")

def read_qr_code(image, max_width=500):
    # Đọc hình ảnh
    image = cv2.imdecode(np.frombuffer(image, np.uint8), cv2.IMREAD_GREYSCALE)

    # Thay đổi kích thước hình ảnh nếu nó quá lớn
    if image.shape[1] > max_width:
        scale_percent = max_width / image.shape[1]
        width = int(image.shape[1] * scale_percent)
        height = int(image.shape[0] * scale_percent)
        image = cv2.resize(image, (width, height), interpolation=cv2.INTER_AREA)

    # Giải mã mã QR từ hình ảnh
    decoded_objects = decode(image)

    # Lặp qua từng đối tượng đã được giải mã và in ra nội dung của mã QR
    if decoded_objects:
        return decoded_objects[0].data.decode('utf-8')
    else:
        return "Không tìm thấy mã QR"
        

@app.route("/login", methods=["POST", "GET"])
def login():
    if request.method == "POST":
        user_name = request.form["name"]
        session.permanent = True
        if user_name:
            session["user"] = user_name
            found_user = User.query.filter_by(name=user_name).first()
            if found_user:
                session["email"] = found_user.email
            else:
                user = User(user_name, "temp@gmail.com")
                db.session.add(user)
                db.session.commit()
                flash("Created in DB!")
            return redirect(url_for("user", user=user_name))
    if "user" in session:
        name = session["user"]
        return redirect(url_for("user", user=user_name))
    return render_template("login.html")

@app.route("/user", methods=["POST", "GET"])
def user():
    email = None
    if "user" in session:
        name = session["user"]
        if request.method == "POST":
            if not request.form["email"] and request.form["name"]:
                User.query.filter_by(name=name).delete()
                db.session.commit()
                flash("Deleted user!")
                return redirect(url_for("log_out"))
            else:
                email = request.form["email"]
                session["email"] = email
                found_user = User.query.filter_by(name=name).first()
                found_user.email = email
                db.session.commit()
                flash("Email updated!")
        elif "email" in session:
            email = session["email"]
        return render_template("user.html", user=name, email=email)
    else:
        return redirect(url_for("login"))

@app.route("/logout")
def log_out():
    session.pop("user", None)
    return redirect(url_for("login"))

# @app.route("/translate", methods=["GET", "POST"])
# def translate():
#     if request.method == "POST":
#         if 'file' not in request.files:
#             return jsonify({'error': 'No file uploaded'}), 400  
#         uploaded_image = request.files['file']
#         if uploaded_image.filename == '':
#             return jsonify({'error': 'Empty file name'}), 400 
#         try:
#             img_data = uploaded_image.read()
#             print(type(img_data))
#             # Chuyển đổi dữ liệu ảnh thành đối tượng có thể truyền lên API
#             files = {'image': img_data} 
#             response = requests.post(url, files=files, headers=headers)
#             #print(response)
#             if response.status_code == 200:
#                 return jsonify({'result': response.json()}), 200
#             else:
#                 return jsonify({'error': 'Error from FPT.AI API'}), response.status_code

#         except Exception as e:
#             print("Error processing image:", e)
#             return jsonify({'error': str(e)}), 500
        
#     return render_template("translate.html", translate=None, uploaded_image=None)


def display_image(image):
    cv2.imshow('Image', image)
    cv2.waitKey(0)
    cv2.destroyAllWindows()
     
@app.route("/translate", methods=["GET", "POST"])
@cross_origin()
def translate():
    if request.method == "POST":
        image_b64 = request.form.get('image') 
        image = base64.b64decode(image_b64) 
        
        try: 
            response_text =  read_qr_code(image)
            data = response_text.split("|")

# In ra từng phần dữ liệu đã tách
            #print(response_text)
            return data

        except Exception as e:
            #print("Error processing image:", e)
            return jsonify({'error': str(e)}), 500
        
    return render_template("translate.html", translate=None, uploaded_image=None)

 


# @app.route("/check", methods=["GET", "POST"])
# @cross_origin()
# def check():
#     if request.method == "POST":
#         files = []
#         for i in range(1, 3):  # Lặp qua 2 trường nhập tệp
#             file_key = 'file{}'.format(i)
#             if file_key not in request.files:
#                 return jsonify({'error': f'No file uploaded for {file_key}'}), 400

#             uploaded_image = request.files[file_key]
#             if uploaded_image.filename == '':
#                 return jsonify({'error': f'Empty file name for {file_key}'}), 400
#             img_data = uploaded_image.read()
#             file = (f'file[]', (f'{file_key}.jpg', img_data))
#             files.append(file)  # Thêm dữ liệu ảnh vào danh sách files 
#         try: 
#             # Định nghĩa tiêu đề API-key 
#             headers = {
#                 'api-key': 'mAzUh2ha1cgLXcvs28KB5xRjEpYII6kl'
#             } 
#             # Gửi yêu cầu POST đến API của FPT.AI
#             response = requests.post('https://api.fpt.ai/dmp/checkface/v1', headers=headers, files=files)
#             # Kiểm tra phản hồi từ API
#             if response.status_code == 200:
#                 return jsonify({'result': response.json()}), 200
#             else:
#                 return jsonify({'error': 'Error from FPT.AI API'}), response.status_code

#         except Exception as e:
#             print("Error processing images:", e)
#             return jsonify({'error': str(e)}), 500
        
#     return render_template("check.html", result=None)
 


# @app.route("/check", methods=["GET", "POST"])
# @cross_origin()
# def check(): 
#     if request.method == "POST":  
#         image_b64 = request.form.get('image1')    
#         image1 = image_b64
#         image_b64 = request.form.get('image2')   
#         image2 = image_b64   
#         result = DeepFace.verify(img1_path = image1, img2_path = image2, enforce_detection=False) 
#         return str(result['verified'])
#         file = (f'file[]', (f'{file_key}.jpg', img_data))
#         files.append(file)  # Thêm dữ liệu ảnh vào danh sách files  
#         # Định nghĩa tiêu đề API-key 
#         headers = {
#             'api-key': 'ofq2MZLOMqVOVek2G7KvcZc0fFHOVj13'
#         }  
#         # Gửi yêu cầu POST đến API của FPT.AI
#         #response = requests.post('https://api.fpt.ai/dmp/checkface/v1', headers=headers, files=files) 
#         # merged_data = response1 + response
#         # Kiểm tra phản hồi từ API
#         return "Check" 
        
#     return render_template("check.html", result=None)
  
 
if __name__ == "__main__":
    if not path.exists("user.db"):
        with app.app_context():
            db.create_all()
        print("Created database!")
    app.run(debug=True)
