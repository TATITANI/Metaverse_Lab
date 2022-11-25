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

    static public SerialPort
        serial = new SerialPort("COM11", 9600); //here change port - where you have connected arduino to computer

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

    private static void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
    {
        Debug.Log($"recv : {serial.ReadLine()}");
    }

    void Start()
    {
        if (AppManager.Instance.IsTest)
        {
            this.enabled = false;
            return;
        }

        Debug.Log($"{serial.DataBits}, {serial.Parity}");
        // serial.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
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
    /// 블루투스 송신
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
        serial.Close();
    }

    public void SendThread()
    {
        serial.Open();

        while (true)
        {
            int pressure1 = (int)(handControllerSo.pressureRight[0].fingerPressure * 100);
            int pressure2 = (int)(handControllerSo.pressureRight[1].fingerPressure * 100);
            int pressure3 = (int)(handControllerSo.pressureRight[2].fingerPressure * 100);
            string msg = $"<{pressure1},{pressure2},{pressure3}>";
            try
            {
                if (serial.IsOpen)
                {
                    serial.Write(msg);
                    //Debug.Log(msg);
                }
                else
                {
                    serial.Open();
                    Debug.LogWarning($"serial reopen");
                }

                Thread.Sleep(200);
            }
            catch (Exception e)
            {
                Debug.LogError($"sendError : {e}");
                if (!serial.IsOpen)
                {
                    serial.Open();
                }
            }
        }

        serial.Close();
    }

    // Update is called once per frame
    void Update()
    {
        if (serial.IsOpen)
        {
            string recvMsg = serial.ReadLine();
            Debug.Log($"recvMsg  : {recvMsg}");
        }
        
        // 블루투스 수신
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
}