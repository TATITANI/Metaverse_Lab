using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class carController : MonoBehaviour {
    float speed = 0;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            this.speed = 0.2f;
        } else if (Input.GetMouseButtonUp(0))
        {
            GetComponent<AudioSource>().Play();
        }
            transform.Translate(this.speed, 0, 0);
        this.speed *= 0.98f;
	}
}
