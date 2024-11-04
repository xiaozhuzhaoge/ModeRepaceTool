using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;
 
public class ABRunTime : MonoBehaviour
{
    public AESConfigScript configScript;
    private void Start()
    {
        var abs = Directory.GetFiles(configScript.outFolder);
        foreach (var ab in abs)
        {
             
        }
       
    }
}
