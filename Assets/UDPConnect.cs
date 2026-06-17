using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

using UnityEngine;

public class UDPConnect : MonoBehaviour
{
    public int listenPort = 5005;
    UdpClient udp;
    IPEndPoint remoteEndPoint;

    public bool hasNewData = false;
    public string latestMessage = "";

    public float latestX, latestY, latestZ, latestRoll, latestPitch, latestYaw;
    

    void Start()
    {
        udp = new UdpClient(listenPort);
        remoteEndPoint = new IPEndPoint(IPAddress.Any, listenPort);
        Debug.Log("UDP Server started on port " + listenPort);
    }
    public void GetMsg()
    {
        if(udp.Available > 0)
        {
            byte[] data = udp.Receive(ref remoteEndPoint);
            string message = Encoding.UTF8.GetString(data);
            Debug.Log("Received: " + message);
            if(message != latestMessage)
            {
                Debug.Log("New UDP message received: " + message);
                latestMessage = message;
                DataProcessor(latestMessage);
                hasNewData = true;
            }
            else
            {
                Debug.Log("Received UDP message is the same as the last one, ignoring.");
            }
        }
        else
        {
            Debug.LogWarning("No UDP data available to read.");
        }
    }
    public void DataProcessor(string msg)
    {
        string[] parts = msg.Split(',');
        if(parts.Length != 6)
        {
            Debug.LogError("Received data does not contain 6 parts: " + msg);
            return;
        }

        float x_temp = float.Parse(parts[0]);
        float y_temp = float.Parse(parts[1]);
        float z_temp = float.Parse(parts[2]);
        float roll_temp = float.Parse(parts[3]);
        float pitch_temp = float.Parse(parts[4]);
        float yaw_temp = float.Parse(parts[5]);

        latestX = x_temp;
        latestY = y_temp;
        latestZ = z_temp;
        latestRoll = roll_temp;
        latestPitch = pitch_temp;
        latestYaw = yaw_temp;
    }
    void OnApplicationQuit()
    {
        if (udp != null)
        {
            udp.Close();
        }
    }
}
