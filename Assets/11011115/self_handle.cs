// using UnityEngine;
// using System.Net;
// using System.Net.Sockets;
// using System.Text;

// public class ServoController : MonoBehaviour
// {
//     public Transform objectToRotate; // Unity对象的Transform

//     private UdpClient udpClient;
//     private string ipAddressString = "192.168.0.148"; // 替换为你的ESP8266的IP地址
//     private int port = 55555; // ESP8266接收数据的端口号
//     private float lastSentAngle = -1;

//     void Start()
//     {
//         udpClient = new UdpClient();
//         udpClient.Connect(ipAddressString, port); // 连接到UDP服务器
//     }

//     void Update()
//     {
        
//         float rotationZ = objectToRotate.localEulerAngles.z;

        
//         if (rotationZ > 180)
//         {
//             rotationZ -= 360;
//         }
        
//         Debug.Log("Object rotationZ: " + rotationZ);

//         float mappedAngle = Map(rotationZ, -20, 20, 180, 0);

        
//         if (Mathf.Abs(mappedAngle - lastSentAngle) > 0.01f)
//         {
//             lastSentAngle = mappedAngle;
//             Debug.Log("Sending mappedAngle: " + mappedAngle);
//             SendUDPMessage(mappedAngle.ToString("F2"));
//         }
//     }

//     float Map(float value, float fromSource, float toSource, float fromTarget, float toTarget)
//     {
//         return (value - fromSource) * (toTarget - fromTarget) / (toSource - fromSource) + fromTarget;
//     }

//     void SendUDPMessage(string message)
//     {
//         try
//         {
//             byte[] data = Encoding.UTF8.GetBytes(message);
//             udpClient.Send(data, data.Length);
//         }
//         catch (System.Exception e)
//         {
//             Debug.LogError("Error sending UDP message: " + e.Message);
//         }
//     }

//     void OnApplicationQuit()
//     {
//         // 关闭UDP连接
//         udpClient.Close();
//     }
// }


//combine version
// using UnityEngine;
// using System.Net;
// using System.Net.Sockets;
// using System.Text;

// public class ServoController : MonoBehaviour
// {
//     public Transform objectToRotate; // Unity对象的Transform

//     private UdpClient udpClient;
//     private string ipAddressString = "192.168.0.148"; // 替换为你的ESP8266的IP地址
//     private int port = 4210; // ESP8266接收数据的端口号
//     private float lastSentAngle = -1;

//     void Start()
//     {
//         udpClient = new UdpClient();
//         udpClient.Connect(ipAddressString, port); // 连接到UDP服务器
//     }

//     void Update()
//     {
        
//         float rotationZ = objectToRotate.localEulerAngles.z;

        
//         if (rotationZ > 180)
//         {
//             rotationZ -= 360;
//         }
        
//         //Debug.Log("Object rotationZ: " + rotationZ);

//         float mappedAngle = Map(rotationZ, -20, 20, 180, 0);

        
//         if (Mathf.Abs(mappedAngle - lastSentAngle) > 0.01f)
//         {
//             lastSentAngle = mappedAngle;
//             //Debug.Log("Sending mappedAngle: " + mappedAngle);
//             //SendUDPMessage(mappedAngle.ToString("F2"));
//             SendUDPMessage($"handle:{mappedAngle:F2}");//這行可以刪掉 如果換其他的
//         }
//     }

//     float Map(float value, float fromSource, float toSource, float fromTarget, float toTarget)
//     {
//         return (value - fromSource) * (toTarget - fromTarget) / (toSource - fromSource) + fromTarget;
//     }

//     void SendUDPMessage(string message)
//     {
//         try
//         {
//             byte[] data = Encoding.UTF8.GetBytes(message);
//             udpClient.Send(data, data.Length);
//         }
//         catch (System.Exception e)
//         {
//             Debug.LogError("Error sending UDP message: " + e.Message);
//         }
//     }

//     void OnApplicationQuit()
//     {
//         // 关闭UDP连接
//         udpClient.Close();
//     }
// }


using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class ServoController : MonoBehaviour
{
    public Transform objectToRotate; // Unity对象的Transform

    private UdpClient udpClient;
    private string ipAddressString = "192.168.0.141"; // 替换为你的ESP8266的IP地址
    private int port = 4210; // ESP8266接收数据的端口
    private float lastSentAngle = -1;

    void Start()
    {
        udpClient = new UdpClient();
        udpClient.Connect(ipAddressString, port); // 连接到UDP服务器
    }

    void Update()
    {
        
        float rotationZ = objectToRotate.localEulerAngles.z;

        
        if (rotationZ > 180)
        {
            rotationZ -= 360;
        }
        
        //Debug.Log("Object rotationZ: " + rotationZ);

        float mappedAngle = Map(rotationZ, -20, 20, 180, 0);

        
        if (Mathf.Abs(mappedAngle - lastSentAngle) > 0.01f)
        {
            lastSentAngle = mappedAngle;
            //Debug.Log("Sending mappedAngle: " + mappedAngle);
            //SendUDPMessage(mappedAngle.ToString("F2"));
            SendUDPMessage($"handle:{mappedAngle:F2}");//這行可以刪掉 如果換其他的
        }
    }

    float Map(float value, float fromSource, float toSource, float fromTarget, float toTarget)
    {
        return (value - fromSource) * (toTarget - fromTarget) / (toSource - fromSource) + fromTarget;
    }

    void SendUDPMessage(string message)
    {
        try
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            udpClient.Send(data, data.Length);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error sending UDP message: " + e.Message);
        }
    }

    void OnApplicationQuit()
    {
        // 关闭UDP连接
        udpClient.Close();
    }
}