using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class InstalledVer : MonoBehaviour
{
    public Text currentVersion;
    public DiscordController DiscordController;
    void Start()
    {
        if (File.Exists("Beat Saber\\Beat Saber.exe"))
            {
            if (File.Exists("Beat Saber\\BeatSaberVersion.txt"))
            {
                currentVersion.text = $"Currently Installed: {File.ReadAllText("Beat Saber\\BeatSaberVersion.txt")}";
                File.WriteAllText("BeatSaberVersion.txt", $"{File.ReadAllText("Beat Saber\\BeatSaberVersion.txt")}");
                DiscordController.Installed = $"Currently Installed: {File.ReadAllText("Beat Saber\\BeatSaberVersion.txt")}";
                DiscordController.BSVersion = $"{File.ReadAllText("Beat Saber\\BeatSaberVersion.txt")}";
                DiscordController.VersionStart();
            }
            else
            {
                currentVersion.text = $"Currently Installed: {File.ReadAllText("BeatSaberVersion.txt")}";
                DiscordController.Installed = $"Currently Installed: {File.ReadAllText("BeatSaberVersion.txt")}";
                DiscordController.BSVersion = $"{File.ReadAllText("BeatSaberVersion.txt")}";
                DiscordController.VersionStart();
            }
        }
        else
        {
            DiscordController.Installed = "No version installed";
            DiscordController.VersionStart();
        }
    }
}
