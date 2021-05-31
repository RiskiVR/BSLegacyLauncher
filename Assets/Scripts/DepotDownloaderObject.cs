using Assets.Scripts;
using DepotDownloader;
using Newtonsoft.Json;
using SteamKit2;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using Yggdrasil.Logging;
using static SteamKit2.SteamUser;

public class DepotDownloaderObject : MonoBehaviour
{
    public static DepotDownloaderObject instance;

    public static List<Version> versions = new List<Version>();

    [Header("Scene Objects")]
    public InputField Username;
    public InputField Password;
    public Button StartButton;
    public Button BackButton;
    public Button ExitButton;
    public Button UpdateButton;
    public GameObject InputFields;
    public GameObject StartButtonObject;

    [Header("Random Elements We Need")]
    public GameObject VersionText1;
    public GameObject VersionText2;
    public GameObject ErrorTextObject;
    public Text ErrorText;
    public TextMesh DownloadingText;
    public Animator DownloadingTextAnim;
    public GameObject DownloadedText;
    public Animator DownloadedTextAnim;
    public Animator LoginTextAnim;
    public TextMesh DownloadDetailText;
    public GameObject ProgressBar;
    public Image InnerProgressBar;
    public Button SelectVersionButton;
    public Text InstalledVersionText;
    public GameObject InstalledVersionObject;
    public Animator InstalledVersionAnim;

    [Header("Animators")]
    public RuntimeAnimatorController TextDismiss;
    public RuntimeAnimatorController TextEnter;
    public RuntimeAnimatorController InstalledVer;
    
    [Header ("Popup Handlers")]
    public GameObject PopupPrefab;
    public GameObject LoadingPopup;
    public GameObject PopupAnchor;
    public GameObject LoadingAnchor;
    [HideInInspector]
    public GameObject LoadingActiveInstance;

    private Thread steamThread;

    private bool isLoggedIn = false;
    private bool attemptedConnection = false;
    private bool loginInProgress = false;
    private bool isDialogDisplayed = false;
    private bool updateDownloading = false;
    private bool isDownloading = false;
    private bool hasSetDownloading = false;
    private bool hasFinishedDownloading = false;
    private Steam3Session session;
    private LogOnDetails details;
    private SteamLoginResponse request = SteamLoginResponse.NONE;
    private string steamCode;
    private string localCurrentDownloadStep;
    private float downloadPercentage;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        Log.Logger.AddTarget(new UnityConsoleLogTarget());

        string versionList = File.ReadAllText("Resources/BSVersions.json");
        versions = JsonConvert.DeserializeObject<List<Version>>(versionList);

