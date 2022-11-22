using System;
using System.Collections;
using System.Collections.Generic;
using Leap.Unity.Interaction;
using UnityEngine;
using UnityEngine.Events;


[RequireComponent(typeof(InteractionBehaviour))]
public class Grabbable : MonoBehaviour
{
    [SerializeField] private HandControllerSO controllerSO;
    private HandController controller;
    private InteractionBehaviour interaction;

    // [SerializeField] private UnityEvent<GrabPos> eventGrab;
    // private GrabPos grabPos = new GrabPos();

    private void Start()
    {
        interaction = GetComponent<InteractionBehaviour>();
        // controller = AppManager.Instance.handController;
        interaction.OnGraspBegin = OnGrabBegin;
        //interaction.OnGraspStay = OnGrab;
        interaction.OnGraspEnd = OnGrabEnd;
    }

    public void OnGrabBegin()
    {
        controllerSO.isGrab = true;
        //SerialCommunicator.Instance.SendDummy("<1,1,1>");
    }

    public void OnGrabEnd()
    {
        controllerSO.isGrab = false;
        //SerialCommunicator.Instance.SendDummy("<0,0,0>");

        // controllerSO.ResetFingerPressure();
    }

    // public void OnGrab()
    // {
    //     RaycastHit hit;
    //     foreach (var hand in interaction.graspingHands)
    //     {
    //         if(hand.isLeft)
    //             continue;
    //         
    //         HandPivot rightHandPivot = controller.rightHandPivot;
    //
    //         var pivots = rightHandPivot.Pivots;
    //         for (int fingerID = 0; fingerID < pivots.Length; fingerID++)
    //         {
    //             Vector3 dir = rightHandPivot.GetGrabDir(fingerID);
    //             Ray ray = new Ray(pivots[fingerID].position, dir);
    //             if (Physics.Raycast(ray, out hit, 0.001f))
    //             {
    //                 if (hit.collider.gameObject == this.gameObject)
    //                 {
    //                     // 직접 닿은 표면에 압력을 주기 위해서
    //                     // 충돌점으로부터 법선벡터 쪽으로 약간 올라간 좌표를 입력. 
    //                     const float hitPointOffset = 0.05f;
    //                     Vector3 contctPos = hit.point + hit.normal * hitPointOffset;
    //
    //                     grabPos.fingerID = fingerID;
    //                     grabPos.pos = contctPos;
    //                     eventGrab?.Invoke(grabPos);
    //                 }
    //             }
    //         }
    //     }
    // }
}