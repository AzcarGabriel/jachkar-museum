using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

namespace Networking.Lobby
{
    public class CharacterSelectDisplay : NetworkBehaviour
    {
        [SerializeField] private CharacterDatabase characterDatabase;
        [SerializeField] private Transform charactersHolder;
        [SerializeField] private CharacterSelectButton selectButtonPrefab;
        [SerializeField] private PlayerCard[] playerCards;
        [SerializeField] private GameObject characterInfoPanel;
        [SerializeField] private TMP_Text characterNameText;
        
        private NetworkList<CharacterSelectState> _players;

        private void Awake()
        {
            _players = new NetworkList<CharacterSelectState>();
        }

        public override void OnNetworkSpawn()
        {
            if (IsClient)
            {
                Character[] allCharacters = characterDatabase.GetAllCharacters();
                Debug.Log(allCharacters);
                foreach (Character character in allCharacters)
                {
                    Debug.Log(character.CharacterName);
                    var selectButtonInstance = Instantiate(selectButtonPrefab, charactersHolder);
                    selectButtonInstance.SetCharacter(this, character);
                }

                _players.OnListChanged += HandlePlayersStateChanged;
            }
            
            if (!IsServer) return;
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;

            foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
            {
                OnClientConnected(client.ClientId);
            }
        }

        public override void OnNetworkDespawn()
        {
            if (IsClient)
            {
                _players.OnListChanged -= HandlePlayersStateChanged;
            }
            
            if (!IsServer) return;
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        }


        private void OnClientConnected(ulong clientId)
        {
            _players.Add(new CharacterSelectState(clientId));
        }

        private void OnClientDisconnected(ulong clientId)
        {
            for (int i = 0 ; i < _players.Count; i++)
            {
                if (_players[i].ClientId != clientId) continue;
                _players.RemoveAt(i);
                break;
            }
        }

        public void Select(Character character)
        {
            characterNameText.text = character.CharacterName;
            characterInfoPanel.SetActive(true);
            
            SelectServerRPC(character.Id);
        }
        
        [ServerRpc(RequireOwnership = false)]
        private void SelectServerRPC(int characterId, ServerRpcParams serverRpcParams = default)
        {
            for (int i = 0; i < _players.Count; i++)
            {
                if (_players[i].ClientId == serverRpcParams.Receive.SenderClientId)
                {
                    _players[i] = new CharacterSelectState(_players[i].ClientId, characterId);
                }
            }
        }

        private void HandlePlayersStateChanged(NetworkListEvent<CharacterSelectState> changeEvent)
        {
            for (int i = 0; i < playerCards.Length; i++)
            {
                if (_players.Count > i)
                {
                    playerCards[i].UpdateDisplay(_players[i]);
                }
                else
                {
                    playerCards[i].DisableDisplay();
                }
            }
        }
    }
}
