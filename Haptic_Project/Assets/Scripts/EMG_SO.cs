using System;
using System.Collections;
using System.Collections.Generic;
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
    
    [SerializeField] int capacity = 64;
    public int Capacity { get; private set; } = 64;
    private UnityEvent<EMGType, int> OnChangedEvent = new UnityEvent<EMGType, int>();
    [SerializeField] private int maxPeak = 300;
    private int PreviousMaxEMG=0;

    public int MaxPeak
    {
        get { return maxPeak; }
    }

    public void PushData(EMGType emgType, int _emg)
    {

        if (emgDatas.ContainsKey(emgType) == false)
        {
            emgDatas.Add(emgType, new Queue<int>());
        }
        if(_emg >PreviousMaxEMG && _emg<maxPeak){
            PreviousMaxEMG=_emg;
        }
        else if(_emg>=maxPeak)
        {
            _emg = PreviousMaxEMG;
            Debug.Log($"PreviousMaxEMG : {PreviousMaxEMG}");
        }
        
        _emg = Mathf.Clamp(_emg, 0, maxPeak);

        emgDatas[emgType].Enqueue(_emg);
        if (emgDatas[emgType].Count > capacity)
        {
            emgDatas[emgType].Dequeue();
        }

        ExecuteOnChangedEvents(emgType, _emg);
    }

    public void RegisterOnChangedEvent(UnityAction<EMGType, int> _action)
    {
        OnChangedEvent.AddListener(_action);
    }

    public void UnRegisterOncChangedEvent(UnityAction<EMGType, int> _action)
    {
        OnChangedEvent.RemoveListener(_action);
    }

    void ExecuteOnChangedEvents(EMGType emgType, int emg)
    {
        OnChangedEvent.Invoke(emgType, emg);
    }
}