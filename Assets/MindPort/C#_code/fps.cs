/*
using UnityEngine;
using TMPro;
using System.IO.Ports;

public class ArduinoDataDisplay : MonoBehaviour
{
    public TextMeshProUGUI dataText;
    SerialPort serialPort;
    string portName = "COM4";
    int baudRate = 9600; 

    void Start()
    {

        serialPort = new SerialPort(portName, baudRate);
        serialPort.Open();
    }

    void Update()
    {
        if (serialPort.IsOpen && serialPort.BytesToRead > 0)
        {
            string data = serialPort.ReadLine();
            dataText.text = "Arduino data:" + data;
        }
    }

    void OnApplicationQuit()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
        }
    }
}
*/
using UnityEngine;
using TMPro;
using System.IO.Ports;

public class ArduinoDataDisplay : MonoBehaviour
{
    public TextMeshProUGUI dataText; // 用于显示Arduino数据的TMP文本
    SerialPort serialPort; // 串口对象
    string portName = "COM5"; // Arduino所连接的串口号
    int baudRate = 9600; // 串口波特率

    void Start()
    {
        // 初始化串口
        serialPort = new SerialPort(portName, baudRate);
        serialPort.Open(); // 打开串口
    }

    void Update()
    {
        if (serialPort.IsOpen && serialPort.BytesToRead > 0)
        {
            // 读取串口数据并显示在TMP文本上
            string data = serialPort.ReadLine();
            dataText.text = "Arduino data:" + data;
        }
    }

    void OnApplicationQuit()
    {
        // 关闭串口连接
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
        }
    }
}
