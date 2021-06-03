using System;
using System.Collections;
using System.Collections.Generic;
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
        string fileName = "";
        
        var sourceDirectoryPath = Path.Combine(Environment.CurrentDirectory, (Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\AppData\\LocalLow\\Hyperbolic Magnetism\\Beat Saber"));
        var sourceDirectoryInfo = new DirectoryInfo(sourceDirectoryPath);

        var targetDirectoryPath = Path.Combine(Environment.CurrentDirectory, (Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\AppData\\LocalLow\\Hyperbolic Magnetism\\Beat Saber Backup"));
        var targetDirectoryInfo = new DirectoryInfo(targetDirectoryPath);

        string destFile = Path.Combine(targetDirectoryPath, fileName);

        if (Directory.Exists(targetDirectoryPath))
        {
            CuteErrorObject.SetActive(false);
            ErrorText.text = "BACKUP ALREADY EXISTS";
            ErrorTextObject.SetActive(false);
            ErrorTextObject.SetActive(true);
            throw new Exception();
        }

        else
        {
            if (!Directory.Exists(targetDirectoryPath))
            {
                Directory.CreateDirectory(targetDirectoryPath);
            }

            if (Directory.Exists(sourceDirectoryPath))
            {
                string[] files = Directory.GetFiles(sourceDirectoryPath);

                // Copy the files and overwrite destination files if they already exist.
                foreach (string s in files)
                {
                    // Use static Path methods to extract only the file name from the path.
                    fileName = Path.GetFileName(s);
                    destFile = Path.Combine(targetDirectoryPath, fileName);
                    File.Copy(s, destFile, true);
                }
            }
            else
            {
                Console.WriteLine("Source path does not exist!");
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
            throw new Exception();
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
            throw new Exception();
        }
    }
}
