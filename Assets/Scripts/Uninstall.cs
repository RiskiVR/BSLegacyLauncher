using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Uninstall : MonoBehaviour
{
    public GameObject ErrorTextObject;
    public Text ErrorText;
    public Text InstalledVer;

    public void DisplayErrorText(string text)
    {
        ErrorText.text = text;
        ErrorTextObject.SetActive(false);
        ErrorTextObject.SetActive(true);
    }

    public void UninstallTrigger()
    {
        if (!AdvancedButtons.IsUserAnAdmin())
        {
            DisplayErrorText("REQUIRES ADMIN PERMISSIONS");
            throw new Exception("REQUIRES ADMIN PERMISSIONS");
        }
        try
        {
            if (Directory.Exists(InstalledVersionToggle.BSDirectory + "CustomSabers"))
                DepotDownloaderObject.MoveDirectory(InstalledVersionToggle.BSDirectory + "CustomSabers", $"Backups/Beat Saber {InstalledVersionToggle.BSVersion}/CustomSabers");

            if (Directory.Exists(InstalledVersionToggle.BSDirectory + "UserData"))
                DepotDownloaderObject.MoveDirectory(InstalledVersionToggle.BSDirectory + "UserData", $"Backups/Beat Saber {InstalledVersionToggle.BSVersion}/UserData");

            if (Directory.Exists(InstalledVersionToggle.BSDirectory + "Plugins"))
                DepotDownloaderObject.MoveDirectory(InstalledVersionToggle.BSDirectory + "Plugins", $"Backups/Old {InstalledVersionToggle.BSVersion} Plugins");

            Directory.Delete(InstalledVersionToggle.BSDirectory, true);

            if (File.Exists(InstalledVersionToggle.BSBaseDir + "BeatSaberVersion.txt"))
            {
                File.Delete(InstalledVersionToggle.BSBaseDir + "BeatSaberVersion.txt");
            }
        }
        
        catch (Exception E)
        {
            DisplayErrorText("PATH IS DENIED OR FOLDER IS EMPTY");
            throw new Exception("UnauthorizedAccessException: Access to the path is denied");
        }
        UninstallCheck.DoUninstallCheck();
    }
    public void ClearVerText()
    {
        InstalledVer.text = "";
    }

}