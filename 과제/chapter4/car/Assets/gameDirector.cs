using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class gameDirector : MonoBehaviour
{
    GameObject car;
    GameObject flag;
    GameObject distance;
    GameObject arrive;
    GameObject speed;

    // Use this for initialization
    void Start()
    {
        this.car = GameObject.Find("car");
        this.flag = GameObject.Find("flag");
        this.distance = GameObject.Find("Distance");
        this.arrive = GameObject.Find("Arrive");
        this.speed = GameObject.Find("speed");
    }

    // Update is called once per frame
    void Update()
    {
        float length = (this.flag.transform.position - this.car.transform.position).magnitude;
        this.distance.GetComponent<Text>().text = "목표지점까지 " + length.ToString("F2") + "m";

        if (length < 1)
        {
            StartCoroutine(ShowArrivingText());
            car.GetComponent<carController>().Reset();
        }

    }

    IEnumerator ShowArrivingText()
    {
        this.arrive.GetComponent<Text>().text = "도착";
        arrive.transform.position = Camera.main.WorldToScreenPoint(car.transform.position);
        yield return new WaitForSeconds(2f);
        this.arrive.GetComponent<Text>().text = "";
    }
    

 
    public void onInput(string Value)
    {
        Debug.Log($"{Value}");
        float xx;
        float.TryParse(Value, out xx); // string -> float
        this.car.transform.Translate(new Vector3(0, xx, 0));
    }
}