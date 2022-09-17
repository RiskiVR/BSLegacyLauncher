using ComputerUtils.CommandLine;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public GameObject Pixel;
    public GameObject PixelModpackButton;
    public Toggle ambientToggle;


    public static Setting vars = new Setting();
    const string settingsLocation = "Beat Saber Legacy Launcher_Data/Settings/settings.json";
    // Start is called before the first frame update
    void Start()
    {
        CommandLineCommandContainer commands = new CommandLineCommandContainer(Environment.GetCommandLineArgs());

        if (commands.HasArgument("--Pixel")) // ignores case
        {
            Pixel.SetActive(true);
            PixelModpackButton.SetActive(true);
        }

        if (!File.Exists(InstalledVersionToggle.BaseDirectory + settingsLocation)) Save();
        try
        {
            vars = JsonConvert.DeserializeObject<Setting>(File.ReadAllText(InstalledVersionToggle.BaseDirectory + settingsLocation));
        }
        catch
        {
            Save();
        }
        ambientToggle.isOn = vars.ambient;
        ambientToggle.onValueChanged.AddListener(value =>
        {
            vars.ambient = value;
            Save();
        });
    }

    public void Save()
    {
        if (!Directory.Exists(Path.GetDirectoryName(InstalledVersionToggle.BaseDirectory + settingsLocation))) Directory.CreateDirectory(Path.GetDirectoryName(InstalledVersionToggle.BaseDirectory + settingsLocation));
        File.WriteAllText(InstalledVersionToggle.BaseDirectory + settingsLocation, JsonConvert.SerializeObject(vars));
    }
}

public class Setting
{
    public bool ambient { get; set; } = true;
}