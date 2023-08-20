using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CommandManager : NetworkBehaviour
{

    [SerializeField] Transform sceneSpawnPoint;
    public void ManageCommands(string command)
    {
       string[] args = command.Trim('/').Split();
        
        if (args[0].ToLower() == "switch") {
            ChangeMap(args);
        }
    }

    private void ChangeMap(string[] args)
    {
        try
        {
            ServerManager.Instance.OpenScene(args[1]);
        }
        catch
        {
            Debug.Log("Map not found");
        }
    }
}
