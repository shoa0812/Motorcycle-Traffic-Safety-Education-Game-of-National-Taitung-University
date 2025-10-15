using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class StraightWalker : MonoBehaviour
{
    public float moveDistance = 20f;
    public float waitTime = 2f;
    public float walkSpeed = 2.0f;
    public float desiredSpeed = 1.6f;

    public Transform modelRoot; 

    private NavMeshAgent agent;
    private Animator animator;
    private float timer;
    private bool isTurning = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        animator.applyRootMotion = false;

        agent.speed = 2.0f;
        agent.acceleration = 8f;
        agent.angularSpeed = 120f;

        timer = waitTime;

        StartCoroutine(DelayedStart());
    }

    IEnumerator DelayedStart()
    {
        yield return null;
        SetForwardDestination();
    }

    void Update()
    {
        float speed = agent.velocity.magnitude;
        animator.SetFloat("Speed", speed);

        if (!isTurning)
        {
            animator.speed = speed / desiredSpeed;
        }

        animator.SetBool("IsTurning", isTurning);

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f && !isTurning)
            {
                float randomAngle = Random.Range(-90f, 90f);
                StartCoroutine(PlayTurnAnimationThenRotate(randomAngle));
                timer = waitTime;
            }
        }

        SnapToGround();
    }

    void LateUpdate()
    {
        if (agent.isOnNavMesh && modelRoot != null)
        {
            // ✅ 對齊模型位置到 NavMeshAgent
            modelRoot.position = agent.nextPosition;

            // ✅ 面向移動方向（自然轉向）
            if (agent.velocity.sqrMagnitude > 0.01f)
            {
                Quaternion lookRot = Quaternion.LookRotation(agent.velocity.normalized);
                modelRoot.rotation = Quaternion.Slerp(modelRoot.rotation, lookRot, 0.2f);
            }
        }
    }

    IEnumerator PlayTurnAnimationThenRotate(float angle)
    {
        isTurning = true;
        animator.SetBool("IsTurning", true);

        // 等待動畫時間（視實際動畫調整）
        yield return new WaitForSeconds(0.8f);

        transform.Rotate(Vector3.up, angle);
        SetForwardDestination();

        isTurning = false;
        animator.SetBool("IsTurning", false);
    }

    void SetForwardDestination()
    {
        Vector3 forwardPoint = transform.position + transform.forward * moveDistance;

        if (NavMesh.SamplePosition(forwardPoint, out NavMeshHit hit, 2f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
        else
        {
            transform.Rotate(Vector3.up, 180f);
        }
    }

    void SnapToGround()
    {
        if (NavMesh.SamplePosition(transform.position, out NavMeshHit groundHit, 2f, NavMesh.AllAreas))
        {
            transform.position = new Vector3(
                transform.position.x,
                groundHit.position.y,
                transform.position.z
            );
        }
    }
}
