using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collision_Index : MonoBehaviour
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
    // Start is called before the first frame update
    void Start()
    {
        this.aud = GetComponent<AudioSource>();
        sr = GameObject.Find("Hand (3)").GetComponent<SerialCommunicator>();
        /*
        indexfirst = new Vector3(0, 0, 0);
        midfirst = new Vector3(0, 0, 0);
        thumbfirst = new Vector3(0, 0, 0);
        */
    }
    void LateUpdate()
    {
        /*
        indexlast=index4.transform.position;
        midlast=mid4.transform.position;
        thumblast = thumb3.transform.position;
        
        indexdiff = indexlast - indexfirst;
        middiff = midlast - midfirst;
        thumbdiff = thumblast - thumbfirst;

        indexfirst = indexlast;
        midfirst = midlast;
        thumbfirst = thumblast;
        */

        //Debug.Log("Index finger : " + DerivedRotation_x.transform.localRotation.x);

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

        if (touchedwoodball && DerivedRotation_x.transform.localRotation.x < -0.16f)// 
        {
            //debug.log("abc"+derivedrotation_x.transform.localrotation.x); //일단 위에 if 조건에서 두번째 and조건 빼면 손가락 마지막 마디에 충돌판정되면 공이 손바닥에 달라붙는다. 그 상태에서 debug.log값을 손가락 straight, bended 상태 에서 확인한다음 if문의 and 조건으로 넣어주자.
            woodball.transform.position = new Vector3(CenterPalm.transform.position.x, CenterPalm.transform.position.y - 0.4f, CenterPalm.transform.position.z);
        }
        else
        {
            touchedwoodball = false;
        }
        if (touchedmetalball && DerivedRotation_x.transform.localRotation.x < -0.16f)// && derivedrotation_x.transform.localrotation.x < -0.4f
        {
            //debug.log("def" + derivedrotation_x.transform.localrotation.x);

            metalball.transform.position = new Vector3(CenterPalm.transform.position.x, CenterPalm.transform.position.y - 0.4f, CenterPalm.transform.position.z);
        }
        else
        {
            touchedmetalball = false;
        }
        if (touchedplasticball && DerivedRotation_x.transform.localRotation.x < -0.16f)// && derivedrotation_x.transform.localrotation.x < -0.4f
        {
            //debug.log("ghi" + derivedrotation_x.transform.localrotation.x);

            plasticball.transform.position = new Vector3(CenterPalm.transform.position.x, CenterPalm.transform.position.y - 0.4f, CenterPalm.transform.position.z);
        }
        else
        {
            touchedplasticball = false;
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
        else if (other.gameObject.tag == "FixedWood")//&& indexdiff.magnitude > 0.1
        {
            PlayingSound(3);
        }

        else if (other.gameObject.tag == "FixedMetal")// && indexdiff.magnitude > 0.1
        {
            PlayingSound(4);
        }
        else if (other.gameObject.tag == "FixedPlastic")//&& indexdiff.magnitude > 0.1
        {
            PlayingSound(5);
        }
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
    /*
    //충돌끝
    void OnCollisionExit(Collision other)
    {
        if (other.gameObject.tag == "Wood")
        {
            Debug.Log("Wood");
            this.aud.PlayOneShot(this.woodsound);
        }
        else if (other.gameObject.tag == "FixedWood")
        {
            Debug.Log("Wood");
            this.aud.PlayOneShot(this.woodsound);
        }
        else if (other.gameObject.tag == "Metal")
        {
            Debug.Log("Metal");
            this.aud.PlayOneShot(this.metalsound);
        }
        else if (other.gameObject.tag == "FixedMetal")
        {
            Debug.Log("Wood");
            this.aud.PlayOneShot(this.woodsound);
        }
        else if (other.gameObject.tag == "Plastic")
        {
            Debug.Log("Plastic");
            this.aud.PlayOneShot(this.plasticsound);
        }
        else if (other.gameObject.tag == "FixedPlastic")
        {
            Debug.Log("Plastic");
            this.aud.PlayOneShot(this.plasticsound);
        }
    }
    */
    bool isGrab()
    {
        if (touchedmetalball || touchedplasticball || touchedwoodball)
            return true;
        return false;

    }
}