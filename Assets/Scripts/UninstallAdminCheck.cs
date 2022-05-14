using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UninstallAdminCheck : MonoBehaviour
{
    public GameObject Prompt;

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            Prompt.SetActive(true);
            gameObject.SetActive(false);
        });
    }

}
