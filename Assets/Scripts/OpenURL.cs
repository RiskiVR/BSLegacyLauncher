using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class OpenURL : MonoBehaviour
{
    public void OpenWebsite()
    {
        Application.OpenURL("https://discord.com/invite/MrwMx5e");
    }

    public void OpenGithub()
    {
        Application.OpenURL("https://github.com/RiskiVR/BSLegacyLauncher/releases/latest");
    }
}
