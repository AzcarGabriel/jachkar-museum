using UnityEngine;
using System.Collections;

public class ContureRendering : MonoBehaviour
{

    private Behaviour halo;

    public Camera topView;
    
    void Start()
    {
        halo = GetComponent<Behaviour>();
    }

    void OnMouseEnter()
    {
        halo.enabled = true;
    }


    void OnMouseExit()
    {
      halo.enabled = false;
    }
}
