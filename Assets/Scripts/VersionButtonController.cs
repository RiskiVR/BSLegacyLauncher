using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using UnityEngine;
using UnityEngine.UI;

public class VersionButtonController : MonoBehaviour
{
    public static List<Version> versions = new List<Version>();
    public static Dictionary<string, Dictionary<int, List<Version>>> versionTable = new Dictionary<string, Dictionary<int, List<Version>>>();
    public static Dictionary<int, List<Version>> toDo = new Dictionary<int, List<Version>>();
    float lastButtonSpawn = 0.0f;

    [Header("Prefabs")]
    public GameObject VersionButtonPrefab;
    public static GameObject PublicYearButtonPrefab;
    public GameObject YearButtonPrefab;

    [Header("Patreons")]
    public GameObject PatreonL;
    public GameObject PatreonR;


    [Header("Other stuff")]
    public GameObject _ClickSound;
    public static GameObject _PublicClickSound;
    public GameObject Plane;
    public static GameObject PublicPlane = null;
    public GameObject Versions;
    public static GameObject PublicVersions = null;
    public GameObject Years;
    public static GameObject PublicYears = null;
    public GameObject Bar;
    public static GameObject PublicBar = null;

    public GameObject DownloadButton;
    public static GameObject PublicDownloadButton;
    public GameObject VersionText2;
    public GameObject ReleaseInfoButton;
    public static GameObject PublicReleaseInfoButton;

    public static bool yearsAdded = false;
    public static int currentRow;
    public static int currentColumn;

    // Just ignore that variable name. I scambled the sorting Riski made and didn't bother to rename the vars
    public static List<int> minors = new List<int>();

    List<string> installedVersions = new List<string>();

    // Start is called before the first frame update
    void Start()
    {
        PublicVersions = Versions;
        PublicYears = Years;
        PublicBar = Bar;
        PublicPlane = Plane;
        PublicYearButtonPrefab = YearButtonPrefab;
        _PublicClickSound = _ClickSound;
        PublicDownloadButton = DownloadButton;
        PublicReleaseInfoButton = ReleaseInfoButton;
        // Don't forget to add back
        try
        {
            // Update cache
            WebClient c = new WebClient();
            string d = c.DownloadString("https://raw.githubusercontent.com/RiskiVR/BSLegacyLauncher/master/Resources/BSVersions.json");
            File.WriteAllText(InstalledVersionToggle.BaseDirectory + "Resources/BSVersions.json", d);
            d = c.DownloadString("https://raw.githubusercontent.com/RiskiVR/BSLegacyLauncher/master/Resources/Patreons.json");
            File.WriteAllText(InstalledVersionToggle.BaseDirectory + "Resources/Patreons.json", d);
        } catch { }

        string versionList = File.ReadAllText(InstalledVersionToggle.BaseDirectory + "Resources/BSVersions.json");
        versions = JsonConvert.DeserializeObject<List<Version>>(versionList);

        // Aperture
        if(ComputersVars.useApertureDeskJob) versions.Add(new Version { BSManifest = "152863936826529847", BSVersion = "Aperture", year = "2022", row = 3 });


        string patreonsList = File.ReadAllText(InstalledVersionToggle.BaseDirectory + "Resources/Patreons.json");
        List<Patreon> patreons = JsonConvert.DeserializeObject<List<Patreon>>(patreonsList);
        // Order by alphabet
        patreons = patreons.OrderBy(x => x.name).ToList();
        PatreonL.GetComponent<Text>().text = "";
        PatreonR.GetComponent<Text>().text = "";
        for (int i = 0; i < patreons.Count; i++)
        {
            (i < patreons.Count / 2 ? PatreonL : PatreonR).GetComponent<Text>().text += patreons[i].name + "\n";
        }
        GenerateDict();
    }

    public static void ClearVersions()
    {
        currentColumn = 0;
        currentRow = 0;
        toDo = new Dictionary<int, List<Version>>();
        foreach (Transform t in PublicVersions.transform)
        {
            Destroy(t.gameObject);
        }
    }

