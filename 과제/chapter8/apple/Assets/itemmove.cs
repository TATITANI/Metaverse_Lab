using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class itemmove : MonoBehaviour {
    public float dropSpeed = -0.03f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.Translate(0, this.dropSpeed, 0);
        if(transform.position.y < -1.0f)
        {
            Destroy(gameObject);
        }
	}
}
