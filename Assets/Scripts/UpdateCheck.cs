using UnityEngine;
using System.Net;
using System.Text;

public class UpdateCheck : MonoBehaviour
{
    public GameObject UpdateCheckObject;
    static string version = "1.3.4";
    string lazyTag = "\"tag_name\": \"v" + version + "\"";
    string incomingData = string.Empty;
    string GitHub = "https://api.github.com/repos/RiskiVR/BSLegacyLauncher/releases";

    // Awake is called before Start
    //void Awake() => getLatestVersion();

    // Start is called before the first frame update
    void Start()
    {
        WebClient web = new WebClient();
        web.Headers["Content-Type"] = "application/json";
        web.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:87.0) Gecko/20100101 Firefox/87.0");
        web.Encoding = Encoding.UTF8;
        incomingData = web.DownloadString(GitHub);

        if (incomingData.Contains(lazyTag))
            Debug.Log("Versions are matching");
        else
        {
            Debug.Log("GitHub version is different than internal version.");
            UpdateCheckObject.SetActive(true);
        }
    }
}
