using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

public class Startup : MonoBehaviour
{
    void Awake()
    {
        if (Environment.GetCommandLineArgs().Contains("--launch"))
        {
            LaunchBeatSaber();
            Application.Quit();
        }

        Screen.SetResolution(Mathf.RoundToInt(1280 * Screen.dpi / 96f), Mathf.RoundToInt(800 * Screen.dpi / 96f), FullScreenMode.Windowed);
        SceneManager.LoadScene(1);
    }

    void LaunchBeatSaber()
    {
        Process process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = Path.Combine("Beat Saber", "Beat Saber.exe"),
                Arguments = "--no-yeet " + string.Join(" ", Environment.GetCommandLineArgs()),
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
                        Debug.LogError("BEAT SABER EXECUTABLE NOT FOUND");
                }
                Debug.LogError("BEAT SABER NOT INSTALLED");
            }
        }
    }
}
