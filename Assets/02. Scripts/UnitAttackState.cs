using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.PostProcessing;

public class UnitAttackState : StateMachineBehaviour
{
    NavMeshAgent agent;
    AttackController attackController;
    UnitController unitController;
    public float stopAttackingDistance = 1.2f;

    public float attackRate = 2.0f;
    private float attackTimer;


    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
        attackController = animator.GetComponent<AttackController>();
        unitController = animator.GetComponent<UnitController>();
        attackController.SetAttackMaterial();
        attackController.muzzleEffect.gameObject.SetActive(true);
        attackTimer = 0.0f;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (attackController.targetToAttack == null || unitController.hasExternalCommand)
        {
            animator.SetBool("isAttacking", false);
            return;
        }
        
            float distance = Vector3.Distance(animator.transform.position, attackController.targetToAttack.position);
                if (distance > stopAttackingDistance)
                {
                    animator.SetBool("isAttacking", false);
                }
                    LookAtTarget(animator.transform);

            if (attackTimer <= 0)
            {
                Attack();
                attackTimer = 1.0f/ attackRate;
            }
            else
            {
                attackTimer -= Time.deltaTime;    
            }

            
    } 
    

    
    private void Attack()
    {

        SoundManager.Instance.PlayInfantryAttackSound();
        var damageToInflict = attackController.unitDamage;
        attackController.targetToAttack.GetComponent<Unit>().TakeDamage(damageToInflict);
    }

    private void LookAtTarget(Transform self)
    {
          Vector3 dir = attackController.targetToAttack.position - self.position;
        dir.y = 0f;

        if (dir.sqrMagnitude > 0.001f)
        {
            self.rotation = Quaternion.LookRotation(dir);
        }

    }
    


    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        attackController.muzzleEffect.gameObject.SetActive(false);
    }
}
    
    

