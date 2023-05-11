/*
    MainMenuScript.cs
    
    @author Gabriel Azócar Cárcamo <azocarcarcamo@gmail.com>
 */

using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using TMPro;

public class MainMenuScript : MonoBehaviour
{
    private enum GameMode { Single, Host, Server, Client }

    public GameObject enterScreen;
    public GameObject loadScreen;
    static string connectionUserName;

    [SerializeField] private GameObject inputUsernameField;

    private StoneService stoneService;
    private GameMode gameMode;
    // private string connectionUserName;

    // Use this for initialization
    void Start ()
    {
        stoneService = gameObject.AddComponent<StoneService>();
        stoneService.loadScreen = this.loadScreen;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (StonesValues.stonesThumbs.Count == 0)
        {
            StartCoroutine(this.stoneService.DownloadThumbs());
        }
    }

    public void OpenScene(String name)
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

        enterScreen.SetActive(true);
        // SceneManager.LoadScene(n, LoadSceneMode.Single);
        switch (gameMode) {
            case GameMode.Server:
                NetworkManager.Singleton.StartServer();
                break;
            case GameMode.Host:
                NetworkManager.Singleton.StartHost();              
                break;
            case GameMode.Single:
                return;
        }
        updatePlayerNameServerRPC(NetworkManager.Singleton.LocalClientId, connectionUserName);
        NetworkManager.Singleton.SceneManager.LoadScene(n, LoadSceneMode.Single);
    }

    public void SetMode(String mode) 
    {
        gameMode = (GameMode)Enum.Parse(typeof(GameMode), mode);
    }

    public void SetUsername() {
        string username = inputUsernameField.GetComponent<TMP_InputField>().text;
        connectionUserName = username;
    }

    public void connectClient() 
    {
        NetworkManager.Singleton.StartClient();
        updatePlayerNameServerRPC(NetworkManager.Singleton.LocalClientId, connectionUserName);
    }

    public void Exit()
    {
        Application.Quit();
    }

    private void updatePlayerNameServerRPC(ulong localId, String name) {
        ClientRpcParams clientRpcParams = new ClientRpcParams {
            Send = new ClientRpcSendParams {
                TargetClientIds = new[] { localId }
            }
        };
        NetworkManager networkManager = NetworkManager.Singleton;
        // ulong id = clientRpcParams.Send.TargetClientIds[0];
        PlayerObject playerObject = networkManager.ConnectedClients[localId].PlayerObject.GetComponent<PlayerObject>();
        playerObject.UpdatePlayerNameServerRpc(name);
    }
}
