/*
    SceneScripts.cs
    
    Gabriel Azocar
 */

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

public class SceneScripts : MonoBehaviour {

    [SerializeField]
    public Transform spawnPoint;
    public GameObject showButton;
    public GameObject hideButton;
    public Canvas addStoneMenu;
    public Canvas editStoneMenu;
    public Canvas saveDialog;
    public Canvas loadDialog;
    public Canvas overwriteDialog;
    public Canvas availableFiles;
    public InputField saveInputField;
    public InputField loadInputField;
    private bool overwrite = false;

    // Use this for initialization
    void Start()
    {
        hideButton.SetActive(false);
        StaticValues.previos_scene = SceneManager.GetActiveScene().name;

        if (StaticValues.back_from_details)
        {
            //Load(false);
            StaticValues.back_from_details = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("e") && !StaticValues.writing)
        {
            StaticValues.back_from_details = true;
            Save(false);
        }
    }

    public void SpawnStone(int stoneId)
    {
        Quaternion rt = StoneSpawnHelper.GetStoneRotationById(stoneId);
        Vector3 sp = spawnPoint.position + StoneSpawnHelper.GetStoneSpawnPointOffsetById(stoneId);
        SpawnStoneWithPositionAndRotation(stoneId, sp, rt);
    }

    public void SpawnStoneWithPositionAndRotation(int stoneId, Vector3 sp, Quaternion rt)
    {
        float scale = StoneSpawnHelper.GetStoneScaleById(stoneId);
        GameObject obj = Instantiate(LoadObjectFromBundle.sceneStones[stoneId - 1], sp, rt);
        obj.transform.localScale *= scale;
    }

    public void ShowMenus()
    {
        addStoneMenu.enabled = true;
        showButton.SetActive(false);
        hideButton.SetActive(true);
    }

    public void HideMenus()
    {
        addStoneMenu.enabled = false;
        showButton.SetActive(true);
        hideButton.SetActive(false);
    }

    public void ShowSaveDialog()
    {
        saveDialog.enabled = true;
    }

    public void ShowLoadDialog()
    {
        loadDialog.enabled = true;
    }

    public void ShowAvailableFiles()
    {
        availableFiles.enabled = true;

        List<string> jsonFiles = new List<string>();
        foreach (string file in System.IO.Directory.GetFiles(Application.persistentDataPath))
        {
            if (file.Contains(".json"))
            {
                string[] f = file.Split('\\');
                string fa = f[f.Length - 1];
                jsonFiles.Add(fa);
            }
        }

        string ans = "";
        foreach (string x in jsonFiles)
        {
            ans = ans + x.Split('.')[0] + "\n";
        }

        Text t = availableFiles.GetComponent<Canvas>().GetComponentInChildren<Text>();
        t.text = ans;
        
    }

    public void Load(Boolean cancel)
    {
        if (cancel)
        {
            loadDialog.enabled = false;
            availableFiles.enabled = false;
            return;
        }

        string f_name = "";
        if (StaticValues.back_from_details)
        {
            f_name = "temp_data_file";
        }
        else
        {
            f_name = loadInputField.text;
        }

        //Recover the values
        SaveGame.Load(f_name);

        //Set values
        object[] obj = FindObjectsOfType(typeof(GameObject));
        List<string> done = new List<string>();

        //If stone is in the scene
        foreach (object o in obj)
        {
            GameObject g = (GameObject)o;

            if (6 < g.name.Length)
            {
                if (g.name.Substring(0, 5).Equals("Stone"))
                {
                    int ind = SaveGame.Instance.StonesNames.IndexOf(g.name);
                    if (ind != -1)
                    {
                        g.transform.position = SaveGame.Instance.StonesPositions[ind];
                        g.transform.rotation = SaveGame.Instance.StonesRotations[ind];
                        done.Add(g.name);
                    }
                    else
                    {
                        Debug.Log("STONE NOT IN LOAD FILE " + g.name);
                    }
                }
                else if (g.name.Contains("Clone"))
                {
                    int ind = SaveGame.Instance.StonesNames.IndexOf(g.name);
                    if (ind != -1)
                    {
                        g.transform.position = SaveGame.Instance.StonesPositions[ind];
                        g.transform.rotation = SaveGame.Instance.StonesRotations[ind];
                        done.Add(g.name);
                    }
                    else
                    {
                        Debug.Log("CLONE NOT IN LOAD FILE " + g.name);
                    }
                }
            }
        }

        //If not
        foreach (string name in SaveGame.Instance.StonesNames)
        {
            if (!done.Contains(name))
            {
                try
                {
                    int ind = SaveGame.Instance.StonesNames.IndexOf(name);
                    if (ind != -1)
                    {
                        Vector3 sp = SaveGame.Instance.StonesPositions[ind];
                        Quaternion rt = SaveGame.Instance.StonesRotations[ind];
                        string string_sID = name.Substring(5).Split('(')[0];
                        Debug.Log(string_sID);
                        int sID = Int32.Parse(string_sID);
                        SpawnStoneWithPositionAndRotation(sID, sp, rt);
                    }
                    else
                    {
                        Debug.Log("NULL SPAWN " + name);
                    }
                }
                catch (FormatException)
                {
                    Debug.Log("ERROR");
                    continue;
                }
            }            
        }

        loadDialog.enabled = false;
        availableFiles.enabled = false;
        loadInputField.text = "";
    }

    public void Save(Boolean cancel)
    {
        if (cancel)
        {
            saveDialog.enabled = false;
            return;
        }

        string f_name = "";
        if (StaticValues.back_from_details)
        {
            f_name = "temp_data_file";
        }
        else
        {
            f_name = saveInputField.text;
        }

        //Get the actual stone's values
        //Modify SaveGame.Instance.Stones
        string filePath = Path.Combine(Application.persistentDataPath, f_name + ".json");

        if (File.Exists(filePath) && !StaticValues.back_from_details && !overwrite)
        {
            overwriteDialog.enabled = true;
            saveDialog.enabled = false;
            overwrite = true;
            return;
        }

        overwrite = false;
        overwriteDialog.enabled = false;

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
                    SaveGame.Instance.StonesNames.Add(g.name);
                    SaveGame.Instance.StonesPositions.Add(g.transform.position);
                    SaveGame.Instance.StonesRotations.Add(g.transform.rotation);
                }
            }
        }

        //Save values
        SaveGame.Save(f_name);

        saveDialog.enabled = false;
        saveInputField.text = "";
    }

}
