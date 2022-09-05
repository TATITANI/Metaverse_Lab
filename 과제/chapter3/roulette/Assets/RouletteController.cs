using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RouletteController : MonoBehaviour {
    float rotSpeed = 0;
    float theta = 0;
	// Use this for initialization
	void Start () {

        transform.position = new Vector3(1f, 0, 0);
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            this.rotSpeed = 0.1f;
        }
        float dx, dy;
        dx = Mathf.Cos(theta * 2f * 3.141592f / 180.0f);
        dy = Mathf.Sin(theta * 2f * 3.141592f / 180.0f);
        transform.Translate(dx, dy, 0);
        
        this.rotSpeed *= 0.96f;

    }
}
