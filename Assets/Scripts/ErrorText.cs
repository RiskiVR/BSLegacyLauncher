using UnityEngine;
using UnityEngine.UI;

public class ErrorText : MonoBehaviour
{
    private static ErrorText i;
    private Animation anim;
    private Text text;
    public Material errorMaterial;
    public Material infoMaterial;
    public enum MessageType
    {
        ERROR,
        INFO
    }
    private void Awake()
    {
        i = this;
        text = GetComponent<Text>();
        anim = GetComponent<Animation>();
    }
    public static void Display(string text, MessageType type = 0)
    {
        i.anim.Stop();
        i.anim.Play(type == 0 ? "ErrorText" : "ErrorText Old");
        i.text.text = text;
        switch (type)
        {
            case MessageType.ERROR: i.text.material = i.errorMaterial; break;
            case MessageType.INFO: i.text.material = i.infoMaterial; break;
        }
    }
    public static void Hide()
    {
        i.anim.Stop();
        i.text.text = "";
    }
}