using System;
using UnityEngine;

namespace StoneEditor
{
    public class MoveArrow : MonoBehaviour
    {
        
        private Vector3 _mousePositionOffset;
        
        private Vector3 GetMouseWorldPosition()
        {
            return Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        private void OnMouseDown()
        {
            Debug.Log(GetMouseWorldPosition());
           _mousePositionOffset = gameObject.transform.position - GetMouseWorldPosition();
        }

        private void OnMouseDrag()
        {
            transform.position = GetMouseWorldPosition() + _mousePositionOffset;
        }
    }
}
