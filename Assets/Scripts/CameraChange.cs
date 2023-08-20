using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Unity.Netcode;

public class CameraChange : MonoBehaviour
{
    [SerializeField] private Camera mainView;
    [SerializeField] private Camera topView;
    [SerializeField] private Texture2D cursorTexture;
    [SerializeField] private CursorMode cursorMode = CursorMode.Auto;
    [SerializeField] private Vector2 hotSpot = Vector2.zero;
    [SerializeField] private GameObject chat;

    //public GameObject FPS;
    public static List<GameObject> playersList;
    public GameObject addStoneMenu;
    public GameObject editStoneButtons;
    public GameObject saveButtons;
    public GameObject showMoreButtons;
    public GameObject saveDialog;
    public GameObject loadDialog;
    public GameObject overwriteDialog;
    public GameObject availableFiles;
    public GameObject helpPane;

    private GameObject hand;

    // Use this for initialization
    void Start ()
    {
        hand = GameObject.Find("SelectionManager");
        // FPS.SetActive(true);
        mainView.enabled = true;
        topView.enabled = false;
        Cursor.visible = false;
        hand.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        addStoneMenu.SetActive(false);
        saveButtons.SetActive(false);
        editStoneButtons.SetActive(false);
        showMoreButtons.SetActive(false);
        saveDialog.SetActive(false);
        loadDialog.SetActive(false);
        overwriteDialog.SetActive(false);
        availableFiles.SetActive(false);
        helpPane.SetActive(false);
    }

    // Update is called once per frame
    void Update ()
    {
        if (EventSystem.current.currentSelectedGameObject != null) return;
        StaticValues.writing = loadDialog.activeSelf || saveDialog.activeSelf;
        if (!StaticValues.writing)
        {
            if (Input.GetKey("m"))
            {
                NetworkManager.Singleton.Shutdown();
                SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
            }

            if (Input.GetKey("t"))
            {
                StaticValues.self_fps.SetActive(false);
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                mainView.enabled = false;
                topView.enabled = true;
                hand.SetActive(true);
                addStoneMenu.SetActive(false);
                saveButtons.SetActive(true);
                showMoreButtons.SetActive(true);
                chat.SetActive(false);
            }

            if (Input.GetKey("p"))
            {
                StaticValues.self_fps.SetActive(true);
                mainView.enabled = true;
                topView.enabled = false;
                Cursor.visible = false;
                hand.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                addStoneMenu.SetActive(false);
                saveButtons.SetActive(false);
                editStoneButtons.SetActive(false);
                showMoreButtons.SetActive(false);
            }

            if (Input.GetKeyDown("h"))
            {
                helpPane.SetActive(!helpPane.activeSelf);
            }
        }
    }

    void OnMouseEnter()
    {
        Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
    }

    void OnMouseExit()
    {
        Cursor.SetCursor(null, Vector2.zero, cursorMode);
    }

    void OnGUI()
    {

    }
}
