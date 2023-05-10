/*
    MainMenuScript.cs
    
    @author Gabriel Azócar Cárcamo <azocarcarcamo@gmail.com>
 */

using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class MainMenuScript : MonoBehaviour
{
    private enum GameMode { Single, Host, Server, Client }

    public GameObject enterScreen;
    public GameObject loadScreen;
    private StoneService stoneService;
    private GameMode gameMode;

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
        NetworkManager.Singleton.SceneManager.LoadScene(n, LoadSceneMode.Single);
    }

    public void SetMode(String mode) 
    {
        gameMode = (GameMode)System.Enum.Parse(typeof(GameMode), mode);
    }

    public void connectClient() 
    {
        NetworkManager.Singleton.StartClient();
    }

    public void Exit()
    {
        Application.Quit();
    }
}
