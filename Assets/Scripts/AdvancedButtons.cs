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
using System.Linq;
using System.Net;
using System.IO.Compression;

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
    public Text ErrorText;
    public Text FeedbackText;
    public Text InfoText;
    public GameObject ErrorTextObject;
    public GameObject CuteErrorObject;
    public GameObject FeedbackTextObject;
    public GameObject InfoTextObject;
    public GameObject InstallingTextObject;
    public Button RelinkFoldersButton;
    public Toggle CustomLevelsToggle;
    public Toggle CustomWIPLevelsToggle;
    public Toggle CustomSongsToggle;
    public Toggle DLCToggle;
    public Toggle OtherToggle;
    public InputField OtherField;

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
        CuteErrorObject.SetActive(false);
        FeedbackTextObject.SetActive(false);
        ErrorTextObject.SetActive(false);
        ErrorTextObject.SetActive(true);
        ErrorText.text = text;
    }
    private void DisplayFeedbackText(string text)
    {
        // Set to false to restart popup animation DON'T CHANGE
        CuteErrorObject.SetActive(false);
        ErrorTextObject.SetActive(false);
        FeedbackTextObject.SetActive(false);
        FeedbackTextObject.SetActive(true);
        FeedbackText.text = text;
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
            CuteErrorObject.SetActive(false);
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
            CuteErrorObject.SetActive(false);
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
            CuteErrorObject.SetActive(false);
            DisplayErrorText("CREATE A BACKUP FIRST");
            throw new Exception("No Backup Found, Cannot clear AppData");
        }
    }
    public void PixelModPackLink()
    {
        Application.OpenURL("https://github.com/iPixelGalaxy/iPixelGalaxy-Beat-Saber-Modpack/releases/latest");
    }
    public void PixelModPack()
    {
        InstallingTextObject.SetActive(true);

        Delayfunc(0.03f, delegate
        {
            if (!Directory.Exists("Temp Files"))
                Directory.CreateDirectory("Temp Files");

            // Download Modpack
            using (var client = new WebClient())
                client.DownloadFile("https://github.com/iPixelGalaxy/iPixelGalaxy-Beat-Saber-Modpack/releases/latest/download/BeatSaberModpack.zip", "BeatSaberModpack.zip");
            ZipFile.ExtractToDirectory("BeatSaberModpack.zip", "Temp Files");
            File.Delete("BeatSaberModpack.zip");

            if (File.Exists("Temp Files\\ModpackInfo"))
            {
                string[] filedata = File.ReadAllLines("Temp Files\\ModpackInfo");
                List<string> Compatible = new List<string>(filedata[0].Split(','));

                string Version = File.ReadAllText($"{InstalledVersionToggle.BSBaseDir}\\BeatSaberVersion.txt");

                //Log.Debug($"Selected version right now is {Version}");

                if (Compatible.IndexOf(Version) == -1)
                {
                    //Version is incompatible
                    DisplayErrorText("INCOMPATIBLE BEAT SABER VERSION");
                    DisplayInfoText($"Compatible versions:\n{filedata[0]}");
                    if (Directory.Exists("Temp Files"))
                        Directory.Delete("Temp Files", true);

                    InstallingTextObject.SetActive(false);
                    throw new Exception("Incompatible Version");
                }

                for (int i = 1; i < filedata.Length; i++)
                {
                    int CutIndex = filedata[i].LastIndexOf('/') + 1;
                    string FileName = filedata[i].Substring(CutIndex, filedata[i].Length - CutIndex);
                    if (FileName.Contains(".zip"))
                    {
                        using (var client = new WebClient())
                            client.DownloadFile(filedata[i], FileName);
                        ZipFile.ExtractToDirectory(FileName, "Temp Files");
                        File.Delete(FileName);
                    }
                    else if (FileName.Contains(".dll"))
                    {
                        using (var client = new WebClient())
                            client.DownloadFile(filedata[i], $"Temp Files\\Plugins\\{FileName}");
                    }
                    else
                    {
                        DisplayErrorText("PIXEL IS A DUMBASS");
                        throw new Exception("Yep, Pixel is a dumbass");
                    }
                }
            }
            MoveDirectory("Temp Files", InstalledVersionToggle.BSDirectory);

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

            ErrorTextObject.SetActive(false);
            DisplayFeedbackText("IPA 4.2.2 INSTALLED");
        }
        catch
        {
            CuteErrorObject.SetActive(false);
            DisplayErrorText("IPA ALREADY INSTALLED");
            throw new Exception("An IPA version is already installed");
        }

        {
            string bspath = InstalledVersionToggle.BSDirectory;
            Process.Start(new ProcessStartInfo
            {
                WorkingDirectory = bspath,
                FileName = bspath + "IPA.exe"
            });
        }
    }
    public void InstallLegacyIPA()
    {
        try
        {
            DirectoryCopy(InstalledVersionToggle.BaseDirectory + "Resources\\BSIPA-Legacy", InstalledVersionToggle.BSDirectory, true);
            ErrorTextObject.SetActive(false);
            DisplayFeedbackText("LEGACY IPA INSTALLED");
        }
        catch
        {
            CuteErrorObject.SetActive(false);
            DisplayErrorText("IPA ALREADY INSTALLED");
            throw new Exception("An IPA version is already installed");
        }

        {
            string bspath = InstalledVersionToggle.BSDirectory;
            Process.Start(new ProcessStartInfo
            {
                WorkingDirectory = bspath,
                FileName = bspath + "IPA.exe"
            });
        }
    }
    public void InstallMods()
    {
        try
        {
            if (!File.Exists("Resources\\BeatSaberModManager.exe"))
            {
                using (var client = new WebClient())
                    client.DownloadFile("https://github.com/affederaffe/BeatSaberModManager/releases/latest/download/BeatSaberModManager-win-x64.zip", "BeatSaberModManager.zip");

                ZipFile.ExtractToDirectory("BeatSaberModManager.zip", "Resources\\");
                File.Delete("BeatSaberModManager.zip");
            }

            Process.Start("Resources\\BeatSaberModManager.exe", $"--path {InstalledVersionToggle.BSDirectory}");
        }
        catch
        {
            DisplayErrorText("COULD NOT DOWNLOAD BEATSABERMODMANAGER");
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
                if ($"{OtherField.text}".Contains("Plugins"))
                {
                    DisplayErrorText("CANNOT LINK PLUGINS");
                    throw new Exception("Sharing this folder is Forbiddon");
                }

                if ($"{OtherField.text}".Contains("CustomLevels"))
                {
                    DisplayErrorText("USE CUSTOMLEVELS TOGGLE");
                    throw new Exception("Use the CustomLevels Toggle to link CustomLevels");
                }

                if ($"{OtherField.text}".Contains("UserData"))
                {
                    try
                    {
                        string json = File.ReadAllText($"Installed Versions\\Beat Saber {InstalledVersionToggle.BSVersion}\\UserData\\Beat Saber IPA.json");
                        dynamic jsonObj = JsonConvert.DeserializeObject(json);
                        jsonObj["YeetMods"] = "false";
                        string output = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
                        File.WriteAllText($"Installed Versions\\Beat Saber {InstalledVersionToggle.BSVersion}\\UserData\\Beat Saber IPA.json", output);
                    }
                    catch
                    {
                        DisplayInfoText("Beat Saber IPA.json was\nnot found, continuing link...");
                    }
                    

                    DisplayInfoText("The YeetMods variable in Beat Saber IPA.json\nhas been set to false to prevent mod removal");
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