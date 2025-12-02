using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventReceiver : MonoBehaviour
{
    #region Player Attack Events
    public void EnableAxeAttack()
    {
        GameManager.Instance.Player.EnableAxeAttack();
    }
    public void DisableAxeAttack()
    {
        GameManager.Instance.Player.DisableAxeAttack();
    }
    #endregion

    #region Enemy Attack Events
    public void TriggerMeleeAttack()
    {
        // T? ??ng l?y EnemyMelee t? cùng GameObject ho?c parent
        EnemyMelee enemyMelee = GetComponentInParent<EnemyMelee>();
        if (enemyMelee != null)
        {
            enemyMelee.DamagePlayer();
        }
        else
        {
            Debug.LogWarning("EnemyMelee không tìm th?y trên GameObject: " + gameObject.name);
        }
    }

    public void TriggerRangedAttack()
    {
        // T? ??ng l?y RangedEnemy t? cùng GameObject ho?c parent
        RangedEnemy rangedEnemy = GetComponentInParent<RangedEnemy>();
        if (rangedEnemy != null)
        {
            rangedEnemy.ShootArrow();
        }
        else
        {
            Debug.LogWarning("RangedEnemy không tìm th?y trên GameObject: " + gameObject.name);
        }
    }
    #endregion

    #region Boss Attack Events
    public void TriggerBossMeleeAttack()
    {
        BossController.Instance.DealDamageToPlayer();
    }

    public void TriggerBossRangedAttack()
    {
        BossController.Instance.ShootLaser();
    }
    #endregion
}
