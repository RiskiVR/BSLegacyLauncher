using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Yggdrasil.Logging;

public class ReleaseURL : MonoBehaviour
{
    public static List<Version> versions = new List<Version>();
    void Start()
    {
        string versionList = File.ReadAllText("Resources/BSVersions.json");
        versions = JsonConvert.DeserializeObject<List<Version>>(versionList);
    }

    public void OpenURL()
    {
        Version selectedVersion = versions.First(x => x.BSVersion.Equals(VersionVar.instance.version));
        Log.Info($"Opened Release info for {selectedVersion.BSVersion} : {selectedVersion.ReleaseURL}");
        Application.OpenURL($"{selectedVersion.ReleaseURL}");
    }
}
