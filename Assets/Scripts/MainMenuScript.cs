/*
    MainMenuScript.cs
    
    @author Gabriel Azócar Cárcamo <azocarcarcamo@gmail.com>
 */

using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    public GameObject enterScreen;

    // Use this for initialization
    void Start ()
    {

    }

    public void OpenScene(String name)
    {
        string[] tokens = name.Split('/');
        string n = "";
        if (1 < tokens.Length)
        {
            n = tokens[1];
        }
        else
        {
            n = name;
        }

        enterScreen.SetActive(true);
        SceneManager.LoadScene(n, LoadSceneMode.Single);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
