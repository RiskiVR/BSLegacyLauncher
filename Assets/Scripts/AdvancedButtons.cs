using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.Security.Principal;
using System.Runtime.InteropServices;

public class AdvancedButtons : MonoBehaviour
{
    public Text ErrorText;
    public Text FeedbackText;
    public GameObject ErrorTextObject;
    public GameObject CuteErrorObject;
    public GameObject FeedbackTextObject;

    private void DisplayErrorText(string text)
    {
        // Set to false to restart popup animation DON'T CHANGE
        FeedbackTextObject.SetActive(false);
        ErrorTextObject.SetActive(false);
        ErrorTextObject.SetActive(true);
        ErrorText.text = text;
    }
    private void DisplayFeedbackText(string text)
    {
        // Set to false to restart popup animation DON'T CHANGE
        ErrorTextObject.SetActive(false);
        FeedbackTextObject.SetActive(false);
        FeedbackTextObject.SetActive(true);
        FeedbackText.text = text;
    }

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
            DisplayErrorText("BACKUP ALREADY EXISTS");
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

            DisplayFeedbackText("BACKUP CREATED");
        }
    }

    public void BrowseGameFiles()
    {
        Process.Start("explorer.exe", InstalledVersionToggle.BSDirectory);
    }

    public void RevertAppdata()
    {
        if (Directory.Exists((Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\AppData\\LocalLow\\Hyperbolic Magnetism\\Beat Saber Backup")))
        {
            if (Directory.Exists((Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\AppData\\LocalLow\\Hyperbolic Magnetism\\Beat Saber")))
            {
                Directory.Delete((Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\AppData\\LocalLow\\Hyperbolic Magnetism\\Beat Saber"), true);
            }

            Directory.Move((Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\AppData\\LocalLow\\Hyperbolic Magnetism\\Beat Saber Backup"), (Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\AppData\\LocalLow\\Hyperbolic Magnetism\\Beat Saber"));

            DisplayFeedbackText("APPDATA RESTORED FROM BACKUP");
        }
    

        else
        {
            CuteErrorObject.SetActive(false);
            DisplayErrorText("NO BACKUP FOUND");
            throw new Exception("No Backup Found");
        }

    }
    public void ClearAppdata()
    {
        if (Directory.Exists((Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\AppData\\LocalLow\\Hyperbolic Magnetism\\Beat Saber Backup")))
        {
            Directory.Delete((Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\AppData\\LocalLow\\Hyperbolic Magnetism\\Beat Saber"), true);

            DisplayErrorText("APPDATA CLEARED");
        }
        else
        {
            CuteErrorObject.SetActive(false);
            DisplayErrorText("CREATE A BACKUP FIRST");
            throw new Exception("No Backup Found, Cannot clear AppData");
        }
    }

    public void InstallIPA()
    {
        try
        {
            DirectoryCopy("Resources\\BSIPA", InstalledVersionToggle.BSDirectory, true);

            ErrorTextObject.SetActive(false);
            DisplayFeedbackText("LEGACY IPA INSTALLED");
        }
        catch
        {
            CuteErrorObject.SetActive(false);
            DisplayErrorText("IPA ALREADY INSTALLED");
            throw new Exception("IPA is already installed");
        }

        {
            string bspath = InstalledVersionToggle.BSDirectory;
            Process.Start(new ProcessStartInfo
            {
                WorkingDirectory = bspath,
                FileName = bspath + "IPA.exe"
            });
        }
    }

    [DllImport("shell32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool IsUserAnAdmin();

    public void CreateSymbolicFolder()
    {
        // Your turn, ComputerElite
        // Thanks Risk, headpats
        if(!IsUserAnAdmin())
        {
            DisplayErrorText("REQUIRES ADMIN PERMISSIONS");
            throw new Exception("REQUIRES ADMIN PERMISSIONS");
        }

        if (!Directory.Exists(InstalledVersionToggle.CustomSongsDirectory)) Directory.CreateDirectory(InstalledVersionToggle.CustomSongsDirectory);
        foreach (string d in Directory.GetDirectories(InstalledVersionToggle.BSBaseDir))
        {
            string customSongsFolder = d + Path.DirectorySeparatorChar + "CustomSongs";
            string customLevelsFolder = d + Path.DirectorySeparatorChar + "Beat Saber_Data" + Path.DirectorySeparatorChar + "CustomLevels";
            string customWIPLevelsFolder = d + Path.DirectorySeparatorChar + "Beat Saber_Data" + Path.DirectorySeparatorChar + "CustomWIPLevels";
            string dLCsFolder = d + Path.DirectorySeparatorChar + "DLC";

            ProcessSymLink(customSongsFolder, InstalledVersionToggle.CustomSongsDirectory);
            ProcessSymLink(customLevelsFolder, InstalledVersionToggle.CustomLevelsDirectory);
            ProcessSymLink(customWIPLevelsFolder, InstalledVersionToggle.CustomWIPLevelsDirectory);
            ProcessSymLink(dLCsFolder, InstalledVersionToggle.DLCDirectory);
        }
        DisplayFeedbackText("CUSTOMLEVELS FOLDER CREATED");
    }

    public static void ProcessSymLink(string path, string target)
    {
        if (Directory.Exists(path) && !new DirectoryInfo(path).Attributes.HasFlag(FileAttributes.ReparsePoint))
            DepotDownloaderObject.MoveDirectory(path, target);
        if(!Directory.Exists(path))
            CreateSymLink(path, target);
    }

    public static void CreateSymLink(string path, string target)
    {
        ProcessStartInfo i = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            Arguments = "/c \"mklink /D \"" + path + "\" \"" + target + ('\\') + "\"\""
        };
        Process p = Process.Start(i);
        p.WaitForExit();
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