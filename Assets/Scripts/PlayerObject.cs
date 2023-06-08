using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using FMGames.Scripts.Menu.Chat;
using Unity.Collections;

public class PlayerObject : NetworkBehaviour
{
    private NetworkVariable<FixedString32Bytes> playerName = new NetworkVariable<FixedString32Bytes>("", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public FixedString32Bytes PlayerName => playerName.Value;
    public Camera playerCamera;

    public override void OnNetworkSpawn() {
        playerCamera = GetComponentInChildren<Camera>();
        Debug.Log(playerCamera);
        if (IsOwner) {
            playerCamera.enabled = true;
            playerName.Value = ChatBehaviour.username;
            Debug.Log(playerCamera);
            Debug.Log("Enabled");
        } else {
            playerCamera.enabled = false;
            Debug.Log(playerCamera);
            Debug.Log("Disabled");
        }
    }



    [ServerRpc(RequireOwnership = false)]
    public void UpdatePlayerNameServerRpc(FixedString32Bytes newName) {
        playerName.Value = newName;
    }
}
