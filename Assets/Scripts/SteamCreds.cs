using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class SteamCreds : MonoBehaviour
{
    public InputField User;
    public InputField Pass;
    public Toggle Toggle;
    public static string steamuser;
    void Awake()
    {
        steamuser = Path.Combine(Application.persistentDataPath, "steamuser.txt");
    }
    void Start()
    {
        if (File.Exists(steamuser))
        {
            User.text = File.ReadAllText(steamuser);
            Toggle.isOn = true;
        }
        else
        {
            Debug.Log("No Saved steamcreds found.");
        }
    }
}