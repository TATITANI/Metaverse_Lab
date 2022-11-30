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
            horLayoutGroup.spacing = graphSize.x / emgSO.Capacity - trfVerticalLine.sizeDelta.x;

            trfVerticalLine.gameObject.SetActive(false);
            for (int i = 0; i < emgSO.Capacity; i++)
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

   
}