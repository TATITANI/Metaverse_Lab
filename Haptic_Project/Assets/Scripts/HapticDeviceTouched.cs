using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HapticDeviceTouched : MonoBehaviour
{
    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.name == "Grabber")
        {
            Debug.Log("OnCollisionEnter");
            //gameObject.GetComponent<Rigidbody>().isKinematic = true;
            gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    void OnCollisionStay(Collision col)
    {
        if (col.gameObject.name == "Grabber")
        {
            Debug.Log("OnCollisionStay");
            //gameObject.GetComponent<Rigidbody>().isKinematic = true;
            //gameObject.GetComponent<Rigidbody>().freezeRotation(true, true, true);
            
        }
    }

    void OnCollisionExit(Collision col)
    {
        if (col.gameObject.name == "Grabber")
        {
            Debug.Log("OnCollisionExit");
            //gameObject.GetComponent<Rigidbody>().isKinematic = false;
            gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
}
