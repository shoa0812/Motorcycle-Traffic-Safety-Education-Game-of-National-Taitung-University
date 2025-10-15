using UnityEngine;
using System;
using System.IO.Ports;

public class YRotationTrackerNew : MonoBehaviour
{
    public GameObject target; // The target object that will receive the Y rotation

    // Event to notify subscribers about the Y-axis rotation change
    public static event Action<float> OnYRotationChanged;

    private SerialPort serialPort;
    private float lastYRotation = -1f; // Initialize to a value that is outside the expected range

    void Start()
    {
        // Initialize the serial port
        serialPort = new SerialPort("COM2", 9600); // Change "COM3" to the appropriate port
        serialPort.Open();
    }

    void Update()
    {
        // Get the current rotation of the object
        Vector3 rotation = transform.eulerAngles;
        
        // Extract Y-axis rotation
        float yRotation = rotation.y;

        // Check if the rotation has changed significantly
        if (Mathf.Abs(yRotation - lastYRotation) > Mathf.Epsilon)
        {
            // Update lastYRotation
            lastYRotation = yRotation;

            // Notify subscribers about the Y-axis rotation
            OnYRotationChanged?.Invoke(yRotation);

            // Send the Y rotation to Arduino
            if (serialPort.IsOpen)
            {
                serialPort.WriteLine(yRotation.ToString());
            }
        }
    }

    void OnDestroy()
    {
        // Close the serial port when the script is destroyed
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
        }
    }
}
