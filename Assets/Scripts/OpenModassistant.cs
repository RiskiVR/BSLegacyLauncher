using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenModassistant : MonoBehaviour
{
    public void OpenModassistantProgram()
    {
        System.Diagnostics.Process.Start(InstalledVersionToggle.BaseDirectory + "Resources/ModAssistant.exe");
    }
}
