using UnityEngine;

public class AutoMove : MonoBehaviour
{
    public float moveSpeed = 3f; // 移动速度

    private void Update()
    {
        // 在物体的本地坐标系中向前移动
        transform.Translate(-Vector3.forward * moveSpeed * Time.deltaTime);
    }
}