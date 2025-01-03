using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetActive : MonoBehaviour
{
    public GameObject Object;
    public void SetActiveTrue()
    {
        Object.SetActive(true);
    }

    public void SetActiveFalse()
    {
        Object.SetActive(false);
    }
}
