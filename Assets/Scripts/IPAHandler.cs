using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class IPAHandler : MonoBehaviour
{
    static IPAHandler i;
    public Button InstallIPAButton;
    public Button UninstallIPAButton;
    void Awake()
    {
        i = this;
        InstallIPAButton.onClick.AddListener(InstallIPA);
        UninstallIPAButton.onClick.AddListener(UninstallIPA);
    }
    void OnEnable()
    {
        if (InstallHandler.Installed)
        {
            CheckIPA();
        }
        else
        {
            UninstallIPAButton.gameObject.SetActive(false);
        }
    }
    void SetButtons(bool ipa)
    {
        UninstallIPAButton.gameObject.SetActive(ipa);
        InstallIPAButton.gameObject.SetActive(!ipa);
    }
    void CheckIPA()
    {
        string IPADir = Path.Combine("Beat Saber", "IPA");
        string IPAexe = Path.Combine("Beat Saber", "IPA.exe");
        i.SetButtons(Directory.Exists(IPADir) && File.Exists(IPAexe));
    }
    void InstallIPA()
    {
        Process process = new Process();
        process.StartInfo = new ProcessStartInfo
        {
            WorkingDirectory = "Beat Saber",
            FileName = "IPA.exe",
            Arguments = "\"Beat Saber.exe\""
        };
        try
        {
            DirectoryUtils.Copy(Path.Combine("Resources", "BSIPA-Legacy"), "Beat Saber", true);
            CheckIPA();
        }
        catch
        {
            ErrorText.Display("IPA ALREADY INSTALLED");
            throw new Exception("IPA Already Installed");
        }

        process.Start();
        ErrorText.Display("LEGACY IPA INSTALLED", ErrorText.MessageType.INFO);
    }
    void UninstallIPA()
    {
        Process process = new Process();
        process.StartInfo = new ProcessStartInfo
        {
            WorkingDirectory = "Beat Saber",
            FileName = "IPA.exe",
            Arguments = "--revert --nowait"
        };
        try
        {
            if (Directory.Exists("Beat Saber"))
            {
                string IPADir = Path.Combine("Beat Saber", "IPA");
                string IPAexe = Path.Combine("Beat Saber", "IPA.exe");
                string MonoCecildll = Path.Combine("Beat Saber", "Mono.Cecil.dll");

                process.Start();
                process.WaitForExit(5000);
                try
                {
                    if (Directory.Exists(IPADir))
                    {
                        Directory.Delete(IPADir, true);
                        File.Delete(IPAexe);
                    }

                    if (File.Exists(MonoCecildll))
                        File.Delete(MonoCecildll);

                    ErrorText.Display("IPA UNINSTALLED", ErrorText.MessageType.INFO);
                    CheckIPA();
                }
                catch
                {
                    ErrorText.Display("FAILED TO DELETE IPA FILES");
                }
            }
        } 
        catch
        {
            ErrorText.Display("IPA NOT INSTALLED");
        }
    }
}