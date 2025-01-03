using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;
using UnityEngine;

public class Person
{
    public string Name { get; set; }
    public string URL { get; set; }
}
public class Links
{
    public List<Person> People { get; set; }
}

public class Hyperlinks : MonoBehaviour
{
    private const string List = "https://raw.githubusercontent.com/RiskiVR/BSLegacyLauncher/media/other/links.json";

    private static List<Person> Person = new List<Person>();

    public void Start()
    {
        var http = new HttpClient();
        var f = http.GetStringAsync(List).GetAwaiter().GetResult();
        Person = JsonConvert.DeserializeObject<List<Person>>(f);
        http.Dispose();
    }

    private static string GetUrl(string input)
    {
        var p = Person.FirstOrDefault(x => x.Name.Equals(input));
        return p?.URL ?? "https://riskivr.com/";
    }
    
    public void Link_Riski() => Application.OpenURL(GetUrl("RiskiVR"));
    public void Link_DDAkebono() => Application.OpenURL(GetUrl("DDAkebono"));
    public void Link_ComputerElite() => Application.OpenURL(GetUrl("ComputerElite"));
}
