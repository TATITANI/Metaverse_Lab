using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ArduinoBluetoothAPI;
using System.IO.Ports;


public class SerialCommunicator : MonoBehaviour
{
    Thread sendThread;

    //static public SerialPort
    //serial = new SerialPort("COM10", 9600); //here change port - where you have connected arduino to computer
    public SerialController serialController;

    private BluetoothHelper helper;
    private string deviceName;
    [SerializeField] private HandControllerSO handControllerSo;
    [SerializeField] private EMG_SO emgSO;

    [SerializeField] public float sampleTime = 0.1f;

    public float SampleTime
    {
        get { return sampleTime; }
    }

    private static SerialCommunicator _instance;

    public static SerialCommunicator Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<SerialCommunicator>();
            if (_instance == null)
            {
                GameObject container = new GameObject("SerialCommunicator");
                _instance = container.AddComponent<SerialCommunicator>();
            }

            return _instance;
        }
    }



    void Start()
    {
        if (AppManager.Instance.IsTest)
        {
            this.enabled = false;
            return;
        }

        sendThread = new Thread(SendThread);
        sendThread.Start();

        #region 블루투스 초기화
        // deviceName = "HC-06";
        // try
        // {
        //     helper = BluetoothHelper.GetInstance(deviceName);
        //     helper.OnConnected += OnConnected;
        //     helper.OnConnectionFailed += OnConnFailed;

        //     helper.setTerminatorBasedStream("\n");

        //     if (helper.isDevicePaired())
        //     {
        //         helper.Connect();
        //         Debug.Log("BT is Paired");
        //     }
        // }
        // catch (BluetoothHelper.BlueToothNotEnabledException ex)
        // {
        //     Debug.LogError($"BlueToothNotEnabledException : {ex.Message}");
        //     this.enabled = false;
        // }
        // catch (Exception e)
        // {
        //     Debug.LogError($"bt error : {e.Message}");
        // }
        // finally
        // {
        //     // gameObject.SetActive(false);
        // }

        //StartCoroutine(Send());
        #endregion 블루투스 초기화

    }

    /// <summary>
    /// �������� �۽�
    /// </summary>
    // IEnumerator Send()
    // {
    //     yield return new WaitUntil(() => helper.isConnected());

    //     while (true)
    //     {
    //         int pressure1 = (int)(handControllerSo.pressureRight[0].fingerPressure * 100);
    //         int pressure2 = (int)(handControllerSo.pressureRight[1].fingerPressure * 100);
    //         int pressure3 = (int)(handControllerSo.pressureRight[2].fingerPressure * 100);
    //         string msg = $"<{pressure1},{pressure2},{pressure3}>";
    //         BTsend(msg);
    //         yield return new WaitForSeconds(0.5f);
    //     }
    // }

    private void OnDestroy()
    {
        // serial.Close();
        sendThread.Abort();
    }

    public void SendThread()
    {
        // serial.Open();

        while (true)
        {
            int pressure1 = (int)(handControllerSo.pressureRight[0].fingerPressure * 100);
            int pressure2 = (int)(handControllerSo.pressureRight[1].fingerPressure * 100);
            int pressure3 = (int)(handControllerSo.pressureRight[2].fingerPressure * 100);
            string msg = $"<{pressure1},{pressure2},{pressure3}>";
            try
            {
                serialController.SendSerialMessage(msg);

                Thread.Sleep(500);
            }
            catch (Exception e)
            {
                Debug.LogError($"sendError : {e}");
            }
        }

        // serial.Close();
    }

    // Update is called once per frame
    void Update()
    {
        ReadMessage();
        // if (serial.IsOpen)
        // {
        //     string recvMsg = serial.ReadLine();
        //     Debug.Log($"recvMsg  : {recvMsg}");
        // }

        // �������� ����
        // if (helper.Available)
        // {
        //     string msg = helper.Read();
        //     string[] emgDatas = msg.Split(',');
        //     int grabEmg = Convert.ToInt32(emgDatas[0]);
        //     int pickEmg = Convert.ToInt32(emgDatas[1]);
        //     // Debug.Log($"recv grabEmg : {grabEmg} / pickEmg : {pickEmg}");
        //     emgSO.PushData(EMG_SO.EMGType.GRAB, grabEmg);
        //     emgSO.PushData(EMG_SO.EMGType.PICK, pickEmg);
        // }
    }

    private void BTsend(string data)
    {
        try
        {
            helper.SendData(data);
        }
        catch (Exception e)
        {
            Debug.LogWarning($"BT send exception : ${e}");
        }
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
        // Debug.Log($"recv Data : {msg}");
    }
}