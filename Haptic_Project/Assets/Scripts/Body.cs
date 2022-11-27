using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity.Interaction;

[RequireComponent(typeof(InteractionBehaviour))]
public class Body : MonoBehaviour
{
    protected Camera cam;
    [SerializeField] protected HandControllerSO controllerSO;
    protected InteractionBehaviour interaction;

    protected virtual void Start()
    {
        interaction = GetComponent<InteractionBehaviour>();
        cam = Camera.main;
        interaction.OnGraspBegin = OnGrabBegin;
        interaction.OnGraspEnd = OnGrabEnd;
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

    protected virtual void OnGrabBegin()
    {
        // 강체 탄성계수 0
        controllerSO.SetGrab(true, 0);
    }

    protected virtual void OnGrabEnd()
    {
        controllerSO.SetGrab(false);
        // controllerSO.ResetFingerPressure();
    }
}