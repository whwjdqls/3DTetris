# 3DTetris
## Hand controlled 3D Tetris
This Repo contains the following
1. C# scripts for implementing 3D tetris in unity
* Handkeypoint
* Menu
* Tetris

2. Hand pose recognition package
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

---
## Communicate with Unity and control 3D tetris
```sh
python main_final.py
```
