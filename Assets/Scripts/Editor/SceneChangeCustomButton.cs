#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class SceneChangeCustomButton : MonoBehaviour
{
    [MenuItem("Scene/Main Menu")]
    static void LoadScene()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/MainMenu.unity");
    }
    [MenuItem("Scene/Noradus")]
    static void LoadNoradus()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/Noradus.unity");
    }
}
#endif