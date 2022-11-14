using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Vector2 = UnityEngine.Vector2;

public class EMGSpectrogram : MonoBehaviour
{
    [SerializeField] private EMG_SO.EMGType emgType;

    [SerializeField] private EMG_SO emgSo;
    [SerializeField] SpectrogramItem spectrogramItem;
    [SerializeField] SpectrogramItem[] spectrogramItems;
    [SerializeField] private HorizontalLayoutGroup horLayoutGroup;
    [SerializeField] private Vector2 blockSize;
    [SerializeField] private RawImage imgColor;
    [SerializeField] private TextMeshProUGUI txtTitle;
    [SerializeField] [Range(0, 1)] private float maxHue = 0.6f; // blue
    private Queue<Complex[]> itemDatas;
    private int spectrogramSize;


    private void Awake()
    {
        spectrogramSize = Mathf.CeilToInt(emgSo.capacity / 2);
        horLayoutGroup.GetComponent<RectTransform>().sizeDelta =
            blockSize * spectrogramSize;

        spectrogramItem.gameObject.SetActive(false);
        spectrogramItems = new SpectrogramItem[spectrogramSize];
        for (int i = 0; i < spectrogramSize; i++)
        {
            SpectrogramItem item =
                Instantiate(spectrogramItem, spectrogramItem.transform.parent);
            item.Init(spectrogramSize, blockSize,maxHue);
            item.gameObject.SetActive(true);
            spectrogramItems[i] = item;
        }

        emgSo.RegisterOnChangedEvent(UpdateData);
        itemDatas = new Queue<Complex[]>();

        txtTitle.text = $"Spectrogram - {emgType.ToString()}";
        InitColorMap();
    }

    void InitColorMap()
    {
        Vector2 size = imgColor.GetComponent<RectTransform>().sizeDelta;
        Texture2D tex = new Texture2D((int)size.x, (int)size.y);

        float dH = maxHue / size.y;
        float h = 0;
        for (int y = (int)size.y-1; y >= 0; y--)
        {
            Color color = Color.HSVToRGB(h, 1, 1);
            for (int x = 0; x < size.x; x++)
            {
                tex.SetPixel(x, y, color);
            }
            h += dH;
            h = Mathf.Clamp(h, 0, maxHue);
        }

        tex.Apply();
        imgColor.material.mainTexture = tex;
    }

    void UpdateData(EMG_SO.EMGType _emgType)
    {
        if (emgType != _emgType)
            return;

        if (emgSo.emgDatas[EMG_SO.EMGType.GRAB].Count < emgSo.capacity)
            return;

        double[] emgDatas = GetEMGDatas();

        void AddFFT()
        {
            Complex[] fftResultRaw = FFTGenerator.FFT(emgDatas, 0, emgDatas.Length - 1);
            int centerID = fftResultRaw.Length - spectrogramSize;
            Complex[] fftResult = fftResultRaw[centerID..];
        
            // Debug.Log($"amp min :{fftResult.Min(r => r.Magnitude)}" +
            // $" max : {fftResult.Max(r => r.Magnitude)}");

            // string strFFT = "";
            // foreach (var complex in fftResult)
            // {
            //     strFFT += $"{complex.Magnitude},";
            // }
            // Debug.Log($"FFT : {strFFT}");

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
        Queue<int> _emgDatas = new Queue<int>(emgSo.emgDatas[this.emgType]);
        double[] emgDatas = new double[_emgDatas.Count];
        for (int i = 0; i < emgDatas.Length; i++)
        {
            emgDatas[i] = _emgDatas.Dequeue();
        }

        return emgDatas;
    }
}