using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    public static UIHandler instance;

    public Text LayerText;

    void Awake()
    {
        instance = this;
    }


    public void UpdateUI(int layers)
    {
        if(layers <= 3)
        {
            LayerText.text = layers.ToString();
        }
        else
        {

            LayerText.text = "3";
        }
        
    }
}
