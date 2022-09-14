using System;
using System.Collections;
using System.IO.Ports;
using System.Collections.Generic;
using UnityEngine;

public class MoveHand : MonoBehaviour
{
    struct mpuRawData
    {
        public Int32 sent_T;
        public Int16 accl_X;
        public Int16 accl_Y;
        public Int16 accl_Z;
        public Int16 gyro_X;
        public Int16 gyro_Y;
        public Int16 gyro_Z;
        public Int16 flexData;
    };

    struct MPUData
    {
        public float sent_T;
        public float accl_X;
        public float accl_Y;
        public float accl_Z;
        public float gyro_X;
        public float gyro_Y;
        public float gyro_Z;
    };

    SerialPort sp;
    Queue<byte> ReadData = new Queue<byte>();

    public mpuRawData mpuRaw;
    public MPUData mpuData;

    public GameObject index1;
    public GameObject index2;
    public GameObject index3;
    public GameObject midfg1;
    public GameObject midfg2;
    public GameObject midfg3;
    public GameObject thumb1;
    public GameObject thumb2;

    Int16[] gyro_Offset = new Int16[3] { 269, 26, 10 };
    Int16[] accel_Offset = new Int16[3] { 0, 0, 0 }; //{ 140, -95, 0 };

    float grav_Acc = 1.05f;

    float[] angle = new float[3] { 0, 0, 0 };
    float[] Acceleration = new float[3] { 0, 0, 0 };
    float[] Speed = new float[3] { 0, 0, 0 };
    float[] Position = new float[3] { 0, 0, 0 };

    // Start is called before the first frame update
    void Start()
    {
        index1 = GameObject.find("index1");
        index2 = GameObject.find("index2");
        index3 = GameObject.find("index3");

        midfg1 = GameObject.find("mid1");
        midfg2 = GameObject.find("mid2");
        midfg3 = GameObject.find("mid3");

        thumb1 = GameObject.find("thumb1");
        thumb2 = GameObject.find("thumb2");
        
        string the_com = "";

        foreach (string mysps in SerialPort.GetPortNames())
        {
            print(mysps);
            if (mysps != "COM1") { the_com = mysps; break; }
        }
        the_com = "COM3";

        sp = new SerialPort("\\\\.\\" + the_com, 115200);

        if (!sp.IsOpen)
        {
            print("Opening " + the_com + ", baud 115200");
            sp.Open();
            sp.ReadTimeout = 100;
            sp.Handshake = Handshake.None;
            if (sp.IsOpen) { print("Open"); }
        }
    }

    // Update is called once per frame
    void Update()
    {
        int size = sp.BytesToRead;

        byte[] BUFFER = new byte[size];
        sp.Read(BUFFER, 0, size);
        for (Int16 c = 0; c < size; c++) ReadData.Enqueue(BUFFER[c]);

        while (ReadData.Count > 18)
        {
            while (true) { if (ReadData.Dequeue() == 170) break; }

            /*
             * Recieve Data From Serial
             */
            mpuRaw.sent_T = BitConverter.ToInt16(new byte[2] { ReadData.Dequeue(), ReadData.Dequeue() }, 0);

            mpuRaw.accl_X = BitConverter.ToInt16(new byte[2] { ReadData.Dequeue(), ReadData.Dequeue() }, 0);
            mpuRaw.accl_Y = BitConverter.ToInt16(new byte[2] { ReadData.Dequeue(), ReadData.Dequeue() }, 0);
            mpuRaw.accl_Z = BitConverter.ToInt16(new byte[2] { ReadData.Dequeue(), ReadData.Dequeue() }, 0);

            mpuRaw.gyro_X = (short)(mpuRaw.gyro_X * 0.9 + 0.1 * (BitConverter.ToInt16(new byte[2] { ReadData.Dequeue(), ReadData.Dequeue() }, 0) - gyro_Offset[0]));
            mpuRaw.gyro_Y = (short)(mpuRaw.gyro_Y * 0.9 + 0.1 * (BitConverter.ToInt16(new byte[2] { ReadData.Dequeue(), ReadData.Dequeue() }, 0) - gyro_Offset[1]));
            mpuRaw.gyro_Z = (short)(mpuRaw.gyro_Z * 0.9 + 0.1 * (BitConverter.ToInt16(new byte[2] { ReadData.Dequeue(), ReadData.Dequeue() }, 0) - gyro_Offset[2]));

            mpuRaw.flexData = BitConverter.ToInt16(new byte[2] { ReadData.Dequeue(), ReadData.Dequeue() }, 0);

            /*
             * Scale Recieved data
             */
            mpuData.sent_T = (float)mpuRaw.sent_T * 0.000001f;

            mpuData.accl_X = (float)mpuRaw.accl_X / 8192.0f;
            mpuData.accl_Y = (float)mpuRaw.accl_Y / 8192.0f;
            mpuData.accl_Z = (float)mpuRaw.accl_Z / 8192.0f;

            mpuData.gyro_X = (float)mpuRaw.gyro_X / 16.4f * mpuData.sent_T;
            mpuData.gyro_Y = (float)mpuRaw.gyro_Y / 16.4f * mpuData.sent_T;
            mpuData.gyro_Z = (float)mpuRaw.gyro_Z / 16.4f * mpuData.sent_T;

            /*
             * Calculate Angles
             */
            float vec;
            vec = (float)Math.Sqrt(mpuData.accl_X * mpuData.accl_X + mpuData.accl_Z * mpuData.accl_Z);
            angle[0] = (angle[0] + mpuData.gyro_X) * .98f
                + (float)Math.Atan2(mpuData.accl_Y, vec) * 180f / 3.141592f * 0.02f;

            vec = (float)Math.Sqrt(mpuData.accl_Y * mpuData.accl_Y + mpuData.accl_Z * mpuData.accl_Z);
            angle[1] = (angle[1] - mpuData.gyro_Y) * .98f
                + (float)Math.Atan2(mpuData.accl_X, vec) * 180f / 3.141592f * 0.02f;

            angle[2] += mpuData.gyro_Z;
        }

        transform.rotation = Quaternion.Euler(-angle[0], angle[2], -angle[1]);

        index1.transform.rotation = Quaternion.Euler(0,0,(float)mpuRaw.flexData * 60 / 512);
        index2.transform.rotation = Quaternion.Euler(0,0,(float)mpuRaw.flexData * 60 / 512);
        index3.transform.rotation = Quaternion.Euler(0,0,(float)mpuRaw.flexData * 60 / 512);
        midfg1.transform.rotation = Quaternion.Euler(0,0,(float)mpuRaw.flexData * 60 / 512);
        midfg2.transform.rotation = Quaternion.Euler(0,0,(float)mpuRaw.flexData * 60 / 512);
        midfg3.transform.rotation = Quaternion.Euler(0,0,(float)mpuRaw.flexData * 60 / 512);
        thumb1.transform.rotation = Quaternion.Euler(0,0,(float)mpuRaw.flexData * 90 / 512);
        thumb2.transform.rotation = Quaternion.Euler(0,0,(float)mpuRaw.flexData * 90 / 512);
    }
}
