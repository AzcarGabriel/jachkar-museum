using UnityEngine;
using UnityEngine.SceneManagement;
using Player;
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
    
    public GameObject addStoneMenu;
    public GameObject editStoneButtons;
    public GameObject saveButtons;
    public GameObject showMoreButtons;
    public GameObject saveDialog;
    public GameObject loadDialog;
    public GameObject overwriteDialog;
    public GameObject availableFiles;
    public GameObject helpPane;

    private GameObject _hand;
    private SelectionScript _handScript;

    // Use this for initialization
    void Start ()
    {
        _hand = GameObject.Find("SelectionManager");
        // FPS.SetActive(true);
        mainView.enabled = true;
        topView.enabled = false;
        Cursor.visible = false;
        _hand.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        addStoneMenu.SetActive(false);
        //saveButtons.SetActive(false);
        editStoneButtons.SetActive(false);
        showMoreButtons.SetActive(false);
        saveDialog.SetActive(false);
        loadDialog.SetActive(false);
        overwriteDialog.SetActive(false);
        availableFiles.SetActive(false);
        helpPane.SetActive(false);
        _handScript = _hand.GetComponent<SelectionScript>();


    }

    private void OnEnable()
    {
        TopViewController.EditorOpenEvent += OpenUI;
        TopViewController.EditorCloseEvent += CloseUI;
    }

    private void OnDisable()
    {
        TopViewController.EditorOpenEvent -= OpenUI;
        TopViewController.EditorCloseEvent -= CloseUI;
    }

    // Update is called once per frame
    void Update ()
    {
        if (EventSystem.current.currentSelectedGameObject != null) return;
        StaticValues.Writing = loadDialog.activeSelf || saveDialog.activeSelf;
        if (StaticValues.Writing) return;
        if (Input.GetKey("m"))
        {
            NetworkManager.Singleton.Shutdown();
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
        }
        
        if (Input.GetKeyDown("h"))
        {
            helpPane.SetActive(!helpPane.activeSelf);
        }
    }

    private void OnMouseEnter()
    {
        Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
    }

    private void OnMouseExit()
    {
        Cursor.SetCursor(null, Vector2.zero, cursorMode);
    }

    private void OpenUI()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        _handScript.topCamera = StaticValues.TopCamera;
        _hand.SetActive(true);
        addStoneMenu.SetActive(false);
        //saveButtons.SetActive(true);
        showMoreButtons.SetActive(true);
        #if USE_MULTIPLAYER
            chat.SetActive(false);
        #endif
    }

    private void CloseUI()
    {
        _hand.SetActive(false);
        addStoneMenu.SetActive(false);
        //saveButtons.SetActive(false);
        editStoneButtons.SetActive(false);
        showMoreButtons.SetActive(false);
        #if USE_MULTIPLAYER
            chat.SetActive(true);
        #endif
    }
}
