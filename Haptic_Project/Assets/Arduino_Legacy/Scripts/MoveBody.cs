using System;
using System.Collections;
using System.IO.Ports;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class MoveBody : MonoBehaviour
{
    List<string> name = new List<string>();
    GameObject obj;
    Vector3 first;

    // Start is called before the first frame update
    void Start()
    {
        obj = GameObject.Find("nose");

        first = new Vector3(0,0,0);

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

        var list = k.getList();

        Vector3 ls = new Vector3((float)list[11].x, -(float)list[11].y, (float)list[11].z);
        Vector3 rs = new Vector3((float)list[12].x, -(float)list[12].y, (float)list[12].z);
        
        float len = Vector3.Magnitude(ls - rs);

        Vector3 now = new Vector3((float)list[t].x, -(float)list[t].y, (float)list[t].z);
        /*
        if (t == 0) 
        {
            now = new Vector3((float)(list[7].x + list[8].x), -(float)(list[7].y + list[8].y), (float)(list[7].z + list[8].z));
            now /= 2.0f;
        }
        */
        now = now - (ls + rs) / 2.0f;
        now = now / len;

        if(t==0)
        {
            now = new Vector3(0f,0f,0f);
        }

        if (t==16) // t==16�϶��� MoveBody Script�� ������ �ո� ����� ��� �ش�ȴ�.
        {   
            Vector3 re = new Vector3((float)list[14].x, - (float)list[14].y, (float)list[14].z); // �ε���14�� ������ �Ȳ�ġ�̴�.
            re = re - (ls + rs) / 2.0f;
            re = re / len;

            var vec = now - re;

            
           // print(Mathf.Atan(vec.y / vec.z));

            if(Mathf.Atan(vec.y / vec.z) > -0.3f)
            {
                now.z = -2.5f;
       
            }
            else
            {
                now.z = -1.0f;
            
            
            }
        }


        Vector3 difference = now - first;
        
        transform.Translate(difference * Time.deltaTime);
        first = transform.position;
    }
}
