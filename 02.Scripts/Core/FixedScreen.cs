using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedScreen : MonoBehaviour
{
    void Start()
    {
        int setWidth = 1920;
        int setHeight = 1080; 

        Screen.SetResolution(setWidth, setHeight, true);
    }
    
}
