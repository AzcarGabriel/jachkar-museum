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

}
