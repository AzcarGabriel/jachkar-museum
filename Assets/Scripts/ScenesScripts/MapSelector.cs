using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapSelector : MonoBehaviour
{
    #if !USE_MULTIPLAYER
    private void Start(){
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public GameObject enterScreen;
    public void OpenScene(String name)
    {
        enterScreen.SetActive(true);
        SceneManager.LoadScene(name, LoadSceneMode.Single);
        ServerManager.Instance.SetCharacter(0, 0); // Set the character to the default one (This shouldn't be hardcoded in multiplayer mode)
    }
    #endif

}