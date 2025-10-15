
using UnityEngine;
using System;

public class YRotationTracker : MonoBehaviour
{
    public GameObject target; // The target object that will receive the Y rotation

    // Event to notify subscribers about the Y-axis rotation change
    public static event Action<float> OnYRotationChanged;

    void Update()
    {
        // Get the current rotation of the object
        Vector3 rotation = transform.eulerAngles;
        
        // Extract Y-axis rotation
        float yRotation = rotation.y;

        // Print Y-axis rotation value
        //Debug.Log("y Rotation: " + yRotation);

        // Notify subscribers about the Y-axis rotation
        OnYRotationChanged?.Invoke(yRotation);
    }
}


