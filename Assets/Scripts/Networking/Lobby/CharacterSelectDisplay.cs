//#define USE_MULTIPLAYER
using System;
using Networking.Character;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
namespace Networking.Lobby
{
#if USE_MULTIPLAYER

    public class CharacterSelectDisplay : NetworkBehaviour
    {
        [SerializeField] private CharacterDatabase characterDatabase;
        [SerializeField] private Transform charactersHolder;
        [SerializeField] private CharacterSelectButton selectButtonPrefab;
        [SerializeField] private PlayerCard[] playerCards;
        [SerializeField] private GameObject characterInfoPanel;
        [SerializeField] private TMP_Text characterNameText;
        [SerializeField] private Button readyButton;
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private GameObject LeaderMark;
        
        private NetworkList<CharacterSelectState> _players;

        private void Awake()
        {
            _players = new NetworkList<CharacterSelectState>();
        }

        private void Start()
        {
            LeaderMark.SetActive(ServerManager.Instance.UseLeader);
        }

        public override void OnNetworkSpawn()
        {
            if (IsClient)
            {
                CharacterData[] allCharacters = characterDatabase.GetAllCharacters();
                foreach (CharacterData character in allCharacters)
                {
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

        [ServerRpc(RequireOwnership = false)]
        private void SetUserNameServerRPC(FixedString32Bytes userName, ServerRpcParams serverRpcParams = default)
        {
            for (int i = 0; i < _players.Count; i++)
            {
                if (_players[i].ClientId != serverRpcParams.Receive.SenderClientId) continue;
                _players[i] = new CharacterSelectState(
                    _players[i].ClientId,
                    _players[i].CharacterId,
                    _players[i].IsReady, 
                    userName
                    );
            }
        }
        
        public void Select(CharacterData character)
        {

            for (int i = 0; i < _players.Count; i++)
            {
                if (_players[i].ClientId != NetworkManager.Singleton.LocalClientId) continue;
                if (_players[i].IsReady) return;
                if (_players[i].CharacterId == character.Id) return;
            }
            
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
                    _players[i] = new CharacterSelectState(_players[i].ClientId, characterId, _players[i].IsReady, _players[i].UserName);
                }
            }
        }
        
        public void LockIn()
        {
            SetUserNameServerRPC(inputField.text);
            LockInServerRPC();
        }
        
        [ServerRpc(RequireOwnership = false)]
        private void LockInServerRPC(ServerRpcParams serverRpcParams = default)
        {
            for (int i = 0; i < _players.Count; i++)
            {
                if (_players[i].ClientId == serverRpcParams.Receive.SenderClientId)
                {
                    _players[i] = new CharacterSelectState(_players[i].ClientId, _players[i].CharacterId,true, _players[i].UserName);
                }
            }

            foreach (CharacterSelectState player in _players)
            {
                if (!player.IsReady) return;
            }

            foreach (CharacterSelectState player in _players)
            {
                ServerManager.Instance.SetCharacter(player.ClientId, player.CharacterId);
                ServerManager.Instance.SetUsername(player.ClientId, player.UserName);
            }

            ServerManager.Instance.StartGame();
        }

        private void HandlePlayersStateChanged(NetworkListEvent<CharacterSelectState> changeEvent)
        {
            for (int i = 0; i < playerCards.Length; i++)
            {
                if (_players.Count > i)
                    playerCards[i].UpdateDisplay(_players[i]);
                else
                    playerCards[i].DisableDisplay();
            }
        }
    }
#else
public class CharacterSelectDisplay : MonoBehaviour
{
    [SerializeField] private CharacterDatabase characterDatabase;
    [SerializeField] private Transform charactersHolder;
    [SerializeField] private CharacterSelectButton selectButtonPrefab;
    [SerializeField] private PlayerCard[] playerCards;
    [SerializeField] private GameObject characterInfoPanel;
    [SerializeField] private TMP_Text characterNameText;
    [SerializeField] private Button readyButton;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private GameObject LeaderMark;
    
    //private NetworkList<CharacterSelectState> _players;

    private void Awake()
    {
        //_players = new NetworkList<CharacterSelectState>();
    }

    private void Start()
    {
        //LeaderMark.SetActive(ServerManager.Instance.UseLeader);
    }

    private void OnClientConnected(ulong clientId)
    {
        //_players.Add(new CharacterSelectState(clientId));
    }

    private void OnClientDisconnected(ulong clientId)
    {
        //for (int i = 0 ; i < _players.Count; i++)
        //{
        //    if (_players[i].ClientId != clientId) continue;
        //    _players.RemoveAt(i);
        //    break;
        //}
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetUserNameServerRPC(FixedString32Bytes userName, ServerRpcParams serverRpcParams = default)
    {
        //for (int i = 0; i < _players.Count; i++)
        //{
        //    if (_players[i].ClientId != serverRpcParams.Receive.SenderClientId) continue;
        //    _players[i] = new CharacterSelectState(
        //        _players[i].ClientId,
        //        _players[i].CharacterId,
        //        _players[i].IsReady, 
        //        userName
        //        );
        //}
    }
    
    public void Select(CharacterData character)
    {

        //for (int i = 0; i < _players.Count; i++)
        //{
        //    if (_players[i].ClientId != NetworkManager.Singleton.LocalClientId) continue;
        //    if (_players[i].IsReady) return;
        //    if (_players[i].CharacterId == character.Id) return;
        //}
        
        characterNameText.text = character.CharacterName;
        characterInfoPanel.SetActive(true);
        
        SelectServerRPC(character.Id);
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void SelectServerRPC(int characterId, ServerRpcParams serverRpcParams = default)
    {
        //for (int i = 0; i < _players.Count; i++)
        //{
        //    if (_players[i].ClientId == serverRpcParams.Receive.SenderClientId)
        //    {
        //        _players[i] = new CharacterSelectState(_players[i].ClientId, characterId, _players[i].IsReady, _players[i].UserName);
        //    }
        //}
    }
    
    public void LockIn()
    {
        SetUserNameServerRPC(inputField.text);
        LockInServerRPC();
    }
    
    private void LockInServerRPC(ServerRpcParams serverRpcParams = default)
    {
        //for (int i = 0; i < _players.Count; i++)
        //{
        //    if (_players[i].ClientId == serverRpcParams.Receive.SenderClientId)
        //    {
        //        _players[i] = new CharacterSelectState(_players[i].ClientId, _players[i].CharacterId,true, _players[i].UserName);
        //    }
        //}
//
        //foreach (CharacterSelectState player in _players)
        //{
        //    if (!player.IsReady) return;
        //}
//
        //foreach (CharacterSelectState player in _players)
        //{
        //    ServerManager.Instance.SetCharacter(player.ClientId, player.CharacterId);
        //    ServerManager.Instance.SetUsername(player.ClientId, player.UserName);
        //}
    
        ServerManager.Instance.SetCharacter(0, 0);
        ServerManager.Instance.StartGame();
    }

}
#endif
}
