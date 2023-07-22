using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

using TMPro;


public class MarkPing : NetworkBehaviour
{
    
    [SerializeField] private float pingDuration;
    [SerializeField] private TMP_Text textField;

    private FixedString32Bytes playerName = "";
    private NetworkVariable<FixedString32Bytes> username = new NetworkVariable<FixedString32Bytes>();

    public override void OnNetworkSpawn() {
        Invoke("DestroyPingServerRPC", pingDuration);

        if (IsServer) { // if this doesn't work in a dedicated server try to update it by itself with a server rpc
            username.Value = playerName;
        }
    }

    private void Update() {
        Camera camera = Camera.main;
        Vector3 target = new Vector3(camera.transform.position.x, transform.position.y, camera.transform.position.z);
        transform.LookAt(target, Vector3.up);
        transform.rotation = Quaternion.LookRotation(transform.position - camera.transform.position);

        textField.text = $"<color=grey>{username.Value}</color>";
    }

    public void setPlayerName(FixedString32Bytes playerName) {
        Debug.Log("Player name is" + playerName);
        this.playerName = playerName;
    }

    [ServerRpc(RequireOwnership = false)]
    public void DestroyPingServerRPC() {
        Destroy(gameObject);
    }

    [ServerRpc]
    public void updateUsernameServerRpc(ServerRpcParams serverRpcParams = default) {
        username.Value = ServerManager.Instance.ClientData[serverRpcParams.Receive.SenderClientId].username;
    }
}
