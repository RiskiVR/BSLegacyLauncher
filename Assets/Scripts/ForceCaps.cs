using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ForceCaps : MonoBehaviour
{
    public InputField Input;
    void Update()
    {
        string text = Input.text;
        if (text != Input.text.ToUpper())
        {
            Input.text = Input.text.ToUpper();
        }
    }
}
