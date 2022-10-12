using System;
using System.Collections;
using System.IO.Ports;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

[Serializable] public class SerialCommunicator : MonoBehaviour
{
    int RxSize = 24; //Receive
    int TxSize = 3; // Transmit


    //클래스 선언에 Serializable 사용하고, 멤버변수에는 SerializeField사용하네 차이점은?
    [Serializable] public struct RxDATA // 센서들로부터 데이터를 받아오기 위한 Receive Data 구조체는 float형 변수 6개 
    {
        [SerializeField] public float yaw; // 요잉
        [SerializeField] public float pitch; // 피칭
        [SerializeField] public float roll; // 롤링
        [SerializeField] public float flx0; // 플렉스센서0
        [SerializeField] public float flx1; // 플렉스센서1
        [SerializeField] public float flx2; // 플렉스센서2
    };

    [Serializable] public struct TxDATA // 서보로 각도를 전송하기 위한 Transmit Data 구조체는 short형 변수 3개
    {
        [SerializeField] public short servoAct0; // 서보각도0
        [SerializeField] public short servoAct1; // 서보각도1
        [SerializeField] public short servoAct2; // 서보각도2
    };

    SerialPort sp; // SerialPort 객체 선언 나중에 파헤쳐보자
    Queue<byte> ReadData = new Queue<byte>(); // C# 의 Queue 구현

    /*
     Queue는 FIFO이다.
     예제
     Queue<int> q = new Queue<int>();
     q.Enqueue(120);
     q.Enqueue(130);
     q.Enqueue(150);

     int next = q.Dequeue(); // 120
     next = q.Dequeue(); // 130 
     */

    //게임오브젝트 선언
    public GameObject wrist;
    public GameObject index1;
    public GameObject index2;
    public GameObject index3;
    public GameObject midfg1;
    public GameObject midfg2;
    public GameObject midfg3;
    public GameObject thumb1;
    public GameObject thumb2;
    public GameObject thumb3;

    [SerializeField] public RxDATA RxBuffer; // 객체 생성
    [SerializeField] public TxDATA TxBuffer; // 객체 생성

    [SerializeField] public float[] angle = new float[3] { 0, 0, 0 }; // float형 배열 선언, 원소 3개

    // Start is called before the first frame update
    void Start()
    {
        string the_com = ""; // 초기화는 empty string이다.


        // SerialPort.GetPortNames()가 string형을 원소로 갖는 배열인가??
        // COM1과 비교하는것으로 봐서 아두이노 포트이름을 의미하는것 같다.
        // 나중에 파헤쳐보자
        foreach (string mysps in SerialPort.GetPortNames()) 
        {
            print(mysps);
            if (mysps != "COM1") { the_com = mysps; break; } 
        }
        the_com = "COM3"; // 이건 아마도 내 아두이노의 포트가 COM3이라서 작성한 코드일것같다.

        sp = new SerialPort("\\\\.\\" + the_com, 115200); // 생성자 public SerialPort(string portName, int baudRate); 이다. 그런데 왜 "\\\\.\\" 이게 있는거임?

        if (!sp.IsOpen) // 만약 serialport가 열려있지 않다면 열고 , open을 프린트하는 조건문
        {
            print("Opening " + the_com + ", baud 115200");
            sp.Open();
            sp.ReadTimeout = 100;
            sp.Handshake = Handshake.None; // enum 사용했다. 
            if (sp.IsOpen) { print("Open"); }
        }
    }

    // Update is called once per frame
    void Update()
    {
        // 업데이트할때마다 시리얼포트가 열려있으면 실행한다.
        if (sp.IsOpen) {
        int size = sp.BytesToRead; // Gets the number of bytes of data in the receive buffer. (수신버퍼에 들어있는 데이터의 바이트 수)

        byte[] BUFFER = new byte[size]; // 배열
        sp.Read(BUFFER, 0, size); // Reads from the SerialPort input buffer. (시리얼포트의 인풋버퍼로부터 읽는다.)
        /*
            Reads a number of bytes from the SerialPort input buffer and writes those bytes into a byte array at the specified offset.
            public int Read (byte[] buffer, int offset, int count);
            buffer : The byte array to write the input to.
            offset : The offset in buffer at which to write the bytes.
            count : The maximum number of bytes to read. Fewer bytes are read if count is greater than the number of bytes in the input buffer.
         */



        // ReadData는 Queue이다. 시리얼포트의 인풋버퍼로 부터 읽어들인 데이터가 BUFFER배열에 저장되어 있고, 이것을 Queue에 Enqueue하는것이다.
        for (Int16 c = 0; c < size; c++) ReadData.Enqueue(BUFFER[c]); // Queue<byte> ReadData = new Queue<byte>(); 위에 선언한거 참조

        while (ReadData.Count > RxSize) // ReadData는 Queue이다. Count 메소드는 The number of elements contained in the Queue. 그리고 RxSize는 위에서 정의된 상수이다.
            {
            while (true) { if (ReadData.Dequeue() == 170) break; }
                // ReadData.Dequeue 매소드의 리턴값은 시리얼포트의 인풋버퍼로 부터 읽어들인 데이터를 버퍼에 저장한값을 Enqueue로 Queue에 넣은값이다. 왜 170과 비교하는가? 나중에 알아보자.
                /*
                Returns a single-precision floating point number converted from four bytes at a specified position in a byte array.

                public static float ToSingle (byte[] value, int startIndex);
                value : An array of bytes.
                startIndex : The starting position within value.

                from four bytes 라는것떄문에 바이트 4개를 원소로갖는 배열이 매개변수로 들어가는것인가? 정확하지는 않다.
                그리고 RxSize가 24인 이유가 yaw pitch roll flx0 flx1 flx2 이렇게 총 6개의 값이 각각 4bytes 라서 6*4=24 인것같다. 정확하지는 않다.
                */
            RxBuffer.yaw = BitConverter.ToSingle(
                new byte[4]{ReadData.Dequeue(), ReadData.Dequeue(), ReadData.Dequeue(), ReadData.Dequeue()},0); //매개변수는 배열과 0 으로 총 2개이다.
            RxBuffer.pitch = BitConverter.ToSingle(
                new byte[4]{ReadData.Dequeue(), ReadData.Dequeue(), ReadData.Dequeue(), ReadData.Dequeue()},0);
            RxBuffer.roll = BitConverter.ToSingle(
                new byte[4]{ReadData.Dequeue(), ReadData.Dequeue(), ReadData.Dequeue(), ReadData.Dequeue()},0);
            
            RxBuffer.flx0 = BitConverter.ToSingle(
                new byte[4]{ReadData.Dequeue(), ReadData.Dequeue(), ReadData.Dequeue(), ReadData.Dequeue()},0);
            RxBuffer.flx1 = BitConverter.ToSingle(
                new byte[4]{ReadData.Dequeue(), ReadData.Dequeue(), ReadData.Dequeue(), ReadData.Dequeue()},0);
            RxBuffer.flx2 = BitConverter.ToSingle(
                new byte[4]{ReadData.Dequeue(), ReadData.Dequeue(), ReadData.Dequeue(), ReadData.Dequeue()},0);
        }

        angle[0] = RxBuffer.yaw * 180.0f / 3.141592f; // Radian Degree 변환
        angle[1] = RxBuffer.pitch * 180.0f / 3.141592f;
        angle[2] = RxBuffer.roll * 180.0f / 3.141592f;
        }

        print(String.Format("{0}, {1}, {2}, {3}, {4}, {5}", angle[0], angle[1], angle[2], RxBuffer.flx0, RxBuffer.flx1, RxBuffer.flx2));

        wrist.transform.rotation = Quaternion.Euler(-angle[2], angle[0], -angle[1]);


        //손가락 관절 회전
        index1.transform.localRotation = Quaternion.Euler(- RxBuffer.flx1 / 3, 0, 0);
        index2.transform.localRotation = Quaternion.Euler(- RxBuffer.flx1 / 3, 0, 0);
        index3.transform.localRotation = Quaternion.Euler(- RxBuffer.flx1 / 3, 0, 0); 
        midfg1.transform.localRotation = Quaternion.Euler(- RxBuffer.flx2 / 3, 0, 0);
        midfg2.transform.localRotation = Quaternion.Euler(- RxBuffer.flx2 / 3, 0, 0);
        midfg3.transform.localRotation = Quaternion.Euler(- RxBuffer.flx2 / 3, 0, 0); 
        thumb2.transform.localRotation = Quaternion.Euler(- RxBuffer.flx0 / 2, 0, 0);
        thumb3.transform.localRotation = Quaternion.Euler(- RxBuffer.flx0 / 2, 0, 0); 

        //TxBuffer.servoAct0 = 0xFF;
        //TxBuffer.servoAct1 = 0xFF;
        //TxBuffer.servoAct2 = 0xFF;
        SerialTx(TxBuffer); // 서보각도 전송
    }


    bool SerialTx(TxDATA tx)
    {
        if (!sp.IsOpen)
        {
            return false;
        }
        /*
         Writes data to the serial port output buffer.

        Writes a specified number of bytes to the serial port using data from a buffer.
        public void Write (byte[] buffer, int offset, int count);

        buffer : The byte array that contains the data to write to the port.
        offset : The zero-based byte offset in the buffer parameter at which to begin copying bytes to the port.
        count : The number of bytes to write.

        */
        sp.Write(new byte[1] {0xAA}, 0, 1); // 시작을 0xAA로 하는것같다.
        print(string.Format("tx : {0}, {1}, {2}", tx.servoAct0, tx.servoAct1, tx.servoAct2));
        sp.Write(System.BitConverter.GetBytes(tx.servoAct0), 0, 2); // 세번째 파라미터가 2인 이유는 TxDATA 구조체의 변수 servoAct0이 2bytes type인 short이기 떄문이라고 생각한다. 정확하지는 않다.
        sp.Write(System.BitConverter.GetBytes(tx.servoAct1), 0, 2);
        sp.Write(System.BitConverter.GetBytes(tx.servoAct2), 0, 2);
        
        return true;
    }
}
