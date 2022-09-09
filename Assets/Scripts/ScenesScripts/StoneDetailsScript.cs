/*
    StoneDetailsScript.cs
    
    @author Gabriel Azócar Cárcamo <azocarcarcamo@gmail.com>
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Xml;

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
    private XmlNode originalNode;

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
            int sID = Int32.Parse(number);
            SpawnStone(sID);
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

    public void SpawnStone(int stoneId)
    {
        float scale = StoneSpawnHelper.GetStoneScaleById(stoneId);
        Vector3 pos = this.stone.transform.position;
        Quaternion q = StoneSpawnHelper.GetStoneRotationById(stoneId);
        StartCoroutine(this.stoneService.SpawnStoneWithPositionAndRotation(stoneId, pos, q, FinishConfig));
    }

    public void FinishConfig()
    {
        FindStoneObject(StaticValues.stone_name);
        StartCoroutine(LoadXml(StaticValues.stone_name));
    }

    IEnumerator LoadXml(string stoneName)
    {
        if (stoneName.Contains("Clone")) {
            stoneName = stoneName.Split('(')[0];
        }

        try
        { 
            int index = Int32.Parse(stoneName.Replace("Stone", ""));
            StoneService.BundleName bundleName = StoneService.CalculateAssetBundleNameByStoneIndex(index);

            TextAsset metadata = null;
            foreach (AssetBundle mab in StonesValues.metadataAssetBundles)
            {
                if (mab.name == bundleName.metadataBundleName)
                {
                    metadata = mab.LoadAsset<TextAsset>(stoneName);
                }
            }

            XmlDocument xmldoc = new();
            xmldoc.LoadXml(metadata.text);
            XmlNodeList xnList = xmldoc.SelectNodes("/Scene/Khachkars/Khachkar");
            foreach (XmlNode xn in xnList)
            {
                originalNode = xn;

                metaText.Add(this.FormatMetaText(Convert.ToString(xn["CoonditionOfPreservation"].InnerText)));
                metaText.Add(this.FormatMetaText(Convert.ToString(xn["ImportantFeatures"].InnerText)));
                metaText.Add(this.FormatMetaText(Convert.ToString(xn["Location"].InnerText)));
                metaText.Add(this.FormatMetaText(Convert.ToString(xn["Scenario"].InnerText)));
                metaText.Add(this.FormatMetaText(Convert.ToString(xn["Accessibility"].InnerText)));
                metaText.Add(this.FormatMetaText(Convert.ToString(xn["Category"].InnerText)));
                metaText.Add(this.FormatMetaText(Convert.ToString(xn["ProductionPeriod"].InnerText)));

                this.FillTexts(); 
            }
        }
        catch (FormatException)
        {
            Debug.Log("Error loading metadata");
        }

        return null;
    }

    private string FormatMetaText(string metaText)
    {
        if (metaText != null && metaText != "")
        {
            if (metaText[metaText.Length - 1] != '.')
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

        this.CreateXml();
    }

    private void CreateXml()
    {
        // Esto se puede optimizar usando el XmlDocument previo
        using XmlWriter writer = XmlWriter.Create("C:/Users/Startnet/Desktop/books.xml");
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
