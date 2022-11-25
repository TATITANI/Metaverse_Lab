using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

public class EMG_Serial_Reader : MonoBehaviour
{
    private static EMG_Serial_Reader _instance;
    public static EMG_Serial_Reader Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<EMG_Serial_Reader>();
            if (_instance == null)
            {
                GameObject container = new GameObject("EMG_Serial_Reader");
                _instance = container.AddComponent<EMG_Serial_Reader>();
            }
            return _instance;
        }
    }

    public SerialController serialController;
    public TextMeshProUGUI receivedText;
    //public Text receivedText;

    [SerializeField] private EMG_SO emgSO;
    bool firstTimeReading = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ReadMessage();
    }
    void ReadMessage()
    {
        string message = serialController.ReadSerialMessage();
        if (message == null)
            return;

        if (ReferenceEquals(message, SerialController.SERIAL_DEVICE_CONNECTED))
            Debug.Log("Connection established");
        else if (ReferenceEquals(message, SerialController.SERIAL_DEVICE_DISCONNECTED))
            Debug.Log("Connection attempt failed or disconnection detected");
        else
        {
            if (message[0] == '#')
                DecryptMessage(message);
            else
                Debug.Log("System message : " + message);
        }
    }
    void DecryptMessage(string message)
    {

        string[] emgDatas = message.Split(',');
        //Vector3 inputVector = new Vector3(-float.Parse(s[2]), float.Parse(s[0]), float.Parse(s[1]));
        int grabEmg = Convert.ToInt32(emgDatas[1]);
        int pickEmg = Convert.ToInt32(emgDatas[2]);
        Debug.Log($"recv grabEmg : {grabEmg} / pickEmg : {pickEmg}");
        emgSO.PushData(EMG_SO.EMGType.GRAB, grabEmg);
        emgSO.PushData(EMG_SO.EMGType.PICK, pickEmg);
        String msg = grabEmg.ToString() + ", " + pickEmg.ToString();
        UpdateReceivedData(msg);
    }
    private void UpdateReceivedData(String msg)
    {
        receivedText.text = msg;
    }
}
