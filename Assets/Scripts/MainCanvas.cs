using UnityEngine;

public class MainCanvas : MonoBehaviour
{
    public GameObject helpPane;

    // Use this for initialization
    void Start()
    {
        helpPane.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown("h"))
        {

            helpPane.SetActive(!helpPane.activeSelf);
        }
    }
}
