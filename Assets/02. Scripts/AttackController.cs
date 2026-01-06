using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackController : MonoBehaviour
{
    public Transform tragetToAttack;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") && tragetToAttack == null)
        {
            tragetToAttack = other.transform;
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy") && tragetToAttack != null)
        {
            tragetToAttack = null;
        }
    }
}
