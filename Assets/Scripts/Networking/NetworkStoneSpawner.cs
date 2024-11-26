using Unity.Netcode;
using UnityEngine;

namespace Networking
{
    [RequireComponent(typeof(StoneService))]
    public class NetworkStoneSpawner : NetworkBehaviour
    {
        #if USE_MULTIPLAYER
        StoneService _stoneService;

        [SerializeField]
        private Transform stones;
        private void Awake()
        {
            _stoneService = GetComponent<StoneService>();
        }

        public override void OnNetworkSpawn()
        {
            StoreInitialStones();
        }

        private void StoreInitialStones()
        {
            if (stones == null)
            {
                Debug.LogWarning("No initial stones?");
                return;
            }
            foreach(Transform stonesTransform in stones)
            {
                if (IsServer)
                {
                    GameObject stoneObject = stonesTransform.gameObject;
                    int newId = ServerManager.Instance.spawnedStones.Count + 1;
                    ServerManager.Instance.AddSpawnedStone(newId, int.Parse(stoneObject.name.Replace("Stone", "")), stoneObject);
                }
                else
                {
                    Destroy(stonesTransform.gameObject);
                }
            }

        }

        [ServerRpc(RequireOwnership = false)]
        public void SpawnStoneServerRpc(int stoneId, Vector3 sp, Quaternion sp_rot, bool addOffset = false)
        {
            int newId = ServerManager.Instance.spawnedStones.Count + 1;
            if (!IsHost)
            {
                ServerManager.Instance.AddSpawnedStone(newId, stoneId, null);
            }

            SpawnStoneClientRpc(newId, stoneId, sp, sp_rot, addOffset);
        }

        [ClientRpc]
        public void SpawnStoneClientRpc(int dictId, int stoneId, Vector3 sp, Quaternion sp_rot, bool addOffset = false)
        {
            StartCoroutine(_stoneService.SpawnStoneWithPositionAndRotation(dictId, stoneId, sp, sp_rot, addOffset: addOffset));
        }

        [ServerRpc(RequireOwnership = false)]
        public void DeleteStoneServerRpc(int stoneId)
        {
            DeleteStoneClientRpc(stoneId);
        }

        [ClientRpc]
        public void DeleteStoneClientRpc(int stoneId)
        {
            ServerManager.Instance.RemoveSpawnedStone(stoneId);
        }

        public void UpdateStone(int stoneId, Transform tf)
        {
            UpdateStoneServerRpc(stoneId, tf.position, tf.rotation, tf.localScale);
        }

        [ServerRpc(RequireOwnership = false)]
        private void UpdateStoneServerRpc(int stoneId, Vector3 sp, Quaternion rot, Vector3 scl, ServerRpcParams serverRpcParams = default)
        {
            UpdateStoneClientRpc(stoneId, sp, rot, scl, serverRpcParams.Receive.SenderClientId);
        }

        [ClientRpc]
        public void UpdateStoneClientRpc(int stoneId, Vector3 sp, Quaternion rot, Vector3 scl, ulong originalSender, ClientRpcParams clientRpcParams = default)
        {
            if (NetworkManager.Singleton.LocalClientId == originalSender) return;
            ServerManager.Instance.UpdateTransform(stoneId, sp, rot, scl);
        }
        #endif
    }
}
