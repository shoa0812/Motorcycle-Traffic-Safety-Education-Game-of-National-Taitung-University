using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TargetFollower : MonoBehaviour
{
    public Transform target;            // 目標物件
    private NavMeshAgent agent;         // NavMesh代理
    public float detectionRange = 20f;  // 檢測範圍
    public float stoppingDistance = 2f; // 停止距離
    public float speed = 20f;          // 物體的移動速度

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent component is missing on this object.");
        }

        // 設定停止距離與速度
        agent.stoppingDistance = stoppingDistance;
        agent.speed = speed; // 設定速度
    }

    void Update()
    {
        if (target != null && agent != null)
        {
            // 計算與目標之間的距離
            float distanceToTarget = Vector3.Distance(transform.position, target.position);

            // 如果目標在檢測範圍內，設定導航目標
            if (distanceToTarget <= detectionRange)
            {
                agent.SetDestination(target.position);
                Debug.Log("Chasing the target!");
            }
            else
            {
                // 如果目標超出範圍，可以停止或保持原位
                agent.ResetPath(); // 清除當前路徑
                Debug.Log("Target is out of range.");
            }
        }
        else
        {
            Debug.LogError("Target or NavMeshAgent is not assigned.");
        }
    }
}

