using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System;
using System.Reflection;

public class CSVSaver : MonoBehaviour
{
    string directory = Application.streamingAssetsPath + "/savedData/";

    private void Start()
    {
        Debug.Log("======");
        // PropertyInfo[] properties = t.GetType().GetProperties();
        // FieldInfo[] fields = t.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic);
        //
        // foreach (var info in fields)
        // {
        //     Debug.Log($"{info.GetValue(t)}, {info.Name}");
        // }
    }
    

    private void WriteCsv(List<string[]> rowData, string filePath)
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

        Stream fileStream = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write);
        StreamWriter outStream = new StreamWriter(fileStream, Encoding.UTF8);
        outStream.WriteLine(stringBuilder);
        outStream.Close();
    }
}
