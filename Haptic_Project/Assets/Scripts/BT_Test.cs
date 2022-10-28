using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BT_Test : MonoBehaviour
{
    public TMP_InputField inputText;
    public string receivedText;

    public bool IsDataReceived =false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            BTsend("E");
        }
        if(IsDataReceived == true){
            UpdateReceivedData();
        }
    }
    public void SendData_Btn(string data ){
        data = inputText.text;
        BTsend(data);
    }

    private void BTsend(string data){
        Debug.Log(data);
        //Serial.write(data);
    }
    private void UpdateReceivedData(){
        receivedText="Data";
    }
}
