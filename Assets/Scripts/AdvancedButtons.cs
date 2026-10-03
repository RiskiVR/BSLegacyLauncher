using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using System.Linq;
using Debug = UnityEngine.Debug;


public class AdvancedButtons : MonoBehaviour
{
    [Header("Button Objects")]
    public GameObject IPA3Button;
    public GameObject UninstallIPAButton;
    
    public List<String> InputString = new List<String>();
    private void Delayfunc(float delay, Action action)
    {
        StartCoroutine(Delay(delay, action));
    }
    private static IEnumerator Delay(float delay, Action action)
    {
        yield return new WaitForSeconds(delay);
        action.Invoke();
    }

    public void BrowseAppdata()
    {
        Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\AppData\\LocalLow\\Hyperbolic Magnetism");
    }
    
    public void BackupAppdata()
    {
        DateTime thisDay = DateTime.Today;

        var lastBackupPath = "Beat Saber AppData Backups\\Latest Backup\\Beat Saber";
        var sourceDirectoryPath = Path.Combine(Environment.CurrentDirectory, (Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\AppData\\LocalLow\\Hyperbolic Magnetism\\Beat Saber"));
        var targetDirectoryPath = Path.Combine(Environment.CurrentDirectory, ("Beat Saber AppData Backups\\" + thisDay.ToString("M")) + "\\Beat Saber");


        if (Directory.Exists(targetDirectoryPath))
        {
            ErrorText.Display("BACKUP ON THAT DATE ALREADY EXISTS");
            Debug.LogError("Backup on that date already exists");
            return;
        }

        if (!Directory.Exists(targetDirectoryPath))
        {
            Directory.CreateDirectory(targetDirectoryPath);
        }

        if (Directory.Exists(lastBackupPath))
        {
            Directory.Delete(lastBackupPath, true);
        }

        if (Directory.Exists(sourceDirectoryPath))
        {
            DirectoryUtils.Copy(sourceDirectoryPath, targetDirectoryPath, true);
            DirectoryUtils.Copy(sourceDirectoryPath, lastBackupPath, true);
        }

        ErrorText.Display("BACKUP CREATED", ErrorText.MessageType.INFO);
    }

    public void BrowseGameFiles()
    {
        Process.Start("Beat Saber");
    }

    public void RevertAppdata()
    {
        DateTime thisDay = DateTime.Today;

        var lastBackupPath = "Beat Saber AppData Backups\\Latest Backup\\Beat Saber";
        var sourceDirectoryPath = Path.Combine(Environment.CurrentDirectory, (Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\AppData\\LocalLow\\Hyperbolic Magnetism\\Beat Saber"));
        var targetDirectoryPath = Path.Combine(Environment.CurrentDirectory, ("Beat Saber AppData Backups\\" + thisDay.ToString("M")) + "\\Beat Saber");

        if (Directory.Exists("Beat Saber AppData Backups"))
        {
            if (Directory.Exists(sourceDirectoryPath))
            {
                Directory.Delete(sourceDirectoryPath, true);
            }
            DirectoryUtils.Copy(lastBackupPath, sourceDirectoryPath, true);
            ErrorText.Display("APPDATA RESTORED TO LATEST BACKUP", ErrorText.MessageType.INFO);
        }
        else
        {
            ErrorText.Display("LAST BACKUP NOT FOUND");
            throw new Exception("Last backup not found");
        }

    }
    public void ClearAppdata()
    {
        DateTime thisDay = DateTime.Today;

        var lastBackupPath = "Beat Saber AppData Backups\\Latest Backup\\Beat Saber";
        var sourceDirectoryPath = Path.Combine(Environment.CurrentDirectory, (Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\AppData\\LocalLow\\Hyperbolic Magnetism\\Beat Saber"));
        var targetDirectoryPath = Path.Combine(Environment.CurrentDirectory, ("Beat Saber AppData Backups\\" + thisDay.ToString("M")) + "\\Beat Saber");

        if (Directory.Exists("Beat Saber AppData Backups"))
        {
            try
            {
                Directory.Delete(sourceDirectoryPath, true);
                ErrorText.Display("APPDATA CLEARED");
            } 
            catch
            {
                ErrorText.Display("APPDATA NOT FOUND");
                throw new Exception("AppData does not exist");
            }
        }
        else
        {
            ErrorText.Display("CREATE A BACKUP FIRST");
            throw new Exception("No backups found, cannot clear AppData");
        }
    }

    public void OpenPatreonURL()
    {
        Application.OpenURL("https://patreon.com/RiskiVR");
    }
}