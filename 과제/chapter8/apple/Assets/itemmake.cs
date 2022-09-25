using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class itemmake : MonoBehaviour
{
    public GameObject applePrefab;
    public GameObject bombPrefab;
    float span = 1.0f;
    float delta = 0;
    int ratio = 2;
    float speed = -0.03f;
    // Use this for initialization
    void Start () {
		
	}
    public void SetParameter(float span, float speed, int ratio)
    {
        this.span = span;
        this.speed = speed;
        this.ratio = ratio;
    }
	
	// Update is called once per frame
	void Update () {
        this.delta += Time.deltaTime;
        if(this.delta > this.span)
        {
            this.delta = 0.0f;
            GameObject item;
            int dice = Random.Range(1,11);
            if(dice <= this.ratio)
            {
                item = Instantiate(bombPrefab) as GameObject;
            } else
            {
                item = Instantiate(applePrefab) as GameObject;
            }
            float x = Random.Range(-1, 2);
            float z = Random.Range(-1, 3);
            item.transform.position = new Vector3(x, 4, z);
        }
		
	}
}
