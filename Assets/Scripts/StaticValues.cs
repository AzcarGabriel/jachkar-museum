using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StaticValues : MonoBehaviour {
    public static string stone_name = "";

    void Start() {

    }

    void Update() {
        if (Input.GetKeyDown("t")) {
            Debug.Log(stone_name);
        }
    }

}
