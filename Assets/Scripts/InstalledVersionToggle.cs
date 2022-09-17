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
    public static string BSBaseDir { get { return BaseDirectory + "Installed Versions" + Path.DirectorySeparatorChar; } }
    public static string BSDirectory = "";
    public static string CustomLevelsDirectory { get { return BaseDirectory + "CustomLevels"; } }
    public static string CustomSongsDirectory { get { return BaseDirectory + "CustomSongs"; } }
    public static string CustomWIPLevelsDirectory { get { return BaseDirectory + "CustomWIPLevels"; } }
    public static string DLCDirectory { get { return BaseDirectory + "DLC"; } }
    public static string BSVersion = "1.0.0";
    public static string BaseDirectory = "";

    public static bool BSInstalledAndSelected { get
        {
            return BSVersion != "";
        } }

    public RuntimeAnimatorController TextEnter;
    public GameObject SelectVersionsObj;
    public Animator SelectVersions;
    public Animator InstalledVersions;

    public void Awake()
    {
        SetBaseDir();
    }

    public static void SetBaseDir()
    {
        BaseDirectory = Application.isEditor ? Environment.CurrentDirectory : System.AppDomain.CurrentDomain.BaseDirectory;
        if (!BaseDirectory.EndsWith(Path.DirectorySeparatorChar.ToString())) BaseDirectory += Path.DirectorySeparatorChar;
    }
    public static void SetBSVersion(string version, bool updatedText = false)
    {
        SetBaseDir();
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

        // Optimized now
        LivConnector.AddAndUpdateAllRegistryEntries(installedVersions);
        return installedVersions;
    }

    public void UpdateList()
    {
        Debug.Log("Updating list");
        Toggle(Directory.GetDirectories(BSBaseDir).Length > 0);

        if (Directory.GetDirectories(BSBaseDir).Length > 0)
        {
            InstalledVersions.runtimeAnimatorController = TextEnter;
        }
        else
        {
            SelectVersions.runtimeAnimatorController = TextEnter;
            SelectVersionsObj.SetActive(true);
        }
    }

    public void Toggle(bool value)
    {
        List<string> versions = GetInstalledVersions();
        Debug.Log(versions.Count + " version downloaded");
        int i = 0;
        int ii = 0;
        installedVersions = value;
        gameObject.SetActive(value);
        Debug.Log("show downloaded versions: " + value);
        VersionButtonController.PublicYears.SetActive(!value);
        VersionButtonController.PublicReleaseInfoButton.SetActive(false);
        VersionButtonController.PublicDownloadButton.SetActive(false);
        if (!value)
        {
            VersionButtonController.PublicVersions.GetComponent<RectTransform>().anchoredPosition = new Vector2(VersionButtonController.PublicVersions.GetComponent<RectTransform>().anchoredPosition.x, -8.5f);
            VersionButtonController.YearClicked(VersionButtonController.versionTable.Keys.OrderByDescending(x => Convert.ToInt32(x)).ToList()[0], VersionButtonController.versionTable.Keys.Count - 1, VersionButtonController.versionTable.Keys.Count);
            return;
        }
        VersionButtonController.PublicVersions.GetComponent<RectTransform>().anchoredPosition = new Vector2(VersionButtonController.PublicVersions.GetComponent<RectTransform>().anchoredPosition.x, 20f);
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
