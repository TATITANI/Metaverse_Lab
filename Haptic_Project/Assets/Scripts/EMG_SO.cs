using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "EMG", menuName = "Scriptable Object/EMG", order = int.MaxValue)]
public class EMG_SO : ScriptableObject
{
    private UnityEvent OnChangedEvent = new UnityEvent();
    public Queue<int> datas { get; private set; } = new Queue<int>();
    public int capacity { get; private set; } = 16;

    public void PushData(int _emg)
    {
        datas.Enqueue(_emg);
        if (datas.Count > capacity)
        {
            datas.Dequeue();
        }
        ExecuteOnChangedEvents();
    }

    public void RegisterOncChangedEvent(UnityAction _action)
    {
        OnChangedEvent.AddListener(_action);
    }
    
    public void UnRegisterOncChangedEvent(UnityAction _action)
    {
        OnChangedEvent.RemoveListener(_action);        
    }

    void ExecuteOnChangedEvents()
    {
        OnChangedEvent.Invoke();
    }
    
    
}