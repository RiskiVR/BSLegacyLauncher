using Assets.Scripts;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using static SteamKit2.SteamUser;
using System;
using System.Diagnostics;
using System.Globalization;
using TMPro;
using Debug = UnityEngine.Debug;

public class DepotDownloaderHandler : MonoBehaviour
{
    [Header("Other scripts")]
    public DiscordController DiscordController;

    [Header("Scene Objects")]
    public InputField Username;
    public InputField Password;
    public Button BackButton;
    public Button ExitButton;
    public GameObject InputFields;
    public GameObject StartButtonObject;

    [Header("Random Elements We Need")]
    public TextMeshProUGUI DownloadDetailText;
    public GameObject ProgressBar;
    public Image InnerProgressBar;
    public GameObject InvalidPasswordTips;

    [Header("Audio Sources")] 
    public AudioSource StartSound;
    
    [Header ("Popup Handlers")]
    public GameObject SteamguardPopup;
    public GameObject LoadingPopup;

    private bool updateDownloading = false;
    private bool isDownloading = false;
    private bool downloadingUIActive;
    public LogOnDetails details;
    private SteamLoginResponse request = SteamLoginResponse.NONE;
    private string localCurrentDownloadStep;
    private float downloadPercentage;
    private float downloadSmoothened;
    private bool requestSteamGuardPopUp = false;
    private bool requestLoginPrompt = false;
    private bool isLoggedIn = false;
    bool downloadFinished = false;

    Process ddProcess;

    void Start()
    {
        Username.onEndEdit.AddListener(value =>
        {
            if (Input.GetKey(KeyCode.Return)) Password.Select();
        });

        Password.onEndEdit.AddListener(value =>
        {
            if(Input.GetKey(KeyCode.Return)) LoginPressed();
        });
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if(Password.isFocused) Username.Select();
            else Password.Select();
            
        }

        if (isDownloading) SetDownloadingLayout();

        if (isLoggedIn)
        {
            HeaderText.Display("Logged in successfully");
            SetLoginObjects(false);
            isLoggedIn = false;
        }

        if (requestSteamGuardPopUp)
        {
            //CreateSteamCodePopup("Approve your login from Steam Guard\nor enter 2FA code");
            HeaderText.Display("Use the Steam Mobile App to confirm your sign in...");
            requestSteamGuardPopUp = false;
        }

        if (requestLoginPrompt)
        {
            Debug.Log("Login prompt requested");
            HeaderText.Display("Sign in");
            SetLoginObjects(true);
            requestLoginPrompt = false;
        }

        if (downloadFinished)
        {
            OnMainThreadDownloadCompleted();
            downloadFinished = false;
            downloadingUIActive = false;
        }

        downloadSmoothened = downloadSmoothened + downloadPercentage / 50 - downloadSmoothened / 50;
        InnerProgressBar.fillAmount = downloadSmoothened / 100;

        if (updateDownloading)
        {
            updateDownloading = false;
            DownloadDetailText.text = $"Downloading... {localCurrentDownloadStep}";

            DiscordController.DownloadProgress = $"Downloading... {localCurrentDownloadStep}";
            DiscordController.DownloadUpdate();
        }

