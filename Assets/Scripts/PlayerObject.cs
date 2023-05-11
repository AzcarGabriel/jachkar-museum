using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerObject : NetworkBehaviour
{
    private NetworkVariable<string> playerName = new NetworkVariable<string>();
    public string PlayerName => playerName.Value;

    [ServerRpc(RequireOwnership = false)]
    public void UpdatePlayerNameServerRpc(string newName) {
        Debug.Log("Aqui");
        playerName.Value = newName;
        OnUpdatePlayerNameClientRpc(playerName.Value);
    }

    [ClientRpc]
    public void OnUpdatePlayerNameClientRpc(string newName) {
        Debug.Log(newName);
    }

    private void Awake() {
        playerName.OnValueChanged += OnPlayerNameChanged;
    }

    private void OnPlayerNameChanged(string oldName, string newName) { }
}
