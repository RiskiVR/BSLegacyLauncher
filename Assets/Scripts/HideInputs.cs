using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class HideInputs : MonoBehaviour
{
    public InputField mainInputField;

    public void ToggleInputType()
    {
        if (this.mainInputField != null)
        {
            if (this.mainInputField.contentType == InputField.ContentType.Password)
            {
                this.mainInputField.contentType = InputField.ContentType.Standard;
            }
            else
            {
                this.mainInputField.contentType = InputField.ContentType.Password;
            }

            this.mainInputField.ForceLabelUpdate();
        }
    }
}
