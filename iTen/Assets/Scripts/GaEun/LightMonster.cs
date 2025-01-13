using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LightMonster : MonoBehaviour
{
    // ���� ���� ����
    public enum MonsterState
    {
        Idle,           // ��� ����
        Patrol,         // ���� ����
        Chase,          // ���� ����
        Attack,         // ���� ����
        Run            // ���� ����
    }

    [Header("Components")]
    [SerializeField] private NavMeshAgent agent;            // NavMesh ���
    [SerializeField] private Transform player;              // Player��ġ ĳ��

    [Header("Settings")]
    [SerializeField] private float attackRange = 2.0f;        // ���� ����
    [SerializeField] private float detectionRange = 10.0f;    // ���� ����
    [SerializeField] private float patrolSpeed = 2.0f;        // ���� �ӵ�
    [SerializeField] private float chaseSpeed = 5.0f;         // ���� �ӵ�
    [SerializeField] private float RandAnRunRadius = 5.0f;    // �������ų� �������� ���� �� �ݰ�
    [SerializeField] private float fieldOfViewAngle = 360.0f;  // �þ߰�

    private MonsterState currentState;                             // �÷��̾� ����

    private GameSystem gameSystem;

    // �̰� �׳� ��̷�
    //[SerializeField] private Color chaseColor;
    //[SerializeField] private Color normalColor;
    //[SerializeField] private MeshRenderer mesh;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        // mesh = GetComponent<MeshRenderer>();
    }

    private void Start()
    {
        gameSystem = GameSystem.Instance;
        ChangeState(MonsterState.Idle);
    }

    private void FixedUpdate()
    {
        // �ֱ������� �þ߸� üũ
        // ���� ����ȭ�� �ʿ��ϴٸ�
        // Time.time�� �Ἥ �ð����� üũ�ϴ� ���� ����
        CheckPlayerDistance();

        // ���� �ӽ�
        switch (currentState)
        {
            case MonsterState.Idle:
                IdleState();
                break;
            case MonsterState.Patrol:
                PatrolState();
                break;
            case MonsterState.Chase:
                ChaseState();
                break;
            case MonsterState.Attack:
                AttackState();
                break;
            case MonsterState.Run:
                RunState();
                break;
        }
    }

    private void CheckPlayerDistance()
    {
        // �Ÿ� ���
        float distanceToPlayer = (player.position - transform.position).sqrMagnitude;
        float sqrDetectionRange = detectionRange * detectionRange;

        // ���º� �Ÿ� üũ
        switch (currentState)
        {
            case MonsterState.Chase:
                if (distanceToPlayer <= attackRange * attackRange)
                {
                    ChangeState(MonsterState.Attack);
                }
                else if (gameSystem.IsSafeZone || gameSystem.IsFlashlightOn) // ���� ���� �߰�
                {
                    ChangeState(MonsterState.Run);
                }
                break;
            case MonsterState.Run:
                if (distanceToPlayer > detectionRange * detectionRange * 4.0f) // ���� ���¿����� �� �� �Ÿ����� Idle�� ��ȯ
                {
                    ChangeState(MonsterState.Idle);
                }
                break;
            default:
                if (distanceToPlayer <= sqrDetectionRange && CanSeePlayer())
                {
                    ChangeState(MonsterState.Chase);
                }
                break;
        }
    }

    private void IdleState()
    {
        // ��� ����
        agent.speed = patrolSpeed;

        // remaingDistance�� ������������ �Ÿ��� �󸶳� ���Ҵ����� ���� �����̴�.
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            SetRandomPosition();
        }

        // ��ο� ������ �÷��̾� ���� �� ���� ���·� ��ȯ
        if (!gameSystem.IsSafeZone && !gameSystem.IsFlashlightOn && CanSeePlayer())
        {
            ChangeState(MonsterState.Chase);
        }
    }

    private void PatrolState()
    {
        // ���� ����
        // ���� �����¶� ����ϴ�.
        IdleState();

        // ��ο� ������ �÷��̾� ���� �� ���� ���·� ��ȯ
        if (!gameSystem.IsSafeZone && !gameSystem.IsFlashlightOn && CanSeePlayer())
        {
            ChangeState(MonsterState.Chase);
        }
    }

    private void ChaseState()
    {
        // ���� ���� ����
        agent.speed = chaseSpeed;
        agent.SetDestination(player.position);
    }

    private float attackDelay = 0.0f;
    private float attackInterval = 1.0f;            // 1�ʸ��� ������
    private void AttackState()
    {
        attackDelay += Time.deltaTime;
        if (attackDelay >= attackInterval)
        {
            // ���� ���� ����
            agent.ResetPath();

            // �÷��̾�� �ٶ󺸱�
            transform.LookAt(player);

            // ���� �ִϸ��̼��� �߰��ϰų� ���� ������ �߰��ϸ� ��
            // �׸��� ���õ����̴� ���⼭ �߰��ص� ��
            Debug.Log("�÷��̾� ����!");
            // �ϴ� Dotween���� �׳� �ִϸ��̼� �����ϰ� ���
            //this.transform.DOPunchScale(Vector3.one, 0.5f);
            ChangeState(MonsterState.Chase);
            attackDelay = 0.0f;
        }
    }

    private void RunState()
    {
        agent.speed = chaseSpeed;

        // ������ �� �� �������� ���� ��Ƴ��� ������ ��
        if (!agent.hasPath || agent.remainingDistance < 0.5f)
        {
            SetRunPosition();
        }
    }

    private bool CanSeePlayer()
    {
        if (player == null) return false;

        // ���Ϳ��� �÷��̾������ ���� ���Ϳ� �Ÿ� ���
        Vector3 directionToPlayer = player.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        // �÷��̾ ���� ���� ���� �ִ��� Ȯ��
        if (distanceToPlayer > detectionRange)
        {
            return false;
        }

        // �þ߰� ���� �ִ��� Ȯ��
        float dotProduct = Vector3.Dot(transform.forward, directionToPlayer.normalized);
        if (dotProduct < Mathf.Cos(fieldOfViewAngle * 0.5f * Mathf.Deg2Rad))
        {
            return false;
        }

        // �� ������ �հ� ���� �÷��̾ ã����
        Debug.Log("�÷��̾� ã��");
        return true;
    }
    private void SetRandomPosition()
    {
        Vector3 randomDirection = Random.insideUnitSphere * RandAnRunRadius;
        randomDirection += transform.position;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, RandAnRunRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    private void SetRunPosition()
    {
        Vector3 fleeDirection = transform.position - player.position;
        Vector3 fleePosition = transform.position + fleeDirection.normalized * RandAnRunRadius;

        NavMeshHit hit;
        // SamplePosition�� ���� ������ �ϴ� ��ġ�� NavMesh�� ������ �Ǵ��� Ȯ���ϰ� ������
        // ��ȿ�� ��ġ�� ��ȯ�ϰ� ��
        if (NavMesh.SamplePosition(fleePosition, out hit, RandAnRunRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    private void ChangeState(MonsterState state)
    {
        if (currentState != state)
        {
            // ���°� �ٲ� ������
            //switch (currentState)
            //{
            //    case MonsterState.Chase:
            //        mesh.material.color = chaseColor;
            //        break;
            //    default:
            //        mesh.material.color = normalColor;
            //        break;
            //}
            currentState = state;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        float halfFOV = fieldOfViewAngle * 0.5f;
        Quaternion leftRayRotation = Quaternion.AngleAxis(-halfFOV, Vector3.up);
        Quaternion rightRayRotation = Quaternion.AngleAxis(halfFOV, Vector3.up);

        Vector3 leftRayDirection = leftRayRotation * transform.forward * detectionRange;
        Vector3 rightRayDirection = rightRayRotation * transform.forward * detectionRange;

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, leftRayDirection);
        Gizmos.DrawRay(transform.position, rightRayDirection);

        if (player != null)
        {
            float sqrDistanceToPlayer = (player.position - transform.position).sqrMagnitude;
            if (sqrDistanceToPlayer <= detectionRange * detectionRange)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, player.position);
            }
        }
    }
#endif
}
