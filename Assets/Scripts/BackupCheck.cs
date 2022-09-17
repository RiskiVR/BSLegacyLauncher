using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

namespace CheckBackup
{
    public class BackupCheck : MonoBehaviour
    {
        public GameObject check;
        public void Check()
        {
            check.SetActive(false);
            if (!Directory.Exists(InstalledVersionToggle.BSDirectory)) return;
            if (Directory.Exists(InstalledVersionToggle.BaseDirectory + $"Backups/Beat Saber {InstalledVersionToggle.BSVersion}/CustomSabers"))
                check.SetActive(true);
            if (Directory.Exists(InstalledVersionToggle.BaseDirectory + $"Backups/Beat Saber {InstalledVersionToggle.BSVersion}/UserData"))
                check.SetActive(true);
        }
        void Start()
        {
            Check();
        }
    }
}
