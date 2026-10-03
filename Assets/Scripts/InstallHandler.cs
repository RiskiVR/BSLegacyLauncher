using System;
using System.IO;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class InstallHandler : MonoBehaviour
{
    public GameObject LaunchOptions;
    public TextMeshProUGUI InstallButtonText;
    static InstallHandler i;
    public static bool Installed = false;
    public void Awake()
    {
        i = this;
        InstallCheck();
    }
    public static void InstallCheck()
    {
        Installed = File.Exists(Path.Combine("Beat Saber", "Beat Saber.exe"));
        i.InstallButtonText.text = Installed ? "MANAGE INSTALL" : "DOWNLOAD 0.11.2";
        i.LaunchOptions.SetActive(Installed);
    }
    public static void Uninstall()
    {
        _ = i.UninstallTask();
    }
    private async Task UninstallTask()
    {
        InstallButtonText.text = "UNINSTALLING...";
        try
        {
            if (Directory.Exists("Beat Saber"))
            {
                await Task.Run(() =>
                {
                    Directory.Delete("Beat Saber", true);
                });
            } 
        }
        catch (Exception)
        {
            ErrorText.Display("UNABLE TO DELETE BEAT SABER");
        }
        InstallCheck();
    }
}