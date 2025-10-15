// using System.Net.Sockets;
// using System.Text;
// using UnityEngine;

// public class UDPCommunication : MonoBehaviour
// {
//     UdpClient udpClient;
//     string ipAddress = "192.168.0.148"; // ESP8266的IP地址
//     int port = 4210; // ESP8266的UDP端口

//     public WheelCollider rearWheel;

//     void Start()
//     {
//         udpClient = new UdpClient();
//         InvokeRepeating("SendWheelData", 0f, 0.02f); // 每20毫秒發送一次數據
//     }

//     void SendWheelData()
//     {
//         float wheelRPM = rearWheel.rpm;

//         // 只要轮子在转动，就持续发送数据
//         if (Mathf.Abs(wheelRPM) > 0.1f) // 如果轮子转速大于0.1
//         {
//             string message = wheelRPM.ToString("F2");
//             byte[] data = Encoding.UTF8.GetBytes(message);
//             try
//             {
//                 udpClient.Send(data, data.Length, ipAddress, port);
//                 Debug.Log($"Wheel RPM sent: {wheelRPM}");
//             }
//             catch (SocketException e)
//             {
//                 Debug.LogError("SocketException: " + e.Message);
//             }
//         }
//     }
// }

//combine version

// using System.Net.Sockets;
// using System.Text;
// using UnityEngine;

// public class UDPCommunication : MonoBehaviour
// {
//     UdpClient udpClient;
//     string ipAddress = "192.168.0.148"; // ESP8266的IP地址
    
//     int port = 55555; // ESP8266的UDP端口 55555

//     public WheelCollider rearWheel;
//     private float lastWheelRPM = 0;
//     private bool isWheelStopped = false; // 记录轮子是否已经停止

//     void Start()
//     {
//         udpClient = new UdpClient();
//         InvokeRepeating("SendWheelData", 0f, 0.02f); // 每20毫秒发送一次数据
//     }

//     void SendWheelData()
//     {
//         float wheelRPM = rearWheel.rpm;

//         if (Mathf.Abs(wheelRPM - lastWheelRPM) > 1f)  // RPM变化大于1时发送数据
//         {
//             lastWheelRPM = wheelRPM;
//             string message = $"wheel:{wheelRPM:F2}";  // 明确指令类型 wheel
//             byte[] data = Encoding.UTF8.GetBytes(message);
//             try
//             {
//                 udpClient.Send(data, data.Length, ipAddress, port);
//                 Debug.Log($"Wheel RPM sent: {wheelRPM}");
//                 isWheelStopped = false;
//             }
//             catch (SocketException e)
//             {
//                 Debug.LogError("SocketException: " + e.Message);
//             }
//         }
//         else if (Mathf.Abs(wheelRPM) <= 0.1f && !isWheelStopped)  // 如果轮子停止，则发送停止指令
//         {
//             SendStopMessage();
//             isWheelStopped = true;  // 确保停止指令只发送一次
//         }
//     }

//     // 发送停止指令给Arduino
//     void SendStopMessage()
//     {
//         string stopMessage = "wheel:stop";  // 明确发送 stop 命令
//         byte[] data = Encoding.UTF8.GetBytes(stopMessage);
//         try
//         {
//             udpClient.Send(data, data.Length, ipAddress, port);
//             Debug.Log("Wheel stopped, stop message sent.");
//         }
//         catch (SocketException e)
//         {
//             Debug.LogError("SocketException: " + e.Message);
//         }
//     }

//     // 当应用暂停时，发送停止指令
//     void OnApplicationPause(bool isPaused)
//     {
//         if (isPaused)
//         {
//             Debug.Log("Application Paused. Sending stop message.");
//             SendStopMessage();  // 应用暂停时发送停止消息
//         }
//     }

//     // 当应用退出时，发送停止指令
//     void OnApplicationQuit()
//     {
//         Debug.Log("Application Quit. Sending stop message.");
//         SendStopMessage();  // 应用退出时发送停止消息
//         udpClient.Close();  // 关闭UDP连接
//     }
// }

// using System.Net.Sockets;
// using System.Text;
// using UnityEngine;

// public class UDPCommunication : MonoBehaviour
// {
//     UdpClient udpClient;
//     string ipAddress = "192.168.0.52"; // ESP8266的IP地址
//     int port = 55555; // ESP8266的UDP端口 55555

//     public WheelCollider rearWheel;
//     private float lastWheelRPM = 0;
//     private bool isWheelStopped = false; // 记录轮子是否已经停止

//     void Start()
//     {
//         udpClient = new UdpClient();
//         InvokeRepeating("SendWheelData", 0f, 0.02f); // 每20毫秒发送一次数据
//     }

