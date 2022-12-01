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
    Thread sendThread, recvThread;

    //static public SerialPort
    //serial = new SerialPort("COM10", 9600); //here change port - where you have connected arduino to computer
    public SerialController serialController;

    private BluetoothHelper helper;
    private string deviceName;
    [SerializeField] private HandControllerSO handControllerSo;
    [SerializeField] private EMG_SO emgSO;

    [SerializeField] public float sampleTime = 0.02f; // 50Hz

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
            recvThread = new Thread(PushDatas_Test);
            recvThread.Start();
        }
        else
        {
            sendThread = new Thread(SendThread);
            sendThread.Start();

            recvThread = new Thread(ReadMessage);
            recvThread.Start();
        }

        StartCoroutine(RecvCoroutine());

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
        sendThread?.Abort();
        recvThread?.Abort();
    }

    public void SendThread()
    {
        // serial.Open();

        while (true)
        {
            int pressure1 = (int)(handControllerSo.pressureRight[0].fingerPressure * 100);
            int pressure2 = (int)(handControllerSo.pressureRight[1].fingerPressure * 100);
            int pressure3 = (int)(handControllerSo.pressureRight[2].fingerPressure * 100);
            int elasticity = handControllerSo.Elasticity;
            string msg = $"<{pressure1},{pressure2},{pressure3},{elasticity}>";
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

    //void Update()
    //{
    // ReadMessage();

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
    //}

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
        int sampleMicroTime = (int)(sampleTime * 1000);
        while (true)
        {
            string message = serialController.ReadSerialMessage();
            if (message == null)
                continue;

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

            Thread.Sleep(sampleMicroTime);
        }
    }

    void DecryptMessage(string message)
    {
        string[] emgDatas = message.Split(',');
        //Vector3 inputVector = new Vector3(-float.Parse(s[2]), float.Parse(s[0]), float.Parse(s[1]));
        int grabEmg = Convert.ToInt32(emgDatas[1]);
        int pickEmg = Convert.ToInt32(emgDatas[2]);
        // Debug.Log($"recv grabEmg : {grabEmg} / pickEmg : {pickEmg}");
        grabDatas.Enqueue(grabEmg);
        pickDatas.Enqueue(pickEmg);

        String msg = grabEmg.ToString() + ", " + pickEmg.ToString();
        UpdateReceivedData(msg);
    }

    private void UpdateReceivedData(String msg)
    {
        // Debug.Log($"recv Data : {msg}");
    }

    private Queue<int> grabDatas = new Queue<int>();
    private Queue<int> pickDatas = new Queue<int>();

    void PushDatas_Test()
    {
        int sampleMicroTime = (int)(sampleTime * 1000);
        const int dataSize = 180;
        int[] testGrabDatas = new int[dataSize]
        {
            289, 36, 81, 25, 121, 324, 36, 16, 25, 9, 121, 9, 289, 225, 16, 64, 25, 1, 25, 4, 9, 25, 0, 36, 49, 121, 1,
            1, 25, 25, 484, 9, 324, 36, 100, 256, 49, 100, 196, 1, 4, 0, 25, 81, 25, 121, 1, 4, 49, 25, 9, 4, 16, 0, 4,
            4, 9, 121, 100, 25, 25, 4, 0, 36, 0, 25, 16, 81, 36, 1, 81, 225, 25, 9, 81, 144, 81, 16, 36, 16, 36, 64, 36,
            169, 256, 1, 16, 1, 25, 144, 1, 25, 9, 16, 4, 0, 64, 16, 16, 16, 4, 1, 49, 16, 49, 4, 25, 144, 49, 144, 121,
            0,
            1, 1, 4, 36, 16, 49, 1, 25, 36, 1, 49, 16, 144, 0, 9, 25, 36, 64, 100, 4, 0, 4, 9, 36, 9, 36, 0, 0, 1, 4,
            36, 9,
            144, 16, 0, 36, 49, 49, 49, 4, 0, 4, 1, 16, 25, 49, 1, 1, 4, 0, 81, 0, 36, 36, 100, 0, 4, 144, 25, 81, 9, 1,
            4,
            36, 25, 36, 49, 16
        };

        int[] testPickDatas = new int[dataSize]
        {
            169, 100, 100, 1, 576, 324, 256, 169, 4, 9, 25, 400, 9, 169, 4, 25, 4, 121, 1, 9, 4, 49, 16, 64, 16, 9, 64,
            0, 9, 0, 1, 36, 9, 36, 1, 16, 0, 36, 0, 25, 4, 4, 25, 25, 4, 9, 64, 0, 4, 1, 36, 0, 36, 4, 16, 81, 1, 1, 25,
            121, 4, 1, 4, 25, 1, 64, 16, 64, 1, 121, 1, 49, 4, 36, 1, 49, 1, 1, 4, 49, 1, 16, 1, 9, 121, 144, 4, 25,
            100, 49, 0, 0, 49, 1, 100, 49, 100, 49, 729, 289, 100, 25, 4, 1, 0, 100, 25, 1, 1, 36, 1, 0, 144, 16, 4, 4,
            0, 1, 49, 4, 25, 9, 36, 0, 9, 9, 1, 1, 9, 4, 9, 9, 9, 1, 36, 0, 4, 36, 0, 1, 0, 0, 36, 16, 1, 4, 0, 4, 9,
            49, 81, 9, 25, 9, 64, 0, 25, 4, 25, 0, 0, 1, 25, 0, 144, 100, 1, 1, 0, 16, 1, 9, 1, 1, 0, 16, 1, 1, 4, 9
        };

        int dataId = 0;
        while (true)
        {
            dataId = (dataId == dataSize - 1) ? 0 : dataId + 1;
            grabDatas.Enqueue(testGrabDatas[dataId]);
            pickDatas.Enqueue(testPickDatas[dataId]);

            Thread.Sleep(sampleMicroTime);
        }
    }

    IEnumerator RecvCoroutine()
    {
        while (true)
        {
            while (grabDatas.Count > 0)
            {
                emgSO.PushData(EMG_SO.EMGType.GRAB, grabDatas.Dequeue());
            }

            while (pickDatas.Count > 0)
            {
                emgSO.PushData(EMG_SO.EMGType.PICK, pickDatas.Dequeue());
            }
            yield return null;
        }

        yield return null;
    }
}