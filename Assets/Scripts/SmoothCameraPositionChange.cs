using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public class SmoothCameraPositionChange : MonoBehaviour
{


    [SerializeField]
    public List<Text> metaText;
    public GameObject scrollView;
    public String sceneName;
    public GameObject mainCamera;
    private Transform endMarker;
    private Transform startMarker;
    private Transform selection = null;
    private int stoneMask = 1 << 9;
    private int groundMask = 1 << 10;
    public float speed = 0.3F;
    private float startTime;
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;
    bool flag = true;
    private Transform targetGameObject;
    private FirstPersonController fps;

    private bool start;
    // Use this for initialization
    void Start()
    {
        start = true;
        fps = mainCamera.gameObject.GetComponent<FirstPersonController>();
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

    Quaternion quat;
    protected IEnumerator MoveCameraToPoint(Transform transform)
    {
        var cameraToStone = transform.position - mainCamera.transform.position;
        cameraToStone.y = 0.0f;
        var cameraForward = mainCamera.transform.forward;
        cameraForward.y = 0.0f;
        var crossProduct = Vector3.Cross(cameraForward.normalized, cameraToStone.normalized);
        var angle = Mathf.Rad2Deg * Mathf.Asin(crossProduct.y) + 22.5f;
        Debug.Log(angle);
        startTime = Time.time;
        quat = mainCamera.transform.rotation * Quaternion.Euler(0.0f, angle, 0.0f);
        while (Mathf.Abs(Quaternion.Angle(mainCamera.transform.rotation, quat)) > Mathf.Epsilon)
        {
            //float distCovered = (Time.time - startTime) * speed;
            //fracJourney = distCovered / journeyLength;
            //mainCamera.transform.position = Vector3.Lerp(startMarker.position, endMarker.position + 10 * -endMarker.up, fracJourney);
            mainCamera.transform.rotation = Quaternion.Lerp(mainCamera.transform.rotation, quat, 0.2f);
            yield return null;
        }

        start = true;
    }



    // Update is called once per frame
    void LateUpdate()
    {
        Transform cameraPosition = mainCamera.transform;
        RaycastHit hit;
        if (Input.GetKeyDown("e") && !fps.enabled)
        {
            targetGameObject = null;
            fps.enabled = true;
            Cursor.visible = false;
            scrollView.SetActive(false);
        }
        else if ((Physics.Raycast(cameraPosition.position, cameraPosition.forward, out hit, 15.0f, stoneMask) &&
                     Vector3.Dot(cameraPosition.forward, hit.transform.up) > 0.0f))
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
                    StartCoroutine(loadXml(name));
                    scrollView.SetActive(true);
                    if (start)
                    {
                        start = false;
                        StartCoroutine(MoveCameraToPoint(targetGameObject.transform));
                    }
                }
            }
            else
            {
                targetGameObject = null;

            }
        }
    }


    IEnumerator loadXml(string name)
    {
        var w = new WWW("http://10.1.0.118:8080/Scenes/Ejmiatsin.xml");//31.7.162.98, inside aua 10.1.0.118

        while (!w.isDone)
        {
            yield return w;
            string s = w.text;
            var data = SceneHelper.GetKhachkarByXML(s);
            metaText[0].text = Convert.ToString(data["Location"]);
            metaText[1].text = Convert.ToString(data["Scenario"]);
            metaText[2].text = Convert.ToString(data["Setting"]);
            metaText[3].text = Convert.ToString(data["Accessibility"]);
            metaText[4].text = Convert.ToString(data["Category"]);
            metaText[5].text = Convert.ToString(data["ProductionPeriod"]);
            metaText[6].text = Convert.ToString(data["CoonditionOfPreservation"]);
            metaText[7].text = Convert.ToString(data["Inscription"]);
            metaText[8].text = Convert.ToString(data["ImportantFeatures"]);
            metaText[9].text = Convert.ToString(data["Referances"]);
        }
    }

    void update()
    {

    }
}
