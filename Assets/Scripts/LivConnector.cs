using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class LivConnector
{
    const string LivApplicationsKey = @"Software\LIV.App\ExternalApplications";
    const string BSLLIdPrefix = "bs-legacy-";

    public struct LivEntry
    {
        public string Id;
        public string Name;
        public string InstallPath;
        public string Arguments;
        public string Executable;
    }

    public static void AddAndUpdateAllRegistryEntries(List<string> versions)
    {
        ClearAllLivEntries(BSLLIdPrefix);
        List<LivEntry> entries = new List<LivEntry>();
        string exeLoc = InstalledVersionToggle.BaseDirectory + "Beat Saber Legacy Launcher.exe";
        foreach (string version in versions)
        {
            if (version == null) continue;
            Debug.Log(version);
            entries.Add(new LivEntry
            {
                Id = BSLLIdPrefix + version.Replace(".", "-"),
                Name = "Beat Saber v" + version,
                InstallPath = InstalledVersionToggle.GetBSDirectory(version),
                Executable = exeLoc,
                Arguments = "--version \"" + version + "\""
            });
        }
        Debug.Log(CreateLivEntries(entries));
    }

    /// <returns>Whether the entire operation was successful or not.</returns>
    public static bool ClearAllLivEntries(string idPrefix)
    {
        idPrefix = idPrefix.ToLowerInvariant();

        try
        {
            using (var hkcu = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64))
            using (var livApplicationsKey = hkcu.CreateSubKey(LivApplicationsKey, true))
            {
                if (livApplicationsKey is null)
                    return false;

                var applicationEntries = livApplicationsKey.GetSubKeyNames();

                foreach (var applicationId in applicationEntries)
                {
                    if (!applicationId.StartsWith(idPrefix, StringComparison.InvariantCultureIgnoreCase))
                        continue;

                    livApplicationsKey.DeleteSubKeyTree(applicationId, false);
                }

                return true;
            }
        }
        catch (Exception ex)
        {
            // You should log this; generally we haven't seen issues.
            Debug.LogError(ex.ToString());
            return false;
        }
    }

    /// <returns>Whether the entire operation was successful or not.</returns>
    public static bool CreateLivEntries(IEnumerable<LivEntry> entries)
    {
        try
        {
            using (var hkcu = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64))
            using (var livApplicationsKey = hkcu.CreateSubKey(LivApplicationsKey, true))
            {
                if (livApplicationsKey is null)
                    return false;

                foreach (var entry in entries)
                {
                    using (var entryKey = livApplicationsKey.CreateSubKey(entry.Id, true))
                    {
                        if (entryKey is null)
                            return false;

                        entryKey.SetValue(@"Name", entry.Name, RegistryValueKind.String);
                        entryKey.SetValue(@"InstallPath", entry.InstallPath, RegistryValueKind.String);
                        entryKey.SetValue(@"Executable", entry.Executable, RegistryValueKind.String);
                        entryKey.SetValue(@"Arguments", entry.Arguments, RegistryValueKind.String);
                    }
                }

                return true;
            }
        }
        catch (Exception ex)
        {
            // You should log this; generally we haven't seen issues.
            Debug.LogError(ex.ToString());
            return false;
        }
    }
}
