using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Networking
{
    public class PlayerObject : NetworkBehaviour
    {
        private readonly NetworkVariable<FixedString32Bytes> _playerName = new();
        private readonly NetworkVariable<bool> _isLeader = new();

        public FixedString32Bytes PlayerName
        {
            get => _playerName.Value.ToString();
            set => _playerName.Value = value;
        }
    
        private Camera _playerCamera;
        private AudioListener _playerAudioListener;

        [SerializeField] private GameObject playerModel;
        [SerializeField] private TMP_Text nameTag; 
    

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            { 
                PlayerName = ServerManager.Instance.GetUsername(OwnerClientId);
                _isLeader.Value = ServerManager.Instance.GetLeadership(OwnerClientId);
            }
            nameTag.text = PlayerName.ToString();
        
            _playerCamera = GetComponentInChildren<Camera>();
            _playerAudioListener = GetComponentInChildren<AudioListener>();
            playerModel.SetActive(true);
            if (IsOwner)
            {
                _playerCamera.enabled = true;
                _playerAudioListener.enabled = true;
                //playerModel.SetActive(false);
                playerModel.transform.localScale = Vector3.zero;
                StaticValues.SelfFPS = transform.gameObject;
            } 
            else
            { 
                _playerCamera.enabled = false;
                _playerAudioListener.enabled = false;
                playerModel.SetActive(true);
            }
        }
    }
}
