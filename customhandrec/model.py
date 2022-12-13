import csv
import numpy as np
import torch
import torch.nn as nn
import pandas as pd
from torch.utils.data import Dataset

def to_onehot(labels,num_classes):
    out = np.zeros((labels.shape[0],num_classes))
    for i in range(labels.shape[0]):
        out[i][labels[i]]=1
    return out

class CustomDataset(Dataset):
    def __init__(self, data_dir,num_class):
        self.data = pd.read_csv(data_dir)
        self.labels = torch.tensor(to_onehot(self.data.iloc[:,0].values,num_class),dtype=torch.float32)
        self.keypoints = torch.tensor(self.data.iloc[:,1:].values, dtype=torch.float32)
    
    def __len__(self):
        return len(self.labels)
    
    def __getitem__(self,idx):
        keypoint = self.keypoints[idx]
        label = self.labels[idx]
        return keypoint, label
        

class model(nn.Module):
    def __init__(self,num_class,dimension):
        super().__init__()
        self.linear1 = nn.Linear(21*dimension,2**(dimension+2))
        self.Dropout1 = nn.Dropout(p = 0.2)
        self.act1 = nn.ReLU()
        
        self.linear2 = nn.Linear(2**(dimension+2),2**(dimension+1))
        self.Dropout2 = nn.Dropout(p=0.2)
        self.act2 = nn.ReLU() 
        
        self.linear3 = nn.Linear(2**(dimension+1),num_class)
        self.act3 = nn.Sigmoid()
        
    def forward(self, x):
        out = self.linear1(x)
        out = self.Dropout1(out)
        out = self.act1(out)
        out = self.linear2(out)
        out = self.Dropout2(out)
        out = self.act2(out)    
        out = self.linear3(out)
        out = self.act3(out)  
        return out
        
class model_2(nn.Module):
    def __init__(self,num_class,dimension):
        super().__init__()
        self.linear1 = nn.Linear(21*dimension,2**(dimension+3)) 
        self.Dropout1 = nn.Dropout(p = 0.2)
        self.act1 = nn.ReLU()
        
        self.linear2 = nn.Linear(2**(dimension+3),2**(dimension+2))
        self.Dropout2 = nn.Dropout(p=0.2)
        self.act2 = nn.ReLU() 
        
        self.linear3 = nn.Linear(2**(dimension+2),2**(dimension+1))
        self.Dropout3 = nn.Dropout(p=0.2)
        self.act3= nn.ReLU() 
        
        self.linear4 = nn.Linear(2**(dimension+1),num_class)
        self.act4 = nn.Sigmoid()
        
    def forward(self, x):
        out = self.linear1(x)
        out = self.Dropout1(out)
        out = self.act1(out)
        out = self.linear2(out)
        out = self.Dropout2(out)
        out = self.act2(out)   
        
        out = self.linear3(out)
        out = self.Dropout3(out)
        out = self.act3(out)  
        
        out = self.linear4(out)
        out = self.act4(out)  
        return out