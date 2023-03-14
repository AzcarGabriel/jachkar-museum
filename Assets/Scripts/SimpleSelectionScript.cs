using UnityEngine;
using UnityEngine.UI;

public class SimpleSelectionScript : MonoBehaviour
{
    public Camera tCamera;
    public GameObject editStoneMenu;

    private Transform selection = null;
    private Transform rotation = null;
    private int stoneMask = 1 << 9;
    private int groundMask = 1 << 10;
    private Vector3 panPosition;
    private Vector3 delta  =  Vector3.zero;
    private Vector3 prevPos = Vector3.zero;
    private bool    panning = false;

    private Vector3 deltaHitdef;

    public GameObject informationPanel;
    public Transform spawnPoint;
    public GameObject helpPanel;

    // Use this for initialization
    void Start () {
        panPosition = new Vector3(0.0f, 0.0f);
        editStoneMenu.SetActive(false);
    }

    // Update is called once per frame
    void LateUpdate() {
        if (tCamera != null) {
            // Moving camera
            if (Input.GetMouseButtonDown(1))
            {
                prevPos = Input.mousePosition;
                deltaHitdef.y = Mathf.Infinity;
            }

            if (Input.GetMouseButton(1))
            {

                Ray ray = tCamera.ScreenPointToRay(Input.mousePosition);

                if (panning == false && selection == null && Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, stoneMask))
                {
                    editStoneMenu.SetActive(true);
                    informationPanel.SetActive(true);
                    helpPanel.SetActive(false);

                    selection = hit.transform;
                    rotation = hit.transform;
                }
                else if (selection == null && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
                {   
                    // Clicking on terrain or anything else (move camera).
                    delta = Input.mousePosition - prevPos;
                    panPosition.x = delta.x * 0.1f;
                    panPosition.y = 0;
                    panPosition.z = delta.y * 0.1f;
                    tCamera.transform.position += panPosition;
                    panning = true;
                    prevPos = Input.mousePosition;
                    rotation = null;
                    editStoneMenu.SetActive(false);
                    informationPanel.SetActive(false);
                    helpPanel.SetActive(false);
                }
            }

            if (Input.GetMouseButtonUp(1))
            {
                selection = null;
                panning = false;
            }

            // Moving stone
            if (Input.GetMouseButtonDown(0))
            {
                prevPos = Input.mousePosition;
                deltaHitdef.y = Mathf.Infinity;
            }

            if (Input.GetMouseButton(0))
            {
                RaycastHit hit;
                RaycastHit terrainHit;
                Ray ray = tCamera.ScreenPointToRay(Input.mousePosition);
                
                if (panning == false && selection == null && Physics.Raycast(ray, out hit, Mathf.Infinity, stoneMask))
                {
                    editStoneMenu.SetActive(true);
                    informationPanel.SetActive(true);
                    helpPanel.SetActive(false);

                    selection = hit.transform;
                    rotation = hit.transform;
                }

                //selection is the object who collides with the cursor
                if (selection != null) {
                   if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundMask))
                   {
                        Vector3 GroundhitPoint = hit.point;
                        if (deltaHitdef.y == Mathf.Infinity)
                        {
                            deltaHitdef = GroundhitPoint;
                        }
                        GroundhitPoint += selection.position - deltaHitdef;
                        deltaHitdef = hit.point;
                        selection.position = GroundhitPoint;
                    }
                    else if (Terrain.activeTerrains.Length > 0 && Terrain.activeTerrain.GetComponent<Collider>().Raycast(ray, out terrainHit, Mathf.Infinity))
                    {
                        Vector3 hitPoint = terrainHit.point;
                        if (deltaHitdef.y == Mathf.Infinity)
                        {
                            deltaHitdef = hitPoint;
                        }
                        deltaHitdef = hitPoint - deltaHitdef;
                        hitPoint += selection.position - hitPoint + deltaHitdef;
                        deltaHitdef = terrainHit.point;
                        selection.position = hitPoint;
                    }
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                selection = null;
                panning = false;
            }
        }
    }
    
    public void RotateUP(){
        if (rotation != null)
        {
            rotation.transform.Rotate(Vector3.down, -20.0f);
        }
    }

    public void RotateDown()
    {
        if (rotation != null)
        {
            rotation.transform.Rotate(Vector3.down, 20.0f);
        }
    }

    public void DeleteObject()
    {
        if (rotation != null)
        {
            informationPanel.SetActive(false);
            editStoneMenu.SetActive(false);
            rotation.gameObject.SetActive(false);
            rotation.gameObject.transform.position = this.spawnPoint.position;
            rotation.gameObject.transform.eulerAngles = new Vector3(0, -90, 0);
        }
    }
}
