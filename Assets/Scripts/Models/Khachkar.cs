using Newtonsoft.Json.Linq;
using System;

public class Khachkar
{
    public int Id { get; set; }
    public string Location { get; set; }
    public string LatLong { get; set; }
    public string Scenario { get; set; }
    public string Setting { get; set; }
    public string Landscape { get; set; }
    public string Accessibility { get; set; }
    public string MastersName { get; set; }
    public string Category { get; set; }
    public string ProductionPeriod { get; set; }
    public string Motive { get; set; }
    public string ConditionOfPreservation { get; set; }
    public string Inscription { get; set; }
    public string ImportantFeatures { get; set; }
    public string BackSide { get; set; }
    public string HistoryOwnership { get; set; }
    public string CommemorativeActivities { get; set; }
    public string Referances { get; set; }

    public Khachkar(string jsonData)
    {
        JObject obj = JObject.Parse(jsonData);
        this.Id = obj["Id"] != null ? Int32.Parse((string)obj["Id"]) : 0;
        this.Location = obj["Location"] != null ? (string)obj["Location"] : "";
        this.LatLong = obj["LatLong"] != null ? (string)obj["LatLong"] : "";
        this.Scenario = obj["Scenario"] != null ? (string)obj["Scenario"] : "";
        this.Setting = obj["Setting"] != null ? (string)obj["Setting"] : "";
        this.Landscape = obj["Landscape"] != null ? (string)obj["Landscape"] : "";
        this.Accessibility = obj["Accessibility"] != null ? (string)obj["Accessibility"] : "";
        this.MastersName = obj["MastersName"] != null ? (string)obj["MastersName"] : "";
        this.Category = obj["Category"] != null ? (string)obj["Category"] : "";
        this.ProductionPeriod = obj["ProductionPeriod"] != null ? (string)obj["ProductionPeriod"] : "";
        this.Motive = obj["Motive"] != null ? (string)obj["Motive"] : "";
        this.ConditionOfPreservation = obj["ConditionOfPreservation"] != null ? (string)obj["ConditionOfPreservation"] : "";
        this.Inscription = obj["Inscription"] != null ? (string)obj["Inscription"] : "";
        this.ImportantFeatures = obj["ImportantFeatures"] != null ? (string)obj["ImportantFeatures"] : "";
        this.BackSide = obj["BackSide"] != null ? (string)obj["BackSide"] : "";
        this.HistoryOwnership = obj["HistoryOwnership"] != null ? (string)obj["HistoryOwnership"] : "";
        this.CommemorativeActivities = obj["CommemorativeActivities"] != null ? (string)obj["CommemorativeActivities"] : "";
        this.Referances = obj["Referances"] != null ? (string)obj["Referances"] : "";
    }
}
