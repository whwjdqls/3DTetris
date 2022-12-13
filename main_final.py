import cv2
from cvzone.HandTrackingModule import HandDetector
import socket 
import time 
from customhandrec import model 
import torch
import numpy as np

####model#####
hand_ges_rec = model.model_2(num_class=5, dimension=3)
hand_ges_rec.load_state_dict(torch.load("./custom_model_5.pth"))
hand_ges_rec.eval()

def inference(limlist):
    keypoints = [list(a)for a in lmList[:21]]
    inputs = []
    for keypoint in keypoints:
        inputs.extend([keypoint[0]-keypoints[0][0],keypoint[1]-keypoints[0][1],keypoint[2]-keypoints[0][2]])

    input_np = np.array(inputs)
    sum = input_np.sum()
    final_input_np = input_np / sum
    input_tensor = torch.tensor(final_input_np, dtype = torch.float32)
    out = hand_ges_rec(input_tensor).detach().numpy().argmax()
    return out


####cam####
width, height = 1028, 720
cap = cv2.VideoCapture(0)
cap.set(3, width) # 3: width
cap.set(4, height) # 4: height
detector = HandDetector(maxHands=2, detectionCon=0.8)

####socket####
sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
serverAddressPort = ("127.0.0.1", 5052) 

timer = 0
start = time.time()
detecting = False


first_time = True
not_detecting_start = 0

while True:
    success, img = cap.read()
    hands, img = detector.findHands(img)
############################for game start###############################
    time_lapsed = time.time() - start
    if time_lapsed < 3:
        cv2.putText(img, 
        "Starting game in {}".format(3-int(time_lapsed)), 
        (50, 50), 
        cv2.FONT_HERSHEY_SIMPLEX, 1, 
        (0, 255, 255), 
        2, 
        cv2.LINE_4)
        cv2.imshow("Image", img) 
        cv2.waitKey(1)  
        continue
###########################################################################
    if not detecting:
        #if it is the first time!
        if hands and first_time :
            first_time = False
            not_detecting_start = time.time()
            continue

        if hands and not first_time: 
            data = []
            hand1 = hands[0]
            lmList = hand1['lmList']
            ################################
            out = inference(lmList)
            ################################
            if len(hands) == 2:
                hand2 = hands[1]
                for i in hand2['lmList']:
                    lmList.append(i)

            for lm in lmList:
                data.extend([lm[0], height - lm[1], lm[2]])
            
            data.append(-1)
            sock.sendto(str.encode(str(data)), serverAddressPort)
            ###########################

            message =""
            if out == 0 :
                message = "stay"
            if out== 1 :
                message = "left"
            if out == 2:
                message = "front"
            if out == 3:
                message = "right"
            if out == 4:
                message = "back"

            cv2.putText(img, 
                "Detecting start in {}".format(3 - int(time.time()-not_detecting_start)), 
                (200, 200), 
                cv2.FONT_HERSHEY_SIMPLEX, 1, 
                (0, 255, 255), 
                2, 
                cv2.LINE_4)
            if time.time()-not_detecting_start>3:
                not_detecting_start = 0
                first_time = True
                detecting = True
            # cv2.imshow("Image", img) 
            # cv2.waitKey(1) 

        if not hands and not first_time:
            cv2.putText(img, 
                "Show your hand to move in {}".format(3 - int(time.time()-not_detecting_start)), 
                (200, 200), 
                cv2.FONT_HERSHEY_SIMPLEX, 1, 
                (0, 255, 255), 
                2, 
                cv2.LINE_4)
            if time.time()-not_detecting_start>3:
                not_detecting_start = 0
                first_time = True
                detecting = True

        if not hands and first_time:
            cv2.putText(img, 
                "Show your hand to trigger detection", 
                (200, 200), 
                cv2.FONT_HERSHEY_SIMPLEX, 1, 
                (0, 255, 255), 
                2, 
                cv2.LINE_4)
        cv2.imshow("Image", img) # "Image": 제목
        cv2.waitKey(1) # Delay: 1ms
           
    else:
        if hands:  
            data = []
            hand1 = hands[0]
            lmList = hand1['lmList']
            ################################
            out = inference(lmList)
            ################################
            if len(hands) == 2:
                hand2 = hands[1]
                for i in hand2['lmList']:
                    lmList.append(i)

            for lm in lmList:
                data.extend([lm[0], height - lm[1], lm[2]])
            
            data.append(out)
            print("sent {} to unity".format(out))
            sock.sendto(str.encode(str(data)), serverAddressPort)
            detecting = False
            ###########################

            message =""
            if out == 0 :
                message = "stay"
            if out == 1 :
                message = "left"
            if out == 2:
                message = "front"
            if out == 3:
                message = "right"
            if out == 4:
                message = "back"

            cv2.putText(img, 
                message + "detected!!!!", 
                (200, 200), 
                cv2.FONT_HERSHEY_SIMPLEX, 1, 
                (0, 255, 255), 
                2, 
                cv2.LINE_4)
        else:
            print("error no hand to detect!!")
            detecting = False

            # time.sleep(0.001)

    cv2.imshow("Image", img) # "Image": 제목
    cv2.waitKey(1) # Delay: 1ms




