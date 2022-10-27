using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UImanager : MonoBehaviour
{
    [SerializeField] float timeStart;
    [SerializeField] float EMGStart; //EMG 현재 센싱 값 / 아두이노에서 받아오기 
    [SerializeField] int EMG_Grab; //EMG 쥐는 동작 카운트 / 아두이노에서 받아오기 
    [SerializeField] int EMG_Pickup; //EMG 집는 동작 카운트 / 아두이노에서 받아오기 

    [SerializeField] TextMeshProUGUI Text_time, Time_startPauseText;
    [SerializeField] TextMeshProUGUI Text_EMG, EMG_startPauseText;
    [SerializeField] TextMeshProUGUI Text_Grab_Count , Text_Pickup_Count;

    bool timeActive = false;
    bool EMG_Active = false;

    // Start is called before the first frame update
    void Start()
    {
        Text_time.text = timeStart.ToString("F2");    
        Text_EMG.text = EMGStart.ToString("F2");    
    }
 // Update is called once per frame
    void Update()
    {
        StartTime();
        EMG_StartEMG();
    }

    void StartTime()
    {
        if (timeActive)
        {
            timeStart += Time.deltaTime;
            Text_time.text = timeStart.ToString("F2");
        }
    }

    public void Time_StartPauseBtn()
    {
        timeActive = !timeActive;
        Time_startPauseText.text = timeActive ? "PAUSE" : "START";
    }

    public void Time_ResetBtn()
    {
        if(timeStart > 0)
        {
            timeStart = 0f;
            Text_time.text = timeStart.ToString("F2");
        }
    }
    void EMG_StartEMG()
    {
        //추후 아두이노에서 EMG를 받아오는 함수로 변경
        if (EMG_Active)
        {
            EMGStart += Time.deltaTime;
            Text_EMG.text = EMGStart.ToString("F2");

            //Test용 시간이 지남에 따라 EMG_Grab과 EMG_Pickup 개수 
            EMG_Grab = (int)EMGStart / 3;
            EMG_Pickup= (int)EMGStart / 2;
            Text_Grab_Count.text=EMG_Grab.ToString();
            Text_Pickup_Count.text=EMG_Pickup.ToString();
        }
    }

    public void EMG_StartPauseBtn()
    {
        EMG_Active = !EMG_Active;
        EMG_startPauseText.text = EMG_Active ? "PAUSE" : "START";
    }

    public void EMG_ResetBtn()
    {
        if(EMGStart > 0)
        {
            EMGStart = 0f;
            Text_EMG.text = EMGStart.ToString("F2");
        }
    }
     public void EMG_ResetCount()
    {
        Text_Grab_Count.text="0";
        Text_Pickup_Count.text="0";
    }
}
