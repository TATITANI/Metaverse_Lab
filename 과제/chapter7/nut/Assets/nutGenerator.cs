using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class nutGenerator : MonoBehaviour {
    public GameObject bamsongiPrefab;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            GameObject nut = Instantiate(bamsongiPrefab) as GameObject;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 worldDir = ray.direction; 
            nut.GetComponent<nutController>().Shoot(worldDir.normalized * 2000);
        }
	}
}
