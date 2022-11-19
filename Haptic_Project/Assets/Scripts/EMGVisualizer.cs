using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EMGVisualizer : MonoBehaviour
{
    [Serializable]
    private class emgGraphElementGroup
    {
        [SerializeField] private RectTransform trfLine;
        public List<RectTransform> lines { get; private set; } = new List<RectTransform>();
        public List<RectTransform> points { get; private set; } = new List<RectTransform>();
        [SerializeField] private Color lineColor;
        [SerializeField] private Image imgDescriptonBox;
        [SerializeField] private TextMeshProUGUI txtDescription;

        public void Init(EMG_SO.EMGType emgtype)
        {
            imgDescriptonBox.color = lineColor;
            txtDescription.text = emgtype.ToString();
        }
        public void AddElement(RectTransform point)
        {
            points.Add(point);
            var line = Instantiate(trfLine.gameObject, trfLine.parent).GetComponent<RectTransform>();
            line.GetComponent<Image>().color = lineColor;
            lines.Add(line);
            trfLine.gameObject.SetActive(false);
        }
    }

    [SerializeField] private EMG_SO emgSO;
    [SerializeField] private TextMeshProUGUI txtPeak;

    [SerializeField] private RectTransform graphBG;
    [SerializeField] private HorizontalLayoutGroup horLayoutGroup;
    [SerializeField] private RectTransform trfVerticalLine;

    [SerializeField] private SerializableDictionary<EMG_SO.EMGType, emgGraphElementGroup> emgElements;

    private Vector2 graphSize;
    private float valueNormalized;

    private void Awake()
    {
        emgSO.RegisterOnChangedEvent(Draw);
    }

    void Start()
    {
        txtPeak.text = $"{emgSO.MaxPeak}";
        graphSize = graphBG.sizeDelta;
        valueNormalized = graphSize.y / emgSO.MaxPeak;

        var enumEmgValues = Enum.GetValues(typeof(EMG_SO.EMGType));
        foreach (var enumEmgValue in enumEmgValues)
        {
            EMG_SO.EMGType emgType = (EMG_SO.EMGType)enumEmgValue;
            emgElements[emgType].Init(emgType);
        }
        void ResizeGraph()
        {
            // (spacing + line_width) * 개수 = bg_width 
            // => spacing = bg_width/개수 - line_width
            horLayoutGroup.spacing = graphSize.x / emgSO.capacity - trfVerticalLine.sizeDelta.x;

            trfVerticalLine.gameObject.SetActive(false);
            for (int i = 0; i < emgSO.capacity; i++)
            {
                var line = Instantiate(trfVerticalLine.gameObject, trfVerticalLine.parent);
                line.gameObject.SetActive(true);
                
                foreach(var enumEmgValue in enumEmgValues)
                {
                    int pointID = Array.IndexOf(enumEmgValues, enumEmgValue);
                    RectTransform point = line.transform.GetChild(pointID).GetComponent<RectTransform>();
                    point.gameObject.SetActive(false);
                    emgElements[(EMG_SO.EMGType)enumEmgValue].AddElement(point);
                }
            }
         
        }

        ResizeGraph();

        if (AppManager.Instance.IsTest)
        {
            StartCoroutine(PushDatas_Test());
        }
    }

    void Draw(EMG_SO.EMGType emgType, int emg)
    {
        Queue<int> _datas = new Queue<int>(emgSO.emgDatas[emgType]);
        List<RectTransform> points = emgElements[emgType].points;
        List<RectTransform> lines = emgElements[emgType].lines;

        int dataCnt = _datas.Count;
        int pointID = points.Count - dataCnt;
        int value;
        while (_datas.TryPeek(out value))
        {
            points[pointID].gameObject.SetActive(true);
            float y = Mathf.Clamp(value * valueNormalized, 0, graphSize.y);
            points[pointID].anchoredPosition = new Vector2(points[pointID].anchoredPosition.x, y);

            void DrawLine()
            {
                Vector2 startPos = points[pointID - 1].position;
                Vector2 endPos = points[pointID].position;
                Vector2 dir = (endPos - startPos).normalized;
                float distance = Vector2.Distance(startPos, endPos);

                int lineID = pointID - 1;
                lines[lineID].sizeDelta = new Vector2(distance, lines[lineID].sizeDelta.y);
                lines[lineID].position = startPos + dir * distance * 0.5f;
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                lines[lineID].eulerAngles = new Vector3(0, 0, angle);
                lines[lineID].gameObject.SetActive(true);
            }

            if (pointID > points.Count - dataCnt)
            {
                DrawLine();
            }

            pointID++;
            _datas.Dequeue();
        }
    }

    IEnumerator PushDatas_Test()
    {
        const int dataSize = 180;
        int[] testGrabDatas = new int[dataSize]
        {
            289, 36, 81, 25, 121, 324, 36, 16, 25, 9, 121, 9, 289, 225, 16, 64, 25, 1, 25, 4, 9, 25, 0, 36, 49, 121, 1,
            1, 25, 25, 484, 9, 324, 36, 100, 256, 49, 100, 196, 1, 4, 0, 25, 81, 25, 121, 1, 4, 49, 25, 9, 4, 16, 0, 4,
            4, 9, 121, 100, 25, 25, 4, 0, 36, 0, 25, 16, 81, 36, 1, 81, 225, 25, 9, 81, 144, 81, 16, 36, 16, 36, 64, 36,
            169, 256, 1, 16, 1, 25, 144, 1, 25, 9, 16, 4, 0, 64, 16, 16, 16, 4, 1, 49, 16, 49, 4, 25, 144, 49, 144, 121,
            0,
            1, 1, 4, 36, 16, 49, 1, 25, 36, 1, 49, 16, 144, 0, 9, 25, 36, 64, 100, 4, 0, 4, 9, 36, 9, 36, 0, 0, 1, 4,
            36, 9,
            144, 16, 0, 36, 49, 49, 49, 4, 0, 4, 1, 16, 25, 49, 1, 1, 4, 0, 81, 0, 36, 36, 100, 0, 4, 144, 25, 81, 9, 1,
            4,
            36, 25, 36, 49, 16
        };

        int[] testPickDatas = new int[dataSize]
        {
            169, 100, 100, 1, 576, 324, 256, 169, 4, 9, 25, 400, 9, 169, 4, 25, 4, 121, 1, 9, 4, 49, 16, 64, 16, 9, 64,
            0, 9, 0, 1, 36, 9, 36, 1, 16, 0, 36, 0, 25, 4, 4, 25, 25, 4, 9, 64, 0, 4, 1, 36, 0, 36, 4, 16, 81, 1, 1, 25,
            121, 4, 1, 4, 25, 1, 64, 16, 64, 1, 121, 1, 49, 4, 36, 1, 49, 1, 1, 4, 49, 1, 16, 1, 9, 121, 144, 4, 25,
            100, 49, 0, 0, 49, 1, 100, 49, 100, 49, 729, 289, 100, 25, 4, 1, 0, 100, 25, 1, 1, 36, 1, 0, 144, 16, 4, 4,
            0, 1, 49, 4, 25, 9, 36, 0, 9, 9, 1, 1, 9, 4, 9, 9, 9, 1, 36, 0, 4, 36, 0, 1, 0, 0, 36, 16, 1, 4, 0, 4, 9,
            49, 81, 9, 25, 9, 64, 0, 25, 4, 25, 0, 0, 1, 25, 0, 144, 100, 1, 1, 0, 16, 1, 9, 1, 1, 0, 16, 1, 1, 4, 9
        };

        int dataId = 0;
        while (true)
        {
            dataId = (dataId == dataSize - 1) ? 0 : dataId + 1;
            emgSO.PushData(EMG_SO.EMGType.GRAB, testGrabDatas[dataId]);//+UnityEngine.Random.Range(0, 100));
            emgSO.PushData(EMG_SO.EMGType.PICK, testPickDatas[dataId]);//+UnityEngine.Random.Range(0, 100));
           
            yield return new WaitForSeconds(0.1f);
        }
    }
}