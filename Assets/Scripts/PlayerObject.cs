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
    private Camera playerCamera;
    private AudioListener playerAudioListener;

    [SerializeField]
    private GameObject playerModel;
    

    public override void OnNetworkSpawn() {
        playerCamera = GetComponentInChildren<Camera>();
        playerAudioListener = GetComponentInChildren<AudioListener>();
        
        
        if (IsOwner) {
            playerCamera.enabled = true;
            playerAudioListener.enabled = true;
            //playerModel.SetActive(false);
            playerName.Value = ChatBehaviour.username;
        } else {
            playerCamera.enabled = false;
            playerAudioListener.enabled = false;
            playerModel.SetActive(true);
        }
    }



    [ServerRpc(RequireOwnership = false)]
    public void UpdatePlayerNameServerRpc(FixedString32Bytes newName) {
        playerName.Value = newName;
    }
}
