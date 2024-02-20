using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Networking
{
    public class ServerManager : NetworkBehaviour
    {
        public static ServerManager Instance { get; private set; }

        private bool _gameStarted;

        private ulong _leader = 100;
    
        public struct StoneAssetData
        {
            public int AssetId;
            public GameObject Prefab; 
        }

        public Dictionary<ulong, ClientData> ClientData { get; } = new();
        public Dictionary<int, StoneAssetData> spawnedStones { get; } = new();
        
        private void Awake()
        { 
            if (Instance != null && Instance != this) {
                Destroy(gameObject);
            }
            else {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        public void StartServer()
        {

            if (NetworkManager.Singleton.GetComponent<UnityTransport>().UseEncryption)
            {
                SecureParameters.CheckCertificates();
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetServerSecrets(SecureParameters.MyGameServerCertificate, SecureParameters.MyGameServerPrivateKey);
            }

            NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
            NetworkManager.Singleton.OnServerStarted += OnNetworkReady;
            NetworkManager.Singleton.StartServer();
        }

        public void StartHost()
        { 
            NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
            NetworkManager.Singleton.OnServerStarted += OnNetworkReady;
            NetworkManager.Singleton.StartHost();
        }

        public void StartClient()
        {
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientSecrets(SecureParameters.ServerCommonName);
            NetworkManager.Singleton.StartClient();
        }

        public void AddClientData(string clientUsername, int characterId, ulong clientId, bool isLeader)
        {
            ClientData newClientData = new(clientId)
            {
                clientId =  clientId,
                characterId = characterId,
                username = clientUsername,
                isLeader = isLeader,
            };

            ClientData.Add(clientId, newClientData);
        }

        #region Stones
        
        private GameObject GetStoneInstanceById(int id)
        {
            return spawnedStones[id].Prefab;
        }

        public int GetIdByStone(GameObject stone)
        {
            return spawnedStones.FirstOrDefault(entry => EqualityComparer<GameObject>.Default.Equals(entry.Value.Prefab, stone)).Key;
        }

        public void UpdateTransform(int stoneId, Vector3 newPosition, Quaternion newRotation, Vector3 newScale)
        {
            GameObject stone = GetStoneInstanceById(stoneId);
            stone.transform.position = newPosition;
            stone.transform.rotation = newRotation;
            stone.transform.localScale = newScale;
        }

        public void AddSpawnedStone(int dictId, int assetId, GameObject stone)
        {
            StoneAssetData newData = new() { AssetId = assetId, Prefab = stone };
            spawnedStones.Add(dictId , newData);
        }

        public void RemoveSpawnedStone(int stoneId)
        {
            GameObject stone = GetStoneInstanceById(stoneId);
            Destroy(stone);
            spawnedStones.Remove(stoneId);
        }
        
        #endregion

        public void OpenScene(string scene)
        {
            NetworkManager.Singleton.SceneManager.LoadScene(scene, LoadSceneMode.Single);
        }

        private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
        {
            if (_gameStarted)
            {
                response.Approved = false;
                return;
            }
            
            response.Approved = true; 
            response.CreatePlayerObject = false;
            response.Pending = false;

            ClientData[request.ClientNetworkId] = new ClientData(request.ClientNetworkId);
            
            // Give leader to the first player joining
            if (ClientData.Count == 1)
                _leader = request.ClientNetworkId;
        }

        private void OnNetworkReady()
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
            OpenScene("LobbyTempScene");
        }

        public void StartGame()
        {
            _gameStarted = true;
            OpenScene("OnlineNoradus");
        }

        private void OnClientDisconnect(ulong clientId)
        {
            if (!ClientData.ContainsKey(clientId)) return;
            ClientData.Remove(clientId);
            
            if (ClientData.Count == 0)
                ResetServer();
        }
        
        public void SetCharacter(ulong clientId, int characterId)
        {
            if (ClientData.TryGetValue(clientId, out ClientData data))
                data.characterId = characterId;
        }

        public void SetUsername(ulong clientId, FixedString32Bytes username)
        {
            if (ClientData.TryGetValue(clientId, out ClientData data))
                data.username = username;
        }

        public FixedString32Bytes GetUsername(ulong clientId)
        {
            return ClientData.TryGetValue(clientId, out ClientData data) ? data.username : "";
        }

        public bool GetLeadership(ulong clientId)
        {
            return ClientData.TryGetValue(clientId, out ClientData data) && data.isLeader;
        }

        private void ResetServer()
        {
            _gameStarted = false;
            OpenScene("LobbyTempScene");
        }
    }
}
