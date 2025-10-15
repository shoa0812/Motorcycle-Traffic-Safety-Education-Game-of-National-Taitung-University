using UnityEngine;
using System.IO;

public class CSVReader : MonoBehaviour
{
    public string csvFilePath = "C:/Users/304/Downloads/CoolTerm Capture_vibration.csv";

    // 在 Unity 编辑器中将目标对象分配给该字段
    public GameObject targetObject;

    private int currentLineIndex = 0;
    private float lastoffest=0;

    void Update()
    {
        // 获取目标对象的 Transform 组件
        Transform targetTransform = targetObject.transform;

        // 读取CSV文件的所有行
        string[] lines = File.ReadAllLines(csvFilePath);

        // 确保当前行索引在有效范围内
        if (currentLineIndex < lines.Length)
        {
            // 获取当前行的值
            string line = lines[currentLineIndex];
            
            // 将当前行的值转换为float
            if (float.TryParse(line, out float yValue))
            {
                // 将Y值应用到目标物体的Y位置上
                Vector3 newPosition = targetTransform.position;
                if(newPosition.y<=0){
                    newPosition.y=0;
                }
                newPosition.y += (float)(yValue*0.1)-lastoffest;
                lastoffest=(float)(yValue*0.1);
                targetTransform.position = newPosition;

                Debug.Log("Y Position set to: " + newPosition);
                // 增加当前行索引，以便下次更新时读取下一行的值
                currentLineIndex++;
                if (currentLineIndex >= lines.Length)
                {
                    currentLineIndex = 0;
                }
                
            
            }
            else
            {
                Debug.LogError("Invalid value in CSV file at line " + (currentLineIndex + 1));
            }
        }
        else
        {
            Debug.Log("Reached the end of CSV file.");
        }
    }
}
