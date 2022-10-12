using UnityEngine;

public class Collision_Thumb : MonoBehaviour
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

    //오디오소스 컴포넌트
    AudioSource aud;

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
    
    //벽 접촉 여부 판단
    bool IsBorder;

    //Grab판단 두번째 조건(손가락 기울기)
    float ThresHold_Value_Thumb = -0.5f;

    //음성출력 메소드
    void PlayingSound(int num)
    {
        if(num==0)
        {
            this.aud.PlayOneShot(this.woodsound);
        }
        else if(num==1)
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
    }


    void LateUpdate()
    {
        if (isGrab())
        {
            sr.TxBuffer.servoAct1 = 30;
            print(string.Format("Thumb ON! {0}", DerivedRotation_x.transform.localRotation.x));
        }
        else
        {
            print(string.Format("Thumb OFF! {0}", DerivedRotation_x.transform.localRotation.x));
            sr.TxBuffer.servoAct1 = 180;
        }

        //Debug.Log("Thumb finger : " + DerivedRotation_x.transform.localRotation.x);
        if ((touchedwoodball && DerivedRotation_x.transform.localRotation.x < ThresHold_Value_Thumb))
            woodball.transform.position = new Vector3(CenterPalm.transform.position.x, CenterPalm.transform.position.y - 0.4f, CenterPalm.transform.position.z);

        else
            touchedwoodball = false;

        if (touchedmetalball && DerivedRotation_x.transform.localRotation.x < ThresHold_Value_Thumb)
            metalball.transform.position = new Vector3(CenterPalm.transform.position.x, CenterPalm.transform.position.y - 0.4f, CenterPalm.transform.position.z);
        else
            touchedmetalball = false;

        if (touchedplasticball && DerivedRotation_x.transform.localRotation.x < ThresHold_Value_Thumb)
            plasticball.transform.position = new Vector3(CenterPalm.transform.position.x, CenterPalm.transform.position.y - 0.4f, CenterPalm.transform.position.z);
        else
            touchedplasticball = false;
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
        {
            PlayingSound(3);
        }

        else if (other.gameObject.tag == "FixedMetal")
        {
            PlayingSound(4);
        }
        else if (other.gameObject.tag == "FixedPlastic")
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
        else if (other.gameObject.tag == "FixedWood")
        {
            PlayingSound(3);
        }

        else if (other.gameObject.tag == "FixedMetal")
        {
            PlayingSound(4);
        }
        else if (other.gameObject.tag == "FixedPlastic")
        {
            PlayingSound(5);
        }
    }
    
    //충돌종료
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
  
    //Grab여부 판단
    bool isGrab()
    {
        if (touchedmetalball || touchedplasticball || touchedwoodball)
            return true;
        return false;
    }


}