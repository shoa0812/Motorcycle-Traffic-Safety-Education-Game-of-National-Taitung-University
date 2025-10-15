// using UnityEngine;
// using System.Collections;
// using System.Net;
// using System.Net.Sockets;
// using System.Text;

// public class honda_Light : MonoBehaviour
// {
//     public Light LeftIndicator; // 左方向灯
//     public Light RightIndicator; // 右方向灯

//     bool isLeftBlinking = false; // 左方向灯是否正在闪烁
//     bool isRightBlinking = false; // 右方向灯是否正在闪烁

//     Coroutine leftBlinkingCoroutine;
//     Coroutine rightBlinkingCoroutine;

//     UdpClient udpClient;
//     IPEndPoint remoteEndPoint;
//     string esp8266IP = "172.20.10.2"; // ESP8266 的 IP 地址
//     int esp8266Port = 12345; // ESP8266 监听的端口号

//     void Start()
//     {
//         udpClient = new UdpClient();
//         remoteEndPoint = new IPEndPoint(IPAddress.Parse(esp8266IP), esp8266Port);

//         LeftIndicator.enabled = false;
//         RightIndicator.enabled = false;

//         LeftIndicator.intensity = 0f;
//         RightIndicator.intensity = 0f;
//     }

//     void Update()
//     {
//         if (Input.GetKeyDown(KeyCode.Q))
//         {
//             if (!isLeftBlinking)
//             {
//                 leftBlinkingCoroutine = StartCoroutine(BlinkLight(LeftIndicator, "LeftOn", "LeftOff"));
//                 isLeftBlinking = true;
//             }
//             else
//             {
//                 StopCoroutine(leftBlinkingCoroutine);
//                 LeftIndicator.enabled = false;
//                 isLeftBlinking = false;
//                 SendUdpMessage("LeftOff");
//             }
//         }

//         if (Input.GetKeyDown(KeyCode.E))
//         {
//             if (!isRightBlinking)
//             {
//                 rightBlinkingCoroutine = StartCoroutine(BlinkLight(RightIndicator, "RightOn", "RightOff"));
//                 isRightBlinking = true;
//             }
//             else
//             {
//                 StopCoroutine(rightBlinkingCoroutine);
//                 RightIndicator.enabled = false;
//                 isRightBlinking = false;
//                 SendUdpMessage("RightOff");
//             }
//         }
//     }

//     IEnumerator BlinkLight(Light indicator, string onMessage, string offMessage)
//     {
//         while (true)
//         {
//             indicator.enabled = true;
//             indicator.intensity = 2.5f;
//             SendUdpMessage(onMessage);
//             yield return new WaitForSeconds(0.5f);

//             indicator.intensity = 0f;
//             indicator.enabled = false;
//             SendUdpMessage(offMessage);
//             yield return new WaitForSeconds(0.5f);
//         }
//     }

//     void SendUdpMessage(string message)
//     {
//         try
//         {
//             byte[] data = Encoding.UTF8.GetBytes(message);
//             udpClient.Send(data, data.Length, remoteEndPoint);
//             Debug.Log("UDP message sent: " + message);
//         }
//         catch (System.Exception e)
//         {
//             Debug.LogError("Error sending UDP message: " + e.Message);
//         }
//     }
// }


using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class honda_Light : MonoBehaviour
{
    public Light LeftIndicator; // 左方向燈
    public Light RightIndicator; // 右方向燈

    bool isLeftBlinking = false; // 左方向燈是否正在閃爍
    bool isRightBlinking = false; // 右方向燈是否正在閃爍

    Coroutine leftBlinkingCoroutine;
    Coroutine rightBlinkingCoroutine;

    UdpClient udpClient;
    IPEndPoint remoteEndPoint;
    string esp8266IP = "172.20.10.2"; // ESP8266 的 IP 地址
    int esp8266Port = 12345; // ESP8266 監聽的埠號

    void Start()
    {
        udpClient = new UdpClient();
        remoteEndPoint = new IPEndPoint(IPAddress.Parse(esp8266IP), esp8266Port);

        LeftIndicator.enabled = false;
        RightIndicator.enabled = false;

        LeftIndicator.intensity = 0f;
        RightIndicator.intensity = 0f;
    }

    void Update()
    {
        // 控制左方向燈
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (!isLeftBlinking)
            {
                if (isRightBlinking)
                {
                    StopCoroutine(rightBlinkingCoroutine);
                    RightIndicator.enabled = false;
                    isRightBlinking = false;
                    SendUdpMessage("RightOff");
                }

                leftBlinkingCoroutine = StartCoroutine(BlinkLight(LeftIndicator, "LeftOn", "LeftOff"));
                isLeftBlinking = true;
            }
            else
            {
                StopCoroutine(leftBlinkingCoroutine);
                LeftIndicator.enabled = false;
                isLeftBlinking = false;
                SendUdpMessage("LeftOff");
            }
        }

        // 控制右方向燈
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!isRightBlinking)
            {
                if (isLeftBlinking)
                {
                    StopCoroutine(leftBlinkingCoroutine);
                    LeftIndicator.enabled = false;
                    isLeftBlinking = false;
                    SendUdpMessage("LeftOff");
                }

                rightBlinkingCoroutine = StartCoroutine(BlinkLight(RightIndicator, "RightOn", "RightOff"));
                isRightBlinking = true;
            }
            else
            {
                StopCoroutine(rightBlinkingCoroutine);
                RightIndicator.enabled = false;
                isRightBlinking = false;
                SendUdpMessage("RightOff");
            }
        }
    }

    IEnumerator BlinkLight(Light indicator, string onMessage, string offMessage)
    {
        while (true)
        {
            indicator.enabled = true;
            indicator.intensity = 2.5f;
            SendUdpMessage(onMessage);
            yield return new WaitForSeconds(0.5f);

            indicator.intensity = 0f;
            indicator.enabled = false;
            SendUdpMessage(offMessage);
            yield return new WaitForSeconds(0.5f);
        }
    }

    void SendUdpMessage(string message)
    {
        try
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            udpClient.Send(data, data.Length, remoteEndPoint);
            Debug.Log("UDP 訊息已發送: " + message);
        }
        catch (System.Exception e)
        {
            Debug.LogError("發送 UDP 訊息錯誤: " + e.Message);
        }
    }
}

