using UnityEngine;
using System.Net;
using System.Text;
using System.Diagnostics;
using UnityEngine.UI;

public class UpdateCheck : MonoBehaviour
{
    public GameObject UpdateCheckObject;
    public Button UpdateButton;
    public Text ErrorText;
    public GameObject ErrorTextObject;

    public static string version;

    string incomingData = string.Empty;
    string lazyTag;
    string GitHub = "https://api.github.com/repos/RiskiVR/BSLegacyLauncher/releases";

    void Start()
    {
        if (Application.isEditor)
            UpdateButton.interactable = false;

        version = Application.version;
        lazyTag = "\"tag_name\": \"v" + version + "\"";
        // Get latest tag
        GetComponent<TextMesh>().text = "v" + version;
        WebClient web = new WebClient();
        web.Headers["Content-Type"] = "application/json";
        web.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:87.0) Gecko/20100101 Firefox/87.0");
        web.Encoding = Encoding.UTF8;
        incomingData = web.DownloadString(GitHub);

        if (incomingData.Contains(lazyTag))
            UnityEngine.Debug.Log("Versions are matching");
        else
        {
            UnityEngine.Debug.Log("GitHub version is different than internal version.");
            UpdateCheckObject.SetActive(true);
        }
    }
    private void DisplayErrorText(string text)
    {
        // Set to false to restart popup animation DON'T CHANGE
        ErrorTextObject.SetActive(false);
        ErrorTextObject.SetActive(true);
        ErrorText.text = text;
    }
    public void RunUpdater()
    {
        try
        {
            Process.Start("Resources\\BSLLUpdater.exe");
        }
        catch
        {
            DisplayErrorText("UNABLE TO RUN UPDATER");
        }
    }
}
