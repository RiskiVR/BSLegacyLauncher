using System;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ModElement : MonoBehaviour
{
    [Header("Mod Info")]
    public bool selected;
    public bool required;
    public bool installed;
    public bool versionMismatch;
    public string modName, modVersion, modLatestVersion, modDescription, modDownloadUrl, modSourceUrl;
    [Range(0, 100)] public int downloadProgress;

    [Header("Element References")]
    [SerializeField] Button modElementButton;
    [SerializeField] Button checkboxButton;
    [SerializeField] Image checkboxFill, checkboxCheck, checkboxOutline, downloadProgressMask, downloadBar;
    [SerializeField] TextMeshProUGUI modNameText, modVersionText, modLatestVersionText, modDescriptionText;
    
    readonly Color checkboxFillColor = new(0.2313726f, 0.509804f, 1);
    float downloadSmoothened;
    
    void Awake()
    {
        checkboxButton.onClick.AddListener(ModSelected);
        modElementButton.onClick.AddListener(ModDetails);
    }
   
    void Update()
    {
        downloadSmoothened = Mathf.Lerp(downloadSmoothened, downloadProgress / 100f, 0.1f);
        downloadProgressMask.fillAmount = downloadSmoothened;
    }

    [ContextMenu("Update Info")]
    public void UpdateInfo()
    {
        if (installed)
        {
            try
            {
                versionMismatch = int.Parse(modLatestVersion.Replace(".", string.Empty)) != int.Parse(modVersion.Replace(".", string.Empty));
            }
            catch (Exception)
            {
                Debug.LogWarning($"Failed to get version for {modName}. Returning no version.");
            }
        }
        modVersion = modVersion ?? "-";
        checkboxButton.interactable = !required;
        checkboxFill.color = required ? Color.white / 2 : checkboxFillColor;
        checkboxCheck.color = required ? Color.white / 2: Color.white;
        checkboxOutline.color = required ? Color.white / 2 : Color.white;
        checkboxFill.gameObject.SetActive(selected);
        modNameText.text = modName;
        modVersionText.text = modVersion;
        modVersionText.color = modVersion != "-" ? versionMismatch
            ? new Color(1, 0.2f, 0.2f)
            : new Color(0.1f, 1, 0.5f) : Color.white;
        modLatestVersionText.text = modLatestVersion;
        modDescriptionText.text = modDescription;
    }

    public void SetInfo(bool selected, string modName, string modVersion, string modLatestVersion, string modDescription, string modDownloadUrl, string modSourceUrl, bool required, bool installed)
    {
        this.selected = selected;
        this.modName = modName;
        this.modVersion = modVersion ?? "-";
        this.modLatestVersion = modLatestVersion;
        this.modDescription = modDescription;
        this.modDownloadUrl = modDownloadUrl;
        this.modSourceUrl = modSourceUrl;
        this.required = required;
        this.installed = installed;
        UpdateInfo();
    }
    
    public void FinishDownload()
    {
        downloadProgress = 100;
        downloadBar.DOFade(0, 1).OnComplete(() =>
        {
            downloadBar.color = new Color(1, 1, 1, 0.2f);
            downloadProgress = 0;
            downloadSmoothened = 0;
        });
    }
    
    void ModSelected()
    {
        selected = !selected;
        checkboxFill.gameObject.SetActive(selected);
        ClickSound.Play();
    }

    void ModDetails()
    {
        ModListHandler.ShowModDetails(modDownloadUrl.Split("/").Last().Replace("%20", " "), installed && !required);
        ModDetailsHandler.SetInfo(modName, modVersion, modDescription, modSourceUrl);
        ClickSound.Play();
    }
}