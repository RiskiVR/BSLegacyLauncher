using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoLaunchManager : MonoBehaviour
{
    public LaunchBS LaunchBS;
    public void Awake()
    {
        if (Environment.CommandLine.Contains("--LaunchBS"))
        {
            LaunchBS.LaunchBeatSaber();
            Application.Quit();
        }
    }
}
