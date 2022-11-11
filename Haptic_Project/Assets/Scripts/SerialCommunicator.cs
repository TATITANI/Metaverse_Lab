using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ArduinoBluetoothAPI;


public class SerialCommunicator : MonoBehaviour
{
    public TMP_InputField inputText;
    public TextMeshProUGUI receivedText;
    private BluetoothHelper helper;
    private string deviceName;
    [SerializeField] private HandControllerSO handControllerSo;
    [SerializeField] private EMG_SO emgSO;

    // Start is called before the first frame update
    void Start()
    {
        deviceName = "HC-06";
        try
        {
            helper = BluetoothHelper.GetInstance(deviceName);
            helper.OnConnected += OnConnected;
            helper.OnConnectionFailed += OnConnFailed;
            
            helper.setTerminatorBasedStream("\n");

            if (helper.isDevicePaired())
            {
                helper.Connect();
                Debug.Log("BT is Paired");
            }

         
        }catch(BluetoothHelper.BlueToothNotEnabledException ex) { }
        
        StartCoroutine(Send());
    }


    IEnumerator Send()
    {
        while (true)
        {
            int pressure1 = (int)handControllerSo.pressureRight[0].fingerPressure;
            int pressure2 = (int)handControllerSo.pressureRight[1].fingerPressure;
            int pressure3 = (int)handControllerSo.pressureRight[2].fingerPressure;
            string msg = $"{pressure1},{pressure2},{pressure3}";
            helper.SendData(msg);
            yield return new WaitForSeconds(0.01f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            BTsend("E");
        }
    
        if (helper.Available)
        {
            string msg = helper.Read();
            Debug.Log(msg);
            
            //     UpdateReceivedData();
            
            //     // todo : emg data 입력
            //     // emgSO.PushData(emg);
        }
      
    }
    public void SendData_Btn(string data ){
        data = inputText.text;
        BTsend(data);
    }

    private void BTsend(string data){
        helper.SendData(data);
    }
    private void UpdateReceivedData(){
        receivedText.text=inputText.text;
    }

    void OnConnected()
    {
        helper.StartListening();
        Debug.Log("BT conncected");
        helper.SendData("Hi Arduino!");
    }

    void OnConnFailed()
    {
        Debug.Log("NO Connection");
    }
}
