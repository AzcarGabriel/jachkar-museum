/*
    SceneScripts.cs
    
    @author Gabriel Azócar Cárcamo <azocarcarcamo@gmail.com>
 */

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;
using System.Text;
using System.IO;

public class SceneScripts : MonoBehaviour
{
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
    public GameObject loadScreen;
    public Slider slider;
    public LayoutGroup addStoneMenuGrid;

    private bool overwrite = false;
    private StoneService stoneService;

    // Use this for initialization
    void Start()
    {
        stoneService = gameObject.AddComponent<StoneService>();
        stoneService.loadScreen = this.loadScreen;
        stoneService.slider = this.slider;

        hideButton.SetActive(false);
        StaticValues.previos_scene = SceneManager.GetActiveScene().name;
        Cursor.lockState = CursorLockMode.Locked;
        
        if (StaticValues.back_from_details)
        {
            StaticValues.back_from_details = false;
        }

        if (StonesValues.stonesThumbs.Count == 0)
        {
            StartCoroutine(this.stoneService.DownloadThumbs(this.constructAddStoneMenu));
        } 
        else
        {
            this.constructAddStoneMenu();
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

    public void constructAddStoneMenu()
    {
        foreach (Sprite sprite in StonesValues.stonesThumbs)
        {
            GameObject thumb = new GameObject();
            thumb.transform.SetParent(this.addStoneMenuGrid.transform);
            thumb.AddComponent<RectTransform>();
            thumb.AddComponent<CanvasRenderer>();
            Image image = thumb.AddComponent<Image>();
            image.sprite = sprite;
            Button button = thumb.AddComponent<Button>();
            button.onClick.AddListener(() => SpawnStone(Int32.Parse(sprite.name)));
            thumb.AddComponent<LayoutElement>();

            // Scroll
            // Vector3 position = this.addStoneMenuGrid.transform.position;
            // this.addStoneMenuGrid.transform.localPosition = new Vector3(11, -2740, 0);
        }
    }

    public void SpawnStone(int stoneId)
    {
        Quaternion rt = StoneSpawnHelper.GetStoneRotationById(stoneId);
        Vector3 sp = spawnPoint.position + StoneSpawnHelper.GetStoneSpawnPointOffsetById(stoneId);
        StartCoroutine(this.stoneService.SpawnStoneWithPositionAndRotation(stoneId, sp, rt));
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

        // Recover the values
        SaveGame.Load(f_name);

        // Clear stones in scene
        object[] obj = FindObjectsOfType(typeof(GameObject));
        foreach (object o in obj)
        {
            GameObject g = (GameObject)o;

            if (6 < g.name.Length)
            {
                if (g.name.Substring(0, 5).Equals("Stone"))
                {
                    Destroy(g);
                }
            }
        }

        // Load stones in scene
        for (int i = 0; i < SaveGame.Instance.StonesNames.Count; i++)
        {
            StartCoroutine(
                this.stoneService.SpawnStoneWithPositionAndRotation(
                    SaveGame.Instance.StonesNames[i],
                    SaveGame.Instance.StonesPositions[i],
                    SaveGame.Instance.StonesRotations[i]
                )
            );
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

        // Get the actual stone's values
        // Modify SaveGame.Instance.Stones
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
                if (g.name.Substring(0, 5).Equals("Stone"))
                {
                    SaveGame.Instance.StonesNames.Add(this.GetStoneIndex(g.name));
                    SaveGame.Instance.StonesPositions.Add(g.transform.position);
                    SaveGame.Instance.StonesRotations.Add(g.transform.rotation);
                }
            }
        }

        // Save values
        SaveGame.Save(f_name);

        saveDialog.enabled = false;
        saveInputField.text = "";
    }

    public int GetStoneIndex(string stoneName)
    {
        string clean = stoneName.Replace("Stone", "");
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < clean.Length; i++)
        {
            char c = clean[i];
            if (int.TryParse(c.ToString(), out _))
            {
                sb.Append(c);
            }
            else
            {
                break;
            }
        }

        return int.Parse(sb.ToString());
    }
}
