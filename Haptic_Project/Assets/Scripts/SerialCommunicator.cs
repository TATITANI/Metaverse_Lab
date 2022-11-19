using System;
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
        }
        catch (BluetoothHelper.BlueToothNotEnabledException ex)
        {
            Debug.LogError($"BlueToothNotEnabledException : {ex.Message}");
            this.enabled = false;
        }
        catch (Exception e)
        {
            Debug.LogError($"bt error : {e.Message}");
        }
        finally
        {
            // gameObject.SetActive(false);
        }

        if (AppManager.Instance.IsTest)
        {
            gameObject.SetActive(false);
            return;
        }

        StartCoroutine(Send());
    }


    IEnumerator Send()
    {
        yield return new WaitUntil(() => helper.isConnected());

        while (true)
        {
            int pressure1 = (int)(handControllerSo.pressureRight[0].fingerPressure*100);
            int pressure2 = (int)(handControllerSo.pressureRight[1].fingerPressure*100);
            int pressure3 = (int)(handControllerSo.pressureRight[2].fingerPressure*100);
            string msg = $"<{pressure1},{pressure2},{pressure3}>";
            Debug.Log($"sendMsg : {msg}");
            BTsend(msg);
            yield return new WaitForSeconds(sampleTime);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (helper.Available)
        {
            string msg = helper.Read();
            string[] emgDatas = msg.Split(',');
            int grabEmg = Convert.ToInt32(emgDatas[0]);
            int pickEmg = Convert.ToInt32(emgDatas[1]);
            // Debug.Log($"recv grabEmg : {grabEmg} / pickEmg : {pickEmg}");
            emgSO.PushData(EMG_SO.EMGType.GRAB, grabEmg);
            emgSO.PushData(EMG_SO.EMGType.PICK, pickEmg);
        }
    }

    public void SendData_Btn(string data)
    {
        data = inputText.text;
        BTsend(data);
    }

    private void BTsend(string data)
    {
        helper.SendData(data);
    }

    private void UpdateReceivedData()
    {
        receivedText.text = inputText.text;
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