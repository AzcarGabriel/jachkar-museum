/*
    SceneScripts.cs
    
    @author Gabriel Azócar Cárcamo <azocarcarcamo@gmail.com>
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Networking;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Cursor = UnityEngine.Cursor;
using Image = UnityEngine.UI.Image;
using Object = UnityEngine.Object;

namespace ScenesScripts
{
    public class SceneScripts : MonoBehaviour
    {

        [SerializeField]
        public Transform spawnPoint;
        public GameObject showButton;
        public GameObject hideButton;
        public GameObject addStoneMenu;
        public GameObject editStoneMenu;
        public GameObject saveDialog;
        public GameObject loadDialog;
        public GameObject overwriteDialog;
        public GameObject availableFiles;
        public InputField saveInputField;
        public InputField loadInputField;
        public VisualElement LoadScreen;
        public LayoutGroup addStoneMenuGrid;
        private bool overwrite;
        private StoneService stoneService;

        [SerializeField] private NetworkStoneSpawner networkStoneSpawner; 

        // Use this for initialization
        void Start() {

            stoneService = gameObject.AddComponent<StoneService>();
            stoneService.LoadScreen = LoadScreen;

            hideButton.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;

            if (StaticValues.BackFromDetails) {
                StaticValues.BackFromDetails = false;
            }

            if (StonesValues.stonesThumbs.Count == 0) {
                StartCoroutine(this.stoneService.DownloadThumbs(this.constructAddStoneMenu));
            }
            else {
                this.constructAddStoneMenu();
            }
        }

        // Update is called once per frame
        void Update() {
            if (Input.GetKeyDown("e") && !StaticValues.Writing) {
                StaticValues.BackFromDetails = true;
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
                button.onClick.AddListener(() => SpawnStone(int.Parse(sprite.name)));
                thumb.AddComponent<LayoutElement>();

                // Scroll
                // Vector3 position = this.addStoneMenuGrid.transform.position;
                // this.addStoneMenuGrid.transform.localPosition = new Vector3(11, -2740, 0);
            }
        }

        public void SpawnStone(int stoneId)
        {
            // Esto se tuvo que cambiar de orden para que no de error (hay que ver como manejar este caso)
            networkStoneSpawner.SpawnStoneServerRpc(stoneId, spawnPoint.position, addOffset: true);
            #if USE_MULTIPLAYER
            if (!StaticValues.IsLeader && ServerManager.Instance.UseLeader) return;
            #endif
        }

        public void ShowMenus()
        {
            addStoneMenu.SetActive(true);
            showButton.SetActive(false);
            hideButton.SetActive(true);
        }

        public void HideMenus()
        {
            addStoneMenu.SetActive(false);
            showButton.SetActive(true);
            hideButton.SetActive(false);
        }

        public void ShowSaveDialog()
        {
            saveDialog.SetActive(true);
        }

        public void ShowLoadDialog()
        {
            loadDialog.SetActive(true);
        }

        public void ShowAvailableFiles()
        {
            availableFiles.SetActive(true);

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
                loadDialog.SetActive(false);
                availableFiles.SetActive(false);
                return;
            }

            string fName = "";
            if (StaticValues.BackFromDetails)
            {
                fName = "temp_data_file";
            }
            else
            {
                fName = loadInputField.text;
            }

            // Recover the values
            SaveGame.Load(fName);

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
                        0, // ESTO DEBERIA GUARDARSE en lugar de estar hardcodeado
                        SaveGame.Instance.StonesNames[i],
                        SaveGame.Instance.StonesPositions[i]
                    )
                );
            }

            loadDialog.SetActive(false);
            availableFiles.SetActive(false);
            loadInputField.text = "";
        }

        public void Save(Boolean cancel)
        {
            if (cancel)
            {
                saveDialog.SetActive(false);
                return;
            }

            string fName = "";
            if (StaticValues.BackFromDetails)
            {
                fName = "temp_data_file";
            }
            else
            {
                fName = saveInputField.text;
            }

            // Get the actual stone's values
            // Modify SaveGame.Instance.Stones
            string filePath = Path.Combine(Application.persistentDataPath, fName + ".json");

            if (File.Exists(filePath) && !StaticValues.BackFromDetails && !overwrite)
            {
                overwriteDialog.SetActive(true);
                saveDialog.SetActive(false);
                overwrite = true;
                return;
            }

            overwrite = false;
            overwriteDialog.SetActive(false);

            SaveGame.Instance.Clear();

            Object[] obj = FindObjectsOfType(typeof(GameObject));
            foreach (var o in obj) {
                GameObject g = (GameObject)o;
                if (6 >= g.name.Length) continue;
                if (!g.name[..5].Equals("Stone")) continue;
                SaveGame.Instance.StonesNames.Add(this.GetStoneIndex(g.name));
                SaveGame.Instance.StonesPositions.Add(g.transform.position);
                SaveGame.Instance.StonesRotations.Add(g.transform.rotation);
            }

            // Save values
            SaveGame.Save(fName);

            saveDialog.SetActive(false);
            saveInputField.text = "";
        }

        private int GetStoneIndex(string stoneName)
        {
            string clean = stoneName.Replace("Stone", "");
            StringBuilder sb = new StringBuilder();
            foreach (var c in clean)
            {
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
}
