using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InstalledVersionToggle : MonoBehaviour
{
    public static bool installedVersions = false;
    public static string BSBaseDir = Environment.CurrentDirectory + Path.DirectorySeparatorChar + "Installed Versions" + Path.DirectorySeparatorChar;
    public static string BSDirectory = Environment.CurrentDirectory + Path.DirectorySeparatorChar + "Beat Saber";
    public static string CustomLevelsDirectory = Environment.CurrentDirectory + Path.DirectorySeparatorChar + "CustomLevels";
    public static string CustomSongsDirectory = Environment.CurrentDirectory + Path.DirectorySeparatorChar + "CustomSongs";
    public static string CustomWIPLevelsDirectory = Environment.CurrentDirectory + Path.DirectorySeparatorChar + "CustomWIPLevels";
    public static string DLCDirectory = Environment.CurrentDirectory + Path.DirectorySeparatorChar + "DLC";
    public static string BSVersion = "1.0.0";
    public static Toggle toggle;
    public static bool BSInstalledAndSelected { get
        {
            return BSVersion != "";
        } }

    public static void SetBSVersion(string version, bool updatedText = false)
    {
        File.WriteAllText(BSBaseDir + "BeatSaberVersion.txt", version);
        BSVersion = version;
        BSDirectory = GetBSDirectory(version);
        if(!updatedText) InstalledVer.UpdateText();
    }

    public static void CreateCustomSongsSymLink()
    {
        if (Directory.Exists(BSDirectory + "CustomSongs")) DepotDownloaderObject.MoveDirectory(BSDirectory + "CustomSongs", CustomSongsDirectory);
    }

    public static string GetBSDirectory(string version)
    {
        return BSBaseDir + "Beat Saber " + version + Path.DirectorySeparatorChar;
    }

    public static List<string> GetInstalledVersions()
    {
        List<string> installedVersions = new List<string>();
        foreach(string s in Directory.GetDirectories(BSBaseDir))
        {
            installedVersions.Add(Path.GetFileName(s).Replace("Beat Saber ", ""));
        }
        return installedVersions;
    }

    // Start is called before the first frame update
    void Start()
    {
        toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(value =>
        {
            Toggle(value);
        });
    }

    public static void Toggle(bool value)
    {
        List<string> versions = GetInstalledVersions();
        int i = 0;
        int ii = 0;
        installedVersions = value;
        VersionButtonController.PublicYears.SetActive(!value);
        VersionButtonController.PublicReleaseInfoButton.SetActive(false);
        VersionButtonController.PublicDownloadButton.SetActive(false);
        if (!value)
        {
            VersionButtonController.PublicVersions.GetComponent<RectTransform>().anchoredPosition = new Vector2(VersionButtonController.PublicVersions.GetComponent<RectTransform>().anchoredPosition.x, -8.5f);
            VersionButtonController.YearClicked(VersionButtonController.versionTable.Keys.OrderByDescending(x => Convert.ToInt32(x)).ToList()[0], VersionButtonController.versionTable.Keys.Count - 1, VersionButtonController.versionTable.Keys.Count);
            return;
        }
        VersionButtonController.PublicVersions.GetComponent<RectTransform>().anchoredPosition = new Vector2(VersionButtonController.PublicVersions.GetComponent<RectTransform>().anchoredPosition.x, 40f);
        VersionButtonController.ClearVersions();
        VersionButtonController.PublicVersions.SetActive(true);
        foreach (string version in versions)
        {
            if (i >= 6)
            {
                i = 0;
                ii++;
            }
            if (!VersionButtonController.toDo.ContainsKey(ii)) VersionButtonController.toDo.Add(ii, new List<Version>());
            VersionButtonController.toDo[ii].Add(new Version { year = "0", BSVersion = version, row = ii });
            i++;
        }
        VersionButtonController.minors = VersionButtonController.toDo.Keys.ToList();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
