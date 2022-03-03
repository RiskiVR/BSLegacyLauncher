﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class UninstallCheck : MonoBehaviour
{
    public GameObject LaunchOptions;
    public GameObject Button;
    public Button VersionsButton;
    public Button GameFilesButton;
    public Button InstallIPAButton;
    public void Start()
    {
        if (Directory.Exists("Beat Saber"))
        {
            if (File.Exists("BeatSaberVersion.txt"))
                Button.SetActive(true);
        }
        if (File.Exists("Beat Saber\\Beat Saber.exe"))
        {
            VersionsButton.interactable = false;
            GameFilesButton.interactable = true;
            InstallIPAButton.interactable = true;
            LaunchOptions.SetActive(true);
        }
    }
}