using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

namespace Networking
{
    [RequireComponent(typeof(StoneService))]
    public class ClientConnectBehaviour : NetworkBehaviour
    {

        [SerializeField] private CharacterDatabase characterDatabase;
        [SerializeField] private VisualElement _loadScreen;
        private StoneService _stoneDownload;

        private struct StoneData
        {
            public int DictId;
            public int StoneId;
            public Vector3 Position;
            public Quaternion Rotation;
            public Vector3 Scale;
        }

        private readonly List<StoneData> _stonesToDownload = new();

        private void Awake()
        {
            _stoneDownload = GetComponent<StoneService>();
            _stoneDownload.LoadScreen = _loadScreen;
        }

        public override void OnNetworkSpawn()
        {
            if (StaticValues.OfflineMode)
            {
                AddPlayerServerRpc("", 0);
                SpawnPlayerCharacterServerRpc(0);
            }
            
            base.OnNetworkSpawn();
            
            if (!IsOwner) return;

            // Spawning selected character
            //int characterId = ServerManager.Instance.preSelectedCharacter;
            //string username = ServerManager.Instance.username;
            //AddPlayerServerRpc(username, characterId);
            //SpawnPlayerCharacterServerRpc(characterId);
            
            // Getting all already spawned stones
            if (!IsServer) RequestStonesServerRpc();

            
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
                int stoneAssetId = entry.Value.AssetId;
                GameObject prefab = entry.Value.Prefab;
                StoreStonesToDownloadClientRpc(entry.Key, stoneAssetId, prefab.transform.position, prefab.transform.rotation, prefab.transform.localScale, clientRpcParams);
            }
            SpawnStoneClientRpc(clientRpcParams);
        }

        [ClientRpc]
        public void StoreStonesToDownloadClientRpc(int dictId, int stoneId, Vector3 sp, Quaternion rt, Vector3 sc, ClientRpcParams clientRpcParams = default)
        {
            _stonesToDownload.Add(new() { DictId = dictId, StoneId = stoneId, Position = sp, Rotation = rt, Scale = sc });
        }


        [ClientRpc]
        public void SpawnStoneClientRpc(ClientRpcParams clientRpcParams = default)
        {
            StartCoroutine(DownloadThumbs());
        }

        private IEnumerator DownloadThumbs()
        {
            foreach(StoneData data in _stonesToDownload)
            {
                yield return StartCoroutine(_stoneDownload.SpawnStoneWithPositionRotationScale(data.DictId, data.StoneId, data.Position, data.Rotation, data.Scale));
            }

        }

    }
}
