using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RouletteController : MonoBehaviour {
    float rotSpeed = 0;
    float theta = 0;
	
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            this.rotSpeed = 10;
        }
        transform.Rotate(0, 0, this.rotSpeed);
        this.rotSpeed *= 0.96f;
    
    }
    
    /// <summary>
    /// 원 운동 기존 코드
    /// </summary>
	// void Update () {
 //        if (Input.GetMouseButtonDown(0))
 //        {
 //            this.rotSpeed = 0.1f;
 //        }
 //
 //        this.rotSpeed *= 0.96f;
 //        float dx, dy;
 //
 //        dx = Mathf.Cos(theta * 2f * 3.141592f / 180.0f);
 //        dy = Mathf.Sin(theta * 2f * 3.141592f / 180.0f);
 //        transform.Translate(dx, dy, 0);
 //    }
}
