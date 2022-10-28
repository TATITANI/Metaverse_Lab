using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ElasticBodyData", menuName = "Scriptable Object/ElasticBodyData", order = int.MaxValue)]
public class ElasticBodySO : ScriptableObject
{
    // 탄성
    [Min(0)] public float elasticity = 5f;
    // 누르는 압력
    [Min(0)] public float power = 5f;
    // 감쇠
    [Min(0)] public float damping = 5f;
    // 압력지점 거리에 따른 감쇠
    [Min(0)] public float attenuation = 15f;

}
