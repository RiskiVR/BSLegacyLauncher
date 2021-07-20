using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class AdvancedButtons : MonoBehaviour
{
    public Text ErrorText;
    public GameObject ErrorTextObject;
    public GameObject CuteErrorObject;
    public void BrowseAppdata()
    {
        Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\AppData\\LocalLow\\Hyperbolic Magnetism");
    }

    public void BackupAppdata()
    {
        var sourceDirectoryPath = Path.Combine(Environment.CurrentDirectory, (Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\AppData\\LocalLow\\Hyperbolic Magnetism\\Beat Saber"));
        var targetDirectoryPath = Path.Combine(Environment.CurrentDirectory, (Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\AppData\\LocalLow\\Hyperbolic Magnetism\\Beat Saber Backup"));

        if (Directory.Exists(targetDirectoryPath))
        {
            CuteErrorObject.SetActive(false);
            ErrorText.text = "BACKUP ALREADY EXISTS";
            ErrorTextObject.SetActive(false);
            ErrorTextObject.SetActive(true);
            throw new Exception("Backup already exists");
        }

        else
        {
            if (!Directory.Exists(targetDirectoryPath))
            {
                Directory.CreateDirectory(targetDirectoryPath);
            }

            if (Directory.Exists(sourceDirectoryPath))
            {
                DirectoryCopy(sourceDirectoryPath, targetDirectoryPath, true);
            }
        }
    }

    public void BrowseGameFiles()
    {
        Process.Start("explorer.exe", "Beat Saber");
    }

    public void RevertAppdata()
    {
        if (Directory.Exists((Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\AppData\\LocalLow\\Hyperbolic Magnetism\\Beat Saber Backup")))
        {
            Directory.Delete((Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\AppData\\LocalLow\\Hyperbolic Magnetism\\Beat Saber"), true);
            Directory.Move((Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\AppData\\LocalLow\\Hyperbolic Magnetism\\Beat Saber Backup"), (Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\AppData\\LocalLow\\Hyperbolic Magnetism\\Beat Saber"));
        }
        else
        {
            CuteErrorObject.SetActive(false);
            ErrorText.text = "NO BACKUP FOUND";
            ErrorTextObject.SetActive(false);
            ErrorTextObject.SetActive(true);
            throw new Exception("No Backup Found");
        }

    }
    public void ClearAppdataBackup()
    {
        if (Directory.Exists((Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\AppData\\LocalLow\\Hyperbolic Magnetism\\Beat Saber Backup")))
        {
            Directory.Delete((Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\AppData\\LocalLow\\Hyperbolic Magnetism\\Beat Saber Backup"), true);
        }
        else
        {
            CuteErrorObject.SetActive(false);
            ErrorText.text = "NO BACKUP FOUND";
            ErrorTextObject.SetActive(false);
            ErrorTextObject.SetActive(true);
            throw new Exception("No Backup Found");
        }
    }

    public void InstallIPA()
    {
        try
        {
            DirectoryCopy("Resources\\BSIPA", "Beat Saber", true);

            
        }
        catch
        {
            CuteErrorObject.SetActive(false);
            ErrorText.text = "IPA ALREADY INSTALLED";
            ErrorTextObject.SetActive(false);
            ErrorTextObject.SetActive(true);
            throw new Exception("IPA is already installed");
        }

        {
            string bspath = Environment.CurrentDirectory + "/Beat Saber";
            Process.Start(new ProcessStartInfo
            {
                WorkingDirectory = bspath,
                FileName = bspath + "/IPA.exe"
            });
        }
    }
    void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
    {
        // Get the subdirectories for the specified directory.
        DirectoryInfo dir = new DirectoryInfo(sourceDirName);

        if (!dir.Exists)
        {
            throw new DirectoryNotFoundException(
                "Source directory does not exist or could not be found: "
                + sourceDirName);
        }
        
        DirectoryInfo[] dirs = dir.GetDirectories();

        // If the destination directory doesn't exist, create it.       
        Directory.CreateDirectory(destDirName);

        // Get the files in the directory and copy them to the new location.
        FileInfo[] files = dir.GetFiles();
        foreach (FileInfo file in files)
        {
            string tempPath = Path.Combine(destDirName, file.Name);
            file.CopyTo(tempPath, false);
        }

        // If copying subdirectories, copy them and their contents to new location.
        if (copySubDirs)
        {
            foreach (DirectoryInfo subdir in dirs)
            {
                string tempPath = Path.Combine(destDirName, subdir.Name);
                DirectoryCopy(subdir.FullName, tempPath, copySubDirs);
            }
        }
    }



    public void OpenPatreonURL()
    {
        Application.OpenURL("https://patreon.com/RiskiVR");
    }

    public void OpenKofiURL()
    {
        Application.OpenURL("https://ko-fi.com/U7U114VMM");
    }
}