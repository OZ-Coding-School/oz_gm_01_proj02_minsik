using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackController : MonoBehaviour
{
    public Transform targetToAttack;
    public Material idleStateMaterial;
    public Material followStateMaterial;
    public Material attackStateMaterial;
    public bool isPlayer;

    public int unitDamage;

    private List<Unit> enemiesInRange = new List<Unit>();

    private void Update()
    {
        CleanedEnemyDead();

        if (targetToAttack == null && enemiesInRange.Count > 0)
        {
            targetToAttack = GetClosestEnemy()?.transform;
        }

    }

    private Unit GetClosestEnemy()
    {
        Unit closet = null;
        float minDist = float.MaxValue;

        foreach (var enemy in enemiesInRange)
        {
            if (enemy == null) continue;

            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closet = enemy;
            }
        }
        return closet;
    }


    private void CleanedEnemyDead()
    {
        enemiesInRange.RemoveAll(e => e == null);

        if (targetToAttack == null) return;

        if (!enemiesInRange.Exists(e => e.transform == targetToAttack))
        {
            targetToAttack = null;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (!isPlayer) return;
        
        Unit unit = other.GetComponent<Unit>();
        if (unit != null && !enemiesInRange.Contains(unit))
        {
            enemiesInRange.Add(unit);
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        Unit unit = other.GetComponent<Unit>();
        if (unit == null) return;

        enemiesInRange.Remove(unit);

        if (targetToAttack == unit.transform)
        {
            targetToAttack = null;
        }
    }

    public void SetIdleMaterial()
    {
        GetComponent<Renderer>().material = idleStateMaterial;
    }

    public void SetFollowMaterial()
    {
        GetComponent<Renderer>().material = followStateMaterial;
    }

    public void SetAttackMaterial()
    {
        GetComponent<Renderer>().material = attackStateMaterial;
    }

    private void OnDrawGizmos()
    {
        //추적
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 10.0f*0.2f);

        //공격
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 1.0f);

        //공격멈춤
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, 1.2f);
    }
}
