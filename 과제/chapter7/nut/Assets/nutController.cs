using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class nutController : MonoBehaviour {

	// Use this for initialization
	void Start () {
       // Shoot(new Vector3(100, 200, 2000));
	}
    // 
    void OnCollisionEnter(Collision other)
    {
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<ParticleSystem>().Play();

        Debug.Log("collision");
    }
    public void Shoot(Vector3 dir)
    {
        Debug.Log("Shoot");
        GetComponent<Rigidbody>().AddForce(dir);
    }
    // Update is called once per frame
    void Update () {
		
	}
}
