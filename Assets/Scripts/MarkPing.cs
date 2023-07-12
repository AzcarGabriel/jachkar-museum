using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

using TMPro;


public class MarkPing : NetworkBehaviour
{
    
    [SerializeField] private float pingDuration;
    [SerializeField] private TMP_Text textField;

    private FixedString32Bytes playerName = "";

    public override void OnNetworkSpawn() {

        Invoke("DestroyPingServerRPC", pingDuration);
    }

    private void Update() {
        Camera camera = Camera.main;
        Vector3 target = new Vector3(camera.transform.position.x, transform.position.y, camera.transform.position.z);
        transform.LookAt(target, Vector3.up); // Look to the opposite direction
        textField.text = $"<color=grey>{playerName}</color>";
    }

    public void setPlayerName(FixedString32Bytes playerName) {
        Debug.Log("Player name is" + playerName);
        this.playerName = playerName;
    }

    [ServerRpc(RequireOwnership = false)]
    public void DestroyPingServerRPC() {
        Destroy(gameObject);
    }
}
