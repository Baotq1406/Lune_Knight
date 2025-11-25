using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventReceiver : MonoBehaviour
{
    [SerializeField] EnemyMelee _enemyMelee;

    public void MeleeAttack()
    {
        _enemyMelee.DamagePlayer();
    }
}
