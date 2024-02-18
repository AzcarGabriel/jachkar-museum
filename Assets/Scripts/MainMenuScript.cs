/*
    MainMenuScript.cs
    
    @author Gabriel Azócar Cárcamo <azocarcarcamo@gmail.com>
 */

using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using TMPro;
using UI.XML;
using Unity.Collections;
using Unity.Netcode.Transports.UTP;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class MainMenuScript : MonoBehaviour
{
    private enum GameMode { Single, Host, Server, Client }

    [SerializeField] private MenuPresenter menuPresenter;
    
    private StoneService _stoneService;
    private GameMode _gameMode;
    // private string connectionUserName;
    
    private void Start ()
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

        string[] args = Environment.GetCommandLineArgs();
        
        if (args.All(arg => arg != "-dedicatedServer")) return;
        Console.WriteLine("Starting server...");
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData("0.0.0.0", 25565);
        ServerManager.Instance.StartServer();
        Console.WriteLine("Server on");
    }

    public void LoadMultiPlayer() {
        SceneManager.LoadScene("OnlineMenu", LoadSceneMode.Single);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
