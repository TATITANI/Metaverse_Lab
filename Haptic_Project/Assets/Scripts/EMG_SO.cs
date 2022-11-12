using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Imaging;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "EMG", menuName = "Scriptable Object/EMG", order = int.MaxValue)]
public class EMG_SO : ScriptableObject
{
    public enum EMGType
    {
        GRAB,
        PICK
    }

    public Dictionary<EMGType, Queue<int>> emgDatas { get; private set; }
        = new Dictionary<EMGType, Queue<int>>();

    public int capacity { get; private set; } = 16;
    private UnityEvent<EMGType> OnChangedEvent = new UnityEvent<EMGType>();

    public void PushData(EMGType emgType, int _emg)
    {
        if (emgDatas.ContainsKey(emgType) == false)
        {
            emgDatas.Add(emgType, new Queue<int>());
        }
        
        emgDatas[emgType].Enqueue(_emg);
        if (emgDatas[emgType].Count > capacity)
        {
            emgDatas[emgType].Dequeue();
        }

        ExecuteOnChangedEvents(emgType);
    }

    public void RegisterOnChangedEvent(UnityAction<EMGType> _action)
    {
        OnChangedEvent.AddListener(_action);
    }

    public void UnRegisterOncChangedEvent(UnityAction<EMGType> _action)
    {
        OnChangedEvent.RemoveListener(_action);
    }

    void ExecuteOnChangedEvents(EMGType emgType)
    {
        OnChangedEvent.Invoke(emgType);
    }
}