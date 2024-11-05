//#define USE_MULTIPLAYER
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

#if USE_MULTIPLAYER

using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
namespace Networking
{
    public class ServerManager : NetworkBehaviour
    {
        public static ServerManager Instance { get; private set; }
        private readonly NetworkVariable<bool> _useLeader = new(false);
        public bool UseLeader
        {
            get => _useLeader.Value;
            set => _useLeader.Value = value;
        }

        private bool _gameStarted;
        private ulong _leader;
    
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
            if (ClientData.Count == 1 && UseLeader)
                ClientData[request.ClientNetworkId].isLeader = true;
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
            OpenScene(StaticValues.OfflineMode ? "Noradus" : "OnlineNoradus");
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
            return ClientData.TryGetValue(clientId, out var data) && data.isLeader;
        }

        private void ResetServer()
        {
            _gameStarted = false;
            OpenScene("LobbyTempScene");
        }
    }
}

#else

public class ServerManager : MonoBehaviour
{
    public static ServerManager Instance { get; private set; }
    //private bool _gameStarted;
    private ulong _leader;
    
    public struct StoneAssetData
    {
        public int AssetId;
        public GameObject Prefab; 
    }
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
        OpenScene("LobbyTempScene");
    }

    public void StartHost()
    { 
        OpenScene("LobbyTempScene");
    }

    public void StartClient()
    {
        //NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientSecrets(SecureParameters.ServerCommonName);
        //NetworkManager.Singleton.StartClient();
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
        SceneManager.LoadScene(scene, LoadSceneMode.Single);
    }

    public void StartGame()
    {
        //_gameStarted = true;
        OpenScene("Noradus");
        //CharacterSpawner.SpawnCharacter();
    }

    private void OnClientDisconnect(ulong clientId)
    {
        //if (!ClientData.ContainsKey(clientId)) return;
        //ClientData.Remove(clientId);
        //
        //if (ClientData.Count == 0)
        //    ResetServer();
    }
    
    public void SetCharacter(ulong clientId, int characterId)
    {
        Debug.Log("Setting character");
        Debug.Log("Client id: " + clientId);
        Debug.Log("Character id: " + characterId);
        var spawnPos = new Vector3(Random.Range(-3f, 3f), 0f, Random.Range(-3f, 3f));
        GameObject characterPrefab = Instantiate(Resources.Load("FPSControllerOLD"), spawnPos, Quaternion.identity) as GameObject;
        DontDestroyOnLoad(characterPrefab);
        //if (ClientData.TryGetValue(clientId, out ClientData data))
        //    data.characterId = characterId;
    }

    public void SetUsername(ulong clientId, FixedString32Bytes username)
    {
        //if (ClientData.TryGetValue(clientId, out ClientData data))
        //    data.username = username;
    }

    public FixedString32Bytes GetUsername(ulong clientId)
    {
        return "aa"; //ClientData.TryGetValue(clientId, out ClientData data) ? data.username : "";
    }

    public bool GetLeadership(ulong clientId)
    {
        return true; //ClientData.TryGetValue(clientId, out var data) && data.isLeader;
    }

    private void ResetServer()
    {
        //_gameStarted = false;
        OpenScene("LobbyTempScene");
    }
}

#endif
