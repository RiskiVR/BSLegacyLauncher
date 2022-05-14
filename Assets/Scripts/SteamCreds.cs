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
        if (File.Exists("Beat Saber Legacy Launcher_Data\\Saved\\steamcreds\\password.txt"))
            File.Delete("Beat Saber Legacy Launcher_Data\\Saved\\steamcreds\\password.txt");

        if (Directory.Exists("Beat Saber Legacy Launcher_Data\\Saved\\steamcreds"))
        {
            string SavedUser;
            SavedUser = File.ReadAllText("Beat Saber Legacy Launcher_Data\\Saved\\steamcreds\\username.txt");
            User.text = $"{SavedUser}";

            Toggle.isOn = true;
        }
        else
        {
            Debug.Log("No Saved steamcreds found.");
        }
    }
}
