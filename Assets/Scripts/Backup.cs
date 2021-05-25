using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Backup : MonoBehaviour
{
    // FUCK FUCK
    public void StartBackup()
    {
        // Delete existing directories if present
        if (Directory.Exists("Beat Saber/CustomSongs"))
            Directory.Delete("Beat Saber/CustomSongs", true);

        if (Directory.Exists("Beat Saber/CustomSabers"))
            Directory.Delete("Beat Saber/CustomSabers", true);

        if (Directory.Exists("Beat Saber/Beat Saber_Data/CustomLevels"))
            Directory.Delete("Beat Saber/Beat Saber_Data/CustomLevels", true);

        if (Directory.Exists("Beat Saber/Beat Saber_Data/CustomWIPLevels"))
            Directory.Delete("Beat Saber/Beat Saber_Data/CustomWIPLevels", true);

        if (Directory.Exists("Beat Saber/UserData"))
            Directory.Delete("Beat Saber/UserData", true);

        // Begin restore process
        if (Directory.Exists("Backups/CustomSongs"))
            Directory.Move("Backups/CustomSongs", "Beat Saber/CustomSongs");

        if (Directory.Exists("Backups/CustomSabers"))
            Directory.Move("Backups/CustomSabers", "Beat Saber/CustomSabers");

        if (Directory.Exists("Backups/CustomLevels"))
            Directory.Move("Backups/CustomLevels", "Beat Saber/Beat Saber_Data/CustomLevels");

        if (Directory.Exists("Backups/CustomWIPLevels"))
            Directory.Move("Backups/CustomWIPLevels", "Beat Saber/Beat Saber_Data/CustomWIPLevels");

        if (Directory.Exists("Backups/UserData"))
            Directory.Move("Backups/UserData", "Beat Saber/UserData");

        Directory.Delete("Backups", true);
    }
}
