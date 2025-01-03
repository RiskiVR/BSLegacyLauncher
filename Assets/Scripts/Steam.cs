using Microsoft.Win32;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Yggdrasil.Logging;

public class Steam : MonoBehaviour
{
    public static List<Version> versions = new List<Version>();
    public TextMesh DownloadingText;

    private void Start()
    {
        string versionList = File.ReadAllText(InstalledVersionToggle.BaseDirectory + "Resources/BSVersions.json");
        versions = JsonConvert.DeserializeObject<List<Version>>(versionList);
    }
    public void SteamDownload()
    {
        DownloadingText.text = $"Downloading {VersionVar.instance.version}...";

        Version selectedVersion = versions.First(x => x.BSVersion.Equals(VersionVar.instance.version));

        var path = "C:\\Program Files (x86)\\Steam\\steam.exe";
        var args = $"+download_depot 620980 620981 [{selectedVersion.BSManifest}]";

        Process.Start(path, args);
    }
}