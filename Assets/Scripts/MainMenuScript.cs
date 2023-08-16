/*
    MainMenuScript.cs
    
    @author Gabriel Azócar Cárcamo <azocarcarcamo@gmail.com>
 */

using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using TMPro;
using Unity.Collections;
using Unity.Netcode.Transports.UTP;

public class MainMenuScript : MonoBehaviour
{
    private enum GameMode { Single, Host, Server, Client }

    public GameObject enterScreen;
    public GameObject loadScreen;

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
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "-dedicatedServer")
            {
                Console.WriteLine("Starting server...");
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData("0.0.0.0", 25565);
                ServerManager.Instance.StartServer();
                Console.WriteLine("Server on");
                break;
            }
        }

    }

    public void LoadMultiPlayer() {
        SceneManager.LoadScene("OnlineMenu", LoadSceneMode.Single);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
