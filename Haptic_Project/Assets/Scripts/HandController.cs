using System;
using Unity.VisualScripting;
using UnityEditor.TerrainTools;
using UnityEngine;


[System.Serializable]
public class HandPivot
{
    // public enum HandType
    // {
    //     LEFT,
    //     RIGHT
    // };

    [SerializeField] private Transform[] pivots = new Transform[3];
    public Transform[] Pivots => pivots;

    public Vector3 GetGrabDir(int fingerID)
    {
        // Note : 쥐는 방향  - 왼손 : up, 오른손 : down
        return -pivots[fingerID].up;
    }
}


public class HandController : MonoBehaviour
{
    [SerializeField] HandControllerSO data;
    [Header("0: 엄지 ~ 2: 중지")] public HandPivot rightHandPivot;

    RaycastHit hit;

    private void Update()
    {
        var pivots = rightHandPivot.Pivots;
        for (int fingerID = 0; fingerID < pivots.Length; fingerID++)
        {
            Vector3 dir = rightHandPivot.GetGrabDir(fingerID);
            Ray ray = new Ray(pivots[fingerID].position, dir);
            data.pressureRight[fingerID].isPress
                = Physics.Raycast(ray, out hit, 0.001f);
            
            Debug.DrawLine(pivots[fingerID].position, pivots[fingerID].position + 0.01f * dir,
                Color.red);
        }
    }

}