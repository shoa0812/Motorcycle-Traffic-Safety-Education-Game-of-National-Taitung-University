using UnityEngine;

public class SyncZRotation : MonoBehaviour
{
    public Transform targetObject;  // 要同步的目標物件

    void Update()
    {
        if (targetObject != null)
        {
            // 取得目標物件的當前 z 軸旋轉
            float targetZRotation = targetObject.rotation.eulerAngles.z;

            // 設定當前物件的 z 軸旋轉與目標一致
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, targetZRotation);
        }
    }
}

