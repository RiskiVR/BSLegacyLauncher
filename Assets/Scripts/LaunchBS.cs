using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class LaunchBS : MonoBehaviour
{
    public Animation manageButton;
    private Animation launchButton;
    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(LaunchBeatSaber);
        launchButton = GetComponent<Animation>();
    }

    void LaunchBeatSaber()
    {
        Process process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = Path.Combine("Beat Saber", "Beat Saber.exe"),
                Arguments = "--no-yeet " + (LaunchOptions.oculus ? "-vrmode oculus " : "")
                                         + (LaunchOptions.verbose ? "--verbose " : ""),
                UseShellExecute = false,
                WorkingDirectory = "Beat Saber",
            }
        };

        if (Process.GetProcessesByName("steam").Length > 0) 
        {
            try
            {
                process.StartInfo.Environment["SteamAppId"] = "620980";
                process.Start();
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
                if (Directory.Exists("Beat Saber"))
                {
                    if (!File.Exists(Path.Combine("Beat Saber", "Beat Saber.exe"))) 
                        ErrorText.Display("BEAT SABER EXECUTABLE NOT FOUND");
                }
                else ErrorText.Display("BEAT SABER NOT INSTALLED");
                return;
            }

            manageButton.Play();
            launchButton.Play();
            return;
        }
        ErrorText.Display("STEAM NOT RUNNING");
        Debug.LogError("Steam Not Running");
    }

    public void ExitTrigger()
    {
        Application.Quit();
    }
}