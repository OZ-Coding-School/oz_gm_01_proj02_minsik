using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitFollowState : StateMachineBehaviour
{
    AttackController attackController;
    UnitController unitController;
    NavMeshAgent agent;
    public float attackingDistance = 1.0f;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        attackController = animator.transform.GetComponent<AttackController>();
        agent = animator.transform.GetComponent<NavMeshAgent>();
        unitController = animator.GetComponent<UnitController>();
        attackController.SetFollowMaterial();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (attackController.targetToAttack == null)
        {
            animator.SetBool("isFollowing", false);
            return;
        }
        
        
        if (unitController.hasExternalCommand)
            return;

         Vector3 targetPos = attackController.targetToAttack.position;

        if (!agent.pathPending)
        {
            agent.SetDestination(targetPos);
        }    
        float distance = Vector3.Distance(animator.transform.position, targetPos);

        if (distance <= attackingDistance)
        {
            animator.SetBool("isAttacking", true);
        }

       
    }


    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // agent.SetDestination(animator.transform.position);
        // agent.isStopped = true;     // 완전히 멈춤
        // agent.ResetPath();   
    }




}
