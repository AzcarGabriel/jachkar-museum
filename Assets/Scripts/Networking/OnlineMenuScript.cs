using System;
using UnityEngine;
using Unity.Netcode;
using TMPro;
public class OnlineMenuScript : NetworkBehaviour
{
    [SerializeField] private GameObject inputUsernameField;
    [SerializeField] private TMP_Text textField;

    public void OpenScene(String name) {
        ServerManager.Instance.OpenScene(name);
    }

    public void preSelectCharacter(int index) {
        ServerManager.Instance.preSelectedCharacter = index;
    }
        
    public void OnConfirmClick() {
        //  Debug.Log("Test " + selectedCharacter);
        ServerManager.Instance.username = textField.text;
    }

    public void StartServer()
    {
        ServerManager.Instance.StartServer();
    }

    public void StartHost() {
        Debug.Log("Host");
        ServerManager.Instance.StartHost();
    }

    public void StartClient() {
        ServerManager.Instance.StartClient();
    }
}
