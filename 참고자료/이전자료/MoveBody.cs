using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBody : MonoBehaviour
{
    List<string> name = new List<string>();
    GameObject obj;

    // Start is called before the first frame update
    void Start()
    {
        obj = GameObject.Find("nose");

        name.Add("nose");
        name.Add("left_eye_inner");
        name.Add("left_eye");
        name.Add("left_eye_outer");
        name.Add("right_eye_inner");
        name.Add("right_eye");
        name.Add("right_eye_outter");
        name.Add("left_ear");
        name.Add("right_ear");
        name.Add("mouth_left");
        name.Add("mouth_right");
        name.Add("left_shoulder");
        name.Add("right_shoulder");
        name.Add("left_elbow");
        name.Add("right_elbow");
        name.Add("left_wrist");
        name.Add("right_wrist");
        name.Add("left_hip");
        name.Add("right_hip");
    }

    // Update is called once per frame
    void Update()
    {
        var k = obj.GetComponent<SocketCommunicator>();
        int t = name.IndexOf(this.gameObject.name);
        float x = (float)k.getList()[t].x;
        float y = (float)k.getList()[t].y;
        float z = (float)k.getList()[t].z;
        x = (1 - x) * 2;
        y = (1 - y) * 2;
        z = (z) * 2;
        transform.position = new Vector3(x, y, z);
    }
}
