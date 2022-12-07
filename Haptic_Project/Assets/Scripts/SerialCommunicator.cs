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
        int[] testGrabDatas =
        {
            18769, 1156, 1764, 1, 1849, 36, 1521, 100, 4, 625, 1, 324, 961, 1600, 1849, 441, 100, 841, 841, 2500, 1936,
            529, 169, 1681, 256, 576, 441, 144, 100, 1600, 1444, 2601, 484, 225, 484, 961, 2304, 1936, 625, 81, 961,
            576, 2500, 1849, 784, 64, 1849, 121, 361, 441, 64, 169, 1444, 1444, 2500, 484, 169, 729, 729, 2500, 1681,
            625, 64, 1681, 225, 400, 400, 49, 256, 1369, 1600, 2704, 676, 144, 961, 529, 2916, 1600, 784, 121, 1936, 25,
            121, 441, 36, 484, 841, 2209, 1764, 676, 81, 961, 484, 3249, 1089, 729, 49, 1849, 25, 81, 400, 9, 841, 576,
            3136, 1156, 900, 169, 2704, 784, 4225, 169, 400, 1681, 324, 361, 441, 25, 1444, 324, 100, 576, 0, 1936, 64,
            1, 484, 16, 1936, 1, 49, 361, 144, 2704, 0, 169, 144, 25, 225, 2025, 576, 484, 121, 3600, 961, 3969, 144,
            169, 2704, 121, 64, 441, 0, 2809, 16, 1, 900, 1, 169, 324, 529, 324, 900, 441, 361, 324, 9, 1024, 400, 3025,
            400, 576, 64, 2401, 1089, 2500, 361, 0, 2209, 49, 0, 144, 36, 2601, 25, 9, 289, 36, 3364, 9, 36, 256, 1,
            529, 2209, 729, 841, 100, 2916, 121, 5625, 36, 484, 324, 1764, 2116, 2116, 576, 16, 2304, 1, 0, 144, 64,
            1936, 289, 3364, 324, 441, 81, 3025, 1369, 2209, 225, 0, 1600, 361, 2500, 400, 196, 49, 2916, 576, 2601, 64,
            36, 1156, 729, 2704, 841, 400, 0, 3969, 169, 4900, 25, 256, 625, 1764, 2916, 1764, 400, 49, 196, 2401, 900,
            441, 169, 256, 2809, 1156, 625, 324, 576, 324, 441, 225, 576, 841, 2704, 1369, 729, 0, 2601, 25, 3481, 16,
            289, 81, 1681, 1369, 1764, 441, 16, 2116, 36, 2704, 100, 289, 49, 2209, 961, 2025, 256, 4, 1089, 576, 2209,
            625
        };

        int[] testPickDatas =
        {
            2209, 12321, 6561, 100, 256, 729, 9216, 2209, 841, 49, 64, 6084, 4, 8100, 1089, 1369, 36, 7396, 144, 7921,
            841, 1444, 64, 8281, 900, 81, 49, 81, 3025, 1296, 7921, 1849, 1089, 49, 5476, 0, 7396, 729, 1089, 4, 6889,
            196, 6561, 729, 1225, 64, 7569, 1024, 196, 36, 100, 3249, 576, 8464, 1089, 1156, 49, 6084, 121, 6724, 576,
            1089, 81, 7396, 841, 121, 36, 121, 3969, 576, 9216, 1089, 1296, 16, 7921, 484, 7744, 256, 1089, 169, 7396,
            1764, 576, 36, 49, 5776, 4, 7396, 784, 1089, 49, 7921, 784, 7744, 36, 841, 169, 7056, 1764, 576, 64, 36,
            6561, 169, 9025, 36, 676, 1764, 3844, 7569, 2916, 1369, 9, 8281, 676, 729, 16, 9, 10201, 1089, 900, 16, 169,
            8281, 2025, 1681, 9, 225, 6084, 2401, 1764, 0, 196, 6889, 4489, 1521, 1, 784, 3721, 3136, 64, 729, 1369,
            3249, 6724, 2209, 1369, 36, 6889, 961, 2025, 16, 81, 13689, 2209, 3600, 225, 324, 8281, 324, 81, 529, 6400,
            196, 144, 0, 144, 3481, 484, 4356, 4, 529, 1296, 576, 4489, 625, 625, 196, 4096, 1444, 1849, 1, 121, 6561,
            2601, 1936, 16, 169, 9216, 5041, 1444, 36, 961, 4096, 3844, 324, 625, 729, 6889, 4900, 3249, 1089, 25, 1521,
            64, 3600, 225, 529, 169, 2916, 1296, 900, 1, 169, 3025, 1089, 4225, 361, 289, 1296, 144, 3600, 225, 576,
            100, 1936, 676, 3249, 144, 289, 1156, 441, 3025, 1089, 676, 121, 2500, 196, 4624, 0, 324, 841, 3025, 3600,
            3364, 1156, 4, 3721, 0, 5041, 400, 256, 841, 3600, 2025, 841, 361, 484, 4761, 2025, 2401, 484, 1681, 5776,
            25, 1, 400, 2601, 16, 2401, 144, 196, 289, 2704, 1764, 3249, 841, 196, 1225, 1, 3364, 121, 676, 169, 1936,
            1024, 3600, 529, 324, 784, 25, 3025, 64, 441, 81, 1681, 484, 2809, 64
        };

        int dataSize = testGrabDatas.Length < testPickDatas.Length ? testGrabDatas.Length : testPickDatas.Length;

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