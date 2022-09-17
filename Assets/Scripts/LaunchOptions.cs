using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class LaunchOptions : MonoBehaviour
{
    public Toggle oculusToggle;
    public Toggle FPFCToggle;
    public Toggle VerboseToggle;

    public static LaunchOptionsVars vars = new LaunchOptionsVars();
    const string jsonLocation = "Beat Saber Legacy Launcher_Data/Settings/launchoptions.json";

    public static void LoadSettings()
    {
        if (!File.Exists(InstalledVersionToggle.BaseDirectory + jsonLocation)) Save();
        try
        {
            vars = JsonConvert.DeserializeObject<LaunchOptionsVars>(File.ReadAllText(InstalledVersionToggle.BaseDirectory + jsonLocation));
        }
        catch
        {
            Save();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        LoadSettings();
        oculusToggle.isOn = vars.oculus;
        oculusToggle.onValueChanged.AddListener(value =>
        {
            vars.oculus = value;
            Save();
        });
        FPFCToggle.isOn = vars.fpfc;
        FPFCToggle.onValueChanged.AddListener(value =>
        {
            vars.fpfc = value;
            Save();
        });
        VerboseToggle.isOn = vars.verbose;
        VerboseToggle.onValueChanged.AddListener(value =>
        {
            vars.verbose = value;
            Save();
        });
    }

    public static void Save()
    {
        if (!Directory.Exists(Path.GetDirectoryName(InstalledVersionToggle.BaseDirectory + jsonLocation))) Directory.CreateDirectory(Path.GetDirectoryName(InstalledVersionToggle.BaseDirectory + jsonLocation));
        File.WriteAllText(InstalledVersionToggle.BaseDirectory + jsonLocation, JsonConvert.SerializeObject(vars));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public class LaunchOptionsVars
{
    public bool oculus { get; set; } = false;
    public bool fpfc { get; set; } = false;
    public bool verbose { get; set; } = false;
}
