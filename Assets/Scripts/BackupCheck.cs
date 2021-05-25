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
            if (Directory.Exists("Beat Saber"))
            {
                if (Directory.Exists("Backups"))
                {
                    if (Directory.Exists("Backups/CustomSongs"))
                        check.SetActive(true);
                    if (Directory.Exists("Backups/CustomSabers"))
                        check.SetActive(true);
                    if (Directory.Exists("Backups/CustomLevels"))
                        check.SetActive(true);
                    if (Directory.Exists("Backups/CustomWIPLevels"))
                        check.SetActive(true);
                    if (Directory.Exists("Backups/UserData"))
                        check.SetActive(true);
                }
            }
        }
        void Start()
        {
            if (Directory.Exists("Beat Saber"))
            {
                if (Directory.Exists("Backups"))
                {
                    if (Directory.Exists("Backups/CustomSongs"))
                        check.SetActive(true);
                    if (Directory.Exists("Backups/CustomSabers"))
                        check.SetActive(true);
                    if (Directory.Exists("Backups/CustomLevels"))
                        check.SetActive(true);
                    if (Directory.Exists("Backups/CustomWIPLevels"))
                        check.SetActive(true);
                    if (Directory.Exists("Backups/UserData"))
                        check.SetActive(true);
                }
            }
        }
    }
}
