using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System;
using System.Reflection;

public class CSVSaver : MonoBehaviour
{
    private static CSVSaver _instance;

    public static CSVSaver Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<CSVSaver>();
            if (_instance == null)
            {
                GameObject container = new GameObject("CSVSaver");
                _instance = container.AddComponent<CSVSaver>();
            }

            return _instance;
        }
    }

    [SerializeField] private bool isSave = false;
    [SerializeField] private string fileName = "grab.csv";
    [SerializeField] private EMG_SO emgSO;

    private string directory = "";

    private Stream fileStream;
    private StreamWriter outStream;

    private void Start()
    {
        directory = Application.dataPath + "/savedData/" + fileName;
        fileStream = new FileStream(directory, FileMode.OpenOrCreate, FileAccess.Write);
        outStream = new StreamWriter(fileStream, Encoding.UTF8);
        
        emgSO.RegisterOnChangedEvent(WriteEMG);

        
        // PropertyInfo[] properties = t.GetType().GetProperties();
        // FieldInfo[] fields = t.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic);
        //
        // foreach (var info in fields)
        // {
        //     Debug.Log($"{info.GetValue(t)}, {info.Name}");
        // }
    
    }
    
    private void OnDestroy()
    {
        outStream.Close();
    }

    void WriteEMG(EMG_SO.EMGType emgType, int emg)
    {
        if (!isSave)
        {
            return;
        }

        if (emgType == EMG_SO.EMGType.GRAB)
        {
            Write(emg.ToString());
        }
        
    }
    public void SetSaveOn(bool on)
    {
        isSave = on;
    }
    public void Write(string data)
    {
        outStream.WriteLine(data);
    }

    public void Write(List<string[]> rowData)
    {
        string[][] output = new string[rowData.Count][];

        for (int i = 0; i < output.Length; i++)
        {
            output[i] = rowData[i];
        }

        int length = output.GetLength(0);
        string delimiter = ",";

        StringBuilder stringBuilder = new StringBuilder();

        for (int index = 0; index < length; index++)
            stringBuilder.AppendLine(string.Join(delimiter, output[index]));

        outStream.WriteLine(stringBuilder);
    }
}