using UnityEngine;
using Random = UnityEngine.Random;

public class arrow : MonoBehaviour {
    GameObject player;

    private DirType dirType;
    enum DirType
    {
        DOWN=0,RIGHT, UP, LEFT,
    }
	void Start () {
        this.player = GameObject.Find("player");
        
        dirType = (DirType)Random.Range(0, 4);
        transform.eulerAngles = new Vector3(0,0,90* (int)dirType);
        Vector3 viewportPos = Vector3.zero;
        switch (dirType)
        {
           case DirType.LEFT:
               viewportPos = new Vector3(1.5f, Random.Range(0f, 1f),0);
               break;
           case DirType.RIGHT:
               viewportPos = new Vector3(-1.5f, Random.Range(0f, 1f),0);
               break;
           case DirType.UP:
               viewportPos = new Vector3(Random.Range(0f, 1f), -1.5f,0);
               break;
           case DirType.DOWN:
               viewportPos = new Vector3(Random.Range(0f, 1f), 1.5f,0);
               break;
           
        }
        transform.position = Camera.main.ViewportToWorldPoint(viewportPos);
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
    }
	
	void Update ()
    {
        transform.Translate(new Vector3(0,-1,0) * 0.1f);

        if(  Mathf.Abs(Camera.main.WorldToViewportPoint(transform.position).y) > 1.5f || 
             Mathf.Abs(Camera.main.WorldToViewportPoint(transform.position).x) > 1.5f)
        {
            Destroy(gameObject);
        }

        Vector2 p1 = transform.position;
        Vector2 p2 = this.player.transform.position;
        Vector2 dir = p1 - p2;
        float d = dir.magnitude;
        float r1 = 0.5f;
        float r2 = 1.0f;
        if (d < r1 + r2)
        {
            Destroy(gameObject);
            GameObject director = GameObject.Find("gameDirector");
            director.GetComponent<gameDirector>().DecreaseHp();
        }
    }
}
