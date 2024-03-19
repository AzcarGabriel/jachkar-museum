using System;
using System.Linq;
using Networking;
using UI.UIScripts;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class MainMenuScript : MonoBehaviour
{
    private enum GameMode { Single, Host, Server, Client }

    [SerializeField] private MenuPresenter menuPresenter;

    private bool _isServer;
    private bool _useLeader;
    
    private StoneService _stoneService;
    private GameMode _gameMode;
    // private string connectionUserName;
    
    private void Start ()
    {
        string[] args = Environment.GetCommandLineArgs();
        if (args.Any(arg => arg == "-dedicatedServer")) _isServer = true;
        if (args.Any(arg => arg == "-leader")) _useLeader = true;

        if (!_isServer)
        {
            VisualElement loadScreen = menuPresenter.LoadScreen;
            _stoneService = gameObject.AddComponent<StoneService>();
            _stoneService.LoadScreen = loadScreen;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            if (StonesValues.stonesThumbs.Count == 0)
            {
                StartCoroutine(this._stoneService.DownloadThumbs());
            }
            
            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 0;
        }
        else
        {
            Console.WriteLine("Starting server...");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData("0.0.0.0", 25565);
            ServerManager.Instance.StartServer();
            ServerManager.Instance.UseLeader = _useLeader;
            Console.WriteLine("Server on");
        }
    }

    public void Exit()
    {
        Application.Quit();
    }
}
