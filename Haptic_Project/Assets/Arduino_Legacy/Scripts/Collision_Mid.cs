using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collision_Mid : MonoBehaviour
{
    public bool touchedwoodball;
    public bool touchedmetalball;
    public bool touchedplasticball;

    /*
    Vector3 indexdiff;
    Vector3 middiff;
    Vector3 thumbdiff;

    Vector3 indexfirst;
    Vector3 midfirst;
    Vector3 thumbfirst;

    Vector3 indexlast;
    Vector3 midlast;
    Vector3 thumblast;
    */

    public AudioClip woodsound;
    public AudioClip metalsound;
    public AudioClip plasticsound;
    public AudioClip fixedwoodsound;
    public AudioClip fixedmetalsound;
    public AudioClip fixedplasticsound;

    AudioSource aud;


    public GameObject index3;
    public GameObject mid3;
    public GameObject thumb3;
    public GameObject CenterPalm;
    public GameObject DerivedRotation_x;
    public GameObject woodball;
    public GameObject metalball;
    public GameObject plasticball;

    public SerialCommunicator sr;

    bool IsBorder;

    void PlayingSound(int num)
    {
        if (num == 0)
        {
            this.aud.PlayOneShot(this.woodsound);
        }
        else if (num == 1)
        {
            this.aud.PlayOneShot(this.metalsound);
        }
        else if (num == 2)
        {
            this.aud.PlayOneShot(this.plasticsound);

        }
        else if (num == 3)
        {
            this.aud.PlayOneShot(this.fixedwoodsound);

        }
        else if (num == 4)
        {
            this.aud.PlayOneShot(this.fixedmetalsound);

        }
        else if (num == 5)
        {
            this.aud.PlayOneShot(this.fixedplasticsound);

        }
    }

    void Start()
    {
        this.aud = GetComponent<AudioSource>();
        sr = GameObject.Find("Hand (3)").GetComponent<SerialCommunicator>();
    }
    void LateUpdate()
    {
        //print("Mid finger : " + DerivedRotation_x.transform.localRotation.x);

        if (isGrab())
        {
            sr.TxBuffer.servoAct2 = 50;
            print(string.Format("Index ON! {0}", DerivedRotation_x.transform.localRotation.x));
        }
        else
        {
            print(string.Format("Index OFF! {0}", DerivedRotation_x.transform.localRotation.x));
            sr.TxBuffer.servoAct2 = 180;
        }

        if (touchedwoodball && DerivedRotation_x.transform.localRotation.x < -0.30f)
        {
            //Debug.Log("Mid finger : " + DerivedRotation_x.transform.localRotation.x);
            woodball.transform.position = new Vector3(CenterPalm.transform.position.x, CenterPalm.transform.position.y - 0.4f, CenterPalm.transform.position.z);
        }
        else
            touchedwoodball = false;

        if (touchedmetalball && DerivedRotation_x.transform.localRotation.x < -0.30f)
        {
            //Debug.Log("def" + DerivedRotation_x.transform.localRotation.x);

            metalball.transform.position = new Vector3(CenterPalm.transform.position.x, CenterPalm.transform.position.y - 0.4f, CenterPalm.transform.position.z);
        }
        else
            touchedmetalball = false;

        if (touchedplasticball && DerivedRotation_x.transform.localRotation.x < -0.30f)
        {
            //Debug.Log("ghi" + DerivedRotation_x.transform.localRotation.x);

            plasticball.transform.position = new Vector3(CenterPalm.transform.position.x, CenterPalm.transform.position.y - 0.4f, CenterPalm.transform.position.z);
        }
        else
            touchedplasticball = false;

        if (isGrab())
        {
            sr.TxBuffer.servoAct0 = 70;
            print(string.Format("Mid ON! {0}", DerivedRotation_x.transform.localRotation.x));
        }
        else
        {
            sr.TxBuffer.servoAct0 = 180;
            print(string.Format("Mid OFF! {0}", DerivedRotation_x.transform.localRotation.x));
        }
    }

    //충돌시작
    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Wood")
        {
            PlayingSound(0);
            touchedwoodball = true;
        }
        else if (other.gameObject.tag == "Metal")
        {
            PlayingSound(1);
            touchedmetalball = true;
        }
        else if (other.gameObject.tag == "Plastic")
        {
            PlayingSound(2);
            touchedplasticball = true;
        }
        else if (other.gameObject.tag == "FixedWood")
            PlayingSound(3);
        else if (other.gameObject.tag == "FixedMetal")
            PlayingSound(4);
        else if (other.gameObject.tag == "FixedPlastic")
            PlayingSound(5);
    }
    //충돌중
    void OnCollisionStay(Collision other)
    {
        if (other.gameObject.tag == "Wood")
        {
            PlayingSound(0);
            touchedwoodball = true;

        }
        else if (other.gameObject.tag == "Metal")
        {
            PlayingSound(1);
            touchedmetalball = true;

        }
        else if (other.gameObject.tag == "Plastic")
        {
            PlayingSound(2);
            touchedplasticball = true;

        }
        else if (other.gameObject.tag == "FixedWood")//&& indexdiff.magnitude>0.1
        {
            PlayingSound(3);
        }

        else if (other.gameObject.tag == "FixedMetal")//&& indexdiff.magnitude > 0.1
        {
            PlayingSound(4);
        }
        else if (other.gameObject.tag == "FixedPlastic")//&& indexdiff.magnitude > 0.1
        {
            PlayingSound(5);
        }
    }
    
    bool isGrab()
    {
        if (touchedmetalball || touchedplasticball || touchedwoodball)
            return true;
        return false;
    }
}