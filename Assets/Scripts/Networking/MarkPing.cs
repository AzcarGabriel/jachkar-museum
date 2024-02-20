using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Networking
{
    public class MarkPing : NetworkBehaviour
    {
    
        [SerializeField] private float pingDuration;
        [SerializeField] private TMP_Text textField;

        private readonly NetworkVariable<FixedString32Bytes> _playerName = new("");
        
        public override void OnNetworkSpawn() {
            Invoke(nameof(DestroyPingServerRPC), pingDuration);
            if (IsServer)
            {
                _playerName.Value = ServerManager.Instance.ClientData[OwnerClientId].username; 
            }
            textField.text = $"<color=grey>{_playerName.Value}</color>";
        }

        private void Update()
        {
            if (!IsClient) return;
            Camera mainCamera = Camera.main;
            if (mainCamera == null) return;
            var cameraPosition = mainCamera.transform.position;
            Vector3 target = new Vector3(cameraPosition.x, transform.position.y, cameraPosition.z);
            transform.LookAt(target, Vector3.up);
            transform.rotation = Quaternion.LookRotation(transform.position - cameraPosition);
        }
        
        [ServerRpc(RequireOwnership = false)]
        public void DestroyPingServerRPC() {
            Destroy(gameObject);
        }
    }
}
