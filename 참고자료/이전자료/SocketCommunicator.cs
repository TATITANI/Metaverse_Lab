using System.Collections;
using System.Collections.Generic;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

[Serializable] public class pointLandmarks
{
    [SerializeField] public double x;
    [SerializeField] public double y;
    [SerializeField] public double z;

    public pointLandmarks(double _x, double _y, double _z, double _visibility)
    {
        this.x = _x;
        this.y = _y;
        this.z = _z;
    }
}

[Serializable] public class Serialization<T>
{
    [SerializeField] List<T> target;
    public List<T> ToList() { return target; }

    public Serialization(List<T> target)
    {
        this.target = target;
    }
}

public class SocketCommunicator : MonoBehaviour
{
    public static List<pointLandmarks> points;

    public List<pointLandmarks>getList()
    {
        return points;
    }

    public static void StartClient()
    {
        // Data buffer for incoming data.  
        byte[] bytes = new byte[8192];

        // Connect to a remote device.  
        try
        {
            // Establish the remote endpoint for the socket.  
            // This example uses port 11000 on the local computer.  
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, 9999);

            // Create a TCP/IP  socket.  
            Socket sender = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            // Connect the socket to the remote endpoint. Catch any errors.  
            try
            {
                sender.Connect(remoteEP);

                print(String.Format("Socket connected to {0}",
                    sender.RemoteEndPoint.ToString()));

                // Encode the data string into a byte array.  
                byte[] msg = Encoding.ASCII.GetBytes("This is a test<EOF>");

                // Send the data through the socket.  
                int bytesSent = sender.Send(msg);

                // Receive the response from the remote device.  
                int bytesRec = sender.Receive(bytes);
                bytesRec = sender.Receive(bytes);

                string jsonData = Encoding.ASCII.GetString(bytes, 0, bytesRec);
                points = JsonUtility.FromJson<Serialization<pointLandmarks>>(jsonData).ToList();
                // Release the socket.  
                sender.Shutdown(SocketShutdown.Both);
                sender.Close();
            }
            catch (ArgumentNullException ane)
            {
                print(String.Format("ArgumentNullException : {0}", ane.ToString()));
            }
            catch (SocketException se)
            {
                print(String.Format("SocketException : {0}", se.ToString()));
            }
            catch (Exception e)
            {
                print(String.Format("Unexpected exception : s{0}", e.ToString()));
            }

        }
        catch (Exception e)
        {
            print(e.ToString());
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        print("Try...\n");
        StartClient();
    }
}
