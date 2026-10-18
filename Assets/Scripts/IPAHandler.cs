using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

public class IPAHandler
{
    public static string Version;
    public static string MainPath = Path.Combine("Beat Saber", "IPA.exe");
    public static bool Installed()
    {
        string IPADir = Path.Combine("Beat Saber", "IPA");
        string IPAexe = Path.Combine("Beat Saber", "IPA.exe");
        return Directory.Exists(IPADir) && File.Exists(IPAexe);
    }
    public static async Task InstallIPA()
    {
        if (Installed() && GetIPAVersion() == Version)
        {
            ModListHandler.Downloading = false;
            return;
        }
        if (Installed() && GetIPAVersion() != Version) await UninstallIPA();

        DirectoryUtils.Copy(Path.Combine("Resources", "BSIPA-Legacy"), "Beat Saber", true);

        Process process = new Process();
        process.StartInfo = new ProcessStartInfo
        {
            WorkingDirectory = "Beat Saber",
            FileName = "IPA.exe",
            Arguments = "\"Beat Saber.exe\""
        };

        process.Start();
        await WaitForExitAsync(process);
        ModListHandler.Downloading = false;
    }
    static async Task UninstallIPA()
    {
        Process process = new Process();
        process.StartInfo = new ProcessStartInfo
        {
            WorkingDirectory = "Beat Saber",
            FileName = "IPA.exe",
            Arguments = "--revert --nowait"
        };
        try
        {
            if (Directory.Exists("Beat Saber"))
            {
                string IPADir = Path.Combine("Beat Saber", "IPA");
                string IPAexe = Path.Combine("Beat Saber", "IPA.exe");
                string MonoCecildll = Path.Combine("Beat Saber", "Mono.Cecil.dll");

                process.Start();
                await WaitForExitAsync(process);
                try
                {
                    if (Directory.Exists(IPADir))
                    {
                        Directory.Delete(IPADir, true);
                        File.Delete(IPAexe);
                    }

                    if (File.Exists(MonoCecildll))
                        File.Delete(MonoCecildll);
                }
                catch
                {
                    ErrorText.Display("FAILED TO DELETE IPA FILES");
                }
            }
        } 
        catch
        {
            ErrorText.Display("IPA NOT INSTALLED");
        }
    }

    public static string GetIPAVersion()
    {
        if (!File.Exists(MainPath)) return null;
        return Regex.Replace(FileVersionInfo.GetVersionInfo(MainPath).FileVersion, "(\\d+\\.\\d+\\.\\d+)\\.\\d+", "$1");
    }

    public static Task WaitForExitAsync(Process process, CancellationToken cancellationToken = default)
    {
        if (process.HasExited) return Task.CompletedTask;

        var tcs = new TaskCompletionSource<object>();
        process.EnableRaisingEvents = true;
        process.Exited += (sender, args) => tcs.TrySetResult(null);
        if (cancellationToken != default) cancellationToken.Register(() => tcs.SetCanceled());

        return process.HasExited ? Task.CompletedTask : tcs.Task;
    }
}