using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UImanager : MonoBehaviour
{
    [SerializeField] float timeStart;
    //[SerializeField] TextMesh Text_time, startPauseText;
    [SerializeField] TextMeshProUGUI Text_time, startPauseText;

    bool timeActive = false;

    // Start is called before the first frame update
    void Start()
    {
        Text_time.text = timeStart.ToString("F2");    
    }
 // Update is called once per frame
    void Update()
    {
        StartTime();
    }

    void StartTime()
    {
        if (timeActive)
        {
            timeStart += Time.deltaTime;
            Text_time.text = timeStart.ToString("F2");
        }
    }

    public void StartPauseBtn()
    {
        timeActive = !timeActive;
        startPauseText.text = timeActive ? "PAUSE" : "START";
    }

    public void ResetBtn()
    {
        if(timeStart > 0)
        {
            timeStart = 0f;
            Text_time.text = timeStart.ToString("F2");
        }
    }
}
