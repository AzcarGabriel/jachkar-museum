using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MuseumScripts : MonoBehaviour {

    [SerializeField]
    public Transform spawnPoint;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    /*0 al 7 EchmiadzinAlly
    8 al 13 Museum
    14 al 26 Noradus
    27 al 45 Noravank
    46 al 58 WallStones*/

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
        SpawnStoneWithPositionAndRotation(stoneId, spawnPoint.position, Quaternion.Euler(-90, 0, 0));
    }

    public void SpawnStoneWithPositionAndRotation(int stoneId, Vector3 sp, Quaternion rt)
    {
        float scale = GetStoneScaleById(stoneId);
        GameObject obj = Instantiate(LoadObjectFromBundle.sceneStones[stoneId - 1], sp, rt);
        obj.transform.localScale *= scale;
    }

    public void Load()
    {
        //Recover the values
        SaveGame.Load();

        //Set values
        object[] obj = FindObjectsOfType(typeof(GameObject));
        foreach (object o in obj)
        {
            GameObject g = (GameObject)o;

            if (6 < g.name.Length)
            {
                //If stone is in the scene
                if (g.name.Substring(0, 5).Equals("Stone"))
                {
                    int ind = SaveGame.Instance.StonesNames.IndexOf(g.scene.name+g.name);
                    if (ind != -1)
                    {
                        g.transform.position = SaveGame.Instance.StonesPositions[ind];
                        g.transform.rotation = SaveGame.Instance.StonesRotations[ind];
                    }
                    else
                    {
                        Debug.Log("NULL");
                    }
                }

                //If is not
                else if (g.name.Contains("Clone"))
                {
                    string name = g.name;
                    Vector3 pos = new Vector3(g.transform.position.x, g.transform.position.y, g.transform.position.z);
                    Destroy(g);
                    string[] firstSplit = name.Split('_');
                    string[] secondSplit = firstSplit[1].Split('(');
                    string number = secondSplit[0].Substring(5);
                    try
                    {
                        int result = Int32.Parse(number);
                        int i = GetStoneId(result, firstSplit[0]);
                        int ind = SaveGame.Instance.StonesNames.IndexOf(g.name);
                        if (ind != -1)
                        {
                            Vector3 sp = SaveGame.Instance.StonesPositions[ind];
                            Quaternion rt = SaveGame.Instance.StonesRotations[ind];
                            SpawnStoneWithPositionAndRotation(i, sp, rt);
                        }
                        else
                        {
                            Debug.Log("NULL");
                        }
                    }
                    catch (FormatException)
                    {
                        Debug.Log("ERROR");
                        continue;
                    }
                }
            }
        }
    }

    public void Save()
    {
        //Get the actual stone's values
        //Modify SaveGame.Instance.Stones
        SaveGame.Instance.Clear();

        object[] obj = FindObjectsOfType(typeof(GameObject));
        foreach (object o in obj) {
            GameObject g = (GameObject)o;
            if (6 < g.name.Length)
            {
                if (g.name.Contains("Clone"))
                {
                    SaveGame.Instance.StonesNames.Add(g.name);
                    SaveGame.Instance.StonesPositions.Add(g.transform.position);
                    SaveGame.Instance.StonesRotations.Add(g.transform.rotation);
                }

                if (g.name.Substring(0, 5).Equals("Stone"))
                {
                    SaveGame.Instance.StonesNames.Add(g.scene.name + g.name);
                    SaveGame.Instance.StonesPositions.Add(g.transform.position);
                    SaveGame.Instance.StonesRotations.Add(g.transform.rotation);
                }
            }
        }

        //Save values
        SaveGame.Save();
    }

}
