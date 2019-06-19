using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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

        // name is "scene_StoneNumber" 
        // ej: "museum_Stone01(Clone)" or just "museum_Stone01"
        string[] firstSplit = name.Split('_');
        string[] secondSplit = firstSplit[1].Split('(');
        string number = secondSplit[0].Substring(5);
        try
        {
            int result = Int32.Parse(number);
            int i = StoneSpawnHelper.GetStoneId(result, firstSplit[0]);
            SpawnStone(i);
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
            if (Input.GetMouseButton(0))
            {
                stone.transform.Rotate(Vector3.forward, -20.0f * Input.GetAxis("Mouse X"));
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
        Quaternion q = stone.transform.rotation;
        stone = Instantiate(LoadObjectFromBundle.sceneStones[stoneId - 1], pos, q);
        stone.transform.localScale *= scale;
    }

    IEnumerator loadXml(string name)
    {
        if (name.Contains("Clone")) {
            name = name.Split('(')[0];
        }

        string filePath = Path.Combine(Application.dataPath, "Scripts/Scenes/"+name+".xml");
        if (File.Exists(filePath))
        {
            string dataText = File.ReadAllText(filePath);
            var data = SceneHelper.GetKhachkarByXML(dataText);
            metaText[0].text = Convert.ToString(data["CoonditionOfPreservation"]);
            metaText[1].text = Convert.ToString(data["ImportantFeatures"]);
            metaText[2].text = Convert.ToString(data["Location"]);
            metaText[3].text = Convert.ToString(data["Scenario"]);
            metaText[4].text = Convert.ToString(data["Accessibility"]);
            metaText[5].text = Convert.ToString(data["Category"]);
            metaText[6].text = Convert.ToString(data["ProductionPeriod"]);

            /* metaText[0].text = Convert.ToString(data["Location"]);
            metaText[1].text = Convert.ToString(data["Scenario"]);
            metaText[2].text = Convert.ToString(data["Setting"]);
            metaText[3].text = Convert.ToString(data["Accessibility"]);
            metaText[4].text = Convert.ToString(data["Category"]);
            metaText[5].text = Convert.ToString(data["ProductionPeriod"]);
            metaText[6].text = Convert.ToString(data["CoonditionOfPreservation"]);
            metaText[7].text = Convert.ToString(data["Inscription"]);
            metaText[8].text = Convert.ToString(data["ImportantFeatures"]);
            metaText[9].text = Convert.ToString(data["Referances"]);*/
        }

        return null;
    }

}
