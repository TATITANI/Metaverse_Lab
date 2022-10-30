using System;
using Unity.VisualScripting;
using UnityEditor.TerrainTools;
using UnityEngine;


public class GrabPos
{
    public int fingerID;
    public Vector3 pos;
};


[System.Serializable]
public class HandPivot
{
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

    private GrabPos grabPos = new GrabPos();
    [SerializeField] private float colDetectDistance = 0.001f;

    private void Update()
    {
        var pivots = rightHandPivot.Pivots;
        for (int fingerID = 0; fingerID < pivots.Length; fingerID++)
        {
            Vector3 dir = rightHandPivot.GetGrabDir(fingerID);
            Vector3 rayStartPos = pivots[fingerID].position - dir * 0.01f;
            Ray ray = new Ray(rayStartPos, dir);

            // data.pressureRight[fingerID].isPress = false;
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, colDetectDistance))
            {
                ElasticBody elasticBody = hit.transform.GetComponent<ElasticBody>();
                data.pressureRight[fingerID].isPress = !ReferenceEquals(elasticBody, null);

                if (data.pressureRight[fingerID].isPress)
                {
                    // 직접 닿은 표면에 압력을 주기 위해서
                    // 충돌점으로부터 법선벡터 쪽으로 약간 올라간 좌표를 입력. 
                    const float hitPointOffset = 0.01f;
                    Vector3 contctPos = hit.point + hit.normal * hitPointOffset;

                    grabPos.fingerID = fingerID;
                    grabPos.pos = contctPos;
                    elasticBody.Press(grabPos);
                }
            }
            Debug.DrawLine(rayStartPos, 
                rayStartPos + colDetectDistance * dir,
                Color.red);
        }
    }

    // private void OnDrawGizmos()
    // {
    //     var pivots = rightHandPivot.Pivots;
    //     for (int fingerID = 0; fingerID < pivots.Length; fingerID++)
    //     {
    //         Gizmos.DrawWireSphere(pivots[fingerID].position, 0.001f);
    //     }
    // }
}