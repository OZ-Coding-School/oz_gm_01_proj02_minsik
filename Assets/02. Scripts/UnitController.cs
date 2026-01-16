using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitController : MonoBehaviour
{
    [Header("Settings")]
    public LayerMask ground;
    public float patrolSpeed = 4.0f;

    public CommandMode currentCommand = CommandMode.None;
    [HideInInspector] public Transform targetToAttack;
    [HideInInspector] public Vector3 moveTarget; 


    NavMeshAgent agent;
    Camera cam;
    bool isPatrollingForward = true;
    private Vector3 patrolStart;
    private Vector3 patrolEnd;
    public bool isCommandedToMove;

    public AttackController attackController { get; private set; }
    public Animator anim { get; private set; }
    
    public bool hasExternalCommand;

    private void Start()
    {
        cam = Camera.main;
        agent = GetComponent<NavMeshAgent>();
        attackController = GetComponent<AttackController>();
        anim = GetComponent<Animator>();
        
    }

    private void Update()
    {
        

    // Debug.Log($"Agent pathPending: {agent.pathPending}, remainingDistance: {agent.remainingDistance}, isStopped: {agent.isStopped}");

        switch (currentCommand)
        {
            case CommandMode.Move:
                HandleMove();
                break;
            case CommandMode.Attack:
                HandleAttack();
                break;
            case CommandMode.AttackMove:
                HandleAttackMove();
                break;
            case CommandMode.Patrol:
                HandlePatrol();
                break;
            case CommandMode.Stop:
                HandleStop();
                break;
            case CommandMode.None:
            default:
                break;

        }
    }
     
    public void MoveTo(Vector3 destination)
    {

        hasExternalCommand = true;
        currentCommand = CommandMode.Move;

        
        moveTarget = destination;
        targetToAttack = null;

        anim.SetBool("isFollowing", false);
        anim.SetBool("isAttacking", false);

        agent.isStopped = false;
        agent.ResetPath();

        if (Vector3.Distance(transform.position, destination) > 0.05f)
        agent.SetDestination(destination);

        else
        {
            Vector3 offset = (destination - transform.position).normalized * 0.1f;
            agent.SetDestination(transform.position + offset);
        }


        

        // isCommandedToMove = true;
        // agent.isStopped = false;
        // agent.SetDestination(moveTarget);
    }


    public void DebugMoveTo(Vector3 destination)
{
    hasExternalCommand = true;
    currentCommand = CommandMode.Move;
    moveTarget = destination;

    agent.isStopped = false;
    agent.ResetPath();
    bool success = agent.SetDestination(destination);

    // SetDestination 결과 확인
    Debug.Log($"SetDestination called. Success: {success}");

    // 경로 계산 상태 확인
    Debug.Log($"PathPending: {agent.pathPending}");
    Debug.Log($"HasPath: {agent.hasPath}");
    Debug.Log($"PathStatus: {agent.pathStatus}");
    Debug.Log($"RemainingDistance: {agent.remainingDistance}");
    Debug.Log($"Destination: {destination}");
}
    private void HandleMove()
    {
        // if (!hasExternalCommand) return;
        // if (agent.pathPending) return;
        if (!hasExternalCommand) return;

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.01f)
        {
            hasExternalCommand = false;
            currentCommand = CommandMode.None;
        }  
    }

    private void HandleAttack()
    {
        if (!hasExternalCommand) return;
        if (targetToAttack == null)
        {
            hasExternalCommand = false;
            currentCommand = CommandMode.None;
            return; 
       }
        
        
    }

    private void HandlePatrol()
    {
        if (patrolStart == patrolEnd) return;

        Vector3 target = isPatrollingForward ? patrolEnd : patrolStart;

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            isPatrollingForward = !isPatrollingForward;
            target = isPatrollingForward ? patrolEnd : patrolStart;
        }

        agent.SetDestination(target);
    }

    public void SetPatrolPoints(Vector3 start, Vector3 end)
    {
        hasExternalCommand = true;

        patrolStart = start;
        patrolEnd = end;
        currentCommand = CommandMode.Patrol;
        isPatrollingForward = true;

        agent.SetDestination(patrolEnd);
    }

    private void HandleStop()
    {
        agent.ResetPath();
        hasExternalCommand = false;
        currentCommand = CommandMode.None;
    }

    public void SetCommandMode(CommandMode mode, Transform target = null)
    {
        currentCommand = mode;
        
        if (mode == CommandMode.Attack)
        {
                hasExternalCommand = true;
                targetToAttack = target;

                anim.SetBool("isFollowing", true);
        }

        else if (mode == CommandMode.AttackMove)
        {
            hasExternalCommand = true;
            targetToAttack = target;
        }    

        if (mode == CommandMode.Stop)
        {
            HandleStop();
        }
    }

    private void HandleAttackMove()
    {
        if (!hasExternalCommand) return;

    // 1. 우선 타겟이 있으면 추적
        if (targetToAttack != null)
        {
            anim.SetBool("isFollowing", true);
            agent.SetDestination(targetToAttack.position);
            return;
        }

        // 2. 없으면 목적지로 이동
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            hasExternalCommand = false;
            currentCommand = CommandMode.None;
        }
    }


}

