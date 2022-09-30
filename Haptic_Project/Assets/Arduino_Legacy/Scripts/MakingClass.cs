using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Collision_made 스크립트를 모든 손가락 끝마디에 적용한다.
public class Collision_made
{
    //공 충돌 판정 변수
    public bool touchedwoodball;
    public bool touchedmetalball;
    public bool touchedplasticball;

    //오디오파일
    public AudioClip woodsound;
    public AudioClip metalsound;
    public AudioClip plasticsound;
    public AudioClip fixedwoodsound;
    public AudioClip fixedmetalsound;
    public AudioClip fixedplasticsound;

    public AudioSource aud;

    //게임오브젝트
    public GameObject index3;
    public GameObject mid3;
    public GameObject thumb3;
    public GameObject CenterPalm;
    public GameObject DerivedRotation_x;
    public GameObject woodball;
    public GameObject metalball;
    public GameObject plasticball;

    public SerialCommunicator sr;

    public bool IsBorder;

    public float ThresHold_Value_Thumb = -0.5f;
    public float ThresHold_Value_Index = -0.16f;
    public float ThresHold_Value_Mid = -0.30f;

    public void PlayingSound(int num)
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



    public bool isGrab()
    {
        if (touchedmetalball || touchedplasticball || touchedwoodball)
            return true;
        return false;
    }
}


//이 스크립트 부터는 새로운 스크립트에 옮기고 손가락 끝마디들을 아웃렛접속시킨다.
public class MakingClass : MonoBehaviour
{
    public Collision_made Thumb3;
    public Collision_made Index3;
    public Collision_made Mid3;

    // Start is called before the first frame update
    void Start()
    {
        Thumb3 = GetComponent<Collision_made>();
        Index3 = GetComponent<Collision_made>();
        Mid3 = GetComponent<Collision_made>();

        Thumb3.aud = GetComponent<AudioSource>();
        Index3.aud = GetComponent<AudioSource>();
        Mid3.aud = GetComponent<AudioSource>();

        Thumb3.sr = GameObject.Find("Hand (3)").GetComponent<SerialCommunicator>();
        Index3.sr = GameObject.Find("Hand (3)").GetComponent<SerialCommunicator>();
        Mid3.sr = GameObject.Find("Hand (3)").GetComponent<SerialCommunicator>();


    }

    // Update is called once per frame
    void LateUpdate()
    {
        //Thumb
        if (Thumb3.isGrab())
        {
            Thumb3.sr.TxBuffer.servoAct1 = 30;
            print(string.Format("Thumb ON! {0}", Thumb3.DerivedRotation_x.transform.localRotation.x));
        }
        else
        {
            print(string.Format("Thumb OFF! {0}", Thumb3.DerivedRotation_x.transform.localRotation.x));
            Thumb3.sr.TxBuffer.servoAct1 = 180;
        }

        if ((Thumb3.touchedwoodball && Thumb3.DerivedRotation_x.transform.localRotation.x < -0.5f))
            Thumb3.woodball.transform.position = new Vector3(Thumb3.CenterPalm.transform.position.x, Thumb3.CenterPalm.transform.position.y - 0.4f, Thumb3.CenterPalm.transform.position.z);

        else
            Thumb3.touchedwoodball = false;

        if (Thumb3.touchedmetalball && Thumb3.DerivedRotation_x.transform.localRotation.x < -0.5f)
            Thumb3.metalball.transform.position = new Vector3(Thumb3.CenterPalm.transform.position.x, Thumb3.CenterPalm.transform.position.y - 0.4f, Thumb3.CenterPalm.transform.position.z);
        else
            Thumb3.touchedmetalball = false;

        if (Thumb3.touchedplasticball && Thumb3.DerivedRotation_x.transform.localRotation.x < -0.5f)
            Thumb3.plasticball.transform.position = new Vector3(Thumb3.CenterPalm.transform.position.x, Thumb3.CenterPalm.transform.position.y - 0.4f, Thumb3.CenterPalm.transform.position.z);
        else
            Thumb3.touchedplasticball = false;




        //Index
        if (Index3.isGrab())
        {
            Index3.sr.TxBuffer.servoAct1 = 30;
            print(string.Format("Thumb ON! {0}", Index3.DerivedRotation_x.transform.localRotation.x));
        }
        else
        {
            print(string.Format("Thumb OFF! {0}", Index3.DerivedRotation_x.transform.localRotation.x));
            Index3.sr.TxBuffer.servoAct1 = 180;
        }

        if ((Index3.touchedwoodball && Index3.DerivedRotation_x.transform.localRotation.x < -0.5f))
            Index3.woodball.transform.position = new Vector3(Index3.CenterPalm.transform.position.x, Index3.CenterPalm.transform.position.y - 0.4f, Index3.CenterPalm.transform.position.z);

        else
            Index3.touchedwoodball = false;

        if (Index3.touchedmetalball && Index3.DerivedRotation_x.transform.localRotation.x < -0.5f)
            Index3.metalball.transform.position = new Vector3(Index3.CenterPalm.transform.position.x, Index3.CenterPalm.transform.position.y - 0.4f, Index3.CenterPalm.transform.position.z);
        else
            Index3.touchedmetalball = false;

        if (Index3.touchedplasticball && Index3.DerivedRotation_x.transform.localRotation.x < -0.5f)
            Index3.plasticball.transform.position = new Vector3(Index3.CenterPalm.transform.position.x, Index3.CenterPalm.transform.position.y - 0.4f, Index3.CenterPalm.transform.position.z);
        else
            Index3.touchedplasticball = false;



        //Mid
        if (Mid3.isGrab())
        {
            Mid3.sr.TxBuffer.servoAct1 = 30;
            print(string.Format("Thumb ON! {0}", Mid3.DerivedRotation_x.transform.localRotation.x));
        }
        else
        {
            print(string.Format("Thumb OFF! {0}", Mid3.DerivedRotation_x.transform.localRotation.x));
            Mid3.sr.TxBuffer.servoAct1 = 180;
        }

        if ((Mid3.touchedwoodball && Mid3.DerivedRotation_x.transform.localRotation.x < -0.5f))
            Mid3.woodball.transform.position = new Vector3(Mid3.CenterPalm.transform.position.x, Mid3.CenterPalm.transform.position.y - 0.4f, Mid3.CenterPalm.transform.position.z);

        else
            Mid3.touchedwoodball = false;

        if (Mid3.touchedmetalball && Mid3.DerivedRotation_x.transform.localRotation.x < -0.5f)
            Mid3.metalball.transform.position = new Vector3(Mid3.CenterPalm.transform.position.x, Mid3.CenterPalm.transform.position.y - 0.4f, Mid3.CenterPalm.transform.position.z);
        else
            Mid3.touchedmetalball = false;

        if (Mid3.touchedplasticball && Mid3.DerivedRotation_x.transform.localRotation.x < -0.5f)
            Mid3.plasticball.transform.position = new Vector3(Mid3.CenterPalm.transform.position.x, Mid3.CenterPalm.transform.position.y - 0.4f, Mid3.CenterPalm.transform.position.z);
        else
            Mid3.touchedplasticball = false;

    }
}

