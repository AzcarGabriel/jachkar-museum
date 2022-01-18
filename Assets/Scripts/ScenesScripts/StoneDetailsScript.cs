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

    // Use this for initialization
    void Start()
    {
        string name = StaticValues.stone_name;
        loadXml(name);

        string[] firstSplit = name.Split('(');
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
        if (Physics.Raycast(r, out rh, 1000))
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");

            if (scroll != 0.0f)
            {
                float trans = scroll < 0 ? 0.9f : 1.1f;
                stone.transform.localScale *= trans;
            }

            if (Input.GetMouseButton(0))
            {
                stone.transform.Rotate(Vector3.forward, -20.0f * Input.GetAxis("Mouse X"));
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
        Vector3 pos = stone.transform.position;
        Quaternion q = StoneSpawnHelper.GetStoneRotationById(stoneId);
        stone = Instantiate(LoadObjectFromBundle.sceneStones[stoneId - 1], pos, q);
        stone.transform.localScale *= scale;
    }

    IEnumerator loadXml(string name)
    {
        if (name.Contains("Clone")) {
            name = name.Split('(')[0];
        }

        TextAsset metadata = null;
        for (int i = 0; i < LoadObjectFromBundle.stonesMetadata.Count; i++)
        {
            if (LoadObjectFromBundle.stonesMetadata[i].name.Equals(name)) {
                metadata = LoadObjectFromBundle.stonesMetadata[i];
                break;
            }
        }
        if (metadata == null) {
            metadata = LoadObjectFromBundle.stonesMetadata[LoadObjectFromBundle.stonesMetadata.Count - 1];
        }

        XmlDocument xmldoc = new XmlDocument();
        xmldoc.LoadXml(metadata.text);
        XmlNodeList xnList = xmldoc.SelectNodes("/Scene/Khachkars/Khachkar");
        foreach (XmlNode xn in xnList)
        {
            metaText[0].text = Convert.ToString(xn["CoonditionOfPreservation"].InnerText);
            metaText[1].text = Convert.ToString(xn["ImportantFeatures"].InnerText);
            metaText[2].text = Convert.ToString(xn["Location"].InnerText);
            metaText[3].text = Convert.ToString(xn["Scenario"].InnerText);
            metaText[4].text = Convert.ToString(xn["Accessibility"].InnerText);
            metaText[5].text = Convert.ToString(xn["Category"].InnerText);
            metaText[6].text = Convert.ToString(xn["ProductionPeriod"].InnerText);
        }

        return null;
    }
}
