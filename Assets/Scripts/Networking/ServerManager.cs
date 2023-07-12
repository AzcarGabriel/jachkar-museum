using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ServerManager : MonoBehaviour
{
    public static ServerManager Instance { get; private set; }

    public Dictionary<ulong, ClientData> ClientData { get; private set; } = new Dictionary<ulong, ClientData>();

    [SerializeField] private CharacterDatabase characterDatabase;

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
    }

    public void StartHost() {
        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;

        NetworkManager.Singleton.StartHost();
    }

    public void StartClient() {
        NetworkManager.Singleton.StartClient();
    }

    public void addIdentity(string name) {
        Debug.Log(NetworkManager.Singleton.LocalClientId);
    }

    /*public void SpawnCharacter(int characterId, ulong id) {
        Debug.Log(characterId);
        Debug.Log("Holaaaa");
        var character = characterDatabase.GetCharacterById(characterId);
        if (character != null) {
            var characterInstance = Instantiate(character.GameplayPrefab);
            characterInstance.SpawnAsPlayerObject(id);
        } else {
            Debug.LogError("Character was null");
        }

    }*/

    public void StoreCharacter(ulong clientId, int characterId) {
        //if (ClientData.TryGetValue(clientId, out )) ;
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response) {
        response.Approved = true; // accept everything for the moment
        response.CreatePlayerObject = false;
        response.Pending = false;
        Debug.Log("Check approved");
        ClientData[request.ClientNetworkId] = new ClientData(request.ClientNetworkId);
    }

}
