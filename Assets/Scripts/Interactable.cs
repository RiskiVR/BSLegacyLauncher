using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Interactable : MonoBehaviour
{
    public Button[] ButtonArray;
    public void InteractableTrue()
    {
        foreach (Button button in ButtonArray)
        {
            button.interactable = true;
        }
    }

    public void InteractableFalse()
    {
        foreach (Button button in ButtonArray)
        {
            button.interactable = false;
        }
    }
}