    public static void GenerateDict()
    {
        versionTable.Clear();
        Dictionary<string, List<Version>> yearTable = new Dictionary<string, List<Version>>();
        // Group by year
        foreach (Version v in versions)
        {
            if (!yearTable.ContainsKey(v.year))
            {
                yearTable.Add(v.year, new List<Version>());
            }
            yearTable[v.year].Add(v);
        }
        // Group by minor version
        foreach (KeyValuePair<string, List<Version>> year in yearTable)
        {
            Dictionary<int, List<Version>> minorVersions = new Dictionary<int, List<Version>>();
            foreach (Version version in year.Value)
            {
                if (!minorVersions.ContainsKey(version.row))
                {
                    minorVersions.Add(version.row, new List<Version>());
                }
                minorVersions[version.row].Add(version);
            }
            versionTable.Add(year.Key, minorVersions);
        }
        versionTable = versionTable.OrderBy(x => Convert.ToInt32(x.Key)).ToDictionary(x => x.Key, x => x.Value);
        if(!yearsAdded)
        {
            AddYearButtons();
            yearsAdded = true;
        }
    }

    public static void AddYearButtons()
    {
        int yearCount = versionTable.Keys.Count;
        const int yearButtonWidth = 87;
        int totalWidth = yearCount * yearButtonWidth;
        int left = 0 - totalWidth / 2;
        int i = -1;
        foreach (string year in versionTable.Keys)
        {
            int ii = i + 1;
            GameObject yearButton = Instantiate(PublicYearButtonPrefab, PublicYears.transform);
            yearButton.GetComponent<Button>().onClick.AddListener(() => { YearClicked(year, ii, yearCount); });
            yearButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(i * yearButtonWidth + 87, 0);
            yearButton.GetComponentInChildren<Text>().text = year;
            i++;
        }
    }

    public static void YearClicked(string year, int barPos, int yearCount)
    {
        DisableYearButtons();
        PublicBar.GetComponent<ButtonController>().set(barPos, yearCount);
        StartVersionDisplay(year);
    }

    public static void StartVersionDisplay(string year)
    {
        ClearVersions();
        foreach(KeyValuePair<int, List<Version>> y in versionTable[year])
        {
            toDo.Add(y.Key, y.Value);
        }
        minors = toDo.Keys.ToList();
    }

    public static void DisableYearButtons()
    {
        GenerateDict();
        PublicVersions.SetActive(true);
        ClearVersions();
        _PublicClickSound.GetComponent<AudioSource>().Play();
        PublicBar.SetActive(true);
        PublicPlane.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (minors.Count > 0 && Time.time - 0.2 > lastButtonSpawn)
        {
            if(toDo.ContainsKey(minors[0]) && toDo[minors[0]].Count > 0)
            {
                string version = toDo[minors[0]][0].BSVersion;
                GameObject button = Instantiate(VersionButtonPrefab, Versions.transform);
                button.GetComponentInChildren<Text>().text = version;
                button.GetComponent<RectTransform>().anchoredPosition = new Vector2(currentColumn * 90, currentRow * -30);
                
                button.GetComponentInChildren<Button>().onClick.AddListener(() =>
                {
                    installedVersions = InstalledVersionToggle.GetInstalledVersions();
                    if (InstalledVersionToggle.installedVersions) InstalledVersionToggle.SetBSVersion(version);
                    VersionText2.SetActive(true);
                    DownloadButton.SetActive(true);
                    DownloadButton.GetComponent<Button>().interactable = !InstalledVersionToggle.installedVersions;
                    Versions.GetComponent<VersionVar>().ListVersion(version);
                    ReleaseInfoButton.SetActive(true);
                    ReleaseInfoButton.GetComponent<Button>().interactable = false;
                    if (versions.FirstOrDefault(x => x.BSVersion == version) != null)
                    {
                        ReleaseInfoButton.GetComponent<Button>().interactable = true;
                    }
                        
                    _ClickSound.GetComponent<AudioSource>().Play();
                });
                if (installedVersions.Contains(version) && !InstalledVersionToggle.installedVersions)
                {
                    button.GetComponentInChildren<Button>().interactable = false;
                }
                currentColumn++;
                toDo[minors[0]].RemoveAt(0);
                if(toDo[minors[0]].Count <= 0) toDo.Remove(minors[0]);
            } else
            {
                currentRow++;
                currentColumn = 0;
                minors.RemoveAt(0);
            }
        }
    }
}
