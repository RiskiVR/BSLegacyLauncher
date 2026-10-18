using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using Button = UnityEngine.UI.Button;
using Debug = UnityEngine.Debug;

[Serializable]
public class ModInfo
{
    public string Name = string.Empty;
    public string Version = string.Empty;
    public string Category = string.Empty;
    public string Description = string.Empty;
    public string DownloadUrl = string.Empty;
    public string SourceUrl = string.Empty;
    public bool Required = false;
    public bool Installed = false;
}
public class ModListHandler : MonoBehaviour
{
    private static ModListHandler i;
    [SerializeField] GameObject modDetailsUI;
    [SerializeField] Button removeButton;
    [SerializeField] Button installButton;
    [SerializeField] BarController installButtonBar;
    [SerializeField] TextMeshProUGUI installText;
    [SerializeField] Transform listTransform;
    [SerializeField] GameObject loadingText;
    [SerializeField] GameObject modElementPrefab;
    [SerializeField] TextMeshProUGUI modHeaderPrefab;
    [SerializeField] List<ModElement> modElements;
    [SerializeField] List<ModInfo> mods;
    [CanBeNull] Queue<ModElement> modsToDownload;
    private ModElement currentMod;
    private string pluginsPath;
    private int downloadProgress;
    public static bool Downloading;
    private string selectedModFilename;

    void Start()
    {
        i = this;
        pluginsPath = Path.Combine("Beat Saber", "Plugins");
        installButton.onClick.AddListener(DownloadMods);
        removeButton.onClick.AddListener(RemoveSelectedMod);
    }

    void OnEnable()
    {
        RefreshModList();
    }

    void Update()
    {
        if (modsToDownload != null)
        {
            if (modsToDownload.Count > 0)
            {
                if (!Downloading)
                {
                    if (currentMod) PostModDownload();
                    ModElement mod = modsToDownload.Dequeue();
                    installButtonBar.SetAmount((float)(mods.Count - modsToDownload.Count) / mods.Count);
                    currentMod = mod;
                    bool ipa = mod.modName == "BSIPA";
                    var filename = mod.modDownloadUrl.Split("/").Last().Replace("%20", " ");
                    var path = Path.Combine(pluginsPath, filename);
                    if (ipa)
                    {
                        Downloading = true;
                        IPAHandler.InstallIPA();
                        return;
                    }
                    DownloadFile(mod.modDownloadUrl, path);
                }
            }
            else if (!Downloading)
            {
                if (currentMod) PostModDownload();
                currentMod = null;
                modsToDownload = null;
            }
        }
        installButton.interactable = !Downloading;
        installText.text = Downloading ? "Downloading..." : "Install / Update";
        if (currentMod) currentMod.downloadProgress = downloadProgress;
    }

    void DoneLoading()
    {
        loadingText.SetActive(false);
    }

    void PostModDownload()
    {
        var filename = currentMod.modDownloadUrl.Split("/").Last().Replace("%20", " ");
        var path = Path.Combine(pluginsPath, filename);
        currentMod.modVersion = currentMod.name == "BSIPA" ? IPAHandler.GetIPAVersion() : GetPluginVersion(path);
        currentMod.FinishDownload();
        currentMod.installed = true;
        currentMod.UpdateInfo();
    }

    void RefreshModList()
    {
        loadingText.SetActive(true);
        modElements.Clear();
        mods.Clear();
        foreach (Transform t in listTransform) Destroy(t.gameObject);
        ListMods();
    }

    async Task ListMods()
    {
        HttpClient client = new HttpClient();
        HttpResponseMessage response = await client.GetAsync("https://mods.bslegacy.com/api/list");
        if (!response.IsSuccessStatusCode) return;
        string json = await response.Content.ReadAsStringAsync();
        mods = JsonConvert.DeserializeObject<List<ModInfo>>(json);
        Dictionary<string, List<ModInfo>> categories = new();
        foreach (ModInfo mod in mods)
        {
            if (!categories.Keys.Contains(mod.Category)) categories.Add(mod.Category, new List<ModInfo>());
            categories[mod.Category].Add(mod);
        }

        foreach (var category in categories)
        {
            var header = Instantiate(modHeaderPrefab, listTransform);
            header.text = category.Key;
            header.name = $"___{category.Key.ToUpper()}___";
            foreach (ModInfo mod in category.Value)
            {
                var element = Instantiate(modElementPrefab, listTransform).GetComponent<ModElement>();
                var filename = mod.DownloadUrl.Split("/").Last().Replace("%20", " ");
                bool ipa = mod.Name == "BSIPA";
                var path = ipa ? IPAHandler.MainPath : Path.Combine(pluginsPath, filename);
                string version = null;
                mod.Installed = File.Exists(path);
                if (ipa) IPAHandler.Version = mod.Version;
                if (mod.Installed) version = ipa ? IPAHandler.GetIPAVersion() : GetPluginVersion(path);
                element.SetInfo(true, mod.Name, version, mod.Version, mod.Description, mod.DownloadUrl, mod.SourceUrl, mod.Required, mod.Installed);
                element.name = mod.Name;
                modElements.Add(element);
            }
        }
        DoneLoading();
    }

