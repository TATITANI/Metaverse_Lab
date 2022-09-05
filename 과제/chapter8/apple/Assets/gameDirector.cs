using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class gameDirector : MonoBehaviour
{
    GameObject timerText;
    GameObject pointText;
    GameObject maker;
    float time = 30.0f;
    int point = 0;
    // Use this for initialization
    void Start ()
    {
        this.timerText = GameObject.Find("Time");
        this.pointText = GameObject.Find("Point");
        this.maker = GameObject.Find("itemmake");
    }
    public void GetApple() {
        Debug.Log("get apple");
        this.point += 100;
    }
    public void GetBomb()
    {
        Debug.Log("get bomb");
        this.point /= 2;
    }

    // Update is called once per frame
    void Update () {
        this.time -= Time.deltaTime;
        if(this.time < 0)
        {
            this.time = 0;
            this.maker.GetComponent<itemmake>().SetParameter(10000.0f, 0, 0);
        }
        else if (0 <= this.time && this.time < 5)
        {
            this.maker.GetComponent<itemmake>().SetParameter(0.7f, -0.04f, 3);
        }
        else if (this.time < 12)
        {
            this.maker.GetComponent<itemmake>().SetParameter(0.8f, -0.05f, 6);
        }
        else if (this.time < 23)
        {
            this.maker.GetComponent<itemmake>().SetParameter(0.8f, -0.04f, 4);
        }
        else if (this.time < 30)
        {
            this.maker.GetComponent<itemmake>().SetParameter(1.0f, -0.03f, 2);
        }
        this.timerText.GetComponent<Text>().text = this.time.ToString("F1");
        this.pointText.GetComponent<Text>().text = this.point.ToString() + " points";
    }
}
