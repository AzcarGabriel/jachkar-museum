using Networking;
using UnityEngine;
using UnityEngine.Serialization;

public class SelectionScript : MonoBehaviour
{
    public Camera topCamera { private get;  set; }
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
    private NetworkStoneSpawner _networkStoneSpawner;

    // Use this for initialization
    private void Start () {
        _panPosition = new Vector3(0.0f, 0.0f);
        editStoneMenu.SetActive(false);
    }


    public void OnStoneSelect()
    {
        
    }
    

    // Update is called once per frame
    private void LateUpdate()
    {
        if (topCamera == null) return;
        
        // Moving stone
        if (Input.GetMouseButtonDown(0))
        {
            _prevPos = Input.mousePosition;
            _deltaHitDef.y = Mathf.Infinity;
        }

        if (Input.GetMouseButton(0))
        {
            Ray ray = topCamera.ScreenPointToRay(Input.mousePosition);
                
            if (_panning == false && _selection == null && Physics.Raycast(ray, out var hit, Mathf.Infinity, StoneMask))
            {
                editStoneMenu.SetActive(true);

                _selection = hit.transform;
                _rotation = hit.transform;
            }

            //selection is the object who collides with the cursor
            if (_selection != null) {
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, GroundMask))
                {
                    Vector3 groundhitPoint = hit.point;
                    if (float.IsPositiveInfinity(_deltaHitDef.y))
                    {
                        _deltaHitDef = groundhitPoint;
                    }
                    groundhitPoint += _selection.position - _deltaHitDef;
                    _deltaHitDef = hit.point;
                    _selection.position = groundhitPoint;
                }
                else if (Terrain.activeTerrains.Length > 0 && Terrain.activeTerrain.GetComponent<Collider>().Raycast(ray, out var terrainHit, Mathf.Infinity))
                {
                    Vector3 hitPoint = terrainHit.point;
                    if (float.IsPositiveInfinity(_deltaHitDef.y))
                    {
                        _deltaHitDef = hitPoint;
                    }
                    _deltaHitDef = hitPoint - _deltaHitDef;
                    hitPoint += _selection.position - hitPoint + _deltaHitDef;
                    _deltaHitDef = terrainHit.point;
                    _selection.position = hitPoint;
                    int stoneId = ServerManager.Instance.GetIdByStone(_selection.gameObject);
                    _networkStoneSpawner.UpdateStone(stoneId, _selection);
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
        _networkStoneSpawner.UpdateStone(stoneId, _rotation);
    }

    public void RotateDown()
    {
        if (_rotation == null) return;
        _rotation.Rotate(Vector3.forward, -20.0f);
        int stoneId = ServerManager.Instance.GetIdByStone(_rotation.gameObject);
        _networkStoneSpawner.UpdateStone(stoneId, _rotation);
    }

    public void DeleteStone()
    {
        if (_rotation == null) return;
        int stoneId = ServerManager.Instance.GetIdByStone(_rotation.gameObject);
        if (stoneId == 0) Object.Destroy(_rotation.gameObject); // Pre spawned stone from asset bundle
        else _networkStoneSpawner.DeleteStoneServerRpc(stoneId); // Spawned after connection stone
    }
}
