using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ModDetailsHandler : MonoBehaviour
{
    private static ModDetailsHandler i;
    [SerializeField] TextMeshProUGUI modNameText;
    [SerializeField] TextMeshProUGUI modDescriptionText;
    [SerializeField] Button sourceButton;
    private string sourceUrl;
    void Awake()
    {
        i = this;
        sourceButton.onClick.AddListener(OpenModSource);
    }
    void OpenModSource()
    {
        Application.OpenURL(sourceUrl);
    }
    public static void SetInfo(string modName, string modVersion, string modDescription, string sourceUrl)
    {
        i.modNameText.text = modName;
        i.modDescriptionText.text = modDescription;
        i.sourceUrl = sourceUrl;
        i.sourceButton.interactable = sourceUrl != "null";
    }
}