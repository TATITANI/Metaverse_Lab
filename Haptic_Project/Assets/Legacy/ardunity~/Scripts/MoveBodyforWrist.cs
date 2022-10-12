using System;
using System.Collections;
using System.IO.Ports;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class MoveBodyforWrist : MonoBehaviour
{
    List<string> name = new List<string>();
    GameObject obj;
    Vector3 first;

    public GameObject palm3;


    public GameObject index3;
    public GameObject mid3;
    public GameObject thumb3;

    //Ray 판단용 bool변수
    bool IsBorderPlasticTable;
    bool IsBorderMetalTable;
    bool IsBorderWoodTable;


    FingerStop fingerstop;

    //Ray가 벽에 닿으면 ISBorderPlasticTable 에 true를 대입한다.
    void StopToWall()
    {
        Debug.DrawRay(transform.position, transform.up *-0.4f, Color.red); // 레이표시
        Debug.DrawRay(index3.transform.position, transform.up * -0.4f, Color.red); // 레이표시
        Debug.DrawRay(mid3.transform.position, transform.up * -0.4f, Color.red); // 레이표시
        Debug.DrawRay(thumb3.transform.position, transform.up * -0.4f, Color.red); // 레이표시
        IsBorderPlasticTable = Physics.Raycast(transform.position,new Vector3(transform.position.x, transform.position.y-1, transform.position.z), 1.0f, LayerMask.GetMask("PlasticTable"))|| Physics.Raycast(palm3.transform.position, new Vector3(palm3.transform.position.x, palm3.transform.position.y - 1, palm3.transform.position.z), 0.4f, LayerMask.GetMask("PlasticTable"));
        IsBorderMetalTable = Physics.Raycast(transform.position, new Vector3(transform.position.x, transform.position.y - 1, transform.position.z), 1.0f, LayerMask.GetMask("MetalTable")) || Physics.Raycast(palm3.transform.position, new Vector3(palm3.transform.position.x, palm3.transform.position.y - 1, palm3.transform.position.z), 0.4f, LayerMask.GetMask("MetalTable"));
        IsBorderWoodTable = Physics.Raycast(transform.position, new Vector3(transform.position.x, transform.position.y - 1, transform.position.z), 1.0f, LayerMask.GetMask("WoodTable")) || Physics.Raycast(palm3.transform.position, new Vector3(palm3.transform.position.x, palm3.transform.position.y - 1, palm3.transform.position.z), 0.4f, LayerMask.GetMask("WoodTable"));

    }
    
    // Start is called before the first frame update
    void Start()
    {
        IsBorderPlasticTable= IsBorderMetalTable= IsBorderWoodTable = false;
        obj = GameObject.Find("nose");


        fingerstop = GetComponent<FingerStop>();


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
    void FixedUpdate()
    {
        var k = obj.GetComponent<SocketCommunicator>();
        int t = name.IndexOf(this.gameObject.name);

        var list = k.getList();

        Vector3 ls = new Vector3((float)list[11].x, -(float)list[11].y, (float)list[11].z); //왼쪽 어꺠 좌표
        Vector3 rs = new Vector3((float)list[12].x, -(float)list[12].y, (float)list[12].z); // 오른쪽 어깨 좌표
        
        float len = Vector3.Magnitude(ls - rs);

        Vector3 now = new Vector3((float)list[t].x, -(float)list[t].y, (float)list[t].z);

        now = now - (ls + rs) / 2.0f;
        now = now / len;




        Vector3 re = new Vector3((float)list[14].x, - (float)list[14].y, (float)list[14].z); // �ε���14�� ������ �Ȳ�ġ�̴�.
        re = re - (ls + rs) / 2.0f;
        re = re / len;
        var vec = now - re;
            
        if(Mathf.Atan(vec.y / vec.z) > -0.3f)
        {
             now.z = -2.5f;
        }
        else
        {
            now.z = -1.0f;            
        }
                    
        StopToWall();

        Vector3 difference = now - first;
        difference = difference * 3;
        var rgd = GetComponent<Rigidbody>();

        if(IsBorderPlasticTable || index3.GetComponent<FingerStop>().CheckContact()||mid3.GetComponent<FingerStop>().CheckContact()||thumb3.GetComponent<FingerStop>().CheckContact())
        {
            difference = new Vector3(difference.x, 0, difference.z);
            rgd.MovePosition(rgd.position + difference * Time.deltaTime);
        }
        //transform.Translate(difference * Time.deltaTime * 0.5f);
        else if (IsBorderMetalTable || index3.GetComponent<FingerStop>().CheckContact() || mid3.GetComponent<FingerStop>().CheckContact() || thumb3.GetComponent<FingerStop>().CheckContact())
        {
            difference = new Vector3(difference.x, 0, difference.z);
            rgd.MovePosition(rgd.position + difference * Time.deltaTime);
        }
        else if (IsBorderMetalTable || index3.GetComponent<FingerStop>().CheckContact() || mid3.GetComponent<FingerStop>().CheckContact() || thumb3.GetComponent<FingerStop>().CheckContact())
        {
            difference = new Vector3(difference.x, 0, difference.z);
            rgd.MovePosition(rgd.position + difference * Time.deltaTime);
        }
        else
        {
            rgd.MovePosition(rgd.position + difference * Time.deltaTime);
        }
        first = rgd.position;
    }
}
