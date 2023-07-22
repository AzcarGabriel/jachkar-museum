using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class ServerManager : NetworkBehaviour
{

    [SerializeField] private CharacterDatabase characterDatabase;
    public static ServerManager Instance { get; private set; }

    public Dictionary<ulong, ClientData> ClientData { get; private set; } = new Dictionary<ulong, ClientData>();
    
    public int preSelectedCharacter = -1;
    public string username = "name";

    void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        }
        else {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void StartServer() {
        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck; // Not necessary yet, but here you could add a player limit

        NetworkManager.Singleton.StartServer();
        NetworkManager.Singleton.SceneManager.SetClientSynchronizationMode(LoadSceneMode.Single);
        NetworkManager.Singleton.SceneManager.LoadScene("Noradus", LoadSceneMode.Single);
    }

    public void StartHost() {
        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
        NetworkManager.Singleton.StartHost();
        NetworkManager.Singleton.SceneManager.SetClientSynchronizationMode(LoadSceneMode.Single);
    }

    public void StartClient() {
        NetworkManager.Singleton.StartClient();
        NetworkManager.Singleton.SceneManager.SetClientSynchronizationMode(LoadSceneMode.Single);
    }

    public void addClientData(string username, int characterId, ulong clientId) {
        ClientData new_client_data = new ClientData(clientId);
        new_client_data.characterId = characterId;
        new_client_data.username = username;

        ClientData.Add(clientId, new_client_data);
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response) {
        response.Approved = true; // accept everything for the moment
        response.CreatePlayerObject = true;
        response.Pending = false;
    }   
}
