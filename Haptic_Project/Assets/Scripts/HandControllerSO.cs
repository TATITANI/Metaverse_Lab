using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "HandControllerData", menuName = "Scriptable Object/HandControllerData",
    order = int.MaxValue)]
public class HandControllerSO : ScriptableObject
{
    public bool isGrab = false;

    [System.Serializable]
    public class PressureInfo
    {
        public float fingerPressure = 0; // 0~1
    }

    public PressureInfo[] pressureRight { get; private set; } = new PressureInfo[3];

    public void SetFingerPressure(int fingerID, float pressure)
    {
        pressureRight[fingerID].fingerPressure = pressure;
    }

    public void ResetFingerPressure()
    {
        Array.ForEach(pressureRight, p => p.fingerPressure = 0);
    }
}