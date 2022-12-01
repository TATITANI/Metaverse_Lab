using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UImanager : MonoBehaviour
{
    private static UImanager _instance;

    public static UImanager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<UImanager>();
            if (_instance == null)
            {
                GameObject container = new GameObject("UImanager");
                _instance = container.AddComponent<UImanager>();
            }

            return _instance;
        }
    }

    float Contents_TimeStart;
    float Task_TimeStart;

    public float EMG_Contents_Grab; //팔뚝 EMG 현재 센싱 값 / 아두이노에서 받아오기 
    public float EMG_Contents_Pickup; //손등 EMG 현재 센싱 값 / 아두이노에서 받아오기 

    public float GrabThreshold = 500;
    public float PickupThreshold = 500;


    public float EMG_Task_Grab; //팔뚝 EMG 현재 센싱 값 / 아두이노에서 받아오기 
    public float EMG_Task_Pickup; //손등 EMG 현재 센싱 값 / 아두이노에서 받아오기 

    int EMG_Count_Grab_Contents = 0; //EMG 쥐는 동작 카운트 / 
    int EMG_Count_Pickup_Contents = 0; //EMG 집는 동작 카운트 / 

    int EMG_Count_Grab_Task = 0; //EMG 쥐는 동작 카운트 / 
    int EMG_Count_Pickup_Task = 0; //EMG 집는 동작 카운트 / 

    float Content_Grab_EMGAvg = 0;
    float Content_Pickup_EMGAvg = 0;
    float Task_Grab_EMGAvg = 0;
    float Task_Pickup_EMGAvg = 0;

    float Content_Grab_EMGSum = 0;
    float Content_Pickup_EMGSum = 0;
    float Task_Grab_EMGSum = 0;
    float Task_Pickup_EMGSum = 0;

    float Task_Grab_EMGMax = 0;
    float Task_Pickup_EMGMax = 0;

    int Contents_count = 0;
    int Task_count = 0;

    bool Contents_timeActive = false;
    bool Task_timeActive = false;
    bool EMG_Contents_Active = false;
    bool EMG_Task_Active = false;

    [SerializeField] private EMG_SO emgSO;
    [SerializeField] TextMeshProUGUI Text_ContentsTimer, Text_TaskTimer, Contents_startPauseText, Task_startPauseText;

    [SerializeField] TextMeshProUGUI Text_Grab_EMG_Contents, Text_PickUp_EMG_Contents;
    [SerializeField] TextMeshProUGUI Text_Grab_Count_Contents, Text_PickUp_Count_Contents;

    [SerializeField] TextMeshProUGUI Text_Grab_Avg_Contents, Text_PickUp_Avg_Contents;
    [SerializeField] TextMeshProUGUI Text_Grab_Avg_Task, Text_PickUp_Avg_Task;


    [SerializeField] TextMeshProUGUI Text_Grab_EMG_Task, Text_PickUp_EMG_Task;
    [SerializeField] TextMeshProUGUI Text_Grab_Count_Task, Text_PickUp_Count_Task;

    [SerializeField] private List<GaugeUI> listPressureUI;
    [SerializeField] private HandControllerSO controllerSO;

    // Start is called before the first frame update
    void Start()
    {
        Text_ContentsTimer.text = Contents_TimeStart.ToString("F2");
        Text_TaskTimer.text = Contents_TimeStart.ToString("F2");

        Text_Grab_EMG_Contents.text = EMG_Contents_Grab.ToString("F2");
        Text_PickUp_EMG_Contents.text = EMG_Contents_Pickup.ToString("F2");

        Text_Grab_EMG_Task.text = EMG_Task_Grab.ToString("F2");
        Text_PickUp_EMG_Task.text = EMG_Task_Pickup.ToString("F2");

        Text_Grab_Count_Contents.text = EMG_Count_Grab_Contents.ToString();
        Text_PickUp_Count_Contents.text = EMG_Count_Pickup_Contents.ToString();

        Text_Grab_Count_Task.text = EMG_Count_Grab_Task.ToString();
        Text_PickUp_Count_Task.text = EMG_Count_Pickup_Task.ToString();

        emgSO.RegisterOnChangedEvent(EMG_Task_StartEMG);
        emgSO.RegisterOnChangedEvent(EMG_Contents_StartEMG);
    }

    // Update is called once per frame
    void Update()
    {
        StartTime();
        UpdatePressure();
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Task_ResetBtn();
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            Task_StartPauseBtn();
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            Contents_ResetBtn();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            Contents_StartPauseBtn();
        }
    }

    void StartTime()
    {
        if (Contents_timeActive)
        {
            Contents_TimeStart += Time.deltaTime;
            Text_ContentsTimer.text = Contents_TimeStart.ToString("F2");
        }

        if (Task_timeActive)
        {
            Task_TimeStart += Time.deltaTime;
            Text_TaskTimer.text = Task_TimeStart.ToString("F2");
        }
    }

    public void Contents_StartPauseBtn()
    {
        Contents_timeActive = !Contents_timeActive;
        Contents_startPauseText.text = Contents_timeActive ? "PAUSE" : "START";
        EMG_Contents_Active = !EMG_Contents_Active;
        CSVSaver.Instance.SetSaveOn(Contents_timeActive);

    }

    public void Task_StartPauseBtn()
    {
        Task_timeActive = !Task_timeActive;
        Task_startPauseText.text = Task_timeActive ? "PAUSE" : "START";
        EMG_Task_Active = !EMG_Task_Active;
    }

    public void Contents_Time_ResetBtn()
    {
        if (Contents_TimeStart > 0)
        {
            Contents_TimeStart = 0f;
            Text_ContentsTimer.text = Contents_TimeStart.ToString("F2");
        }
    }

    void EMG_Contents_StartEMG(EMG_SO.EMGType _emgType, int emg)
    {
        //추후 아두이노에서 EMG를 받아오는 함수로 변경
        if (EMG_Contents_Active)
        {
            Contents_count++;

            switch (_emgType)
            {
                case EMG_SO.EMGType.GRAB:
                    EMG_Contents_Grab = emg;

                    //합
                    Content_Grab_EMGSum += EMG_Contents_Grab;
                    //평균
                    Content_Grab_EMGAvg = Content_Grab_EMGSum / Contents_count;
                    Text_Grab_Avg_Contents.text = Content_Grab_EMGAvg.ToString("F2");
                    Text_Grab_EMG_Contents.text = EMG_Contents_Grab.ToString("F2");
                    Text_Grab_Count_Contents.text = EMG_Count_Grab_Contents.ToString();

                    if (EMG_Contents_Grab > GrabThreshold) //임계값보다 크고 충분히 시간이 지난 후 다시 카운팅
                    {
                        EMG_Count_Grab_Contents++;
                    }
                    
                    break;
                
                case EMG_SO.EMGType.PICK:
                    EMG_Contents_Pickup = emgSO.emgDatas[_emgType].Peek();

                    Content_Pickup_EMGSum += EMG_Contents_Pickup;
                    Content_Pickup_EMGAvg = Content_Pickup_EMGSum / Contents_count;

                    Text_PickUp_Avg_Contents.text = Content_Pickup_EMGAvg.ToString("F2");
                    Text_PickUp_EMG_Contents.text = EMG_Contents_Pickup.ToString("F2");

                    Text_PickUp_Count_Contents.text = EMG_Count_Pickup_Contents.ToString();

                    if (EMG_Contents_Pickup > GrabThreshold) //임계값보다 크고 충분히 시간이 지난 후 다시 카운팅
                    {
                        EMG_Count_Pickup_Contents++;
                    }

                    break;
            }
            //EMG_Contents_Grab = Random.Range(0, 1500);
            //EMG_Contents_Pickup = Random.Range(0, 1500);
            //Content_Grab_EMGAvg = 0;
        }
    }

    void EMG_Task_StartEMG(EMG_SO.EMGType _emgType, int emg)
    {
        //추후 아두이노에서 EMG를 받아오는 함수로 변경
        if (EMG_Task_Active)
        {
            Task_count++;
            //EMG_Contents_Grab
            Text_Grab_EMG_Task.text = EMG_Contents_Grab.ToString("F2");
            //합
            Task_Grab_EMGSum += EMG_Contents_Grab;
            //평균
            Task_Grab_EMGAvg = Task_Grab_EMGSum / Task_count;
            //최대값
            if (Task_Grab_EMGMax < EMG_Contents_Grab)
                Task_Grab_EMGMax = EMG_Contents_Grab;
            Text_Grab_Avg_Task.text = Task_Grab_EMGAvg.ToString("F2");

            if (EMG_Contents_Grab > GrabThreshold) //임계값
            {
                EMG_Count_Grab_Task++;
            }
            //EMG_Task_Pickup = emgSO.emgDatas[_emgType].Peek();

            Text_PickUp_EMG_Task.text = EMG_Contents_Pickup.ToString("F2");
            Task_Pickup_EMGSum += EMG_Contents_Pickup;
            Task_Pickup_EMGAvg = Task_Pickup_EMGSum / Task_count;
            if (Task_Pickup_EMGMax < EMG_Contents_Pickup)
                Task_Pickup_EMGMax = EMG_Contents_Pickup;
            Text_PickUp_Avg_Task.text = Task_Pickup_EMGAvg.ToString("F2");

            if (EMG_Contents_Pickup > PickupThreshold)
            {
                EMG_Count_Pickup_Task++;
            }
            Text_Grab_Count_Task.text = Task_Grab_EMGMax.ToString();
            Text_PickUp_Count_Task.text = Task_Pickup_EMGMax.ToString();

        }
    }

    public void Contents_ResetBtn()
    {
        Contents_TimeStart = 0f;
        Text_ContentsTimer.text = Contents_TimeStart.ToString("F2");

        EMG_Contents_Grab = 0f;
        EMG_Contents_Pickup = 0f;
        EMG_Count_Grab_Contents = 0;
        EMG_Count_Pickup_Contents = 0;

        Text_Grab_EMG_Contents.text = EMG_Contents_Grab.ToString("F2");
        Text_PickUp_EMG_Contents.text = EMG_Contents_Pickup.ToString("F2");
        Text_Grab_Count_Contents.text = EMG_Count_Grab_Contents.ToString();
        Text_PickUp_Count_Contents.text = EMG_Count_Pickup_Contents.ToString();


        Content_Grab_EMGAvg = 0f;
        Content_Pickup_EMGAvg = 0f;
        Text_Grab_Avg_Contents.text = Content_Grab_EMGAvg.ToString("F2");
        Text_PickUp_Avg_Contents.text = Content_Pickup_EMGAvg.ToString("F2");
    }

    public void Task_ResetBtn()
    {
        Debug.Log("Reset");
        Task_TimeStart = 0f;
        Text_TaskTimer.text = Task_TimeStart.ToString("F2");

        EMG_Task_Grab = 0f;
        EMG_Task_Pickup = 0f;
        EMG_Count_Grab_Task = 0;
        EMG_Count_Pickup_Task = 0;

        Text_Grab_EMG_Task.text = EMG_Task_Grab.ToString("F2");
        Text_PickUp_EMG_Task.text = EMG_Task_Pickup.ToString("F2");
        Text_Grab_Count_Task.text = EMG_Count_Grab_Task.ToString();
        Text_PickUp_Count_Task.text = EMG_Count_Pickup_Task.ToString();

        Task_Grab_EMGAvg = 0f;
        Task_Pickup_EMGAvg = 0f;
        Text_Grab_Avg_Task.text = Task_Grab_EMGAvg.ToString("F2");
        Text_PickUp_Avg_Task.text = Task_Pickup_EMGAvg.ToString("F2");

        Task_Grab_EMGMax = 0f;
        Task_Pickup_EMGMax = 0f;
        Text_Grab_Count_Task.text = Task_Grab_EMGMax.ToString();
        Text_PickUp_Count_Task.text = Task_Pickup_EMGMax.ToString();
    }

    public void EMG_StartPauseBtn()
    {
        EMG_Contents_Active = !EMG_Contents_Active;
    }

    public void EMG_ResetBtn()
    {
        if (EMG_Contents_Grab > 0)
        {
            EMG_Contents_Grab = 0f;
            Text_Grab_EMG_Contents.text = EMG_Contents_Grab.ToString("F2");
        }
    }

    public void EMG_ResetCount()
    {
        Text_Grab_Count_Contents.text = "0";
        Text_PickUp_Count_Contents.text = "0";
    }

    public void UpdatePressure()
    {
        for (int i = 0; i < listPressureUI.Count; i++)
        {
            listPressureUI[i].SetState(controllerSO.pressureRight[i].fingerPressure);
        }
    }

    public void UpdatePressure(int id, float pressure)
    {
        listPressureUI[id].SetState(pressure);
    }
}