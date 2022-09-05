using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moonMove : MonoBehaviour
{
    //[SerializeField] GameObject earth; // world coordinate
    // float R = 7f; // world coordinate
    float R = 7f/5f; // local coordinate
    float omegat = 3.6f; // one turn in every Time.deltaTime * 500;   
    float theta = 0f;
    // Use this for initialization
    void Start()
    {
    }
    // Update is called once per frame
    void Update()
    {
        theta += omegat;
        if (theta > 360f) theta = 0f;
        float costh = Mathf.Cos(theta * 3.141592f / 180f);
        float sinth = Mathf.Sin(theta * 3.141592f / 180f);
        float r = R * (10f + sinth) / 10f;
        // Vector3 ep = earth.transform.position;  // world coordinate
        //transform.position = new Vector3(ep.x + r * costh, ep.y, ep.z + r * sinth); // world coordinate
        transform.localPosition = new Vector3(r * costh, 0f, r * sinth);
    }
}