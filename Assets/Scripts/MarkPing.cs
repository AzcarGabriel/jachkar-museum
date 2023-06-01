using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class MarkPing : NetworkBehaviour
{
    
    [SerializeField] private float pingDuration;
    [SerializeField] Image img;

    public override void OnNetworkSpawn() {

        Invoke("DestroyPing", pingDuration);
        Debug.Log("Spawned");
        Destroy(gameObject, pingDuration);
    }

    private void Update() {
        NetworkManager networkManager = NetworkManager.Singleton;
        PlayerObject playerObject = networkManager.ConnectedClients[OwnerClientId].PlayerObject.GetComponent<PlayerObject>();
        Camera camera = playerObject.playerCamera;
        transform.LookAt(camera.transform.position, Vector3.up);
    }

    public void DestroyPing() {
        Debug.Log("Destruir");
        Destroy(gameObject);
    }
}
