using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;
using Vector2 = UnityEngine.Vector2;

public class EMGSpectrogram : MonoBehaviour
{
    [SerializeField] SpectrogramItem spectrogramItem;
    [SerializeField] SpectrogramItem[] spectrogramItems;
    [SerializeField] private HorizontalLayoutGroup horLayoutGroup;
    [SerializeField] private Vector2 blockSize;
    [SerializeField] private EMG_SO emgSo;

    private Queue<Complex[]> itemDatas;

    private void Awake()
    {
        int capacity = emgSo.capacity;
        horLayoutGroup.GetComponent<RectTransform>().sizeDelta =
            blockSize * capacity;

        spectrogramItem.gameObject.SetActive(false);
        spectrogramItems = new SpectrogramItem[capacity];
        for (int i = 0; i < capacity; i++)
        {
            SpectrogramItem item =
                Instantiate(spectrogramItem, spectrogramItem.transform.parent);
            item.Init(capacity, blockSize);
            item.gameObject.SetActive(true);
            spectrogramItems[i] = item;
        }

        emgSo.RegisterOnChangedEvent(UpdateData);
        itemDatas = new Queue<Complex[]>();
    }

    void UpdateData()
    {
        if (emgSo.datas.Count < emgSo.capacity)
            return;

        double[] emgDatas = GetEMGDatas();

        void AddFFT()
        {
            Complex[] fftResult = FFTGenerator.FFT(emgDatas, 0, emgDatas.Length - 1);
            // Debug.Log($"amp min :{fftResult.Min(r => r.Magnitude)}" +
                      // $" max : {fftResult.Max(r => r.Magnitude)}");
            itemDatas.Enqueue(fftResult);
            if (itemDatas.Count > spectrogramItems.Length)
            {
                itemDatas.Dequeue();
            }
        }
        AddFFT();

        void UpdateItems()
        {
            Queue<Complex[]> _itemDatas = new Queue<Complex[]>(itemDatas);
            int itemID = spectrogramItems.Length - _itemDatas.Count;
            while (_itemDatas.Count > 0)
            {
                Complex[] itemData = _itemDatas.Dequeue();
                spectrogramItems[itemID].UpdateData(itemData);
                itemID++;
            }
        }
        UpdateItems();

    }
    
    double[] GetEMGDatas()
    {
        Queue<int> _emgDatas = new Queue<int>(emgSo.datas);
        double[] emgDatas = new double[_emgDatas.Count];
        for (int i = 0; i < emgDatas.Length; i++)
        {
            emgDatas[i] = _emgDatas.Dequeue();
        }
        return emgDatas;
    }
}