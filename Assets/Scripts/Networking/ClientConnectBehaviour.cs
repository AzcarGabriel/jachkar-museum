using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(StoneService))]
public class ClientConnectBehaviour : NetworkBehaviour
{

    [SerializeField] private CharacterDatabase characterDatabase;
    [SerializeField] private GameObject loadScreen;
    private StoneService stoneDownload;


    private void Awake()
    {
        stoneDownload = GetComponent<StoneService>();
        stoneDownload.loadScreen = this.loadScreen;
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        // Spawning selected character
        int characterId = ServerManager.Instance.preSelectedCharacter;
        string username = ServerManager.Instance.username;
        SpawnPlayerCharacterServerRpc(characterId);
        AddPlayerServerRpc(username, characterId);

        // Getting all already spawned stones
        Debug.Log("Rquesting stones to server...");
        RequestStonesServerRpc();

        base.OnNetworkSpawn();
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnPlayerCharacterServerRpc(int characterId, ServerRpcParams serverRpcParams = default)
    {
        var character = characterDatabase.GetCharacterById(characterId);
        if (character != null)
        {
            var characterInstance = Instantiate(character.GameplayPrefab);
            characterInstance.SpawnAsPlayerObject(serverRpcParams.Receive.SenderClientId);
        }
        else
        {
            Debug.LogError("Character was null");
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void AddPlayerServerRpc(string username, int characterId, ServerRpcParams serverRpcParams = default)
    {
        ServerManager.Instance.AddClientData(username, characterId, serverRpcParams.Receive.SenderClientId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestStonesServerRpc(ServerRpcParams serverRpcParams = default)
    {
        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { serverRpcParams.Receive.SenderClientId }
            }
        };

        foreach (KeyValuePair<int, ServerManager.StoneAssetData> entry in ServerManager.Instance.spawnedStones)
        {
            int stoneAssetId = entry.Value.assetId;
            GameObject prefab = entry.Value.prefab;
            SpawnStoneClientRpc(stoneAssetId, prefab.transform.position, prefab.transform.rotation, clientRpcParams);
        }
    }

    [ClientRpc]
    public void SpawnStoneClientRpc(int stoneId, Vector3 sp, Quaternion rt, ClientRpcParams clientRpcParams = default)
    {
        StartCoroutine(stoneDownload.SpawnStoneWithPositionAndRotation(stoneId, sp, rt));
    }
}
