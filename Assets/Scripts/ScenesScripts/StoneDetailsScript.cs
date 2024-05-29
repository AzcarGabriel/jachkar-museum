/*
    StoneDetailsScript.cs
    
    @author Gabriel Azócar Cárcamo <azocarcarcamo@gmail.com>
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Slider = UnityEngine.UI.Slider;

namespace ScenesScripts
{
    public class StoneDetailsScript : MonoBehaviour
    {
        [SerializeField]
        public List<Text> metaText;
        public GameObject scrollView;
        public GameObject mainCamera;
        public GameObject stone;
        public VisualElement LoadScreen;
        public Slider slider;
        private StoneService stoneService;

        // Use this for initialization
        void Start()
        {
            stoneService = gameObject.AddComponent<StoneService>();
            stoneService.LoadScreen = LoadScreen;

            string[] firstSplit = StaticValues.StoneName.Split('(');
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
                StaticValues.ShouldLock = true;
                StaticValues.ShouldEnable = true;
                Destroy(stone);
                SceneManager.UnloadSceneAsync("StoneDetails");
            
            }
        }

        public void SpawnStone(int stoneId)
        {
            Vector3 pos = this.stone.transform.position;
            Debug.Log("Spawning stone " + stoneId);
            StartCoroutine(this.stoneService.SpawnStoneWithPositionAndRotation(-1, stoneId, pos, FinishConfig, true, addOffset: true));
        }

        public void FinishConfig()
        {
            FindStoneObject("detailStone");
            loadJSON(StaticValues.StoneName);
        }

        IEnumerator loadJSON(string stoneName)
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
            Khachkar khachkar = JsonUtility.FromJson<Khachkar>(metadata.text);
            metaText[0].text = khachkar.conditionOfPreservation;
            metaText[1].text = khachkar.importantFeatures;
            metaText[2].text = khachkar.location;
            metaText[4].text = "Accessibility: " + khachkar.accessibility;
            metaText[6].text = "Production Period: " + khachkar.productionPeriod;
            
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
}
