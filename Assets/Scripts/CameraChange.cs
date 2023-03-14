using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraChange : MonoBehaviour
{
    [SerializeField]
    public Camera mainView;
    public Camera topView;
    public Texture2D cursorTexture;
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;

    public GameObject FPS;
    public GameObject addStoneMenu;
    public GameObject editStoneButtons;
    public GameObject saveButtons;
    public GameObject showMoreButtons;
    public GameObject saveDialog;
    public GameObject loadDialog;
    public GameObject overwriteDialog;
    public GameObject availableFiles;
    public GameObject helpPanel;

    private GameObject hand;

    // Use this for initialization
    void Start ()
    {
        hand = GameObject.Find("SelectionManager");
        FPS.SetActive(true);
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
        helpPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update ()
    {
        StaticValues.writing = loadDialog.activeSelf || saveDialog.activeSelf;
        if (!StaticValues.writing)
        {
            if (Input.GetKey("m"))
            {
                SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
            }

            if (Input.GetKey("t"))
            {
                FPS.SetActive(false);
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                mainView.enabled = false;
                topView.enabled = true;
                hand.SetActive(true);
                addStoneMenu.SetActive(false);
                saveButtons.SetActive(true);
                showMoreButtons.SetActive(true);
            }

            if (Input.GetKey("p"))
            {
                FPS.SetActive(true);
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
                helpPanel.SetActive(!helpPanel.activeSelf);
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
