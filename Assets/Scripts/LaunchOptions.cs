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

    // Start is called before the first frame update
    void Start()
    {
        if (!File.Exists(jsonLocation)) Save();
        try
        {
            vars = JsonConvert.DeserializeObject<LaunchOptionsVars>(File.ReadAllText(jsonLocation));
        }
        catch
        {
            Save();
        }
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

    public void Save()
    {
        if (!Directory.Exists(Path.GetDirectoryName(jsonLocation))) Directory.CreateDirectory(Path.GetDirectoryName(jsonLocation));
        File.WriteAllText(jsonLocation, JsonConvert.SerializeObject(vars));
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
