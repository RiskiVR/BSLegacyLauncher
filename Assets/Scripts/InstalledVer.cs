﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class InstalledVer : MonoBehaviour
{
    public Text currentVersion;
    void Start()
    {
        if (File.Exists("Beat Saber\\Beat Saber.exe"))
        {
        if (File.Exists("Beat Saber\\BeatSaberVersion.txt"))
            {
                currentVersion.text = $"Currently Installed: {File.ReadAllText("Beat Saber\\BeatSaberVersion.txt")}";
                File.WriteAllText("BeatSaberVersion.txt", $"{File.ReadAllText("Beat Saber\\BeatSaberVersion.txt")}");
            }
            else
            {
                currentVersion.text = $"Currently Installed: {File.ReadAllText("BeatSaberVersion.txt")}";
            }
        }
    }
}