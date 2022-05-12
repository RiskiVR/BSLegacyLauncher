using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
public class LaunchBS : MonoBehaviour
{
    public Button LaunchButton;

    [Header("Error Text Objects")]
    public GameObject ErrorTextObject;
    public Text ErrorText;

    private void Delayfunc(float delay, Action action)
    {
        StartCoroutine(Delay(delay, action));
    }

    private static IEnumerator Delay(float delay, Action action)
    {
        yield return new WaitForSeconds(delay);
        action.Invoke();
    }

    public void LaunchBeatSaber()
    {
        var process = new Process()
        {
            StartInfo = new ProcessStartInfo($"{InstalledVersionToggle.BSDirectory}Beat Saber.exe", (LaunchOptions.vars.oculus ? "-vrmode oculus " : "") + (LaunchOptions.vars.verbose ? "--verbose " : "") + (LaunchOptions.vars.fpfc ? "fpfc " : ""))
            {
                UseShellExecute = false,
                WorkingDirectory = InstalledVersionToggle.BSDirectory
            }
        };

        if (Process.GetProcessesByName("steam").Length > 0) 
        {
            try
            {
                process.StartInfo.Environment["SteamAppId"] = "620980";
                process.Start();

                if (LaunchOptions.vars.verbose)
                {
                    throw new Exception();
                }
            }
            catch (Exception E)
            {
                if (LaunchOptions.vars.verbose)
                {
                    LaunchButton.interactable = false;
                    Delayfunc(5, delegate { LaunchButton.interactable = true; });
                    throw new Exception("Opening in Debug mode (Launcher remaining open)");
                }

                if (Directory.Exists(InstalledVersionToggle.BSDirectory))
                {
                    if (!File.Exists(InstalledVersionToggle.BSDirectory + "Beat Saber.exe"))
                        ErrorText.text = "BAD INSTALL! PLEASE REINSTALL BEAT SABER";
                }
                else ErrorText.text = "BEAT SABER NOT INSTALLED";
                ErrorTextObject.SetActive(false);
                ErrorTextObject.SetActive(true);
                throw new Exception("Beat Saber Not Installed");
            }
        }
        else
        {
            ErrorText.text = "STEAM NOT RUNNING";
            ErrorTextObject.SetActive(false);
            ErrorTextObject.SetActive(true);
            throw new Exception("Steam Not Running");
        }
    }
        
}

