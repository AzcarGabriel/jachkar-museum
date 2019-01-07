using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIScripts : MonoBehaviour {

    [SerializeField]
    public Transform spawnPoint;
    public bool scale;
   



    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    int incr = -1;
    public void SpawnStone(int stoneId) {
        if (scale)
        {
        	Debug.Log(LoadObjectFromBundle.sceneStones.Count);
            GameObject obj = Instantiate(LoadObjectFromBundle.sceneStones[stoneId - 1], spawnPoint.position, Quaternion.Euler(-90, 0, 0));
            obj.transform.localScale += new Vector3(150.0f, 150.0f, 150.0f);
        }
        else
        {
        	Debug.Log(LoadObjectFromBundle.sceneStones.Count);
            GameObject g = LoadObjectFromBundle.sceneStones[stoneId - 1];
            Instantiate(g, spawnPoint.position, Quaternion.Euler(-90, 0, 0));
        }
    }

    public void Load() {
        //Recover the values
        SaveGame.Load();
        Debug.Log(SaveGame.Instance.Stones.Count);

        //Set values
        object[] obj = FindObjectsOfType(typeof(GameObject));
        foreach (object o in obj)
        {
            GameObject g = (GameObject)o;

            if (6 < g.name.Length)
            {
                if (g.name.Substring(0, 5).Equals("Stone"))
                {
                    SaveGame.SavedStone sStone = SearchByName(SaveGame.Instance.Stones, g.name);
                    if (sStone != null)
                    {
                        g.transform.position = sStone.GetPosition();
                    }
                    else {
                        Debug.Log("NULL");
                    }
                }
            }
        }
    }

    public void Save() {
        //Get the actual stone's values
        //Modify SaveGame.Instance.Stones
        SaveGame.Instance.Stones.Clear();
        object[] obj = FindObjectsOfType(typeof(GameObject));
        foreach (object o in obj) {
            GameObject g = (GameObject)o;

            if (6 < g.name.Length)
            {
                if (g.name.Substring(0, 5).Equals("Stone"))
                {
                    if (g.name.Equals("Stone01"))
                        Debug.Log(g.transform.position);
                    SaveGame.Instance.Stones.Add(new SaveGame.SavedStone(g.name, g.transform.position, g.transform.rotation));
                }
            }
        }

        Debug.Log(SaveGame.Instance.Stones.Count);
        //Save values
        SaveGame.Save();
    }

    SaveGame.SavedStone SearchByName(List<SaveGame.SavedStone> objs, string n) {

        foreach (SaveGame.SavedStone ss in objs) {
            if (ss.GetName().Equals(n))
                return ss;
        }

        return null;
    }

}
