using System;
using System.Collections;
using System.Collections.Generic;
using Leap.Unity.Interaction;
using UnityEngine;
using UnityEngine.Events;


[RequireComponent(typeof(InteractionBehaviour))]
public class Grabbable : MonoBehaviour
{
    //[SerializeField] private HandControllerSO controllerSO;
    //private HandController controller;
    //private InteractionBehaviour interaction;

    //// [SerializeField] private UnityEvent<GrabPos> eventGrab;
    //// private GrabPos grabPos = new GrabPos();

    //private void Start()
    //{
    //    interaction = GetComponent<InteractionBehaviour>();
    //    // controller = AppManager.Instance.handController;
    //    interaction.OnGraspBegin = OnGrabBegin;
    //    //interaction.OnGraspStay = OnGrab;
    //    interaction.OnGraspEnd = OnGrabEnd;
    //}

    //public void OnGrabBegin()
    //{
    //    controllerSO.SetGrab(true);
    //    //SerialCommunicator.Instance.SendDummy("<1,1,1>");
    //}

    //public void OnGrabEnd()
    //{
    //    controllerSO.SetGrab(false);
    //    //SerialCommunicator.Instance.SendDummy("<0,0,0>");

    //    // controllerSO.ResetFingerPressure();
    //}

}