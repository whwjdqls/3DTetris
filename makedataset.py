import csv
import numpy as np
import torch
import torch.nn as nn
import pandas as pd


import cv2
from cvzone.HandTrackingModule import HandDetector
import time



####이것만 바꾸세요####
filming_time = 120
sleep_time = 0.2
##filming_time 초동안 찍고 sleep_time초마다 찍을 꺼에요##
##
#######################

width, height = 1028, 720
cap = cv2.VideoCapture(0)
cap.set(3, width) # 3: width
cap.set(4, height) # 4: height
detector = HandDetector(maxHands=1, detectionCon=0.8)


########################
#"what class are you collecting?" 라는 문구가 보이면 
#0-4 중 찍고 싶은  class를 입력하세요
#########오른손으로 찍을것(0-4)#########
# 0 = 주먹(가만히)
# 1 = 완쪽
# 2 = 앞
# 3 = 오른쪽
# 4 = 뒷쪽
#########################################
label = int(input("what class are you collecting?"))

final = []
start = time.time()

while time.time()-start <filming_time+3:
    success, img = cap.read()
    hands, img = detector.findHands(img)
    if time.time()-start < 3:
        cv2.putText(img, 
        "Start making dataset in {}".format(3-int(time.time()-start)), 
        (50, 50), 
        cv2.FONT_HERSHEY_SIMPLEX, 1, 
        (0, 255, 255), 
        2, 
        cv2.LINE_4)
        cv2.imshow("Image", img) 
        cv2.waitKey(1) 
        continue

    data = []
    message = str(int(time.time()-start-3))
    cv2.putText(img, 
    message+ " seconds has passed", 
    (50, 50), 
    cv2.FONT_HERSHEY_SIMPLEX, 1, 
    (0, 255, 255), 
    2, 
                cv2.LINE_4)
 
    if hands:  
        hand = hands[0]
        lmList = hand['lmList']
        
        outputs = []
        for keypoint in lmList:
            outputs.extend([keypoint[0]-lmList[0][0],keypoint[1]-lmList[0][1],keypoint[2]-lmList[0][2]])
        final.append(outputs)
        time.sleep(sleep_time)
            
    
    cv2.imshow("Image", img) 
    cv2.waitKey(1) 

final_np = np.array(final)

##normalize##
row_sum = final_np.sum(axis=1)
final_normalized_np = final_np / row_sum[:,np.newaxis]

##concat with label nparray
label_np = np.full((final_np.shape[0], 1), label)
final_normalized_np = np.concatenate((label_np,final_normalized_np),axis=1)


np.save("./class{}".format(label),final_normalized_np)
cap.release() 
