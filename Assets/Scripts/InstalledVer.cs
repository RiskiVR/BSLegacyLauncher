using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class InstalledVer : MonoBehaviour
{
    public Text currentVersion;
    void Start()
    {
        if (Directory.Exists("Beat Saber"))
            currentVersion.text = $"Currently Installed: {File.ReadAllText("BeatSaberVersion.txt")}";
    }
}