    void DownloadMods()
    {
        modsToDownload = new();
        for (int i = 0; i < mods.Count; i++)
        {
            if (modElements[i].selected)
            {
                modsToDownload.Enqueue(modElements[i]);
            }
        }
    }

    private void DownloadFile(string url, string destination)
    {
        var webClient = new WebClient();
        webClient.DownloadProgressChanged += DownloadProgressChanged;
        webClient.DownloadFileCompleted += DownloadFileCompleted;

        string path = destination.Substring(0, destination.LastIndexOf(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal));
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        Downloading = true;
        downloadProgress = 0;
        var uri = new Uri(url);
        webClient.DownloadFileAsync(uri, destination);
    }

    private void DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
    {
        downloadProgress = e.ProgressPercentage;
    }

    private void DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
    {
        Downloading = false;
    }

    static string GetPluginVersion(string dllPath)
    {
        string filename = dllPath.Split("\\").Last();
        string version;
        try
        {
            AssemblyDefinition.ReadAssembly(dllPath);
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to read assembly for {filename}: {e}");
            return null;
        }
        AssemblyDefinition asm = AssemblyDefinition.ReadAssembly(dllPath);
        TypeDefinition type = asm.MainModule.GetTypes().FirstOrDefault(t => t.Name == "Plugin");
        if (type == null) return null;

        IEnumerable<MethodDefinition> getters = type.Properties.Where(p => p.Name == "Version" || p.Name.EndsWith(".Version")).Select(p => p.GetMethod).Where(m => m != null && m.HasBody);

        foreach (MethodDefinition getter in getters)
        {
            Collection<Instruction> il = getter.Body.Instructions;

            Instruction ldstr = il.FirstOrDefault(i => i.OpCode == OpCodes.Ldstr);
            if (ldstr != null)
            {
                version = (string)ldstr.Operand;
                version = Regex.Replace(version, "[a-z]", string.Empty, RegexOptions.IgnoreCase);
                return version;
            }

            Instruction ldfld = il.FirstOrDefault(i => i.OpCode == OpCodes.Ldfld || i.OpCode == OpCodes.Ldsfld);
            if (ldfld != null)
            {
                string s = FindFieldLiteral(type, (FieldReference)ldfld.Operand);
                if (s != null)
                {
                    version = s;
                    version = Regex.Replace(version, "[a-z]", string.Empty, RegexOptions.IgnoreCase);
                    return version;
                }
            }
        }

        FieldDefinition field = type.Fields.FirstOrDefault(f => f.Name == "Version");
        if (field != null)
        {
            version = FindFieldLiteral(type, field);
            version = Regex.Replace(version, "[a-z]", string.Empty, RegexOptions.IgnoreCase);
            return version;
        }

        return null;
    }

    // nuclear option >:(
    static string FindFieldLiteral(TypeDefinition type, FieldReference field)
    {
        foreach (MethodDefinition ctor in type.Methods.Where(m => m.IsConstructor && m.HasBody))
        {
            Collection<Instruction> il = ctor.Body.Instructions;

            for (int i = 1; i < il.Count; i++)
            {
                if ((il[i].OpCode == OpCodes.Stfld || il[i].OpCode == OpCodes.Stsfld)
                    && ((FieldReference)il[i].Operand).Name == field.Name
                    && il[i - 1].OpCode == OpCodes.Ldstr)
                {
                    return (string)il[i - 1].Operand;
                }
            }
        }

        return null;
    }

    public static void ShowModDetails(string filename, bool removable)
    {
        i.modDetailsUI.SetActive(true);
        i.removeButton.interactable = removable;
        i.selectedModFilename = filename;
    }

    void RemoveSelectedMod()
    {
        var path = Path.Combine(pluginsPath, selectedModFilename);
        File.Delete(path);
        RefreshModList();
    }
}