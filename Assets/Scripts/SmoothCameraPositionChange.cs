using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.FirstPerson;
using Unity.Netcode;

public class SmoothCameraPositionChange : MonoBehaviour
{
    [SerializeField]
    public List<Text> metaText;
    public GameObject scrollView;
    public GameObject mainCamera;
    public String sceneName;
    public float speed = 0.3F;
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;

    private Transform targetGameObject;
    private readonly int stoneMask = 1 << 9;
    private float startTime;
    private FirstPersonController fps;

    Quaternion quat;

    // Use this for initialization
    void Start()
    {
        fps = mainCamera.gameObject.GetComponent<FirstPersonController>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (StaticValues.writing)
        {
            return;
        }

        Transform cameraPosition = mainCamera.transform;
        RaycastHit hit;
        Vector3 pond = new Vector3(0.0f, 1.0f, 20.0f);

        if (Input.GetKeyDown("e") && !fps.enabled)
        {
            targetGameObject = null;
            fps.enabled = true;
            Cursor.visible = false;
            scrollView.SetActive(false);
        }
        else if (Physics.Raycast(cameraPosition.position, cameraPosition.forward, out hit, 21.0f, stoneMask)
                 && Vector3.Dot(cameraPosition.forward, hit.transform.up) > 0.0f)
        {
            targetGameObject = hit.transform;

            if (Input.GetKeyDown("e"))
            {
                if (fps.enabled)
                {
                    fps.enabled = false;
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                    string name = hit.transform.gameObject.name;
                    StaticValues.stone_name = name;
                    SceneManager.LoadScene("StoneDetails", LoadSceneMode.Additive);
                    // NetworkManager.Singleton.SceneManager.LoadScene("StoneDetails", LoadSceneMode.Single);
                }
            }
            else
            {
                targetGameObject = null;
            }
        }
    }

    void OnGUI()
    {
        if (targetGameObject)
        {
            GUI.Label(new Rect(Screen.width / 2, Screen.height / 2, 200.0f, 20.0f), "press E to see info");
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(mainCamera.transform.position, mainCamera.transform.forward * 50.0f);
    }

    protected IEnumerator MoveCameraToPoint(Transform transform)
    {
        mainCamera.transform.position = transform.position + transform.up * -20.0f + transform.forward * 5.0f;

        Vector3 cameraToStone = transform.position - mainCamera.transform.position;
        Vector3 cameraForward = mainCamera.transform.forward;
        Vector3 crossProduct = Vector3.Cross(cameraForward.normalized, cameraToStone.normalized);
        float angle = Mathf.Rad2Deg * Mathf.Asin(crossProduct.y) + 22.5f;
        startTime = Time.time;
        quat = mainCamera.transform.rotation * Quaternion.Euler(0.0f, angle, 0.0f);

        while (Mathf.Abs(Quaternion.Angle(mainCamera.transform.rotation, quat)) > Mathf.Epsilon)
        {
            mainCamera.transform.rotation = Quaternion.Lerp(mainCamera.transform.rotation, quat, 1.0f);
        }

        return null;
    }
}
