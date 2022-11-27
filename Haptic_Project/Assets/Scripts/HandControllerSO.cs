using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[CreateAssetMenu(fileName = "HandControllerData", menuName = "Scriptable Object/HandControllerData",
    order = int.MaxValue)]
public class HandControllerSO : ScriptableObject
{
    private bool isGrab = false;
    public bool IsGrab { get { return isGrab; }  }

    // 잡은 물체의 탄성계수
    // 100 : 안잡음.
    int elasticity = 100; 
    public int Elasticity { get { return elasticity; }  }

    [System.Serializable]
    public class PressureInfo
    {
        public float fingerPressure = 0; // 0~1
    }

    public UnityEvent<int> OnChangedGrab = new UnityEvent< int>(); // pressure


    public PressureInfo[] pressureRight { get; private set; } = new PressureInfo[3];

    public void SetFingerPressure(int fingerID, float pressure)
    {
        pressureRight[fingerID].fingerPressure = pressure;
    }

    /// <param name="elasticity"> 잡지 않았을 때 : 100 </param>
    public void SetGrab(bool isGrab, int elasticity = 100)
    {
        this.isGrab = isGrab;
        this.elasticity = elasticity;
    }
    public void ResetFingerPressure()
    {
        Array.ForEach(pressureRight, p => p.fingerPressure = 0);
    }
}