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
    public AudioSource ErrorSound;
    public GameObject ErrorTextObject;
    public Text ErrorText;
    public static LaunchBS instace;
    void Awake()
    {
        instace = GetComponent<LaunchBS>();
    }

    private void DisplayErrorText(string text)
    {
        // Set to false to restart popup animation DON'T CHANGE
        ErrorTextObject.SetActive(false);
        ErrorTextObject.SetActive(true);
        ErrorText.text = text;
        ErrorSound.Play();
    }

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
            StartInfo = new ProcessStartInfo
            {
                FileName = InstalledVersionToggle.BSDirectory + "Beat Saber.exe",
                Arguments = "--no-yeet " + (LaunchOptions.vars.oculus ? "-vrmode oculus " : "") + (LaunchOptions.vars.verbose ? "--verbose " : "") + (LaunchOptions.vars.fpfc ? "fpfc " : ""),
                UseShellExecute = false,
                WorkingDirectory = InstalledVersionToggle.BSDirectory,
            }
        };

        if (Process.GetProcessesByName("steam").Length > 0) 
        {
            try
            {
                process.StartInfo.Environment["SteamAppId"] = "620980";
                process.Start();

                if (LaunchOptions.vars.fpfc)
                {
                    try
                    {
                        File.Move("C:\\Program Files (x86)\\Steam\\steamapps\\common\\SteamVR", "C:\\Program Files (x86)\\Steam\\steamapps\\common\\SteamVR.bak");
                        Delayfunc(3, delegate { File.Move("C:\\Program Files (x86)\\Steam\\steamapps\\common\\SteamVR.bak", "C:\\Program Files (x86)\\Steam\\steamapps\\common\\SteamVR"); });
                    }
                    catch
                    {
                        DisplayErrorText("FAILED TO STOP STEAMVR");
                    }
                }

                if (LaunchOptions.vars.verbose)
                {
                    throw new Exception();
                }
            }
            catch (Exception E)
            {
                UnityEngine.Debug.LogError(E.ToString());
                if (LaunchOptions.vars.verbose)
                {
                    LaunchButton.interactable = false;
                    Delayfunc(5, delegate { LaunchButton.interactable = true; });
                    throw new Exception("Opening in Debug mode (Launcher remaining open)");
                }

                if (Directory.Exists(InstalledVersionToggle.BSDirectory))
                {
                    if (!File.Exists(InstalledVersionToggle.BSDirectory + "Beat Saber.exe"))
                        DisplayErrorText("BEAT SABER.EXE NOT FOUND");
                }
                else DisplayErrorText("BEAT SABER NOT INSTALLED");
                throw new Exception("Beat Saber Not Installed");
            }
        }
        else
        {
            DisplayErrorText("STEAM NOT RUNNING");
            throw new Exception("Steam Not Running");
        }
    }
        
}

