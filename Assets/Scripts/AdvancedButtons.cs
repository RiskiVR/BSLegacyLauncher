using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.Security.Principal;
using System.Runtime.InteropServices;
using Yggdrasil.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Net;
using System.IO.Compression;
using System.Net.Http;
using SteamKit2.Internal;
using Debug = UnityEngine.Debug;

[Serializable]
public class SymLinkLocations
{
    public List<string> folders = new List<string>();
    public bool customLevels = false;
    public bool customWipLevels = false;
    public bool customSongs = false;
    public bool dlcs = false;

    public void SaveToFile(string Path)
    {
        StreamWriter FileStream = new StreamWriter(Path, false);
        FileStream.Write(JsonUtility.ToJson(this));
        FileStream.Close();
    }
    public static SymLinkLocations LoadFile(string Path)
    {
        StreamReader Reader = new StreamReader(Path);
        string Value = Reader.ReadToEnd();
        Reader.Close();
        return JsonUtility.FromJson<SymLinkLocations>(Value);
    }
}
    public class AdvancedButtons : MonoBehaviour
{
    [Header("Text")]
    public Text ErrorText;
    public Text FeedbackText;
    public Text InfoText;
    
    [Header("Text Objects")]
    public GameObject ErrorTextObject;
    public GameObject FeedbackTextObject;
    public GameObject InfoTextObject;
    public GameObject InstallingTextObject;
    
    [Header("Button Objects")]
    public GameObject IPA4Button;
    public GameObject IPA3Button;
    public GameObject UninstallIPAButton;
    
    [Header("Buttons")]
    public Button RelinkFoldersButton;
    
    [Header("Toggles & Fields")]
    public Toggle CustomLevelsToggle;
    public Toggle CustomWIPLevelsToggle;
    public Toggle CustomSongsToggle;
    public Toggle DLCToggle;
    public Toggle OtherToggle;
    public InputField OtherField;

    [Header("Audio & Others")] 
    public AudioSource ErrorSound;
    public AudioSource FeedbackSound;
    
    public const string jsonLocation = "Beat Saber Legacy Launcher_Data/Settings/LinkedFolders.json";
    public string settingsLocation = "Beat Saber Legacy Launcher_Data/Settings";
    public static SymLinkLocations locations = new SymLinkLocations();
    public static void Save()
    {
        if (!Directory.Exists(Path.GetDirectoryName(InstalledVersionToggle.BaseDirectory + jsonLocation))) Directory.CreateDirectory(Path.GetDirectoryName(InstalledVersionToggle.BaseDirectory + jsonLocation));
        File.WriteAllText(InstalledVersionToggle.BaseDirectory + jsonLocation, JsonConvert.SerializeObject(locations));
    }

