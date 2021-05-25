using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
public class LaunchBS : MonoBehaviour
{
    public GameObject ErrorTextObject;
    public Text ErrorText;
    public Toggle OculusToggle;
    public Toggle fpfcToggle;
    public void LaunchBeatSaber()
    {
        var process = new Process()
        {
            StartInfo = new ProcessStartInfo($"{System.Environment.CurrentDirectory}\\Beat Saber\\Beat Saber.exe", (OculusToggle.isOn ? "-vrmode oculus " : "") + (fpfcToggle.isOn ? "fpfc" : ""))
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
            ErrorText.text = "BEAT SABER NOT INSTALLED";
            ErrorTextObject.SetActive(false);
            ErrorTextObject.SetActive(true);
            throw new Exception();
        }
    }
}

