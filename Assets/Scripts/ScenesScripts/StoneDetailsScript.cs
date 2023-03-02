/*
    StoneDetailsScript.cs
    
    @author Gabriel Azócar Cárcamo <azocarcarcamo@gmail.com>
 */

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System.Xml;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

public class StoneDetailsScript : MonoBehaviour
{
    public GameObject scrollView;
    public GameObject mainCamera;
    public GameObject stone;
    public GameObject loadScreen;
    public Slider slider;

    // Metadata edit components
    public InputField conditionInput;
    public InputField infoInput;
    public InputField locationInput;
    public InputField scenarioInput;
    public InputField accessInput;
    public InputField categoryInput;
    public InputField productionPeriodInput;
    public GameObject editButton;
    public GameObject submitButton;
    public GameObject cancelButton;

    private StoneService stoneService;
    private List<string> metaText;
    private readonly XmlNode originalNode;
    private int stoneId = -1;

    // Use this for initialization
    void Start()
    {
        stoneService = gameObject.AddComponent<StoneService>();
        stoneService.loadScreen = this.loadScreen;
        stoneService.slider = this.slider;

        string[] firstSplit = StaticValues.stone_name.Split('(');
        string number = firstSplit[0][5..];
        try
        {
            this.stoneId = Int32.Parse(number);
            SpawnStone();
            metaText = new List<string>();
        }
        catch (FormatException)
        {
            Debug.Log("Start Details Error");
        }
    }

