using UnityEngine;


// 車輛類別
public class Car : MonoBehaviour
{
    // 車輛的速度
    public float speed = 10f;

    // 車輛的方向：1表示向前，-1表示向後
    private int direction = -1;

    public float DestroyTime=20f;
    
    public Vector3 rot = new Vector3(0,-90,0);
    void Start()
    {
        transform.rotation = Quaternion.Euler(rot);
        // 開始時，設定車輛向前移動
        Move();
        Destroy(gameObject,DestroyTime);
    }

    void Update()
    {
        // 每幀更新車輛的位置
        Move();
    }

    // 移動車輛
    private void Move()
    {
        // 設定車輛的移動方向和速度
        transform.Translate(Vector3.forward * direction * speed * Time.deltaTime);
        
    }
}

