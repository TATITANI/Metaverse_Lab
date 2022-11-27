using System;
using Unity.VisualScripting;
using UnityEditor.TerrainTools;
using UnityEngine;


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

    /// <summary>
    /// press는 touch 판정 범위 안에 속함
    /// 구분 이유 : 손으로 물체를 누르면 순간적으로 충돌지점이
    /// press 범위를 벗어나 press가 0이 되는 현상 방지
    /// </summary>
    [SerializeField] private float touchCheckingPosOffset = 0.01f;

    [SerializeField] private float pressCheckingDistance = 0.01f;
    [SerializeField] private float touchRange = 2; // 터치 판정 조정


    private void Update()
    {
        var pivots = rightHandPivot.Pivots;
        for (int fingerID = 0; fingerID < pivots.Length; fingerID++)
        {
            Vector3 dir = rightHandPivot.GetGrabDir(fingerID);
            Vector3 rayStartPos = pivots[fingerID].position - dir * touchCheckingPosOffset;
            Ray ray = new Ray(rayStartPos, dir);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, pressCheckingDistance * touchRange))
            {
                if (Vector3.Distance(rayStartPos, hit.point) < pressCheckingDistance)
                {
                    Body body = hit.transform.GetComponent<Body>();
                    if (!ReferenceEquals(body, null))
                    {
                        // 직접 닿은 표면에 압력을 주기 위해서
                        // 충돌점으로부터 법선벡터 쪽으로 약간 올라간 좌표를 입력. 
                        const float hitPointOffset = 0.01f;
                        Vector3 contactPos = hit.point + hit.normal * hitPointOffset;

                        body.Press(fingerID, contactPos);
                    }
                    else if (hit.transform.name == "StateButton")
                    {
                        Animator btnAni = hit.transform.GetComponent<Animator>();
                        if(btnAni.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
                        {
                            btnAni.Play("btnDown");
                            AppManager.Instance.ChangeStage();
                        }
                    }
                }
            }
            else
            {
                data.SetFingerPressure(fingerID, 0);
            }

            Debug.DrawLine(rayStartPos, rayStartPos + pressCheckingDistance * dir, Color.red);
            Debug.DrawLine(rayStartPos, rayStartPos + pressCheckingDistance * touchRange * dir, Color.blue);
        }
    }
}