//     void SendWheelData()
//     {
//         float wheelRPM = rearWheel.rpm;

//         if (Mathf.Abs(wheelRPM - lastWheelRPM) > 1f)  // RPM变化大于1时发送数据
//         {
//             lastWheelRPM = wheelRPM;
//             string message = $"wheel:{wheelRPM:F2}";  // 明确指令类型 wheel
//             byte[] data = Encoding.UTF8.GetBytes(message);
//             try
//             {
//                 udpClient.Send(data, data.Length, ipAddress, port);
//                 Debug.Log($"Wheel RPM sent: {wheelRPM}");
//                 isWheelStopped = false;
//             }
//             catch (SocketException e)
//             {
//                 Debug.LogError("SocketException: " + e.Message);
//             }
//         }
//         else if (Mathf.Abs(wheelRPM) <= 0.1f && !isWheelStopped)  // 如果轮子停止，则发送停止指令
//         {
//             SendStopMessage();
//             isWheelStopped = true;  // 确保停止指令只发送一次
//         }
//     }

//     // 发送停止指令给Arduino
//     void SendStopMessage()
//     {
//         string stopMessage = "wheel:stop";  // 明确发送 stop 命令
//         byte[] data = Encoding.UTF8.GetBytes(stopMessage);
//         try
//         {
//             udpClient.Send(data, data.Length, ipAddress, port);
//             Debug.Log("Wheel stopped, stop message sent.");
//         }
//         catch (SocketException e)
//         {
//             Debug.LogError("SocketException: " + e.Message);
//         }
//     }

//     // 当应用暂停时，发送停止指令
//     void OnApplicationPause(bool isPaused)
//     {
//         if (isPaused)
//         {
//             Debug.Log("Application Paused. Sending stop message.");
//             SendStopMessage();  // 应用暂停时发送停止消息
//         }
//     }

//     // 当应用退出时，发送停止指令
//     void OnApplicationQuit()
//     {
//         Debug.Log("Application Quit. Sending stop message.");
//         SendStopMessage();  // 应用退出时发送停止消息
//         udpClient.Close();  // 关闭UDP连接
//     }
// }

using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class UDPCommunication : MonoBehaviour
{
    UdpClient udpClient;
    string ipAddress = "192.168.0.141"; // ESP8266的IP地址
    int port = 55555; // ESP8266的UDP端口 55555

    public WheelCollider rearWheel;
    private float lastWheelRPM = 0;
    private bool isWheelStopped = false; // 记录轮子是否已经停止

    void Start()
    {
        udpClient = new UdpClient();
        InvokeRepeating("SendWheelData", 0f, 0.02f); // 每20毫秒发送一次数据
    }

    void SendWheelData()
    {
        float wheelRPM = rearWheel.rpm;

        if (Mathf.Abs(wheelRPM - lastWheelRPM) > 1f)  // RPM变化大于1时发送数据
        {
            lastWheelRPM = wheelRPM;
            string message = $"wheel:{wheelRPM:F2}";  // 明确指令类型 wheel
            byte[] data = Encoding.UTF8.GetBytes(message);
            try
            {
                udpClient.Send(data, data.Length, ipAddress, port);
                Debug.Log($"Wheel RPM sent: {wheelRPM}");
                isWheelStopped = false;
            }
            catch (SocketException e)
            {
                Debug.LogError("SocketException: " + e.Message);
            }
        }
        else if (Mathf.Abs(wheelRPM) <= 0.1f && !isWheelStopped)  // 如果轮子停止，则发送停止指令
        {
            SendStopMessage();
            isWheelStopped = true;  // 确保停止指令只发送一次
        }
    }

    // 发送停止指令给Arduino
    void SendStopMessage()
    {
        string stopMessage = "wheel:stop";  // 明确发送 stop 命令
        byte[] data = Encoding.UTF8.GetBytes(stopMessage);
        try
        {
            udpClient.Send(data, data.Length, ipAddress, port);
            Debug.Log("Wheel stopped, stop message sent.");
        }
        catch (SocketException e)
        {
            Debug.LogError("SocketException: " + e.Message);
        }
    }

    // 当应用暂停时，发送停止指令
    void OnApplicationPause(bool isPaused)
    {
        if (isPaused)
        {
            Debug.Log("Application Paused. Sending stop message.");
            SendStopMessage();  // 应用暂停时发送停止消息
        }
    }

    // 当应用退出时，发送停止指令
    void OnApplicationQuit()
    {
        Debug.Log("Application Quit. Sending stop message.");
        SendStopMessage();  // 应用退出时发送停止消息
        udpClient.Close();  // 关闭UDP连接
    }
}
