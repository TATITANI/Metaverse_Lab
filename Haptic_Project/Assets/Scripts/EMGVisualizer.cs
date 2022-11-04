using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class EMGVisualizer : MonoBehaviour
{
    [SerializeField] private EMG_SO emgSO;
    [SerializeField] private int maxPeak = 300;
    [SerializeField] private TextMeshProUGUI txtPeak;

    [SerializeField] private RectTransform graphBG;
    [SerializeField] private HorizontalLayoutGroup horLayoutGroup;
    [SerializeField] private RectTransform trfVerticalLine;
    [SerializeField] private RectTransform trfLine;
    [SerializeField] private List<RectTransform> points;
    private List<RectTransform> lines = new List<RectTransform>();

    private Vector2 graphSize;
    private float valueNormalized;
    private void Awake()
    {
        emgSO.RegisterOncChangedEvent(Draw);
    }

    void Start()
    {
        txtPeak.text = $"{maxPeak}";
        graphSize = graphBG.sizeDelta;
        valueNormalized = graphSize.y / maxPeak;

        // (spacing + line_width) * 개수 = bg_width 
        // => spacing = bg_width/개수 - line_width
        horLayoutGroup.spacing = graphSize.x / emgSO.capacity - trfVerticalLine.sizeDelta.x;
        
        points.Clear();
        trfVerticalLine.gameObject.SetActive(false);
        for (int i = 0; i < emgSO.capacity; i++)
        {
            var line = Instantiate(trfVerticalLine.gameObject, trfVerticalLine.parent);
            line.gameObject.SetActive(true);
            var point = line.GetComponentInChildren<RectTransform>().GetChild(0).GetComponent<RectTransform>();
            point.gameObject.SetActive(false);
            points.Add(point); 
        }

        trfLine.gameObject.SetActive(false);
        for (int i = 0; i < points.Count - 1; i++)
        {
            RectTransform _trfLine = Instantiate(trfLine.gameObject, trfLine.parent).GetComponent<RectTransform>();
            lines.Add(_trfLine);
        }
        
        StartCoroutine(PushDatas_Test());
    }

    void Draw()
    {
        Queue<int> _datas = new Queue<int>(emgSO.datas);

        int value = 0;
        int id = 0;
        while (_datas.TryPeek(out value))
        {
            points[id].gameObject.SetActive(true);
            float y = value * valueNormalized;
            points[id].anchoredPosition = new Vector2(points[id].anchoredPosition.x, y);
            if (id > 0)
            {
                void DrawLine()
                {
                    Vector2 startPos = points[id - 1].position;
                    Vector2 endPos = points[id].position;
                    Vector2 dir = (endPos - startPos).normalized;
                    float distance = Vector2.Distance(startPos, endPos);

                    int lineID = id - 1;
                    lines[lineID].sizeDelta = new Vector2(distance, lines[lineID].sizeDelta.y);
                    lines[lineID].position = startPos + dir * distance * 0.5f;
                    float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                    lines[lineID].eulerAngles = new Vector3(0, 0, angle);
                    lines[lineID].gameObject.SetActive(true);
                }

                DrawLine();
            }

            id++;
            _datas.Dequeue();
        }
    }

    IEnumerator PushDatas_Test()
    {
        int[] testDatas =
        {
            289, 36, 81, 25, 121, 324, 36, 16, 25, 9, 121, 9, 289, 225, 16, 64, 25, 1, 25, 4, 9, 25, 0, 36, 49, 121, 1,
            1, 25, 25, 484, 9, 324, 36, 100, 256, 49, 100, 196, 1, 4, 0, 25, 81, 25, 121, 1, 4, 49, 25, 9, 4, 16, 0, 4, 4,
            9, 121, 100, 25, 25, 4, 0, 36, 0, 25, 16, 81, 36, 1, 81, 225, 25, 9, 81, 144, 81, 16, 36, 16, 36, 64, 36, 169,
            256, 1, 16, 1, 25, 144, 1, 25, 9, 16, 4, 0, 64, 16, 16, 16, 4, 1, 49, 16, 49, 4, 25, 144, 49, 144, 121, 0, 1, 1,
            4, 36, 16, 49, 1, 25, 36, 1, 49, 16, 144, 0, 9, 25, 36, 64, 100, 4, 0, 4, 9, 36, 9, 36, 0, 0, 1, 4, 36, 9, 144,
            16, 0, 36, 49, 49, 49, 4, 0, 4, 1, 16, 25, 49, 1, 1, 4, 0, 81, 0, 36, 36, 100, 0, 4, 144, 25, 81, 9, 1, 4, 36,
            25, 36, 49, 16
        };

        int dataId = 0;
        while (true)
        {
            int value = testDatas[dataId];
            dataId = (dataId == testDatas.Length - 1) ? 0 : dataId + 1;
            value = Math.Clamp(value, 0, maxPeak);
            emgSO.PushData(value);

            yield return new WaitForSeconds(0.1f);
        }
    }
}