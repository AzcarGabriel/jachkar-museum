using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPointScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        float offsetX = Random.Range(-5, 5);
        float offsetZ = Random.Range(-5, 5);
        if (StaticValues.self_fps != null)
        {
            StaticValues.self_fps.transform.position = transform.position + new Vector3(offsetX, 0, offsetZ);
        }
    }
}
