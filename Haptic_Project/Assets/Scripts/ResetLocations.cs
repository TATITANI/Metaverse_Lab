using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetLocations : MonoBehaviour
{
    private MouseDraggable[] ResetObjects; //리셋할 오브젝트 배열
    private Vector3[] InitialLocations;   // 오브젝트들의 처음 위치 
    private Vector3[] InitialRotations;   // 오브젝트들의 처음 위치 
    // Start is called before the first frame update
    void Start()
    {
        //FindObjectOfType<UImanager>();
        ResetObjects = FindObjectsOfType<MouseDraggable>(); //MouseDraggable 스크립트가 있는 오브젝트만 적용
        InitialLocations = new Vector3 [(ResetObjects.Length)]; //
        InitialRotations = new Vector3 [(ResetObjects.Length)]; //
        for(int i =0; i< ResetObjects.Length; i++){
            // print(ResetObjects[i].name);
            InitialLocations[i]= new Vector3(ResetObjects[i].transform.position.x, ResetObjects[i].transform.position.y, ResetObjects[i].transform.position.z);
            InitialRotations[i]= new Vector3(ResetObjects[i].transform.rotation.x, ResetObjects[i].transform.rotation.y, ResetObjects[i].transform.rotation.z);

        }
        
    }

    public void ResetLocation(){
        for(int i =0; i< ResetObjects.Length; i++){
                    print(ResetObjects[i].name+" : "+InitialLocations[i]);
                    //print(ResetObjects[i].name+" : "+ResetObjects[i].transform.position);
                    ResetObjects[i].transform.position = new Vector3(InitialLocations[i].x,InitialLocations[i].y,InitialLocations[i].z);
                    ResetObjects[i].transform.rotation = Quaternion.Euler(InitialRotations[i].x, InitialRotations[i].y, InitialRotations[i].z);

        }
    }
}
