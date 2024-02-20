using Networking;
using UnityEngine;

public class SelectionScript : MonoBehaviour
{
    public Camera tCamera;
    public GameObject editStoneMenu;

    private Transform _selection;
    private Transform _rotation;
    private const int StoneMask = 1 << 9;
    private const int GroundMask = 1 << 10;
    private Vector3 _panPosition;
    private Vector3 _delta  =  Vector3.zero;
    private Vector3 _prevPos = Vector3.zero;
    private bool _panning;
    private float _cameraY;

    private Vector3 _deltaHitDef;

    [SerializeField]
    private NetworkStoneSpawner networkStoneSpawner;

    // Use this for initialization
    private void Start () {
        _panPosition = new Vector3(0.0f, 0.0f);
        editStoneMenu.SetActive(false);
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        if (tCamera == null) return;
        
        // Moving stone
        if (Input.GetMouseButtonDown(0))
        {
            _prevPos = Input.mousePosition;
            _deltaHitDef.y = Mathf.Infinity;
        }

        if (Input.GetMouseButton(0))
        {

            RaycastHit hit;
            RaycastHit terrainHit;
            Ray ray = tCamera.ScreenPointToRay(Input.mousePosition);
                
            if (_panning == false && _selection == null && Physics.Raycast(ray, out hit, Mathf.Infinity, StoneMask))
            {
                editStoneMenu.SetActive(true);

                _selection = hit.transform;
                _rotation = hit.transform;
            }

            //selection is the object who collides with the cursor
            if (_selection != null) {
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, GroundMask))
                {
                    Vector3 GroundhitPoint = hit.point;
                    if (_deltaHitDef.y == Mathf.Infinity)
                    {
                        _deltaHitDef = GroundhitPoint;
                    }
                    GroundhitPoint += _selection.position - _deltaHitDef;
                    _deltaHitDef = hit.point;
                    _selection.position = GroundhitPoint;
                }
                else if (Terrain.activeTerrains.Length > 0 && Terrain.activeTerrain.GetComponent<Collider>().Raycast(ray, out terrainHit, Mathf.Infinity))
                {
                    Vector3 hitPoint = terrainHit.point;
                    if (_deltaHitDef.y == Mathf.Infinity)
                    {
                        _deltaHitDef = hitPoint;
                    }
                    _deltaHitDef = hitPoint - _deltaHitDef;
                    hitPoint += _selection.position - hitPoint + _deltaHitDef;
                    _deltaHitDef = terrainHit.point;
                    _selection.position = hitPoint;
                    int stoneId = ServerManager.Instance.GetIdByStone(_selection.gameObject);
                    networkStoneSpawner.UpdateStone(stoneId, _selection);
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            _selection = null;
            _panning = false;
        }
    }
    
    public void RotateUp()
    {
        if (_rotation == null) return;
        _rotation.Rotate(Vector3.forward, 20.0f);
        int stoneId = ServerManager.Instance.GetIdByStone(_rotation.gameObject);
        networkStoneSpawner.UpdateStone(stoneId, _rotation);
    }

    public void rotateDown()
    {
        if (_rotation == null) return;
        _rotation.Rotate(Vector3.forward, -20.0f);
        int stoneId = ServerManager.Instance.GetIdByStone(_rotation.gameObject);
        networkStoneSpawner.UpdateStone(stoneId, _rotation);
    }

    public void DeleteStone()
    {
        if (_rotation == null) return;
        int stoneId = ServerManager.Instance.GetIdByStone(_rotation.gameObject);
        if (stoneId == 0) Destroy(_rotation.gameObject); // Pre spawned stone from asset bundle
        else networkStoneSpawner.DeleteStoneServerRpc(stoneId); // Spawned after connection stone
    }
}
