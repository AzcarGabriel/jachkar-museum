using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using System;
using System.Linq;
using Unity.Netcode.Transports.UTP;

public class ServerManager : NetworkBehaviour
{

    [SerializeField] private CharacterDatabase characterDatabase;
    public static ServerManager Instance { get; private set; }


    public struct StoneAssetData
    {
        public int AssetId;
        public GameObject Prefab; 
    }

    public Dictionary<ulong, ClientData> ClientData { get; } = new();
    public Dictionary<int, StoneAssetData> spawnedStones { get; } = new();
    
    public int preSelectedCharacter = -1;
    public string username = "name";

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

        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck; // Not necessary yet, but here you could add a player limit
        NetworkManager.Singleton.StartServer();
        NetworkManager.Singleton.SceneManager.SetClientSynchronizationMode(LoadSceneMode.Single);
        NetworkManager.Singleton.SceneManager.LoadScene("Noradus", LoadSceneMode.Single);
    }

    public void StartHost()
    { 
        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
        NetworkManager.Singleton.StartHost();
        NetworkManager.Singleton.SceneManager.SetClientSynchronizationMode(LoadSceneMode.Single);
    }

    public void StartClient()
    {
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientSecrets(SecureParameters.ServerCommonName);
        NetworkManager.Singleton.StartClient();
    }

    public void AddClientData(string clientUsername, int characterId, ulong clientId)
    {
        ClientData newClientData = new(clientId)
        {
            clientId =  clientId,
            characterId = characterId,
            username = clientUsername
        };

        ClientData.Add(clientId, newClientData);
    }

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

    public void OpenScene(string name)
    {
        string[] tokens = name.Split('/');
        string n;
        if (1 < tokens.Length)
        {
            n = tokens[1];
        }
        else
        {
            n = name;
        }

        NetworkManager.Singleton.SceneManager.LoadScene(n, LoadSceneMode.Single);
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    { 
        response.Approved = true; // accept everything for the moment
        response.CreatePlayerObject = true;
        response.Pending = false;
    }   
}
