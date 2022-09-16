using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private Text txtCurrentScore;
    [SerializeField] private Text txtTotalScore;
    
    [System.Serializable]
    private class ScoreData
    {
        public int score;
        public Transform pivot;
    }

    [SerializeField] private Transform center;
    [SerializeField] private List<ScoreData> scoreDatas;

    private int totalScore = 0;
    
    private static ScoreManager _instance;
    public static ScoreManager Instance
    {
        get => _instance;
    }

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        scoreDatas = scoreDatas.OrderByDescending(d => d.score).ToList();
        txtCurrentScore.text = $"얻은 점수 : 0";
        txtTotalScore.text = $"누적 점수 : {totalScore}";
    }

    public void ApplyScore(Vector2 contactPos)
    {
        Vector2 centerPos = center.transform.position;
        float distance = Vector2.Distance(contactPos, centerPos);
        var data = scoreDatas.FirstOrDefault(d => 
            distance < Vector2.Distance(d.pivot.position, centerPos));

        int score = data?.score ?? 0; // 맞췄을 때 호출되므로 기본점수 1점
        totalScore += score;
        txtCurrentScore.text = $"얻은 점수 : {score}";
        txtTotalScore.text = $"누적 점수 : {totalScore}";
    }
}