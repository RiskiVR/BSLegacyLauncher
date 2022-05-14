using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UninstallAdminCheck : MonoBehaviour
{
    public GameObject Prompt;

    public GameObject ErrorTextObject;
    public Text ErrorText;

    public void DisplayErrorText(string text)
    {
        ErrorText.text = text;
        ErrorTextObject.SetActive(false);
        ErrorTextObject.SetActive(true);
    }
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            if (false)//!AdvancedButtons.IsUserAnAdmin())
            {
                DisplayErrorText("REQUIRES ADMIN PERMISSIONS");
                Prompt.SetActive(false);
                gameObject.SetActive(true);
            } else
            {
                Prompt.SetActive(true);
                gameObject.SetActive(false);
            }
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
