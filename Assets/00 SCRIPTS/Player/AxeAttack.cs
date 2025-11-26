using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeAttack : MonoBehaviour
{
    [SerializeField] private int _attackDamage = 25;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(CONSTANT.ENEMY_ORC_TAG))
        {
            Debug.LogError("AxeAttack hit enemy");
            // xu ly sat thuong o day
            GameManager.Instance.EnemyMelee.TakeDamage(_attackDamage);
        }

        if (collision.CompareTag(CONSTANT.ENEMY_ARCHER_TAG))
        {
            Debug.LogError("AxeAttack hit enemy");
            // xu ly sat thuong o day
            GameManager.Instance.RangedEnemy.TakeDamage(_attackDamage);
        }
    }
}