        ContentDownloader.ProgressUpdateHandlers += OnProgressUpdate;
        ContentDownloader.DownloadCompleted += OnDownloadCompleted;
    }

    // Update is called once per frame
    void Update()
    {
        if(!loginInProgress && !StartButton.interactable && !isDialogDisplayed && !hasSetDownloading)
        {
            SetLoginObjects(true);
        }

        if (isDownloading)
        {
            SetDownloadingLayout();
        }

        if (updateDownloading)
        {
            updateDownloading = false;
            InnerProgressBar.fillAmount = downloadPercentage;
            DownloadDetailText.text = localCurrentDownloadStep;
        }

        if (hasFinishedDownloading)
        {
            hasFinishedDownloading = false;
            OnMainThreadDownloadCompleted();
        }

        if (!request.Equals(SteamLoginResponse.NONE))
        {
            switch (request)
            {
                case SteamLoginResponse.STEAMGUARD:
                    request = SteamLoginResponse.NONE;
                    createSteamCodePopup("Enter the Steam Guard code sent to your email address", SteamLoginResponse.STEAMGUARD);
                    break;
                case SteamLoginResponse.TWOFACTOR:
                    request = SteamLoginResponse.NONE;
                    createSteamCodePopup("Enter the two factor code from your 2FA device", SteamLoginResponse.TWOFACTOR);
                    break;
                case SteamLoginResponse.INVALID_PASSWORD:
                    request = SteamLoginResponse.NONE;
                    DisplayErrorText("INVALID PASSWORD");
                    break;
                case SteamLoginResponse.PASSWORDUNSET:
                    request = SteamLoginResponse.NONE;
                    DisplayErrorText("INVALID CREDENTIALS");
                    break;
                case SteamLoginResponse.RATELIMIT:
                    request = SteamLoginResponse.NONE;
                    DisplayErrorText("LOGIN RATELIMIT EXCEEDED");
                    break;
                case SteamLoginResponse.INVALIDLOGINAUTHCODE:
                    request = SteamLoginResponse.NONE;
                    DisplayErrorText("INVALID CODE");
                    createSteamCodePopup("Enter the Steam Guard code sent to your email address", SteamLoginResponse.STEAMGUARD);
                    break;
                case SteamLoginResponse.EXPIREDLOGINAUTHCODE:
                    request = SteamLoginResponse.NONE;
                    DisplayErrorText("CODE EXPIRED, PLEASE TRY AGAIN");
                    createSteamCodePopup("Enter the Steam Guard code sent to your email address", SteamLoginResponse.STEAMGUARD);
                    break;
                case SteamLoginResponse.EXCEPTION:
                    request = SteamLoginResponse.NONE;
                    DisplayErrorText("AN UNKNOWN ERROR OCCURED, TRY AGAIN");
                    break;
                case SteamLoginResponse.BEATSABERNOTOWNED:
                    request = SteamLoginResponse.NONE;
                    DisplayErrorText("BEAT SABER IS NOT PURCHASED ON THIS ACCOUNT!");
                    break;
            }
        }
    }

    private void SetDownloadingLayout()
    {
        SetLoginObjects(false);
        VersionText1.SetActive(false);
        VersionText2.SetActive(false);
        DownloadingText.text = $"Downloading {VersionVar.instance.version}...";
        DownloadingTextAnim.runtimeAnimatorController = TextEnter;
        LoginTextAnim.runtimeAnimatorController = TextDismiss;
        ProgressBar.SetActive(true);
        ExitButton.interactable = false;
        UpdateButton.interactable = false;

        isDownloading = false;
        hasSetDownloading = true;
    }

    private void DisplayErrorText(string error)
    {
        ErrorTextObject.SetActive(false);
        ErrorTextObject.SetActive(true);
        ErrorText.text = error;
    }

    public void LoginPressed()
    {
        Log.Info("Triggered login again?");

        loginInProgress = true;

        SetLoginObjects(false);

        if (!string.IsNullOrEmpty(Username.text) && !string.IsNullOrEmpty(Password.text))
        {
            AccountSettingsStore.LoadFromFile("Resources\\steamcreds");

            if(!attemptedConnection)
                details = new LogOnDetails() { Username = Username.text, Password = Password.text, ShouldRememberPassword = true, LoginKey = "", LoginID = 0x534B32 };

            session = new Steam3Session(details);

            session.SteamGuardHandlers += SteamGuardTriggered;
            session.TwoFactorHandlers += TwoFactorTriggered;
            session.BadLoginHandlers += BadLoginResponse;
            session.ConnectionFailedHandlers += ConnectionFailed;
            session.LoginSuccessHandlers += LoginSuccess;
            session.DepotNotOwnedHandlers += OnDepotNotOwned;
            session.DepotOwnedHandlers += OnDepotIsOwned;

            steamThread = new Thread(RunSteamLoginThread);
            steamThread.Start();
        }
        else
        {
            loginInProgress = false;
        }
    }

    private void OnDepotNotOwned()
    {
        Log.Error("This user doesn't own the requested repo!");
        request = SteamLoginResponse.BEATSABERNOTOWNED;
        hasSetDownloading = false;
        attemptedConnection = false;
        ContentDownloader.ShutdownSteam3();
        Directory.Delete("Beat Saber", true);
    }

    private void OnDepotIsOwned()
    {
        Log.Info("Steam account owns Beat Saber!");
        isDownloading = true;
    }

    private void OnDownloadCompleted()
    {
        Log.Info("Download completed!");
        hasFinishedDownloading = true;
    }

    private void OnMainThreadDownloadCompleted()
    {
        SelectVersionButton.interactable = false;
        ContentDownloader.ShutdownSteam3();
        BackButton.interactable = true;
        DownloadDetailText.text = "Download completed! Ready to Launch!";
        DownloadedText.gameObject.SetActive(true);
        DownloadingTextAnim.runtimeAnimatorController = TextDismiss;
        DownloadedTextAnim.runtimeAnimatorController = TextEnter;
        ExitButton.interactable = true;
        UpdateButton.interactable = true;
        File.WriteAllText("BeatSaberVersion.txt", $"{$"{VersionVar.instance.version}"}");
        InstalledVersionObject.SetActive(true);
        InstalledVersionText.text = $"Currently Installed: {File.ReadAllText("BeatSaberVersion.txt")}";
        InstalledVersionAnim.runtimeAnimatorController = InstalledVer;
    }

    private void OnProgressUpdate(string current, float percentage)
    {
        localCurrentDownloadStep = current;
        downloadPercentage = percentage;
        updateDownloading = true;
    }

    private void createSteamCodePopup(string description, SteamLoginResponse request)
    {
        isDialogDisplayed = true;
        SteamCodePopup popup = GameObject.Instantiate(PopupPrefab, PopupAnchor.transform).GetComponent<SteamCodePopup>();
        popup.callback = steamCodePopupCallback;
        popup.Description.text = description;
        popup.request = request;
        popup.gameObject.SetActive(true);
        VersionText1.SetActive(false);
        VersionText2.SetActive(false);
        InputFields.SetActive(false);
        BackButton.interactable = false;
        StartButtonObject.gameObject.SetActive(false);
    }

    private void steamCodePopupCallback(string code, SteamLoginResponse request)
    {
        isDialogDisplayed = false;
        VersionText1.SetActive(true);
        VersionText2.SetActive(true);
        if(request.Equals(SteamLoginResponse.STEAMGUARD))
            details.AuthCode = code;
        if (request.Equals(SteamLoginResponse.TWOFACTOR))
            details.TwoFactorCode = code;
        LoginPressed();
    }

    private void RunSteamLoginThread()
    {
        isLoggedIn = ContentDownloader.InitializeSteam3(session);

        loginInProgress = false;

        StartDownload();
    }

    private void SetLoginObjects(bool state)
    {
        StartButton.interactable = state;
        BackButton.interactable = state;
        StartButtonObject.gameObject.SetActive(state);
        InputFields.SetActive(state);
        if (!state && !isDownloading)
            LoadingActiveInstance = GameObject.Instantiate(LoadingPopup, LoadingAnchor.transform);

        else
        {
            if (LoadingActiveInstance != null)
                GameObject.Destroy(LoadingActiveInstance);
        }

    }

    public void SteamGuardTriggered()
    {
        attemptedConnection = true;
        Log.Debug("SteamGuard Triggered");
        request = SteamLoginResponse.STEAMGUARD;
    }

    public void TwoFactorTriggered()
    {
        attemptedConnection = true;
        Log.Debug("Steam Account has Two Factor Auth enabled!");
        request = SteamLoginResponse.TWOFACTOR;
    }

    public void BadLoginResponse(string message)
    {
        attemptedConnection = true;
        Log.Debug($"Bad Login! RESP: {message}");
    }

    public void ConnectionFailed(string message)
    {
        attemptedConnection = false;
        request = SteamLoginResponse.EXCEPTION;
        Log.Error($"Connection Failed! RESP: {message}");

        if (message.Contains("TwoFactorCodeMismatch"))
        {
            //Rerun 2FA
            request = SteamLoginResponse.TWOFACTOR;
            attemptedConnection = true;
        }

        if (message.Contains("InvalidPassword"))
        {
            request = SteamLoginResponse.INVALID_PASSWORD;
        }

        if (message.Contains("PasswordUnset"))
        {
            Log.Info("Steam Account doesn't exist");
            request = SteamLoginResponse.PASSWORDUNSET;
        }

        if (message.Contains("InvalidLoginAuthCode"))
        {
            request = SteamLoginResponse.INVALIDLOGINAUTHCODE;
            attemptedConnection = true;
        }

        if (message.Contains("ExpiredLoginAuthCode"))
        {
            request = SteamLoginResponse.EXPIREDLOGINAUTHCODE;
            attemptedConnection = true;
        }

        if (message.Contains("RateLimitExceeded"))
        {
            request = SteamLoginResponse.RATELIMIT;
        }
    }

    public void LoginSuccess()
    {
        Log.Info("Successfully connected to Steam3 service!");
    }

    public void StartDownload()
    {
        if (isLoggedIn)
        {
            hasSetDownloading = true;
            Version selectedVersion = versions.First(x => x.BSVersion.Equals(VersionVar.instance.version));
            Log.Info($"You selected version {selectedVersion.BSVersion}:{selectedVersion.BSManifest}");
            ContentDownloader.Config.InstallDirectory = "Beat Saber";
            ContentDownloader.DownloadAppAsync(620980, 620981, ulong.Parse(selectedVersion.BSManifest), "public");
        }
    }
}

enum SteamLoginResponse
{
    NONE,
    TWOFACTOR,
    STEAMGUARD,
    INVALID_PASSWORD,
    PASSWORDUNSET,
    RATELIMIT,
    EXCEPTION,
    INVALIDLOGINAUTHCODE,
    EXPIREDLOGINAUTHCODE,
    BEATSABERNOTOWNED
}
