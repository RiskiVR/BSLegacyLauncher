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
            AdvancedButtons.locations = SymLinkLocations.LoadFile(AdvancedButtons.jsonLocation);
            foreach (string folder in AdvancedButtons.locations.folders)
            {
                ProcessSymlinkDelete(InstalledVersionToggle.BSDirectory + folder);
            }

            ProcessSymlinkDelete(InstalledVersionToggle.BSDirectory + "CustomSongs");
            ProcessSymlinkDelete(InstalledVersionToggle.BSDirectory + "Beat Saber_Data" + Path.DirectorySeparatorChar + "CustomLevels");
            ProcessSymlinkDelete(InstalledVersionToggle.BSDirectory + "Beat Saber_Data" + Path.DirectorySeparatorChar + "CustomWIPLevels");
            ProcessSymlinkDelete(InstalledVersionToggle.BSDirectory + "DLC");

            Directory.Delete(InstalledVersionToggle.BSDirectory, true);

            if (File.Exists(InstalledVersionToggle.BSBaseDir + "BeatSaberVersion.txt"))
            {
                File.Delete(InstalledVersionToggle.BSBaseDir + "BeatSaberVersion.txt");
            }
        }
        catch (Exception E)
        {
            DisplayErrorText($"{E.GetType().Name.ToUpper()}: {E.Message.ToUpper()}");
            throw E;
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
                Arguments = $"/c rd /q \"{path}\\\""
            };
            Process p = Process.Start(i);
            p.WaitForExit();
            p.Dispose();
        }
    }

    public void ClearVerText()
    {
        InstalledVer.text = "";
    }

}