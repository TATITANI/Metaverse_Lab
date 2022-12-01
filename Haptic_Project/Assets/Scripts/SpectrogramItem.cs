using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;
using Vector2 = UnityEngine.Vector2;

public class SpectrogramItem : MonoBehaviour
{
    [SerializeField] private Image imgBlock;
    [SerializeField] private Image[] imgBlocks;
    public Complex[] datas { get; private set; }
    private float maxHue;
    public void Init(int blockCnt, Vector2 blockSize, float _maxHue)
    {
        imgBlock.gameObject.SetActive(false);
        imgBlocks = new Image[blockCnt];
        maxHue = _maxHue;
        for (int i = 0; i < blockCnt; i++)
        {
            Image img = i == 0 ? imgBlock : Instantiate(imgBlock, imgBlock.transform.parent);
            GetComponent<RectTransform>().sizeDelta = blockSize;
            img.GetComponent<RectTransform>().sizeDelta = blockSize;
            img.gameObject.SetActive(true);
            imgBlocks[i] = img;
            imgBlocks[i].color = Color.HSVToRGB(maxHue,1,1);
        }
    }

    public void UpdateData(Complex[] _datas)
    {
        datas = _datas;
        if (datas.Length != imgBlocks.Length)
        {
            Debug.LogError($"Spectrogram data length not match ");
            return;
        }

        for (int i = 0; i < datas.Length; i++)
        {
            float dBMagnitude = 10 * Mathf.Log10((float)datas[i].Magnitude);
            //50db 최대
            float normalizedMag = (dBMagnitude) * 0.02f * maxHue; // 0~1
            float h = Mathf.Clamp(maxHue- normalizedMag, 0, maxHue); // amplitude 낮을수록 파랑
            // 아래부터 저주파
            imgBlocks[datas.Length-1-i].color = Color.HSVToRGB(h,1,1);
            
            // Debug.Log($"{dBMagnitude} / {normalizedMag}");
        }
    }
}