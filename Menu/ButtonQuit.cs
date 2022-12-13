using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonQuit : MonoBehaviour
{
    public void Quit()
    {
        Application.Quit();
    }
    // Start is called before the first frame update
    void Start()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Quit();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
