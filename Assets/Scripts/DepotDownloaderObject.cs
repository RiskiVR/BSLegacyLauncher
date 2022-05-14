using Assets.Scripts;
using DepotDownloader;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using Yggdrasil.Logging;
using static SteamKit2.SteamUser;
using System;
using System.Diagnostics;

public class DepotDownloaderObject : MonoBehaviour
{
    public static DepotDownloaderObject instance;

    [Header("Other scripts")]
    public DiscordController DiscordController;

    [Header("Scene Objects")]
    public InputField Username;
    public InputField Password;
    public Button BackButton;
    public Button ExitButton;
    public Button UpdateButton;
    public GameObject InputFields;
    public GameObject StartButtonObject;
    public TextMesh DebugText;

    [Header("Random Elements We Need")]
    public GameObject VersionText1;
    public GameObject VersionText2;
    public GameObject ErrorTextObject;
    public Text ErrorText;
    public TextMesh DownloadingText;
    public Animator DownloadingTextAnim;
    public GameObject DownloadedText;
    public Animator DownloadedTextAnim;
    public Animator PreAllocatingTextAnim;
    public Animator LoginTextAnim;
    public TextMesh DownloadDetailText;
    public GameObject ProgressBar;
    public Image InnerProgressBar;
    public Text InstalledVersionText;
    public GameObject InstalledVersionObject;
    public Button LocalGameFilesButton;
    public Button InstallIPAButton;
    public GameObject InvalidPasswordTips;

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

    private bool updateDownloading = false;
    private bool isDownloading = false;
    public LogOnDetails details;
    private SteamLoginResponse request = SteamLoginResponse.NONE;
    private string localCurrentDownloadStep;
    private float downloadPercentage;
    private float downloadSmoothened;
    private bool requestSteamGuardPopUp = false;
    private bool requestLoginPrompt = false;

    bool downloadFinished = false;
    Process dd = null;
    public int ddStartedTimes = 0;



    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        Log.Logger.AddTarget(new UnityConsoleLogTarget());

        Username.onEndEdit.AddListener(value =>
        {
            if (Input.GetKey(KeyCode.Return)) Password.Select();
        });

