/*
    UIScripts.cs
    
    @author Gabriel Azócar Cárcamo <azocarcarcamo@gmail.com>
 */

using UnityEngine;
public class UIScripts : MonoBehaviour
{
    [SerializeField]
    public Transform spawnPoint;
    public bool scale;
   
    // Use this for initialization
    void Start()
    {

    }
	
	// Update is called once per frame
	void Update()
    {
	
	}

    int incr = -1;
    public void SpawnStone(int stoneId)
    {
        Debug.Log(LoadObjectFromBundle.sceneStones.Count);
        if (scale)
        {
        	Debug.Log("scale");
            GameObject obj = Instantiate(LoadObjectFromBundle.sceneStones[stoneId - 1], spawnPoint.position, Quaternion.Euler(-90, 0, 0));
            Debug.Log(obj.name);
            obj.transform.localScale += new Vector3(150.0f, 150.0f, 150.0f);
        }
        else
        {
        	Debug.Log("no scale");
            GameObject g = LoadObjectFromBundle.sceneStones[stoneId - 1];
            Debug.Log(g.name);
            Instantiate(g, spawnPoint.position, Quaternion.Euler(-90, 0, 0));
        }
    }

    /*
        0 al 7 EchmiadzinAlly
        8 al 13 Museum
        14 al 26 Noradus
        27 al 45 Noravank
        46 al 58 WallStones
    */
    float GetStoneScale(int stoneId)
    {
        float[] scales = new float[] { 1.0f, 0.5f, 0.5f, 0.55f, 0.5f };
        int index = 0;
        if (0 <= stoneId && stoneId <= 7)
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

    public void SpawnStoneExp(int stoneId)
    {
        float scale = GetStoneScale(stoneId);
        GameObject obj = Instantiate(LoadObjectFromBundle.sceneStones[stoneId - 1], spawnPoint.position, Quaternion.Euler(-90, 0, 0));
        obj.transform.localScale *= scale;
    }
}
