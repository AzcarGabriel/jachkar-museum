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
        if (!IsOwner) return;
        
        playerName.Value = ChatBehaviour.username;

        if (IsLocalPlayer) {
            playerCamera = GetComponentInChildren<Camera>();
            playerCamera.enabled = true;
        }
    }



    [ServerRpc(RequireOwnership = false)]
    public void UpdatePlayerNameServerRpc(FixedString32Bytes newName) {
        Debug.Log("Aqui");
        playerName.Value = newName;
        OnUpdatePlayerNameClientRpc(playerName.Value);
    }

    [ClientRpc]
    public void OnUpdatePlayerNameClientRpc(FixedString32Bytes newName) {
        Debug.Log(newName);
    }

    private void Awake() {
        playerName.OnValueChanged += OnPlayerNameChanged;
    }

    private void OnPlayerNameChanged(FixedString32Bytes oldName, FixedString32Bytes newName) { }
}