        switch (request)
        {
            case SteamLoginResponse.NONE:
                break;
            case SteamLoginResponse.INVALIDPASSWORD:
                ErrorText.Display("INVALID PASSWORD");
                InvalidPasswordTips.SetActive(true);
                break;
            case SteamLoginResponse.PASSWORDUNSET:
                ErrorText.Display("INVALID CREDENTIALS");
                break;
            case SteamLoginResponse.RATELIMIT:
                ErrorText.Display("LOGIN RATELIMIT EXCEEDED");
                break;
            case SteamLoginResponse.INVALIDLOGINAUTHCODE:
                ErrorText.Display("INVALID CODE");
                break;
            case SteamLoginResponse.EXPIREDLOGINAUTHCODE:
                ErrorText.Display("CODE EXPIRED, PLEASE TRY AGAIN");
                break;
            case SteamLoginResponse.NOTENOUGHSPACE:
                ErrorText.Display("NOT ENOUGH SPACE ON DISK");
                break;
            case SteamLoginResponse.EXCEPTION:
                ErrorText.Display("AN UNKNOWN ERROR OCCURED, TRY AGAIN");
                break;
            case SteamLoginResponse.BEATSABERNOTOWNED:
                ErrorText.Display("BEAT SABER IS NOT PURCHASED ON THIS ACCOUNT");
                break;
            case SteamLoginResponse.CONNECTIONFAILED:
                ErrorText.Display("STEAM CONNECTION FAILED, TRY AGAIN LATER");
                break;
            case SteamLoginResponse.NETNOTINSTALLED:
                request = SteamLoginResponse.NONE;
                ErrorText.Display("PLEASE INSTALL .NET 6.0");
                break;
            case SteamLoginResponse.PATHDENIED:
                ErrorText.Display("PATH IS DENIED");
                break;
            case SteamLoginResponse.UNAUTHORIZED:
                ErrorText.Display("UNAUTHORIZED");
                break;
            case SteamLoginResponse.PREALLOCATING:
                HeaderText.Display("PRE-ALLOCATING...");
                break;
        }
        request = SteamLoginResponse.NONE;
    }

    private void SetDownloadingLayout()
    {
        if (downloadingUIActive) return;
        SetLoginObjects(false);
        HeaderText.Display($"Downloading 0.11.2...");
        ProgressBar.SetActive(true);
        ExitButton.interactable = false;
        downloadingUIActive = true;
    }

    public void LoginPressed()
    {
        if (!File.Exists(Path.Combine("Resources", "DepotDownloader", "DepotDownloader.exe"))) 
        {
            ErrorText.Display("DEPOTDOWNLOADER NOT FOUND");
            Debug.Log("DepotDownloader doesn't exist in Resources");
            return;
        }
        if (string.IsNullOrEmpty(Username.text) || string.IsNullOrEmpty(Password.text))
        {
            requestLoginPrompt = true;
            request = SteamLoginResponse.PASSWORDUNSET;
            return;
        }

        if (!File.Exists(SteamCreds.steamuser)) File.Create(SteamCreds.steamuser);
        StreamWriter streamWriter = new StreamWriter(SteamCreds.steamuser);
        streamWriter.Write(Username.text);
        streamWriter.Close();

        Debug.Log("Triggered login");

        SetLoginObjects(false);

        InvalidPasswordTips.SetActive(false);

        details = new LogOnDetails
        {
            Username = Username.text,
            Password = Password.text
        };

        StartDownload();
    }

    private void OnDepotNotOwned()
    {
        Debug.LogError("Doesn't own Beat Saber!");
        request = SteamLoginResponse.BEATSABERNOTOWNED;
        requestLoginPrompt = true;
        Directory.Delete("Beat Saber", true);
    }

    private void OnMainThreadDownloadCompleted()
    {
        isLoggedIn = false;
        isDownloading = false;
        downloadPercentage = 100;
        BackButton.interactable = true;
        DownloadDetailText.text = "Download completed! Ready to Launch!";
        HeaderText.Display("Finished downloading");
        ExitButton.interactable = true;
        DiscordController.DownloadProgress = "Download Finished";
        DiscordController.DownloadUpdate();
        StartSound.Play();
    }

    private void OnProgressUpdate(string current, float percentage)
    {
        isDownloading = true;
        localCurrentDownloadStep = current;
        downloadPercentage = percentage;
        updateDownloading = true;
    }

    private void CreateSteamCodePopup(string description)
    {
        LoadingPopup.SetActive(false);
        SteamguardPopup.SetActive(true);
        SteamCodePopup popup = SteamguardPopup.GetComponent<SteamCodePopup>();
        popup.callback = SteamCodePopupCallback;
        popup.Description.text = description;
        InputFields.SetActive(false);
        BackButton.interactable = false;
        StartButtonObject.gameObject.SetActive(false);
    }

    private void SteamCodePopupCallback(string code)
    {
        LoadingPopup.SetActive(true);
        details.TwoFactorCode = code;
        Debug.Log("Entering code " + details.TwoFactorCode + " into DD");
        ddProcess.StandardInput.WriteLine(details.TwoFactorCode);
    }

    private void SetLoginObjects(bool state)
    {
        BackButton.interactable = state;
        StartButtonObject.gameObject.SetActive(state);
        InputFields.SetActive(state);
        //SteamguardPopup.SetActive(false);
        if (isDownloading) LoadingPopup.SetActive(false);
        else LoadingPopup.SetActive(!state);
    }

    private void StartDownload()
    {
        StartSound.Play();
        ErrorText.Hide();

        if (Directory.Exists("Beat Saber")) Directory.Delete("Beat Saber");
        ProcessStartInfo ddInfo = new ProcessStartInfo
        {
            FileName = Path.Combine("Resources", "DepotDownloader", "DepotDownloader.exe"),
            Arguments = $"-username \"{details.Username}\" -password \"{details.Password}\" -manifest 2707973953401625222 -dir \"Beat Saber\" -depot 620981 -app 620980",
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };
        Thread downloadThread = new Thread(() =>
        {
            try
            {
                int stdLines = 0;
                string stdLine;
                downloadFinished = false;
                ddProcess = Process.Start(ddInfo);
                Debug.Log("Started DepotDownloader Process");
                while (!ddProcess.StandardOutput.EndOfStream)
                {
                    stdLine = ddProcess.StandardOutput.ReadLine();
                    stdLines++;
                    Debug.Log(stdLine);
                    ProcessLine(stdLine);
                }

                if (stdLines <= 0)
                {
                    request = SteamLoginResponse.NETNOTINSTALLED;
                    Process.Start("https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/sdk-6.0.408-windows-x64-installer");
                    requestLoginPrompt = true;
                }
            }
            catch (Exception ex)
            {
                requestLoginPrompt = true;
                request = SteamLoginResponse.EXCEPTION;
                Debug.LogError(ex.ToString());
            }
        });
        downloadThread.Start();
    }

    void ProcessLine(string line)
    {
        if (line.Contains("STEAM GUARD")) // This line isn't showing up for some reason
        {
            Debug.Log("SteamGuard prompt requested");
            requestSteamGuardPopUp = true;
        }
        if (line.Contains("Logging") && line.Contains("into"))
        {
            Debug.Log("Logging into Steam... SteamGuard prompt requested");
            requestSteamGuardPopUp = true;
        }
        if (line == " Done!")
        {
            Debug.Log("Logged into Steam!");
            isLoggedIn = true;
        }
        if (line == "Unable to get steam3 credentials.")
        {
            requestLoginPrompt = true;
            request = SteamLoginResponse.UNAUTHORIZED;
            return;
        }
        if (line.Contains("LogOn requires a username and password to be set in"))
        {
            requestLoginPrompt = true;
            Debug.Log("Nothing Entered");
            return;
        }
        if (line.Contains("Unset"))
        {
            requestLoginPrompt = true;
            request = SteamLoginResponse.PASSWORDUNSET;
            Debug.Log("PASSWORDUNSET");
            return;
        }
        if (line.Contains("InvalidPassword"))
        {
            requestLoginPrompt = true;
            request = SteamLoginResponse.INVALIDPASSWORD;
            Debug.Log("INVALIDPASSWORD");
            return;
        }
        if (line.Contains("404 for depot manifest") || line.Contains("App") && line.Contains("is not available from this account"))
        {
            requestLoginPrompt = true;
            OnDepotNotOwned();
            Debug.Log("DEPOTNOTOWNED");
            return;
        }
        if(line.Contains("401 for depot manifest"))
        {
            requestLoginPrompt = true;
            request = SteamLoginResponse.UNAUTHORIZED;
            Directory.Delete("Beat Saber", true);
            return;
        }
        if (line.Contains("Got depot key"))
        {
            // Depot is owned
            Debug.Log("Owns Beat Saber");
        }
        if (line.Contains("Connection to Steam failed"))
        {
            requestLoginPrompt = true;
            try
            {
                ddProcess.Kill();
            } catch { }
            
            request = SteamLoginResponse.CONNECTIONFAILED;

            Debug.LogError("CONNECTIONFAILED");
            return;
        }
        if (line.Contains("RateLimitExceeded"))
        {
            requestLoginPrompt = true;
            request = SteamLoginResponse.RATELIMIT;
        }
        if (line.Contains("InvalidLoginAuthCode"))
        {
            try
            {
                ddProcess.Kill();
            }
            catch { }
            request = SteamLoginResponse.INVALIDLOGINAUTHCODE;
            StartDownload();

            Debug.LogError("INVALIDLOGINAUTHCODE");
            return;
        }
        if (line.Contains("ExpiredLoginAuthCode"))
        {
            try
            {
                ddProcess.Kill();
            }
            catch { }
            request = SteamLoginResponse.EXPIREDLOGINAUTHCODE;
            StartDownload();

            Debug.LogError("EXPIREDLOGINAUTHCODE");
            return;
        }
        if (line.Contains("There is not enough space"))
        {
            requestLoginPrompt = true;
            request = SteamLoginResponse.NOTENOUGHSPACE;
            Debug.LogError("There is not enough space on the disk");
            return;
        }
        if (line.Contains("Access to the path is denied"))
        {
            requestLoginPrompt = true;
            request = SteamLoginResponse.PATHDENIED;
            Debug.LogError("Access to the path is denied");
            return;
        }
        if (line.Contains("Pre-allocating"))
        {
            request = SteamLoginResponse.PREALLOCATING;
            Debug.Log("Pre-allocating Disk Space for Beat Saber");
            return;
        }
        if (line.Contains("Total downloaded:"))
        {
            // Download finished (maybe only partially but idc)
            downloadFinished = true;
        }
        if (line.Contains("%"))
        {
            string percentage = line.Split('%')[0];
            try
            {
                float per = float.Parse(percentage.Replace(",", "."), CultureInfo.InvariantCulture);
                OnProgressUpdate(String.Format("{0:0.0}", per) + "%", per);
            }
            catch (Exception ex)
            {
                Debug.Log("Fuck you DD" + ex.ToString());
            }
        }
    }
}

public class Folders
{
    public string Source { get; private set; }
    public string Target { get; private set; }

    public Folders(string source, string target)
    {
        Source = source;
        Target = target;
    }
}

enum SteamLoginResponse
{
    NONE,
    TWOFACTOR,
    STEAMGUARD,
    INVALIDPASSWORD,
    PASSWORDUNSET,
    RATELIMIT,
    EXCEPTION,
    INVALIDLOGINAUTHCODE,
    EXPIREDLOGINAUTHCODE,
    BEATSABERNOTOWNED,
    CONNECTIONFAILED,
    NETNOTINSTALLED,
    NOTENOUGHSPACE,
    PREALLOCATING,
    PATHDENIED,
    UNAUTHORIZED
}
