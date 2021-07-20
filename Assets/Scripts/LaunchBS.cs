using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
public class LaunchBS : MonoBehaviour
{
    public GameObject ErrorTextObject;
    public Text ErrorText;
    public Toggle OculusToggle;
    public Toggle fpfcToggle;
    public Toggle verboseToggle;
    public void LaunchBeatSaber()
    {
        var process = new Process()
        {
            StartInfo = new ProcessStartInfo($"{System.Environment.CurrentDirectory}\\Beat Saber\\Beat Saber.exe", (OculusToggle.isOn ? "-vrmode oculus " : "") + (verboseToggle.isOn ? "--verbose" : "") + (fpfcToggle.isOn ? "fpfc" : ""))
            {
                UseShellExecute = false,
                WorkingDirectory = $"{System.Environment.CurrentDirectory}\\Beat Saber"
            }
        };


        try
        {
            process.StartInfo.Environment["SteamAppId"] = "620980";
            process.Start();
        }
        catch (Exception E)
        {
            if (Directory.Exists("Beat Saber"))
            {
                if (!File.Exists("Beat Saber\\Beat Saber.exe"))
                    ErrorText.text = "BAD INSTALL! PLEASE REINSTALL BEAT SABER";
            }
            else ErrorText.text = "BEAT SABER NOT INSTALLED";
            ErrorTextObject.SetActive(false);
            ErrorTextObject.SetActive(true);
            throw new Exception("Beat Saber Not Installed");
        }
    }
}