    // Update is called once per frame
    void Update()
    {
        Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(r, out _, 100))
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0.0f && Physics.Raycast(r, out _, 50))
            {
                float trans = scroll < 0 ? 0.9f : 1.1f;
                this.stone.transform.localScale *= trans;
            }

            if (Input.GetMouseButton(0))
            {
                this.stone.transform.Rotate(Vector3.forward, -20.0f * Input.GetAxis("Mouse X"));
            }

            if (Input.GetMouseButton(1))
            {
                // Mover la piedra en un plano
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(StaticValues.previos_scene, LoadSceneMode.Single);
        }
    }

    public void SpawnStone()
    {
        float scale = StoneSpawnHelper.GetStoneScaleById(this.stoneId);
        Vector3 pos = this.stone.transform.position;
        Quaternion q = StoneSpawnHelper.GetStoneRotationById(this.stoneId);
        StartCoroutine(this.stoneService.SpawnStoneWithPositionAndRotation(this.stoneId, pos, q, SearchMetadata));
    }

    public void SearchMetadata()
    {
        StartCoroutine(this.stoneService.DownloadAndStorageMetadata(this.stoneId, FinishConfig));
    }

    public void FinishConfig()
    {
        FindStoneObject(StaticValues.stone_name);
        LoadXml(StaticValues.stone_name);
    }

    public void LoadXml(string stoneName)
    {
        if (stoneName.Contains("Clone")) {
            stoneName = stoneName.Split('(')[0];
        }

        try
        { 
            int index = Int32.Parse(stoneName.Replace("Stone", ""));
            Khachkar metadata = (Khachkar) StonesValues.metadataHashtable[index];
            Debug.Log(metadata.ConditionOfPreservation);
            metaText.Add(this.FormatMetaText(metadata.ConditionOfPreservation));
            metaText.Add(this.FormatMetaText(metadata.ImportantFeatures));
            metaText.Add(this.FormatMetaText(metadata.Location));
            metaText.Add(this.FormatMetaText(metadata.Scenario));
            metaText.Add(this.FormatMetaText(metadata.Accessibility));
            metaText.Add(this.FormatMetaText(metadata.Category));
            metaText.Add(this.FormatMetaText(metadata.ProductionPeriod));

            this.FillTexts(); 
        }
        catch (FormatException)
        {
            Debug.Log("Error loading metadata");
        }
    }

    private string FormatMetaText(string metaText = "")
    {
        if (metaText != null && metaText != "")
        {
            if (metaText[^1] != '.')
            {
                return metaText.Trim() + ".";
            }
            return metaText.Trim();
        }

        return "No data.";
    }

    public void FillTexts()
    {
        conditionInput.text = metaText[0];
        infoInput.text = metaText[1];
        locationInput.text = metaText[2];
        scenarioInput.text = metaText[3];
        accessInput.text = metaText[4];
        categoryInput.text = metaText[5];
        productionPeriodInput.text = metaText[6];
    }

    public void StartEditing()
    {
        conditionInput.interactable = true;
        infoInput.interactable = true;
        locationInput.interactable = true;
        scenarioInput.interactable = true;
        accessInput.interactable = true;
        categoryInput.interactable = true;
        productionPeriodInput.interactable = true;

        editButton.SetActive(false);
        submitButton.SetActive(true);
        cancelButton.SetActive(true);
    }

    public void CancelEditing()
    {
        conditionInput.interactable = false;
        infoInput.interactable = false;
        locationInput.interactable = false;
        scenarioInput.interactable = false;
        accessInput.interactable = false;
        categoryInput.interactable = false;
        productionPeriodInput.interactable = false;

        editButton.SetActive(true);
        submitButton.SetActive(false);
        cancelButton.SetActive(false);

        this.FillTexts();
    }

    public void SubmitInfo()
    {
        editButton.SetActive(true);
        submitButton.SetActive(false);
        cancelButton.SetActive(false);

        _ = this.CreateAndSubmitXmlAsync();
    }

    private async Task CreateAndSubmitXmlAsync()
    {
        Debug.Log("START");

        // Esto se puede optimizar usando el XmlDocument previo
        string fileName = StaticValues.stone_name + ".xml";
        string filePath = Path.Combine(Application.persistentDataPath, fileName);
        using XmlWriter writer = XmlWriter.Create(filePath);
        writer.WriteStartElement("Scene");
        writer.WriteStartElement("Khachkars");
        writer.WriteStartElement("Khachkar");

        writer.WriteElementString("Id", originalNode["Id"].InnerText);
        writer.WriteElementString("Location", locationInput.text);
        writer.WriteElementString("LatLong", originalNode["LatLong"].InnerText);
        writer.WriteElementString("Scenario", scenarioInput.text);
        writer.WriteElementString("Setting", originalNode["Setting"].InnerText);
        writer.WriteElementString("Landscape", originalNode["Landscape"].InnerText);
        writer.WriteElementString("Accessibility", accessInput.text);
        writer.WriteElementString("MastersName", originalNode["MastersName"].InnerText);
        writer.WriteElementString("Category", categoryInput.text);
        writer.WriteElementString("ProductionPeriod", productionPeriodInput.text);
        writer.WriteElementString("Motive", originalNode["Motive"].InnerText);
        writer.WriteElementString("CoonditionOfPreservation", conditionInput.text);
        writer.WriteElementString("Inscription", originalNode["Inscription"].InnerText);
        writer.WriteElementString("ImportantFeatures", infoInput.text);
        writer.WriteElementString("BackSide", originalNode["BackSide"].InnerText);
        writer.WriteElementString("HistoryOwnership", originalNode["HistoryOwnership"].InnerText);
        writer.WriteElementString("CommemorativeActivities", originalNode["CommemorativeActivities"].InnerText);
        writer.WriteElementString("Referances", originalNode["Referances"].InnerText);

        writer.WriteEndElement();
        writer.WriteEndElement();
        writer.WriteEndElement();
        writer.Flush();
        Debug.Log("ready file");

        // SEND
        using var multipartFormContent = new MultipartFormDataContent();
        Debug.Log("1");

        // Load file name
        var stringContent = new StringContent(fileName);
        multipartFormContent.Add(stringContent, name: "file_name");
        Debug.Log("2");

        // Load and add the file and set the file's Content-Type header
        Debug.Log(filePath);
        FileStream f = File.OpenRead(filePath);
        Debug.Log(f.Name);
        var fileStreamContent = new StreamContent(File.OpenRead(filePath));
        fileStreamContent.Headers.ContentType = new MediaTypeHeaderValue("application/xml");
        multipartFormContent.Add(fileStreamContent, name: "file", fileName: fileName);
        Debug.Log("3");

        // Send it
        Debug.Log("sending");
        HttpClient httpClient = new();
        var response = await httpClient.PostAsync("https://localhost:8000/upload/", multipartFormContent);
        httpClient.Dispose();
        string sd = response.Content.ReadAsStringAsync().Result;
        Debug.Log(sd);

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }

    private void FindStoneObject(string stoneName)
    {
        foreach (GameObject obj in GameObject.FindObjectsOfType<GameObject>())
        {
            if (obj.name.Contains(stoneName))
            {
                this.stone = obj;
            }
        }
    }
}
