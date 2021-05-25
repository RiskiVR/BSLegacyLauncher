using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;
using DepotDownloader;
using SteamKit2;
using UnityEngine.UI;

public class VersionVar : MonoBehaviour
{
    public static VersionVar instance;

    public TextMesh VersionText;

    [HideInInspector]
    public string version;

    public void ListVersion(string Version)
    {
        this.version = Version;
        VersionText.text = Version;
        VersionText.gameObject.SetActive(false);
        VersionText.gameObject.SetActive(true);
    }

    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
