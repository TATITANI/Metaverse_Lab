using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body : MonoBehaviour
{
    protected Camera cam;
    [SerializeField] protected HandControllerSO controllerSO;

    private void Start()
    {
        cam = Camera.main;
    }

    protected virtual void Update()
    {
#if UNITY_EDITOR
        ProcessInput();
#endif
    }

    protected void ProcessInput()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Input.GetMouseButton(0))
        {
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject == this.gameObject)
                {
                    // 직접 닿은 표면에 압력을 주기 위해서
                    // 충돌점으로부터 법선벡터 쪽으로 약간 올라간 좌표를 입력. 
                    const float hitPointOffset = 0.1f;
                    Vector3 point = hit.point + hit.normal * hitPointOffset;
                    Press(0, point);
                    //
                }
                //
            }
        }
    }

    public virtual void Press(int fingerId, Vector3 pos)
    {
        controllerSO.SetFingerPressure(fingerId, 1);
    }
}