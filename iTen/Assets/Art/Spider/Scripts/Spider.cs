﻿using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class Spider : MonoBehaviour {

    // 빛의 몬스터 상태
    public enum MonsterState
    {
        Idle,           // 대기 상태
        Patrol,         // 정찰 상태
        Chase,          // 추적 상태
        Attack,         // 공격 상태
        Run            // 도망 상태
    }

    [Header("Components")]
    [SerializeField] private NavMeshAgent agent;            // NavMesh 사용
    [SerializeField] private Transform player;              // Player위치 캐싱

    [Header("Settings")]
    [SerializeField] private float attackRange = 2.0f;        // 공격 범위
    [SerializeField] private float detectionRange = 10.0f;    // 감지 범위
    [SerializeField] private float patrolSpeed = 2.0f;        // 정찰 속도
    [SerializeField] private float chaseSpeed = 5.0f;         // 추적 속도
    [SerializeField] private float RandAnRunRadius = 5.0f;    // 도망가거나 랜덤으로 잡을 때 반경
    [SerializeField] private float fieldOfViewAngle = 360.0f;  // 시야각

    private MonsterState currentState;                             // 플레이어 상태

    private GameSystem gameSystem;
    public Animator spider;
    private IEnumerator coroutine;


    //void Update () {
    //    if (Input.GetKey(KeyCode.Alpha1))
    //    {
    //        spider.SetBool("idle", true);
    //        spider.SetBool("running", false);
    //        spider.SetBool("walking", false);
    //        spider.SetBool("attack", false);
    //        spider.SetBool("jumping", false);
    //    }
    //    if (Input.GetKey("up"))
    //    {
    //        spider.SetBool("running", true);
    //        spider.SetBool("idle", false);
    //        spider.SetBool("walking", false);
    //        spider.SetBool("turnright", false);
    //        spider.SetBool("turnleft", false);
    //    }
    //    if (Input.GetKey("down"))
    //    {
    //        spider.SetBool("running", false);
    //        spider.SetBool("walking", true);
    //        spider.SetBool("idle", false);
    //        spider.SetBool("turnleft", false);
    //        spider.SetBool("turnright", false);
    //    }
    //    if (Input.GetKey(KeyCode.Alpha2))
    //    {
    //        spider.SetBool("attack", true);
    //        spider.SetBool("walking", false);
    //        spider.SetBool("idle", false);
    //        spider.SetBool("running", false);
    //        StartCoroutine("idle");
    //        idle();
    //    }
    //    if (Input.GetKey(KeyCode.Alpha3))
    //    {
    //        spider.SetBool("attack2", true);
    //        spider.SetBool("attack", false);
    //        spider.SetBool("idle", false);
    //        spider.SetBool("running", false);
    //        StartCoroutine("idle2");
    //        idle2();
    //    }
    //    if (Input.GetKey(KeyCode.Space))
    //    {
    //        spider.SetBool("idle", false);
    //        spider.SetBool("jumping", true);
    //    }
    //    if (Input.GetKey(KeyCode.Alpha5))
    //    {
    //        spider.SetBool("idle", false);
    //        spider.SetBool("hited", true);
    //        StartCoroutine("idle");
    //        idle();
    //    }
    //    if (Input.GetKey(KeyCode.Alpha6))
    //    {
    //        spider.SetBool("idle", false);
    //        spider.SetBool("died", true);
    //    }
    //    if (Input.GetKey("left"))
    //    {
    //        spider.SetBool("turnleft", true);
    //        spider.SetBool("walking", false);
    //        spider.SetBool("turnright", false);
    //        spider.SetBool("idle", false);
    //        spider.SetBool("running", false);
    //        StartCoroutine("idle2");
    //        idle2();
    //    }
    //    if (Input.GetKey("right"))
    //    {
    //        spider.SetBool("turnright", true);
    //        spider.SetBool("walking", false);
    //        spider.SetBool("turnleft", false);
    //        spider.SetBool("idle", false);
    //        spider.SetBool("running", false);
    //        StartCoroutine("idle2");
    //        idle2();
    //    }
    //}

    IEnumerator idle()
    {
        yield return new WaitForSeconds(0.35f);
        spider.SetBool("attack", false);
        spider.SetBool("attack2", false);
        spider.SetBool("idle", true);
        spider.SetBool("hited", false);
    }
    IEnumerator idle2()
    {
        yield return new WaitForSeconds(1.0f);
        spider.SetBool("attack", false);
        spider.SetBool("attack2", false);
        spider.SetBool("idle", true);
        spider.SetBool("turnleft", false);
        spider.SetBool("turnright", false);
    }


    // 이건 그냥 재미로
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
        // 주기적으로 시야를 체크
        // 만약 최적화가 필요하다면
        // Time.time을 써서 시간별로 체크하는 것이 좋음
        CheckPlayerDistance();

        // 상태 머신
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
        // 거리 계산
        float distanceToPlayer = (player.position - transform.position).sqrMagnitude;
        float sqrDetectionRange = detectionRange * detectionRange;

        // 상태별 거리 체크
        switch (currentState)
        {
            case MonsterState.Chase:
                if (distanceToPlayer <= attackRange * attackRange)
                {
                    ChangeState(MonsterState.Attack);
                }
                else if (gameSystem.IsSafeZone || gameSystem.IsFlashlightOn) // 도망 조건 추가
                {
                    ChangeState(MonsterState.Run);
                }
                break;
            case MonsterState.Run:
                if (distanceToPlayer > detectionRange * detectionRange * 4.0f) // 도망 상태에서는 더 먼 거리에서 Idle로 전환
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
        spider.SetBool("idle", true);
        spider.SetBool("running", false);
        spider.SetBool("walking", false);
        spider.SetBool("attack", false);
        spider.SetBool("jumping", false);

        // 대기 상태
        agent.speed = patrolSpeed;

        // remaingDistance는 목적지까지의 거리가 얼마나 남았는지에 대한 여부이다.
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            SetRandomPosition();
        }

        // 어두운 곳에서 플레이어 감지 시 추적 상태로 전환
        if (!gameSystem.IsSafeZone && !gameSystem.IsFlashlightOn && CanSeePlayer())
        {
            ChangeState(MonsterState.Chase);
        }
    }

    private void PatrolState()
    {
        // 순찰 상태
        // 무언가 대기상태랑 비슷하다.
        IdleState();

        // 어두운 곳에서 플레이어 감지 시 추적 상태로 전환
        if (!gameSystem.IsSafeZone && !gameSystem.IsFlashlightOn && CanSeePlayer())
        {
            ChangeState(MonsterState.Chase);
        }
    }

    private void ChaseState()
    {
        // 추적 상태 로직
        agent.speed = chaseSpeed;
        agent.SetDestination(player.position);
    }

    private float attackDelay = 0.0f;
    private float attackInterval = 1.0f;            // 1초마다 때리기
    private void AttackState()
    {
        attackDelay += Time.deltaTime;
        if (attackDelay >= attackInterval)
        {
            // 공격 상태 로직
            agent.ResetPath();

            // 플레이어로 바라보기
            transform.LookAt(player);

            // 공격 애니메이션을 추가하거나 피해 로직을 추가하면 됨
            spider.SetBool("attack", true);
            spider.SetBool("walking", false);
            spider.SetBool("idle", false);
            spider.SetBool("running", false);
            StartCoroutine("idle");
            idle();

            // 그리고 어택딜레이는 여기서 추가해도 됨
            Debug.Log("플레이어 공격!");
            // 일단 Dotween으로 그냥 애니메이션 간단하게 재생
            //this.transform.DOPunchScale(Vector3.one, 0.5f);
            ChangeState(MonsterState.Chase);
            attackDelay = 0.0f;
        }
    }

    private void RunState()
    {
        spider.SetBool("running", true);
        spider.SetBool("idle", false);
        spider.SetBool("walking", false);
        spider.SetBool("turnright", false);
        spider.SetBool("turnleft", false);

        agent.speed = chaseSpeed;

        // 도망갈 때 런 포지션을 따로 잡아놓는 것으로 함
        if (!agent.hasPath || agent.remainingDistance < 0.5f)
        {
            SetRunPosition();
        }
    }

    private bool CanSeePlayer()
    {
        if (player == null) return false;

        // 몬스터에서 플레이어까지의 방향 벡터와 거리 계산
        Vector3 directionToPlayer = player.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        // 플레이어가 감지 범위 내에 있는지 확인
        if (distanceToPlayer > detectionRange)
        {
            return false;
        }

        // 시야각 내에 있는지 확인
        float dotProduct = Vector3.Dot(transform.forward, directionToPlayer.normalized);
        if (dotProduct < Mathf.Cos(fieldOfViewAngle * 0.5f * Mathf.Deg2Rad))
        {
            return false;
        }

        // 이 조건을 뚫고 오면 플레이어를 찾은거
        Debug.Log("플레이어 찾음");
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
        // SamplePosition은 내가 가려고 하는 위치가 NavMesh에 포함이 되는지 확인하고 있으면
        // 유효한 위치를 반환하게 됨
        if (NavMesh.SamplePosition(fleePosition, out hit, RandAnRunRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    private void ChangeState(MonsterState state)
    {
        if (currentState != state)
        {
            // 상태가 바뀔 때마다
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
