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
    [SerializeField]
    public List<Text> metaText;
    public GameObject scrollView;
    public GameObject mainCamera;
    public GameObject stone;
    public GameObject loadScreen;
    public Slider slider;
    private StoneService stoneService;

    // Use this for initialization
    void Start()
    {
        stoneService = gameObject.AddComponent<StoneService>();
        stoneService.loadScreen = this.loadScreen;
        stoneService.slider = this.slider;

        string[] firstSplit = StaticValues.stone_name.Split('(');
        string number = firstSplit[0].Substring(5);
        try
        {
            int sID = Int32.Parse(number);
            SpawnStone(sID);
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
        RaycastHit rh;
        if (Physics.Raycast(r, out rh, 100))
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");

            if (scroll != 0.0f && Physics.Raycast(r, out rh, 50))
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
            StaticValues.should_lock = true;
            StaticValues.should_enable = true;
            Destroy(stone);
            SceneManager.UnloadSceneAsync("StoneDetails");
            
        }
    }

    public void SpawnStone(int stoneId)
    {
        float scale = StoneSpawnHelper.GetStoneScaleById(stoneId);
        Vector3 pos = this.stone.transform.position;
        Quaternion q = StoneSpawnHelper.GetStoneRotationById(stoneId);
        StartCoroutine(this.stoneService.SpawnStoneWithPositionAndRotation(-1, stoneId, pos, q, FinishConfig, true));
    }

    public void FinishConfig()
    {
        FindStoneObject("detailStone");
        loadXml(StaticValues.stone_name);
    }

    IEnumerator loadXml(string stoneName)
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

            XmlDocument xmldoc = new XmlDocument();
            xmldoc.LoadXml(metadata.text);
            XmlNodeList xnList = xmldoc.SelectNodes("/Scene/Khachkars/Khachkar");
            foreach (XmlNode xn in xnList)
            {
                metaText[0].text = this.FormatMetaText(Convert.ToString(xn["CoonditionOfPreservation"].InnerText));
                metaText[1].text = this.FormatMetaText(Convert.ToString(xn["ImportantFeatures"].InnerText));
                metaText[2].text = this.FormatMetaText(Convert.ToString(xn["Location"].InnerText));
                metaText[3].text = "Scenario: " + this.FormatMetaText(Convert.ToString(xn["Scenario"].InnerText));
                metaText[4].text = "Accessibility: " + this.FormatMetaText(Convert.ToString(xn["Accessibility"].InnerText));
                metaText[5].text = "Category: " + this.FormatMetaText(Convert.ToString(xn["Category"].InnerText));
                metaText[6].text = "Production Period: " + this.FormatMetaText(Convert.ToString(xn["ProductionPeriod"].InnerText));
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

    private void FindStoneObject(string stoneName)
    {
        Debug.Log("Doing last");
        foreach (GameObject obj in FindObjectsOfType<GameObject>())
        {
            if (obj.name.Contains(stoneName))
            {
                this.stone = obj;
            }
        }
    }
}
