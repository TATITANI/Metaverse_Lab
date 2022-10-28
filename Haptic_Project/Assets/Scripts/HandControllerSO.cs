using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "HandControllerData", menuName = "Scriptable Object/HandControllerData", order = int.MaxValue)]
public class HandControllerSO : ScriptableObject
{
    public bool isGrab;
    [System.Serializable]
    public class PressureInfo
    {
        public int vertexID = -1;
        public float fingerPressure = 0;
        public bool isPress = false;
    }
    public PressureInfo[] pressureLeft { get; private set; } = new PressureInfo[5];
    public PressureInfo[] pressureRight { get; private set; } = new PressureInfo[5];
    
    public void SetFingerPressingVertex(bool isLeft, int fingerID, int vertexID)
    {
        if (isLeft)
            pressureLeft[fingerID].vertexID = vertexID;
        else
            pressureRight[fingerID].vertexID = vertexID;
    }

    public void SetFingerPressure(bool isLeft, int fingerID, float pressure)
    {
        if (isLeft)
            pressureLeft[fingerID].fingerPressure = pressure;
        else
        {
            pressureRight[fingerID].fingerPressure = pressure;
        }
    }

    public void ResetFingerPressure()
    {
        Array.ForEach(pressureLeft, p => p.fingerPressure = 0);
        Array.ForEach(pressureRight, p => p.fingerPressure = 0);
    }
}
