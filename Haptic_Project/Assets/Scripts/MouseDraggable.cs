using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseDraggable : MonoBehaviour
{
    Vector3 mousePositionOffset;
    private Vector3 GetMouseWorldPosition(){
        //Capture mouse position & return worldPoint

        var mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = Camera.main.WorldToScreenPoint(transform.position).z;

        return Camera.main.ScreenToWorldPoint(mouseScreenPos);
    }
    
    private  void OnMouseDown() {
        //Capture the mouse offset
        mousePositionOffset = gameObject.transform.position - GetMouseWorldPosition();
        print("mousePositionOffset: "+mousePositionOffset+" = "+gameObject.transform.position+" - "+GetMouseWorldPosition());
    }

    private void OnMouseDrag() {
        gameObject.transform.position = GetMouseWorldPosition() + mousePositionOffset;
        print("Dragging...");
        print("Position : "+gameObject.transform.position);
    }
   
}
