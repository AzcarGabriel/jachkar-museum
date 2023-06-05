using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;

public class MarkPing : NetworkBehaviour
{
    
    [SerializeField] private float pingDuration;
    [SerializeField] private Image img;
    [SerializeField] private TMP_Text textField;

    public string playerName = "";

    public override void OnNetworkSpawn() {

        Invoke("DestroyPing", pingDuration);
        Destroy(gameObject, pingDuration);
    }

    private void Update() {
        Camera camera = Camera.main;
        transform.LookAt(camera.transform.position, Vector3.up);
        textField.text = $"<color=grey>{playerName}</color>";
    }

    public void DestroyPing() {
        Debug.Log("Destruir");
        Destroy(gameObject);
    }
}
