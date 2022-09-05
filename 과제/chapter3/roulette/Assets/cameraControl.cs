using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraControl : MonoBehaviour {

    float rotSpeed = 0;
    // Use this for initialization
    void Start()
    {
    
    }
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            this.rotSpeed = 10;
        }
        transform.Rotate(0, 0, this.rotSpeed);
        this.rotSpeed *= 0.96f;
    
    }
    
    
}
