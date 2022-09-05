using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class earthMove : MonoBehaviour {
    float R = 40f;
    float y0 = 5f;
    float omegat = 0.72f; // one turn in every Time.deltaTime * 500; 
    float theta = 0f; 
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        theta += omegat;
        if (theta > 360f) theta = 0f;
        float costh = Mathf.Cos(theta * 3.141592f / 180f);
        float sinth = Mathf.Sin(theta * 3.141592f / 180f);
        float r = R * (10f + costh) / 10f;
        transform.position = new Vector3(r*costh, y0, r*sinth);
	}
}
