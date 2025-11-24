using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventReceiver : MonoBehaviour
{
    //[SerializeField] EnemyMelee _enemyMelee;
    //[SerializeField] RangedEnemy _rangedEnemy;

    public void MeleeAttack()
    {
        //_enemyMelee.DamagePlayer();
        GameManager.Instance.EnemyMelee.DamagePlayer();
    }

    public void RangedAttack()
    {
        //_rangedEnemy.ShootArrow();
        GameManager.Instance.RangedEnemy.ShootArrow();
    }
}
