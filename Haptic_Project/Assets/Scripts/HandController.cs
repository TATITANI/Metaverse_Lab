using System;
using UnityEngine;


public class HandController : MonoBehaviour
{
    [Serializable]
    public class HandPivot
    {
        public enum HandType
        {
            LEFT,
            RIGHT
        };

        [SerializeField] HandType handType = HandType.LEFT;
        [SerializeField] private Transform[] pivots = new Transform[5];
        public Transform[] Pivots => pivots;
        public Vector3 GetGrabDir(int fingerID)
        {
            // Note : 쥐는 방향  - 왼손 : up, 오른손 : down
            return pivots[fingerID].up * (handType == HandType.LEFT ? 1 : -1);
        }
    }

    [Header("0: 엄지 ~ 5: 새끼 손가락")] public HandPivot leftHandPivot, rightHandPivot;

    private void Update()
    {
        HandPivot[] handPivots = { leftHandPivot, rightHandPivot };
        foreach (var handPivot in handPivots)
        {
            var pivots = handPivot.Pivots;
            for (int i = 0; i < pivots.Length; i++)
            {
                Vector3 dir = handPivot.GetGrabDir(i);
                Debug.DrawLine(pivots[i].position, pivots[i].position + 0.001f * dir,
                    Color.red);
            }
        }
    }
}