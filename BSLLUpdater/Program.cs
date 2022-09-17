using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;

class Program
{
    static void Main()
    {
        Console.WriteLine("Updating BSLegacyLauncher...");
        using (var client = new WebClient())
            client.DownloadFile("https://github.com/RiskiVR/BSLegacyLauncher/releases/latest/download/BSLegacyLauncher.zip", "BSLegacyLauncher.zip");
        ZipFile.ExtractToDirectory("BSLegacyLauncher.zip", "../", true);
        File.Delete("BSLegacyLauncher.zip");
        Console.WriteLine("Finished Updating");
        try { Process.Start("Beat Saber Legacy Launcher.exe"); }
        catch { Console.WriteLine("Please launch the Launcher."); }
    }
}