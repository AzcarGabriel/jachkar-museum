/*
    DummySceneScripts.cs
    
    @author Gabriel Azócar Cárcamo <azocarcarcamo@gmail.com>
 */

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DummySceneScripts : MonoBehaviour
{
    public Transform spawnPoint;
    public GameObject addStoneMenu;
    public GameObject editStoneMenu;
    public Slider slider;
    public LayoutGroup addStoneMenuGrid;
    public GameObject informationPanel;
    public Text informationText;
    private StoneService stoneService;

    // Objects
    public GameObject firstObject;

    // Use this for initialization
    void Start()
    {
        stoneService = gameObject.AddComponent<StoneService>();
        stoneService.slider = this.slider;

        StaticValues.previos_scene = SceneManager.GetActiveScene().name;
        Cursor.lockState = CursorLockMode.Locked;

        firstObject.SetActive(false);
        informationPanel.SetActive(false);
    }

    public void ShowObject(int objectId)
    {
        if (objectId == 1)
        {
            firstObject.SetActive(true);
            informationText.text = "Information:\nEs una escultura de yeso, de dos sujetos sentados uno al lado del otro.\nLength: 20 cm\nWidth: 5 cm\nHeight: 20 cm";
        }
    }
}
