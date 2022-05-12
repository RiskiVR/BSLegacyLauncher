using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class InstalledVer : MonoBehaviour
{
    public Text currentVersion;
    public static Text publicCurrentVersion;
    public static DiscordController DiscordController = new DiscordController();
    void Start()
    {
        DiscordController.BSVersion = InstalledVersionToggle.BSVersion;
        publicCurrentVersion = currentVersion;
        UpdateText(true);
    }

    public static void UpdateText(bool first = false)
    {
        // Handle old installs
        if(!Directory.Exists(InstalledVersionToggle.BSBaseDir)) Directory.CreateDirectory(InstalledVersionToggle.BSBaseDir);
        string version = "";
        // Make sure the Beat Saber version gets recognized and then saved to the new location
        if (File.Exists("BeatSaberVersion.txt"))
        {
            version = File.ReadAllText("BeatSaberVersion.txt");
            File.Delete("BeatSaberVersion.txt");
        }
        if (File.Exists("Beat Saber\\BeatSaberVersion.txt"))
        {
            version = File.ReadAllText("Beat Saber\\BeatSaberVersion.txt");
            File.Delete("Beat Saber\\BeatSaberVersion.txt");
        }

        // Write to new location if old found
        if (version != "") File.WriteAllText(InstalledVersionToggle.BSBaseDir + "BeatSaberVersion.txt", version);

        // Move Beat Saber Install to corresponding new folder
        if (Directory.Exists("Beat Saber"))
        {
            InstalledVersionToggle.SetBSVersion(version, true);
            Directory.Move("Beat Saber", InstalledVersionToggle.BSDirectory);
            File.WriteAllText(InstalledVersionToggle.BSDirectory + "BeatSaberVersion.txt", version);
        }

        // actually update text
        if(File.Exists(InstalledVersionToggle.BSBaseDir + "BeatSaberVersion.txt"))
        {
            version = File.ReadAllText(InstalledVersionToggle.BSBaseDir + "BeatSaberVersion.txt");
            Debug.Log(version);
            publicCurrentVersion.text = "Currently Selected: " + version;
            DiscordController.Installed = $"Currently Selected: {InstalledVersionToggle.BSVersion}";
        } else DiscordController.Installed = "No version installed";
        
        InstalledVersionToggle.SetBSVersion(version, true);
        DiscordController.VersionStart();
        UninstallCheck.DoUninstallCheck(first);
    }
}
