using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Collections;

[RequireComponent(typeof(StoneService))]
public class ClientConnectBehaviour : NetworkBehaviour
{

    [SerializeField] private CharacterDatabase characterDatabase;
    [SerializeField] private GameObject loadScreen;
    private StoneService stoneDownload;

    private struct StoneData
    {
        public int dictId;
        public int stoneId;
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;
    }

    private List<StoneData> stonesToDownload = new();

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
        if (!IsServer) RequestStonesServerRpc();

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
        ClientRpcParams clientRpcParams = new()
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
            StoreStonesToDownloadClientRpc(entry.Key, stoneAssetId, prefab.transform.position, prefab.transform.rotation, prefab.transform.localScale, clientRpcParams);
        }
        SpawnStoneClientRpc(clientRpcParams);
    }

    [ClientRpc]
    public void StoreStonesToDownloadClientRpc(int dictId, int stoneId, Vector3 sp, Quaternion rt, Vector3 sc, ClientRpcParams clientRpcParams = default)
    {
        stonesToDownload.Add(new() { dictId = dictId, stoneId = stoneId, position = sp, rotation = rt, scale = sc });
    }


    [ClientRpc]
    public void SpawnStoneClientRpc(ClientRpcParams clientRpcParams = default)
    {
        StartCoroutine(DownloadThumbs());
    }

   public IEnumerator DownloadThumbs()
    {
        foreach(StoneData data in stonesToDownload)
        {
            yield return StartCoroutine(stoneDownload.SpawnStoneWithPositionRotationScale(data.dictId, data.stoneId, data.position, data.rotation, data.scale));
        }

    }

}
