using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class SteamCreds : MonoBehaviour
{
    public InputField User;
    public InputField Pass;
    public Toggle Toggle;
    void Start()
    {
        if (Directory.Exists("Beat Saber Legacy Launcher_Data\\Saved\\steamcreds"))
        {
            string SavedUser;
            SavedUser = File.ReadAllText("Beat Saber Legacy Launcher_Data\\Saved\\steamcreds\\username.txt");
            User.text = $"{SavedUser}";

            string SavedPass;
            SavedPass = File.ReadAllText("Beat Saber Legacy Launcher_Data\\Saved\\steamcreds\\password.txt");
            Pass.text = $"{SavedPass}";

            Toggle.isOn = true;
        }
        else
        {
            Debug.Log("No Saved steamcreds found.");
        }
    }
}
