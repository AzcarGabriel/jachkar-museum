using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandManager : MonoBehaviour
{
    public void ManageCommands(string command)
    {
       string[] args = command.Trim('/').Split();
        
        if (args[0] == "switch") {
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
