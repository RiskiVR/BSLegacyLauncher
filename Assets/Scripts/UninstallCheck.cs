using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class UninstallCheck : MonoBehaviour
{
    public GameObject LaunchOptions;
    public GameObject Button;
    public Button VersionsButton;
    public void Start()
    {
        if (Directory.Exists("Beat Saber"))
        {
            VersionsButton.interactable = false;
            LaunchOptions.SetActive(true);
            if (File.Exists("BeatSaberVersion.txt"))
                Button.SetActive(true);
        }
    }
}
