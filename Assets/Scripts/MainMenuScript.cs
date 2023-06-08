/*
    MainMenuScript.cs
    
    @author Gabriel Azócar Cárcamo <azocarcarcamo@gmail.com>
 */

using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using TMPro;
using FMGames.Scripts.Menu.Chat;
using Unity.Collections;
using Unity.Netcode.Transports.UTP;

public class MainMenuScript : MonoBehaviour
{
    private enum GameMode { Single, Host, Server, Client }

    public GameObject enterScreen;
    public GameObject loadScreen;

    [SerializeField] private GameObject inputUsernameField;
    [SerializeField] private TMP_Text textField;

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

        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;

        string[] args = Environment.GetCommandLineArgs();
        Debug.Log(args.Length);
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "-dedicatedServer")
            {
                Console.WriteLine("Starting server...");
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData("186.189.90.143", 8000);
                NetworkManager.Singleton.StartServer();
                Console.WriteLine("Server on");
                break;
            }
        }

    }

    private void Awake()
    {
       


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

        //NetworkManager network = NetworkManager.Singleton;
        //enterScreen.SetActive(true);
        SceneManager.LoadScene(n, LoadSceneMode.Single);
    }

    public void SetMode(String mode) 
    {
        gameMode = (GameMode)Enum.Parse(typeof(GameMode), mode);
    }

    public void connectClient() 
    {
        if (gameMode != GameMode.Client) return;
        FixedString32Bytes newName = inputUsernameField.GetComponent<TMP_InputField>().text;
        ChatBehaviour.username = newName;
        NetworkManager.Singleton.StartClient();
    }

    public void connectHost() {
        if (gameMode != GameMode.Host) return;
        FixedString32Bytes newName = inputUsernameField.GetComponent<TMP_InputField>().text;
        ChatBehaviour.username = newName;
        NetworkManager.Singleton.StartHost();
    }

    public void connectServer() {
        NetworkManager.Singleton.StartServer();
    }

    public void Exit()
    {
        Application.Quit();
    }
}