    public static void LoadSettings()
    {
        if (!File.Exists(InstalledVersionToggle.BaseDirectory + jsonLocation)) Save();
        try
        {
            locations = JsonConvert.DeserializeObject<SymLinkLocations>(File.ReadAllText(InstalledVersionToggle.BaseDirectory + jsonLocation));
        }
        catch
        {
            Save();
        }
    }
    private void DisplayErrorText(string text)
    {
        // Set to false to restart popup animation DON'T CHANGE
        FeedbackTextObject.SetActive(false);
        ErrorTextObject.SetActive(false);
        ErrorTextObject.SetActive(true);
        ErrorText.text = text;
        ErrorSound.Play();
    }
    private void DisplayFeedbackText(string text)
    {
        // Set to false to restart popup animation DON'T CHANGE
        ErrorTextObject.SetActive(false);
        FeedbackTextObject.SetActive(false);
        FeedbackTextObject.SetActive(true);
        FeedbackText.text = text;
        FeedbackSound.Play();
    }
    private void DisplayInfoText(string text)
    {
        // Set to false to restart popup animation DON'T CHANGE
        InfoTextObject.SetActive(false);
        InfoTextObject.SetActive(true);
        InfoText.text = text;
    }
    public void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
    {
        // Get the subdirectories for the specified directory.
        DirectoryInfo dir = new DirectoryInfo(sourceDirName);

        if (!dir.Exists)
        {
            throw new DirectoryNotFoundException(
                "Source directory does not exist or could not be found: "
                + sourceDirName);
        }

        DirectoryInfo[] dirs = dir.GetDirectories();

        // If the destination directory doesn't exist, create it.       
        Directory.CreateDirectory(destDirName);

        // Get the files in the directory and copy them to the new location.
        FileInfo[] files = dir.GetFiles();
        foreach (FileInfo file in files)
        {
            string tempPath = Path.Combine(destDirName, file.Name);
            file.CopyTo(tempPath, false);
        }

        // If copying subdirectories, copy them and their contents to new location.
        if (copySubDirs)
        {
            foreach (DirectoryInfo subdir in dirs)
            {
                string tempPath = Path.Combine(destDirName, subdir.Name);
                DirectoryCopy(subdir.FullName, tempPath, copySubDirs);
            }
        }
    }
    public void MoveDirectory(string source, string target)
    {
        var sourcePath = source.TrimEnd('\\', ' ');
        var targetPath = target.TrimEnd('\\', ' ');
        var files = Directory.EnumerateFiles(sourcePath, "*", SearchOption.AllDirectories)
                             .GroupBy(s => Path.GetDirectoryName(s));
        foreach (var folder in files)
        {
            var targetFolder = folder.Key.Replace(sourcePath, targetPath);
            Directory.CreateDirectory(targetFolder);
            foreach (var file in folder)
            {
                var targetFile = Path.Combine(targetFolder, Path.GetFileName(file));
                if (File.Exists(targetFile)) File.Delete(targetFile);
                File.Move(file, targetFile);
            }
        }
        Directory.Delete(source, true);
    }
    public List<String> InputString = new List<String>();
    private void Delayfunc(float delay, Action action)
    {
        StartCoroutine(Delay(delay, action));
    }
    private static IEnumerator Delay(float delay, Action action)
    {
        yield return new WaitForSeconds(delay);
        action.Invoke();
    }

    public void BrowseAppdata()
    {
        Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\AppData\\LocalLow\\Hyperbolic Magnetism");
    }

    
    void Start()
    {
        if (File.Exists(jsonLocation))
        {
            locations = SymLinkLocations.LoadFile(jsonLocation);
        }
        else
        {
            RelinkFoldersButton.interactable = false;
        }
    }
    
