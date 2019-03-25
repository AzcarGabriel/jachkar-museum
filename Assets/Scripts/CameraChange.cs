using UnityEngine;
using System.Collections;

public class CameraChange : MonoBehaviour {
    [SerializeField]
    public Camera mainView;
    public Camera topView;
    public Texture2D cursorTexture;
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;
    bool flag = true;

    public GameObject FPS;
    public Canvas stonesSpawn;
    public Canvas editStoneButtons;
    public Canvas saveButtons;
    public Canvas showMoreButtons;

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
        stonesSpawn.enabled = false;
        saveButtons.enabled = false;
        editStoneButtons.enabled = false;
        showMoreButtons.enabled = false;
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

    // Update is called once per frame
    void Update ()
    {
        if (Input.GetKey("t"))
        {
            FPS.SetActive(false);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            mainView.enabled = false;
            topView.enabled = true;
            hand.SetActive(true);
            stonesSpawn.enabled = false;
            saveButtons.enabled = false;
            showMoreButtons.enabled = true;
        }

        if (Input.GetKey("p"))
        {
            FPS.SetActive(true);
            mainView.enabled = true;
            topView.enabled  = false;
            Cursor.visible   = false;
            hand.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            stonesSpawn.enabled = false;
            saveButtons.enabled = false;
            editStoneButtons.enabled = false;
            showMoreButtons.enabled = false;
        }
    }
}
