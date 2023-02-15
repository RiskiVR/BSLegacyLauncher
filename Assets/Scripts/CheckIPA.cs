using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class CheckIPA : MonoBehaviour
{
    public GameObject IPA4Button;
    public GameObject IPA3Button;
    public GameObject UninstallIPAButton;
    public void Start()
    {
        if (File.Exists(InstalledVersionToggle.BSDirectory + "Beat Saber.exe"))
        {
            IPA();
        }
        else
        {
            UninstallIPAButton.SetActive(false);
        }
    }
    public void IPAInstalled()
    {
        UninstallIPAButton.SetActive(true);
        IPA3Button.SetActive(false);
        IPA4Button.SetActive(false);
    }

    public void IPANotInstalled()
    {
        UninstallIPAButton.SetActive(false);
        IPA3Button.SetActive(true);
        IPA4Button.SetActive(true);
    }
    
    public void IPA()
    {
        string IPADir = $"{InstalledVersionToggle.BSDirectory}IPA";
        string IPAexe = $"{InstalledVersionToggle.BSDirectory}IPA.exe";
        
        if (Directory.Exists(IPADir) && File.Exists(IPAexe)) {
            Debug.Log("IPA Exists");
            IPAInstalled();
            return;
        }
        Debug.Log("IPA Does Not Exist");
        IPANotInstalled();
    }
}