    public void BackupAppdata()
    {
        DateTime thisDay = DateTime.Today;

        var lastBackupPath = "Beat Saber AppData Backups\\Latest Backup\\Beat Saber";
        var sourceDirectoryPath = Path.Combine(Environment.CurrentDirectory, (Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\AppData\\LocalLow\\Hyperbolic Magnetism\\Beat Saber"));
        var targetDirectoryPath = Path.Combine(Environment.CurrentDirectory, ("Beat Saber AppData Backups\\" + thisDay.ToString("M")) + "\\Beat Saber");


        if (Directory.Exists(targetDirectoryPath))
        {
            DisplayErrorText("BACKUP ON THAT DATE ALREADY EXISTS");
            throw new Exception("Backup on that date already exists");
        }
        else
        {
            if (!Directory.Exists(targetDirectoryPath))
            {
                Directory.CreateDirectory(targetDirectoryPath);
            }

            if (Directory.Exists(lastBackupPath))
            {
                Directory.Delete(lastBackupPath, true);
            }

            if (Directory.Exists(sourceDirectoryPath))
            {
                DirectoryCopy(sourceDirectoryPath, targetDirectoryPath, true);
                DirectoryCopy(sourceDirectoryPath, lastBackupPath, true);
            }

            DisplayFeedbackText("BACKUP CREATED");
        }
    }

    public void BrowseGameFiles()
    {
        Process.Start(InstalledVersionToggle.BSDirectory);
    }

    public void RevertAppdata()
    {
        DateTime thisDay = DateTime.Today;

        var lastBackupPath = "Beat Saber AppData Backups\\Latest Backup\\Beat Saber";
        var sourceDirectoryPath = Path.Combine(Environment.CurrentDirectory, (Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\AppData\\LocalLow\\Hyperbolic Magnetism\\Beat Saber"));
        var targetDirectoryPath = Path.Combine(Environment.CurrentDirectory, ("Beat Saber AppData Backups\\" + thisDay.ToString("M")) + "\\Beat Saber");

        if (Directory.Exists("Beat Saber AppData Backups"))
        {
            if (Directory.Exists(sourceDirectoryPath))
            {
                Directory.Delete(sourceDirectoryPath, true);
            }
            DirectoryCopy(lastBackupPath, sourceDirectoryPath, true);
            DisplayFeedbackText("APPDATA RESTORED TO LATEST BACKUP");
        }
        else
        {
            DisplayErrorText("LAST BACKUP NOT FOUND");
            throw new Exception("Last backup not found");
        }

    }
    public void ClearAppdata()
    {
        DateTime thisDay = DateTime.Today;

        var lastBackupPath = "Beat Saber AppData Backups\\Latest Backup\\Beat Saber";
        var sourceDirectoryPath = Path.Combine(Environment.CurrentDirectory, (Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\AppData\\LocalLow\\Hyperbolic Magnetism\\Beat Saber"));
        var targetDirectoryPath = Path.Combine(Environment.CurrentDirectory, ("Beat Saber AppData Backups\\" + thisDay.ToString("M")) + "\\Beat Saber");

        if (Directory.Exists("Beat Saber AppData Backups"))
        {
            try
            {
                Directory.Delete(sourceDirectoryPath, true);
                DisplayErrorText("APPDATA CLEARED");
            } 
            catch
            {
                DisplayErrorText("APPDATA NOT FOUND");
                throw new Exception("AppData does not exist");
            }
        }
        else
        {
            DisplayErrorText("CREATE A BACKUP FIRST");
            throw new Exception("No backups found, cannot clear AppData");
        }
    }
    public void PixelModPackLink()
    {
        Application.OpenURL("https://github.com/iPixelGalaxy/iPixelGalaxy-Beat-Saber-Modpack");
    }
    public void PixelModPack()
    {
        InstallingTextObject.SetActive(true);

        Delayfunc(0.3f, delegate
        {
            if (!Directory.Exists("Temp Files"))
                Directory.CreateDirectory("Temp Files");

            List<string> modpackversions = new List<string>();

            JObject parsed;
            
            // Check Version
            using (var client = new WebClient())
            {
                string json = client.DownloadString("https://raw.githubusercontent.com/iPixelGalaxy/iPixelGalaxy-Beat-Saber-Modpack/main/BSLegacyVersionCheck.json");
                parsed = JObject.Parse(json);

                foreach (JProperty version in parsed.Properties())
                {
                    modpackversions.Add(version.Name);
                }
            }
            
            string Version = File.ReadAllText($"{InstalledVersionToggle.BSBaseDir}\\BeatSaberVersion.txt");
            int VersionIndex = modpackversions.IndexOf(Version);

            if (VersionIndex == -1)
            {
                //Version is incompatible
                DisplayErrorText("INCOMPATIBLE BEAT SABER VERSION");
                DisplayInfoText($"Latest Compatible Version:\n{modpackversions[0]}");
                if (Directory.Exists("Temp Files"))
                    Directory.Delete("Temp Files", true);

                if (File.Exists($"{InstalledVersionToggle.BSDirectory}\\ModpackInfo"))
                    File.Delete($"{InstalledVersionToggle.BSDirectory}\\ModpackInfo");
                
                InstallingTextObject.SetActive(false);
                throw new Exception("Incompatible Version");
            }

            string[] externalMods = parsed[Version].SelectToken("externalmods").ToObject<string[]>();
            string modpackurl = parsed[Version]["modpackurl"].ToObject<string>();
            string message = parsed[Version]["message"].ToObject<string>();
                
            // Display Message
            if (message != "")
            {
                string messageBuffer = "Installed Modpack for Beat Saber " + Version + "\n" + message;
                DisplayInfoText(messageBuffer);
            }
            else
            {
                string messageBuffer = "Installed Modpack for Beat Saber "  + Version;
                DisplayInfoText(messageBuffer);
            }
            
            // Download Modpack
            using (var client = new WebClient())
                client.DownloadFile(modpackurl, "BeatSaberModpack.zip");
            ZipFile.ExtractToDirectory("BeatSaberModpack.zip", "Temp Files");
            File.Delete("BeatSaberModpack.zip");

                for (int i = 1; i < externalMods.Length; i++)
                {
                    int CutIndex = externalMods[i].LastIndexOf('/') + 1;
                    string FileName = externalMods[i].Substring(CutIndex, externalMods[i].Length - CutIndex);
                    
                    if (FileName.Contains(".zip"))
                    {
                        using (var client = new WebClient())
                            client.DownloadFile(externalMods[i], FileName);
                        ZipFile.ExtractToDirectory(FileName, "Temp Files");
                        File.Delete(FileName);
                    }
                    else if (FileName.Contains(".dll"))
                    {
                        using (var client = new WebClient())
                            client.DownloadFile(externalMods[i], $"Temp Files\\Plugins\\{FileName}");
                    }
                    else
                    {
                        DisplayErrorText("PIXEL IS A DUMBASS");
                        throw new Exception("Yep, Pixel is a dumbass");
                    }
                }
            
            MoveDirectory("Temp Files", InstalledVersionToggle.BSDirectory);

            if (File.Exists($"{InstalledVersionToggle.BSDirectory}\\ModpackInfo"))
                File.Delete($"{InstalledVersionToggle.BSDirectory}\\ModpackInfo");

            if (Directory.Exists("Temp Files"))
                Directory.Delete("Temp Files", true);

            InstallingTextObject.SetActive(false);
            DisplayFeedbackText("INSTALLED MODPACK");
        });
        

    }
    public void InstallNewIPA()
    {
        try
        {
            DirectoryCopy(InstalledVersionToggle.BaseDirectory + "Resources\\BSIPA-4.2.2", InstalledVersionToggle.BSDirectory, true);

            var IPA = FindObjectOfType<CheckIPA>();
            IPA.IPAInstalled();

            ErrorTextObject.SetActive(false);
            DisplayFeedbackText("IPA 4.2.2 INSTALLED");
            IPA3Button.SetActive(false);
            IPA4Button.SetActive(false);
            UninstallIPAButton.SetActive(true);
        }
        catch
        {
            DisplayErrorText("IPA ALREADY INSTALLED");
            throw new Exception("An IPA version is already installed");
        }

        {
            string bspath = InstalledVersionToggle.BSDirectory;
            Process.Start(new ProcessStartInfo
            {
                WorkingDirectory = bspath,
                FileName = bspath + "IPA.exe",
                Arguments = "-n"
            });
        }
    }
    public void InstallLegacyIPA()
    {
        try
        {
            DirectoryCopy(InstalledVersionToggle.BaseDirectory + "Resources\\BSIPA-Legacy", InstalledVersionToggle.BSDirectory, true);
            DisplayFeedbackText("LEGACY IPA INSTALLED");
            var IPA = FindObjectOfType<CheckIPA>();
            IPA.IPAInstalled();
        }
        catch
        {
            DisplayErrorText("IPA ALREADY INSTALLED");
            throw new Exception("An IPA version is already installed");
        }

        {
            string bspath = InstalledVersionToggle.BSDirectory;
            Process.Start(new ProcessStartInfo
            {
                WorkingDirectory = bspath,
                FileName = "IPA.exe",
                Arguments = "\"Beat Saber.exe\""
            }); 
        }
    }
    public void UninstallIPA()
    {
        try
        {
            if (Directory.Exists(InstalledVersionToggle.BSDirectory))
            {
                string bspath = InstalledVersionToggle.BSDirectory;
                string IPADir = $"{InstalledVersionToggle.BSDirectory}IPA";
                string IPAexe = $"{InstalledVersionToggle.BSDirectory}IPA.exe";
                string IPAconfig = $"{InstalledVersionToggle.BSDirectory}IPA.exe.config";
                string IPAruntime = $"{InstalledVersionToggle.BSDirectory}IPA.runtimeconfig.json";
                string MonoCecildll = $"{InstalledVersionToggle.BSDirectory}Mono.Cecil.dll";

                ProcessStartInfo i = new ProcessStartInfo
                {
                    WorkingDirectory = bspath,
                    FileName = "IPA.exe",
                    Arguments = "--revert --nowait"
                };
                Process p = Process.Start(i);
                p.WaitForExit(5000);
                try
                {
                    if (Directory.Exists(IPADir))
                    {
                        Directory.Delete(IPADir, true);
                        File.Delete(IPAexe);
                    }

                    if (File.Exists(IPAconfig))
                        File.Delete(IPAconfig);

                    if (File.Exists(IPAruntime))
                        File.Delete(IPAruntime);

                    if (File.Exists(MonoCecildll))
                        File.Delete(MonoCecildll);

                    DisplayFeedbackText("IPA UNINSTALLED");

                    var IPA = FindObjectOfType<CheckIPA>();
                    IPA.IPANotInstalled();
                }
                catch
                {
                    DisplayErrorText("FAILED TO DELETE IPA FILES");
                }
            }
        } catch
        {
            {
                DisplayErrorText("IPA NOT INSTALLED");
                throw new Exception("No BSIPA Installation has been found");
            }
        }
    }
    
    [DllImport("shell32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool IsUserAnAdmin();
    public void CreateLinkedFolders()
    {
        // Your turn, ComputerElite
        // Thanks Risk, headpats


        foreach (string d in Directory.GetDirectories(InstalledVersionToggle.BSBaseDir))
        {
            string customSongsFolder = d + Path.DirectorySeparatorChar + "CustomSongs";
            string customLevelsFolder = d + Path.DirectorySeparatorChar + "Beat Saber_Data" + Path.DirectorySeparatorChar + "CustomLevels";
            string customWIPLevelsFolder = d + Path.DirectorySeparatorChar + "Beat Saber_Data" + Path.DirectorySeparatorChar + "CustomWIPLevels";
            string dLCsFolder = d + Path.DirectorySeparatorChar + "DLC";

            if (CustomLevelsToggle.isOn)
            {
                if (!Directory.Exists(InstalledVersionToggle.CustomLevelsDirectory)) Directory.CreateDirectory(InstalledVersionToggle.CustomLevelsDirectory);
                ProcessLink(customLevelsFolder, InstalledVersionToggle.CustomLevelsDirectory);
                locations.customLevels = true;
                Save();
            }
            if (CustomWIPLevelsToggle.isOn)
            {
                if (!Directory.Exists(InstalledVersionToggle.CustomWIPLevelsDirectory)) Directory.CreateDirectory(InstalledVersionToggle.CustomWIPLevelsDirectory);
                ProcessLink(customWIPLevelsFolder, InstalledVersionToggle.CustomWIPLevelsDirectory);
                locations.customWipLevels = true;
                Save();
            }
            if (CustomSongsToggle.isOn)
            {
                if (!Directory.Exists(InstalledVersionToggle.CustomSongsDirectory)) Directory.CreateDirectory(InstalledVersionToggle.CustomSongsDirectory);
                ProcessLink(customSongsFolder, InstalledVersionToggle.CustomSongsDirectory);
                locations.customSongs = true;
                Save();
            }
            if (DLCToggle.isOn)
            {
                if (!Directory.Exists(InstalledVersionToggle.DLCDirectory)) Directory.CreateDirectory(InstalledVersionToggle.DLCDirectory);
                ProcessLink(dLCsFolder, InstalledVersionToggle.DLCDirectory);
                locations.dlcs = true;
                Save();
            }
            if (OtherToggle.isOn)
            {
                if (OtherField.text.Contains("CustomLevels"))
                {
                    DisplayErrorText("USE CUSTOMLEVELS TOGGLE");
                    throw new Exception("Use the CustomLevels Toggle to link CustomLevels");
                }
                
                if (OtherField.text.Contains("CustomWIPLevels"))
                {
                    DisplayErrorText("USE CUSTOMWIPLEVELS TOGGLE");
                    throw new Exception("Use the CustomWIPLevels Toggle to link CustomLevels");
                }
                
                if (OtherField.text.Contains("Plugins"))
                {
                    DisplayErrorText("CANNOT LINK PLUGINS");
                    throw new Exception("Sharing this folder is Forbidden");
                }

                if (OtherField.text.Contains("UserData"))
                {
                    string BSIPAJSON = $"Installed Versions\\Beat Saber {InstalledVersionToggle.BSVersion}\\UserData\\Beat Saber IPA.json";
                    if (File.Exists(BSIPAJSON))
                    {
                        string json = File.ReadAllText(BSIPAJSON);
                        dynamic jsonObj = JsonConvert.DeserializeObject(json);
                        jsonObj["YeetMods"] = "false";
                        string output = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
                        File.WriteAllText($"Installed Versions\\Beat Saber {InstalledVersionToggle.BSVersion}\\UserData\\Beat Saber IPA.json", output);
                        DisplayInfoText("The YeetMods variable in Beat Saber IPA.json\nhas been set to false to prevent mod removal");
                    }
                }

                if (OtherField.text.Contains(""))
                {
                    DisplayErrorText("PLEASE INPUT A FOLDER");
                    throw new Exception("Please input something in the custom folder field");
                }

                if (!locations.folders.Any(x => x == OtherField.text)) // maybe add .ToLower() if needed
                {
                    locations.folders.Add(OtherField.text);
                    Save();
                }

                if (!Directory.Exists($"{d}\\{OtherField.text}")) Directory.CreateDirectory($"{d}\\{OtherField.text}");
                ProcessLink($"{d}\\{OtherField.text}", $"{OtherField.text}");

                string[] inputStrings = new string[] { "" };
                string input = "";

                foreach (string x in inputStrings)
                {
                    input += "\n" + OtherField.text;
                }
            }
        }

        Log.Debug("Folders Linked");
        locations.SaveToFile(jsonLocation);
        RelinkFoldersButton.interactable = true;
        DisplayFeedbackText("FOLDERS CREATED");
    }

    public static void AddAllSymlinksToSelectedBeatSaberFolder()
    {
        string d = InstalledVersionToggle.BSDirectory;
        string customSongsFolder = d + Path.DirectorySeparatorChar + "CustomSongs";
        string customLevelsFolder = d + Path.DirectorySeparatorChar + "Beat Saber_Data" + Path.DirectorySeparatorChar + "CustomLevels";
        string customWIPLevelsFolder = d + Path.DirectorySeparatorChar + "Beat Saber_Data" + Path.DirectorySeparatorChar + "CustomWIPLevels";
        string dLCsFolder = d + Path.DirectorySeparatorChar + "DLC";

        if (locations.customLevels)
        {
            if (!Directory.Exists(InstalledVersionToggle.CustomLevelsDirectory)) Directory.CreateDirectory(InstalledVersionToggle.CustomLevelsDirectory);
            ProcessLink(customLevelsFolder, InstalledVersionToggle.CustomLevelsDirectory);
        }
        if (locations.customWipLevels)
        {
            if (!Directory.Exists(InstalledVersionToggle.CustomWIPLevelsDirectory)) Directory.CreateDirectory(InstalledVersionToggle.CustomWIPLevelsDirectory);
            ProcessLink(customWIPLevelsFolder, InstalledVersionToggle.CustomWIPLevelsDirectory);
        }
        if (locations.customSongs)
        {
            if (!Directory.Exists(InstalledVersionToggle.CustomSongsDirectory)) Directory.CreateDirectory(InstalledVersionToggle.CustomSongsDirectory);
            ProcessLink(customSongsFolder, InstalledVersionToggle.CustomSongsDirectory);
        }
        if (locations.dlcs)
        {
            if (!Directory.Exists(InstalledVersionToggle.DLCDirectory)) Directory.CreateDirectory(InstalledVersionToggle.DLCDirectory);
            ProcessLink(dLCsFolder, InstalledVersionToggle.DLCDirectory);
        }
        foreach (string s in locations.folders)
        {
            if (!Directory.Exists($"{d}\\{s}")) Directory.CreateDirectory($"{d}\\{s}");
            ProcessLink($"{d}\\{s}", $"{s}");
        }
    }

    public static void ProcessLink(string path, string target)
    {
        if (Directory.Exists(path) && !new DirectoryInfo(path).Attributes.HasFlag(FileAttributes.ReparsePoint))
            DepotDownloaderObject.MoveDirectory(path, target);
        if(!Directory.Exists(path))
            CreateLink(path, target);
    }

    public static void CreateLink(string path, string target)
    {
        ProcessStartInfo i = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            Arguments = "/c \"mklink /J \"" + path + "\" \"" + target + ('\\') + "\"\""
        };
        Process p = Process.Start(i);
        p.WaitForExit();
    }
    public void RelinkFolders()
    {
        string d = InstalledVersionToggle.BSDirectory;
        string customSongsFolder = d + Path.DirectorySeparatorChar + "CustomSongs";
        string customLevelsFolder = d + Path.DirectorySeparatorChar + "Beat Saber_Data" + Path.DirectorySeparatorChar + "CustomLevels";
        string customWIPLevelsFolder = d + Path.DirectorySeparatorChar + "Beat Saber_Data" + Path.DirectorySeparatorChar + "CustomWIPLevels";
        string dLCsFolder = d + Path.DirectorySeparatorChar + "DLC";

        if (locations.customLevels)
        {
            if (!Directory.Exists(InstalledVersionToggle.CustomLevelsDirectory)) Directory.CreateDirectory(InstalledVersionToggle.CustomLevelsDirectory);
            ProcessLink(customLevelsFolder, InstalledVersionToggle.CustomLevelsDirectory);
        }
        if (locations.customWipLevels)
        {
            if (!Directory.Exists(InstalledVersionToggle.CustomWIPLevelsDirectory)) Directory.CreateDirectory(InstalledVersionToggle.CustomWIPLevelsDirectory);
            ProcessLink(customWIPLevelsFolder, InstalledVersionToggle.CustomWIPLevelsDirectory);
        }
        if (locations.customSongs)
        {
            if (!Directory.Exists(InstalledVersionToggle.CustomSongsDirectory)) Directory.CreateDirectory(InstalledVersionToggle.CustomSongsDirectory);
            ProcessLink(customSongsFolder, InstalledVersionToggle.CustomSongsDirectory);
        }
        if (locations.dlcs)
        {
            if (!Directory.Exists(InstalledVersionToggle.DLCDirectory)) Directory.CreateDirectory(InstalledVersionToggle.DLCDirectory);
            ProcessLink(dLCsFolder, InstalledVersionToggle.DLCDirectory);
        }

        foreach (string folder in locations.folders)
        {
            if (!Directory.Exists($"{d}\\{folder}")) Directory.CreateDirectory($"{d}\\{folder}");
            ProcessLink($"{d}\\{folder}", $"{folder}");
        }
        
        string BSIPAJSON = $"Installed Versions\\Beat Saber {InstalledVersionToggle.BSVersion}\\UserData\\Beat Saber IPA.json";
        if (File.Exists(BSIPAJSON))
        {
            string json = File.ReadAllText(BSIPAJSON);
            dynamic jsonObj = JsonConvert.DeserializeObject(json);
            jsonObj["YeetMods"] = "false";
            string output = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
            File.WriteAllText($"Installed Versions\\Beat Saber {InstalledVersionToggle.BSVersion}\\UserData\\Beat Saber IPA.json", output);
            DisplayInfoText("The YeetMods variable in Beat Saber IPA.json\nhas been set to false to prevent mod removal");
        }
                    

        DisplayInfoText("The YeetMods variable in Beat Saber IPA.json\nhas been set to false to prevent mod removal");

        DisplayFeedbackText("FOLDERS RELINKED");
    }

    public void OpenPatreonURL()
    {
        Application.OpenURL("https://patreon.com/RiskiVR");
    }

    public void OpenKofiURL()
    {
        Application.OpenURL("https://ko-fi.com/U7U114VMM");
    }
}