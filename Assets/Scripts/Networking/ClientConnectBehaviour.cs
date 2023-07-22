using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ClientConnectBehaviour : NetworkBehaviour
{

    [SerializeField] private CharacterDatabase characterDatabase;

    public override void OnNetworkSpawn() {
        if (!IsOwner) return;
        Debug.Log("Conectado: " + OwnerClientId);
        int characterId = ServerManager.Instance.preSelectedCharacter;
        string username = ServerManager.Instance.username;
        Debug.Log("Character id: " + characterId);
        spawnPlayerCharacterServerRpc(characterId);
        addPlayerServerRpc(username, characterId);
        base.OnNetworkSpawn();
    }

    [ServerRpc(RequireOwnership = false)]
    private void spawnPlayerCharacterServerRpc(int characterId, ServerRpcParams serverRpcParams = default) {
        Debug.Log("Server RPC con id " + characterId);
        var character = characterDatabase.GetCharacterById(characterId);
        if (character != null) {
            var characterInstance = Instantiate(character.GameplayPrefab);
            characterInstance.SpawnAsPlayerObject(serverRpcParams.Receive.SenderClientId);
        }
        else {
            Debug.LogError("Character was null");
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void addPlayerServerRpc(string username, int characterId, ServerRpcParams ServerRpcParams = default) {
        ServerManager.Instance.addClientData(username, characterId, ServerRpcParams.Receive.SenderClientId);
    }

}
