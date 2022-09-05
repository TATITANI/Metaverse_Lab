using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class myCube : MonoBehaviour {

    public GameObject myX;
    //public RandomBall ball;

    // Use this for initialization
    void Start () {
        this.myX = GameObject.Find("myxloc");
    }

    // Update is called once per frame
    void Update () {
		
	}
    public void showMyX(string xx,float x)
    {
        this.myX.GetComponent<Text>().text = xx + x +"cm.";
    }
  
	public void OnInput()
    {
        int p;
        int.TryParse(GameObject.Find("xInMeter").GetComponent<InputField>().text, out p);
        this.transform.position += new Vector3(p, 0f, 0f); 
        showMyX("Shifted dx =",10.0f * (float)p);
    }
    public void OnSliding(float p)
    {
        showMyX("Shifted dz =", 10.0f * (float)p);
        this.transform.position = new Vector3(0f, 0f, p*10f);
    }
}
