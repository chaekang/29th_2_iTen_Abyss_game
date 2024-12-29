using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public enum MonsterState
{
    Watch,
    Walk,
    Run,
    Attack
}

public class SoundMonster : MonoBehaviour
{
    private Animator animator;
    private MonsterState currentState;

    private NavMeshAgent agent;
    private Vector3? currentTarget;
    private bool isChasing = false;

    private float wanderTimer = 0f;
    private float wanderInterval = 20f; // �������� �ȴ� ����

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        SetWalkingState(0); // Idle �ʱ� ����
        StartCoroutine(WanderRandomly());
    }

    public void OnSoundHeard(Vector3 soundPos)
    {
        currentTarget = soundPos;

        if (!isChasing)
        {
            StartChasing();
        }
    }

    private void StartChasing()
    {
        if (currentTarget.HasValue)
        {
            agent.SetDestination(currentTarget.Value);
            isChasing = true;
            currentState = MonsterState.Run;
            animator.SetInteger("isWalking", 1);
            StopCoroutine(WanderRandomly());
            StartCoroutine(UpdateChasingTarget());
        }
    }

    private IEnumerator UpdateChasingTarget()
    {
        while (isChasing)
        {
            if (currentTarget.HasValue)
            {
                agent.SetDestination(currentTarget.Value);

                if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
                {
                    StopChasing();
                }
            }
            yield return new WaitForSeconds(0.2f);
        }
    }

    private void StopChasing()
    {
        isChasing = false;
        currentTarget = null;
        SetWalkingState(0);
        StartCoroutine(WanderRandomly());
    }

    public void SetWalkingState(int walkingState)
    {
        animator.SetInteger("isWalking", walkingState);

        if (walkingState == 1 && currentState != MonsterState.Run)
        {
            StartCoroutine(SwitchToRunAfterIdle());
        }
        else if (walkingState == 0)
        {
            currentState = MonsterState.Walk;
        }
    }

    private IEnumerator SwitchToRunAfterIdle()
    {
        currentState = MonsterState.Watch;
        animator.SetInteger("isWalking", 0); // ��� Idle ���� ����
        yield return new WaitForSeconds(0.2f); // ��� ���� ��
        animator.SetInteger("isWalking", 1);   // Run ���·� ��ȯ
        currentState = MonsterState.Run;
    }

    private IEnumerator WanderRandomly()
    {
        while (!isChasing) // ���� ���� �ƴ� ���� ���� �ȱ� ����
        {
            wanderTimer += Time.deltaTime;
            if (wanderTimer >= wanderInterval)
            {
                Vector3 randomDirection = Random.insideUnitSphere * 30f; // ���� ���� ����
                randomDirection += transform.position;

                if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, 10f, NavMesh.AllAreas))
                {
                    agent.SetDestination(hit.position);
                    animator.SetInteger("isWalking", 1); // �ȴ� �ִϸ��̼� Ȱ��ȭ
                    currentState = MonsterState.Walk;
                }

                wanderTimer = 0f;
            }
            yield return null;
        }
    }

    public void TriggerAttack()
    {
        animator.SetTrigger("isAttack");
        currentState = MonsterState.Attack;
    }

    public void TriggerWatch()
    {
        animator.SetTrigger("isWatching");
        currentState = MonsterState.Watch;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            SetWalkingState(0); // Walking
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            SetWalkingState(1); // Running
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            TriggerAttack(); // Attack
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            TriggerWatch(); // Watch
        }

        if (isChasing && currentTarget.HasValue)
        {
            Debug.Log($"Monster is chasing target at {currentTarget.Value}");
        }
    }
}
