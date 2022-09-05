using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ballControl : MonoBehaviour {
    public GameObject ballPreFab;
    float span = 1.0f;
    float delta = 0f;
    void Start()
    {
        int step = 3;
        Debug.Log("Hello World" + step);
    }
	// Update is called once per frame
	void Update () {
        this.delta += Time.deltaTime;
        if(this.delta > this.span)
        {
            GameObject go = Instantiate(ballPreFab) as GameObject;
            this.delta = 0f;
        }        
	}
}
