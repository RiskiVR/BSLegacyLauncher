using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class UninstallCheck : MonoBehaviour
{
    public GameObject LaunchOptions;
    public GameObject UninstallButton;
    public Button VersionsButton;
    public Button GameFilesButton;
    public Button InstallIPAButton;
    public void Start()
    {
        if (Directory.Exists("Beat Saber"))
        {
            if (File.Exists("BeatSaberVersion.txt"))
            {
                LaunchOptions.SetActive(true);
                InstallIPAButton.interactable = true;
            }
        }
        if (File.Exists("Beat Saber\\Beat Saber.exe"))
        {
            VersionsButton.interactable = false;
            GameFilesButton.interactable = true;
            UninstallButton.SetActive(true);
        }
    }
}
