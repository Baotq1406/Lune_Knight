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
        GameManager.Instance.EnemyMelee.DamagePlayer();
    }

    public void TriggerRangedAttack()
    {
        GameManager.Instance.RangedEnemy.ShootArrow();
    }
    #endregion
}
