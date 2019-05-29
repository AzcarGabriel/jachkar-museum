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
            int i = GetStoneId(result, firstSplit[0]);
            SpawnStone(i);
        }
        catch (FormatException)
        {
            Debug.Log("ERROR");
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
            SceneManager.LoadScene("Museum", LoadSceneMode.Single);
        }
    }

    float GetStoneScaleById(int stoneId)
    {
        float[] scales = new float[] { 1.0f, 1.0f, 1.0f, 0.55f, 1.0f };
        int index = 0;
        if (stoneId <= 7)
        {
            index = 0;
        }
        else if (stoneId <= 13)
        {
            index = 1;
        }
        else if (stoneId <= 26)
        {
            index = 2;
        }
        else if (stoneId <= 45)
        {
            index = 3;
        }
        else if (stoneId <= 58)
        {
            index = 4;
        }

        return scales[index];
    }

    int GetStoneId(int number, string name)
    {
        int id = 0;
        if (name.Equals("EchmiadzinAlly"))
        {
            id = number;
        }
        else if (name.Equals("museum"))
        {
            id = number + 7;
        }
        else if (name.Equals("Noradus"))
        {
            id = number + 13;
        }
        else if (name.Equals("Noravank"))
        {
            id = number + 26;
        }
        else if (name.Equals("wallStones"))
        {
            id = number + 45;
        }

        return id + 1;
    }

    public void SpawnStone(int stoneId)
    {
        float scale = GetStoneScaleById(stoneId);
        Vector3 pos = stone.transform.position;
        Quaternion q = stone.transform.rotation;
        stone = Instantiate(LoadObjectFromBundle.sceneStones[stoneId - 1], pos, q);
        stone.transform.localScale *= scale;
    }

    IEnumerator loadXml(string name)
    {
        string filePath = Path.Combine(Application.dataPath, "Scripts/Scenes/"+name+".xml");
        if (File.Exists(filePath))
        {
            string dataText = File.ReadAllText(filePath);
            var data = SceneHelper.GetKhachkarByXML(dataText);
            metaText[0].text = Convert.ToString(data["CoonditionOfPreservation"]);
            metaText[1].text = Convert.ToString(data["ImportantFeatures"]);

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
