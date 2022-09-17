using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusMute : MonoBehaviour
{
    void Update()
    {
        if (Application.isFocused)
        {
            GetComponent<AudioSource>().mute = (!Settings.vars.ambient);
        }
        else
        {
            GetComponent<AudioSource>().mute = true;
        }
    }
}
