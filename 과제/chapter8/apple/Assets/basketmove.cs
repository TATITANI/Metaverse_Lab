using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class basketmove : MonoBehaviour {
    public AudioClip appleSE;
    public AudioClip bombSE;
    AudioSource aud;
    GameObject director;
	// Use this for initialization
	void Start () {
        this.director = GameObject.Find("gameDirector");
        this.aud = GetComponent<AudioSource>();
	}
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "apple")
        {
            Debug.Log("사과 잡았다!");
            this.director.GetComponent<gameDirector>().GetApple();
            this.aud.PlayOneShot(this.appleSE);
        } else         {
            Debug.Log("폭탄 잡았다 ㅠㅠ");
            this.director.GetComponent<gameDirector>().GetBomb();
            this.aud.PlayOneShot(this.bombSE);
        }
        Destroy(other.gameObject);
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray,out hit, Mathf.Infinity))
            {
                float x = Mathf.RoundToInt(hit.point.x);
                float z = Mathf.RoundToInt(hit.point.z);
                transform.position = new Vector3(x, 0.0f, z);
            }
        }
		
	}
}
