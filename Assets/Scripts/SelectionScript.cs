using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionScript : MonoBehaviour {

    public Camera tCamera;
    public Canvas panel;

    private Transform selection = null;
    private Transform rotation = null;
    private int stoneMask = 1 << 9;
    private int groundMask = 1 << 10;
    private Vector3 panPosition;
    private Vector3 delta  =  Vector3.zero;
    private Vector3 prevPos = Vector3.zero;
    private bool    panning = false;
    private float   cameraY = 0.0f;

    private Vector3 deltaHitdef ;

    // Use this for initialization
    void Start () {
        panPosition = new Vector3(0.0f, 0.0f);
        panel.enabled = false;
    }

    // Update is called once per frame
    void LateUpdate() {

        if (tCamera != null) {

            float scroll =  Input.GetAxis("Mouse ScrollWheel");

            if(scroll != 0.0f)
            {
                //print(scroll);
                cameraY = tCamera.transform.position.y + (-scroll * 20.0f);
                panPosition.x = tCamera.transform.position.x;
                panPosition.y = Mathf.Max(Mathf.Min(cameraY,100),30);
                panPosition.z = tCamera.transform.position.z;
                tCamera.transform.position = panPosition;
            }

            // Moving camera
            if (Input.GetMouseButtonDown(1))
            {
                prevPos = Input.mousePosition;
                deltaHitdef.y = Mathf.Infinity;
            }

            if (Input.GetMouseButton(1))
            {

                RaycastHit hit;
                RaycastHit terrainHit;
                Ray ray = tCamera.ScreenPointToRay(Input.mousePosition);

                if (panning == false && selection == null && Physics.Raycast(ray, out hit, Mathf.Infinity, stoneMask))
                {
                    Transform p;
                    panel.enabled = true;

                    selection = hit.transform;
                    rotation = hit.transform;
                }
                else if (selection == null && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
                {   // Clicking on terrain or anything else (move camera).
                    delta = Input.mousePosition - prevPos;
                    //print(delta);
                    panPosition.x = delta.x * 0.1f;
                    panPosition.y = 0;
                    panPosition.z = delta.y * 0.1f;
                    tCamera.transform.position += panPosition;
                    panning = true;
                    prevPos = Input.mousePosition;
                    rotation = null;
                    panel.enabled = false;
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
                    Transform p;
                    panel.enabled = true;

                    selection = hit.transform;
                    rotation = hit.transform;
                }

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
                        //print(selection.position);
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
    
    public void rotateUP(){
        if (rotation != null)
        {
            rotation.transform.Rotate(Vector3.forward, 20.0f);
        }
    }
    public void rotateDown()
    {
        if (rotation != null)
        {
            rotation.transform.Rotate(Vector3.forward, -20.0f);
        }
    }

    public void deleteStone()
    {
        if (rotation != null)
        {
            Destroy(rotation.gameObject);
        }
    }

}
