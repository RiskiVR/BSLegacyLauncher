using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MainButtons : MonoBehaviour
{
    [SerializeField] Button versionButton, launchButton;
    [SerializeField] UnityEvent onDownloadClick, onManageClick, onLaunchClick;

    void Start()
    {
        versionButton.onClick.AddListener(DownloadManageButtonClicked);
        launchButton.onClick.AddListener(LaunchButtonClicked);
    }

    void DownloadManageButtonClicked()
    {
        if (InstallHandler.Installed)
        {
            onManageClick.Invoke();
        }
        else
        {
            onDownloadClick.Invoke();
        }
    }

    void LaunchButtonClicked()
    {
        if (InstallHandler.Installed)
        {
            onLaunchClick.Invoke();
        }
    }
}
