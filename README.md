# 3DTetris
## Hand controlled 3D Tetris
This Repo contains the following
1. Materials and Prefabs for 3D tetris in unity

2. C# scripts for implementing 3D tetris in unity
* Handkeypoint
* Menu
* Tetris

3. Hand pose recognition package
* Hand pose recognition model
* Making Custom Hand pose dataset
* Communicating with Unity for control via hand pose

---
## Model
Simple MLP network to inference hand pose with hand skeleton coordinate inputs

---
## Make Dataset
Run the following command to make custom dataset

```sh
python makedataset.py 
```
then the following will show on prompt
```sh
what class are you collecting?
```
type in the class number (label) of the hand pose you are trying to collect

you can change the filming time and sleep time in order to control the number of data you will collect
default setting are the following 
```
filming_time = 120
sleep_time = 0.2
```
which means film for 120 seconds with 0.2 second period
---
## Train Model
With npy files made with makedataset.py train custum hand pose recognition model with train_np.ipynb

---
## Communicate with Unity and control 3D tetris
```sh
python main_final.py
```
