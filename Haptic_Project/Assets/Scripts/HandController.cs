using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HandController : MonoBehaviour
{
    [Serializable]
    public class HandPivot
    {
        [SerializeField] private Transform pivot1;
        [SerializeField] private Transform pivot2;
        [SerializeField] private Transform pivot3;

        public Transform Pivot1 => pivot1;
        public Transform Pivot2 => pivot2;
        public Transform Pivot3 => pivot3;
    }

    public HandPivot leftHandPivot, rightHandPivot;


}