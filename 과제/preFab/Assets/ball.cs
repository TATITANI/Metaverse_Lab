using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ball : MonoBehaviour {

	// Use this for initialization
	void Start () {
        float x, y, z;
        x = Random.Range(-5f, 5f);
        y = Random.Range(4f, 6f);
        z = Random.Range(-5f, 5f);
        this.transform.position = new Vector3(x, y, z);
        Debug.Log("(" + x + "," + y + "," + z);
    }
	
	// Update is called once per frame
	void Update () {
        transform.Translate(0, -0.1f, 0);
        if(transform.position.y < -5f)
        {
            Destroy(gameObject);
        }
	}
}
