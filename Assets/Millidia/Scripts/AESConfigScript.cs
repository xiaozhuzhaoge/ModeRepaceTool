using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AESConfig", menuName = "ScriptableObject/AESConfig", order = 0)]
public class AESConfigScript : ScriptableObject
{
    public string keys;
    public string iv;
    public string inputFolder;
    public string outFolder;
    public string outDesFolder;
}
