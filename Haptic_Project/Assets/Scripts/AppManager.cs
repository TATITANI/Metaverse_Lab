using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AppManager : MonoBehaviour
{
    private static AppManager _instance;

    public static AppManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<AppManager>();
            if (_instance == null)
            {
                GameObject container = new GameObject("AppManager");
                _instance = container.AddComponent<AppManager>();
            }

            return _instance;
        }
    }

    [SerializeField] private bool isTest;
    [SerializeField] private int targetFrame = 60;
    public bool IsTest
    {
        get { return isTest; }
    }

    private void Awake()
    {
        Application.targetFrameRate = targetFrame;
    }
}