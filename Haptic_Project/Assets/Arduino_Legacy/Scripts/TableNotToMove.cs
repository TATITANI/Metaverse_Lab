using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableNotToMove : MonoBehaviour
{
    Rigidbody rb;

    Collision_Thumb collision_thumb;
    Collision_Mid collision_mid;
    Collision_Index collision_index;

    // Start is called before the first frame update
    void Start()
    {
        collision_thumb = GetComponent<Collision_Thumb>();
        collision_mid= GetComponent<Collision_Mid>();
        collision_index = GetComponent<Collision_Index>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void LateUpdate()
    {

        rb.AddForce(Vector3.down * 999999);

        /*
        if( collision_thumb.touchedmetalball  || collision_thumb.touchedplasticball || collision_thumb.touchedwoodball ||
            collision_mid.touchedmetalball || collision_mid.touchedplasticball || collision_mid.touchedwoodball ||
            collision_index.touchedmetalball || collision_index.touchedplasticball || collision_index.touchedwoodball)
        {
            sr.TxBuffer.servoAct0 = 70;//값은 레이를 통해서 결정해라.
            sr.TxBuffer.servoAct1 = 70;
            sr.TxBuffer.servoAct2 = 70;
            print("abc : Grap!");

        }
        else
        {
            sr.TxBuffer.servoAct0 = 180;//값은 레이를 통해서 결정해라.
            sr.TxBuffer.servoAct1 = 180;
            sr.TxBuffer.servoAct2 = 180;
            print("abc : Release");
        }
        */
    }
}