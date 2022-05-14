using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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
        try
        {
            if (Directory.Exists(InstalledVersionToggle.BSDirectory + "CustomSabers"))
                DepotDownloaderObject.MoveDirectory(InstalledVersionToggle.BSDirectory + "CustomSabers", $"Backups/Beat Saber {InstalledVersionToggle.BSVersion}/CustomSabers");

            if (Directory.Exists(InstalledVersionToggle.BSDirectory + "UserData"))
                DepotDownloaderObject.MoveDirectory(InstalledVersionToggle.BSDirectory + "UserData", $"Backups/Beat Saber {InstalledVersionToggle.BSVersion}/UserData");

            if (Directory.Exists(InstalledVersionToggle.BSDirectory + "Plugins"))
                DepotDownloaderObject.MoveDirectory(InstalledVersionToggle.BSDirectory + "Plugins", $"Backups/Old {InstalledVersionToggle.BSVersion} Plugins");


            ProcessSymlinkDelete(InstalledVersionToggle.BSDirectory + Path.DirectorySeparatorChar + "CustomSongs");
            ProcessSymlinkDelete(InstalledVersionToggle.BSDirectory + Path.DirectorySeparatorChar + "Beat Saber_Data" + Path.DirectorySeparatorChar + "CustomLevels");
            ProcessSymlinkDelete(InstalledVersionToggle.BSDirectory + Path.DirectorySeparatorChar + "Beat Saber_Data" + Path.DirectorySeparatorChar + "CustomWIPLevels");
            ProcessSymlinkDelete(InstalledVersionToggle.BSDirectory + Path.DirectorySeparatorChar + "DLC");

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

    public void ProcessSymlinkDelete(string path)
    {
        if (Directory.Exists(path) && new DirectoryInfo(path).Attributes.HasFlag(FileAttributes.ReparsePoint))
        {
            ProcessStartInfo i = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                Arguments = "/c \"rmdir \"" + path + "\""
            };
            Process p = Process.Start(i);
            p.WaitForExit();
        }
            
    }

    public void ClearVerText()
    {
        InstalledVer.text = "";
    }

}