        Password.onEndEdit.AddListener(value =>
        {
            if(Input.GetKey(KeyCode.Return)) LoginPressed();
        });
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if(Password.isFocused) Username.Select();
            else Password.Select();
            
        }

        if (isDownloading) SetDownloadingLayout();

        if (requestLoginPrompt)
        {
            Log.Info("Login prompt requested");
            SetLoginObjects(true);
            requestLoginPrompt = false;
        }

        if (requestSteamGuardPopUp)
        {
            Log.Info("SteamGuard prompt requested");
            requestSteamGuardPopUp = false;
            createSteamCodePopup("Check the code from your 2FA or Steam Guard");
        }

        if(downloadFinished)
        {
            OnMainThreadDownloadCompleted();
            downloadFinished = false;
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
                DisplayErrorText("INVALID PASSWORD");
                InvalidPasswordTips.SetActive(true);
                break;
            case SteamLoginResponse.PASSWORDUNSET:
                DisplayErrorText("INVALID CREDENTIALS");
                break;
            case SteamLoginResponse.RATELIMIT:
                DisplayErrorText("LOGIN RATELIMIT EXCEEDED");
                break;
            case SteamLoginResponse.INVALIDLOGINAUTHCODE:
                DisplayErrorText("INVALID CODE");
                break;
            case SteamLoginResponse.EXPIREDLOGINAUTHCODE:
                DisplayErrorText("CODE EXPIRED, PLEASE TRY AGAIN");
                break;
            case SteamLoginResponse.NOTENOUGHSPACE:
                DisplayErrorText("NOT ENOUGH SPACE ON DISK");
                break;
            case SteamLoginResponse.EXCEPTION:
                SetLoginObjects(false);
                DisplayErrorText("AN UNKNOWN ERROR OCCURED, TRY AGAIN");
                break;
            case SteamLoginResponse.BEATSABERNOTOWNED:
                DisplayErrorText("BEAT SABER IS NOT PURCHASED ON THIS ACCOUNT");
                break;
            case SteamLoginResponse.CONNECTIONFAILED:
                DisplayErrorText("STEAM CONNECTION FAILED, TRY AGAIN LATER");
                break;
            case SteamLoginResponse.NETNOTINSTALLED:
                request = SteamLoginResponse.NONE;
                DisplayErrorText("PLEASE INSTALL .NET 6.0.7");
                break;
            case SteamLoginResponse.PATHDENIED:
                DisplayErrorText("PATH IS DENIED");
                break;
            case SteamLoginResponse.NOTAUTHORIZED:
                DisplayErrorText("NOT AUTHORIZED TO DOWNLOAD DEPOT.\nTHIS ERROR MAY SHOW EVEN IF YOU OWN BEAT SABER");
                break;
            case SteamLoginResponse.PREALLOCATING:
                LoginTextAnim.runtimeAnimatorController = TextDismiss;
                PreAllocatingTextAnim.runtimeAnimatorController = TextEnter;
                break;
        }
        request = SteamLoginResponse.NONE;
    }

    private void SetDownloadingLayout()
    {
        GameObject.Destroy(LoadingActiveInstance);
        SetLoginObjects(false);
        VersionText1.SetActive(false);
        VersionText2.SetActive(false);
        DownloadingText.text = $"Downloading {VersionVar.instance.version}...";
        DownloadingTextAnim.runtimeAnimatorController = TextEnter;
        PreAllocatingTextAnim.runtimeAnimatorController = TextDismiss;
        ProgressBar.SetActive(true);
        ExitButton.interactable = false;
        UpdateButton.interactable = false;
        InstalledVersionObject.SetActive(false);

        isDownloading = false;
    }

    private void DisplayErrorText(string error)
    {
        // Set to false to restart popup animation DON'T CHANGE
        ErrorTextObject.SetActive(false);
        ErrorTextObject.SetActive(true);
        ErrorText.text = error;
    }

    public void LoginPressed()
    {
        if (!File.Exists("Resources\\DepotDownloader\\DepotDownloader.exe"))
        {
            DisplayErrorText("DEPOTDOWNLOADER NOT FOUND");
            Log.Info("DepotDownloader doesn't exist in /Resources/");
            return;
        }
        if (string.IsNullOrEmpty(Username.text) || string.IsNullOrEmpty(Password.text))
        {
            requestLoginPrompt = true;
            request = SteamLoginResponse.PASSWORDUNSET;
            return;
        }

        if (!Directory.Exists("Beat Saber Legacy Launcher_Data\\Saved\\steamcreds")) Directory.CreateDirectory("Beat Saber Legacy Launcher_Data\\Saved\\steamcreds");
        File.WriteAllText("Beat Saber Legacy Launcher_Data\\Saved\\steamcreds\\username.txt", $"{Username.text}");

        Log.Info("Triggered login");

        SetLoginObjects(false);

        InvalidPasswordTips.SetActive(false);

        details = new LogOnDetails()
        {
            Username = Username.text,
            Password = Password.text
        };

        StartDownload();
    }

    private void OnDepotNotOwned()
    {
        Log.Error("This user doesn't own the requested repo!");
        request = SteamLoginResponse.BEATSABERNOTOWNED;
        requestLoginPrompt = true;
        Directory.Delete(InstalledVersionToggle.GetBSDirectory(VersionVar.instance.version), true);
    }

    private void OnMainThreadDownloadCompleted()
    {
        downloadPercentage = 100;
        BackButton.interactable = true;
        DownloadDetailText.text = "Download completed! Ready to Launch!";
        DownloadedText.gameObject.SetActive(true);
        DownloadingTextAnim.runtimeAnimatorController = TextDismiss;
        DownloadedTextAnim.runtimeAnimatorController = TextEnter;
        ExitButton.interactable = true;
        UpdateButton.interactable = true;
        InstallIPAButton.interactable = true;
        LocalGameFilesButton.interactable = true;
        InstalledVersionToggle.SetBSVersion(VersionVar.instance.version);
        if(ComputersVars.useApertureDeskJob)
        {
            File.WriteAllText(InstalledVersionToggle.BSDirectory + "Beat Saber.exe", ":D This is real Beat Saber");
        }
        // InstalledVersionObject.SetActive(false); this just hides it forever
        DiscordController.DownloadProgress = "Download Finished";
        DiscordController.DownloadUpdate();
        Destroy(LoadingActiveInstance);
    }

    private void OnProgressUpdate(string current, float percentage)
    {
        isDownloading = true;
        localCurrentDownloadStep = current;
        downloadPercentage = percentage;
        updateDownloading = true;
    }

    private void createSteamCodePopup(string description)
    {
        Destroy(LoadingActiveInstance);
        SteamCodePopup popup = GameObject.Instantiate(PopupPrefab, PopupAnchor.transform).GetComponent<SteamCodePopup>();
        popup.callback = steamCodePopupCallback;
        popup.Description.text = description;
        popup.gameObject.SetActive(true);
        VersionText1.SetActive(false);
        VersionText2.SetActive(false);
        InputFields.SetActive(false);
        BackButton.interactable = false;
        StartButtonObject.gameObject.SetActive(false);
    }

    private void steamCodePopupCallback(string code)
    {
        LoadingActiveInstance = GameObject.Instantiate(LoadingPopup, LoadingAnchor.transform);
        VersionText1.SetActive(true);
        VersionText2.SetActive(true);
        details.TwoFactorCode = code;
        Log.Debug("Entering code " + details.TwoFactorCode + " into DD");
        dd.StandardInput.WriteLine(details.TwoFactorCode);
    }

    private void SetLoginObjects(bool state)
    {
        BackButton.interactable = state;
        StartButtonObject.gameObject.SetActive(state);
        InputFields.SetActive(state);

        if (!state && !isDownloading) LoadingActiveInstance = GameObject.Instantiate(LoadingPopup, LoadingAnchor.transform);
        else if (LoadingActiveInstance != null) GameObject.Destroy(LoadingActiveInstance);

    }

    public static void MoveDirectory(string source, string target)
    {
        var stack = new Stack<Folders>();
        stack.Push(new Folders(source, target));

        while (stack.Count > 0)
        {
            var folders = stack.Pop();
            if(!Directory.Exists(folders.Target)) Directory.CreateDirectory(folders.Target);
            foreach (var file in Directory.GetFiles(folders.Source, "*.*"))
            {
                string targetFile = Path.Combine(folders.Target, Path.GetFileName(file));
                if (File.Exists(targetFile)) File.Delete(targetFile);
                File.Move(file, targetFile);
            }

            foreach (var folder in Directory.GetDirectories(folders.Source))
            {
                stack.Push(new Folders(folder, Path.Combine(folders.Target, Path.GetFileName(folder))));
            }
        }
        Directory.Delete(source, true);
    }

    public void StartDownload()
    {
        ErrorTextObject.SetActive(false);
        Version selectedVersion = VersionButtonController.versions.First(x => x.BSVersion.Equals(VersionVar.instance.version));
        Log.Info($"You selected version {selectedVersion.BSVersion} : {selectedVersion.BSManifest}");
        DiscordController.BSVersion = $"{selectedVersion.BSVersion}";

        if (Directory.Exists(InstalledVersionToggle.GetBSDirectory(selectedVersion.BSVersion))) Directory.Delete(InstalledVersionToggle.GetBSDirectory(selectedVersion.BSVersion), true);
        ProcessStartInfo ddInfo = new ProcessStartInfo
        {
            FileName = Environment.CurrentDirectory + "\\Resources\\DepotDownloader\\DepotDownloader.exe",
            // Don't forget to change depot back to Beat Saber (-depot 620981 -app 620980), Aperture Desk job (-depot 1902492 -app 1902490)
            Arguments = "-username \"" + details.Username + "\" -password \"" + details.Password.Replace("\"", "\\\"") + "\" -manifest " + ulong.Parse(selectedVersion.BSManifest) + " -dir \"" + InstalledVersionToggle.GetBSDirectory(selectedVersion.BSVersion).TrimEnd('\\') + "\" -depot 620981 -app 620980",
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };
        if (ComputersVars.useApertureDeskJob)
            ddInfo.Arguments = "-username \"" + details.Username + "\" -password \"" + details.Password.Replace("\"", "\\\"") + "\" -manifest " + ulong.Parse(selectedVersion.BSManifest) + " -dir \"" + InstalledVersionToggle.GetBSDirectory(selectedVersion.BSVersion).TrimEnd('\\') + "\" -depot 1902492 -app 1902490";
        Thread t = new Thread(() =>
        {
            try
            {
                ddStartedTimes++;
                int myDDProcess = ddStartedTimes;
                int lines = 0;
                downloadFinished = false;
                dd = Process.Start(ddInfo);
                Log.Debug("Started dd");
                //dd.WaitForInputIdle();
                string line = "";
                while (!dd.StandardOutput.EndOfStream && myDDProcess == ddStartedTimes)
                {
                    if (line.EndsWith(Environment.NewLine) || line.Contains(" code "))
                    {
                        Log.Debug(line);
                        ProcessLine(line);
                        line = "";
                        lines++;
                    }
                    line += (char)dd.StandardOutput.Read();
                }

                if (lines <= 0)
                {
                    request = SteamLoginResponse.NETNOTINSTALLED;
                    Process.Start("https://link.bslegacy.com/dotNET_6-0-3");
                    requestLoginPrompt = true;
                }
            }
            catch (Exception ex)
            {
                request = SteamLoginResponse.EXCEPTION;
                requestLoginPrompt = true;
                Log.Error(ex.ToString());
            }
        });
        t.Start();
    }

    public void ProcessLine(string line)
    {
        if (line.Contains("This account is protected"))
        {
            // Invoke prompt here
            requestSteamGuardPopUp = true;
        }
        if (line.Contains("LogOn requires a username and password to be set in"))
        {
            requestLoginPrompt = true;
            request = SteamLoginResponse.PASSWORDUNSET;
            Log.Debug("PASSWORDUNSET");
            return;
        }
        if (line.Contains("InvalidPassword"))
        {
            requestLoginPrompt = true;
            request = SteamLoginResponse.INVALIDPASSWORD;
            Log.Debug("INVALIDPASSWORD");
            return;
        }
        if (line.Contains("404 for depot manifest") || line.Contains("App") && line.Contains("is not available from this account"))
        {
            requestLoginPrompt = true;
            OnDepotNotOwned();
            Log.Debug("DEPOTNOTOWNED");
            return;
        }
        if(line.Contains("404 for depot manifest"))
        {
            requestLoginPrompt = true;
            request = SteamLoginResponse.NOTAUTHORIZED;
            OnDepotNotOwned();
        }
        if (line.Contains("Got depot key"))
        {
            // Depot is owned
        }
        if (line.Contains("Connection to Steam failed"))
        {
            requestLoginPrompt = true;
            try
            {
                dd.Kill();
            } catch { }
            
            request = SteamLoginResponse.CONNECTIONFAILED;

            Log.Debug("CONNECTIONFAILED");
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
                dd.Kill();
            }
            catch { }
            request = SteamLoginResponse.INVALIDLOGINAUTHCODE;
            StartDownload();

            Log.Debug("INVALIDLOGINAUTHCODE");
            return;
        }
        if (line.Contains("ExpiredLoginAuthCode"))
        {
            try
            {
                dd.Kill();
            }
            catch { }
            request = SteamLoginResponse.EXPIREDLOGINAUTHCODE;
            StartDownload();

            Log.Debug("EXPIREDLOGINAUTHCODE");
            return;
        }
        if (line.Contains("There is not enough space"))
        {
            requestLoginPrompt = true;
            request = SteamLoginResponse.NOTENOUGHSPACE;
            Log.Debug("There is not enough space on the disk");
            return;
        }
        if (line.Contains("Access to the path is denied"))
        {
            requestLoginPrompt = true;
            request = SteamLoginResponse.PATHDENIED;
            Log.Debug("Access to the path is denied");
            return;
        }
        if (line.Contains("401"))
        {
            requestLoginPrompt = true;
            request = SteamLoginResponse.ERROR401;
            Log.Debug("Error 401 Encountered");
            return;
        }
        if (line.Contains("Pre-allocating"))
        {
            request = SteamLoginResponse.PREALLOCATING;
            Log.Debug("Pre-allocating Disk Space for Beat Saber");
            return;
        }
        if (line.Contains("Got session token"))
        {
            //Logged in
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
                float per = float.Parse(percentage);
                OnProgressUpdate(String.Format("{0:0.0}", per) + "%", per);
            }
            catch (Exception ex)
            {
                Log.Debug("Fuck you DD" + ex.ToString());
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
    NOTAUTHORIZED
}
