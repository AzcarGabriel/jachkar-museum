using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using TMPro;
public class OnlineMenuScript : NetworkBehaviour
{
    [SerializeField] private GameObject inputUsernameField;
    [SerializeField] private TMP_Text textField;

    public void OpenScene(String name) {
        string[] tokens = name.Split('/');
        string n;
        if (1 < tokens.Length) {
            n = tokens[1];
        }
        else {
            n = name;
        }

        //NetworkManager network = NetworkManager.Singleton;
        //enterScreen.SetActive(true);
        NetworkManager.Singleton.SceneManager.LoadScene(n, LoadSceneMode.Single);
    }

    public void preSelectCharacter(int index) {
        ServerManager.Instance.preSelectedCharacter = index;
    }
        
    public void OnConfirmClick() {
        //  Debug.Log("Test " + selectedCharacter);
        ServerManager.Instance.username = textField.text;
    }

    public void StartHost() {
        ServerManager.Instance.StartHost();
    }

    public void StartClient() {
        ServerManager.Instance.StartClient();
    }
}
