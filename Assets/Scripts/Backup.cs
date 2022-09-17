using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Backup : MonoBehaviour
{
    public void StartBackup()
    {
        // Begin restore process
        if (Directory.Exists($"Backups/Beat Saber {InstalledVersionToggle.BSVersion}/CustomSabers"))
            Directory.Move($"Backups/Beat Saber {InstalledVersionToggle.BSVersion}/CustomSabers", InstalledVersionToggle.BSDirectory + "CustomSabers");

        if (Directory.Exists($"Backups/Beat Saber {InstalledVersionToggle.BSVersion}/UserData"))
            Directory.Move($"Backups/Beat Saber {InstalledVersionToggle.BSVersion}/UserData", InstalledVersionToggle.BSDirectory + "UserData");
        if (Directory.Exists($"Backups/Old {InstalledVersionToggle.BSVersion} Plugins"))
            Directory.Move($"Backups/Old {InstalledVersionToggle.BSVersion} Plugins", InstalledVersionToggle.BSDirectory + $"Old {InstalledVersionToggle.BSVersion} Plugins");
    }
}
