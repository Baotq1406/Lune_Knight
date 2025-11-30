using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeAttack : MonoBehaviour
{
    [SerializeField] private int _attackDamage = 25;

    private void Start()
    {
        UIManager.Instance.ShowAttackText(_attackDamage);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(CONSTANT.ENEMY_ORC_TAG))
        {
            Debug.LogError("AxeAttack hit enemy");
            // xu ly sat thuong o day
            GameManager.Instance.EnemyMelee.TakeDamage(_attackDamage);
            GameManager.Instance.Player.GainSoul(1);
        }

        if (collision.CompareTag(CONSTANT.ENEMY_ARCHER_TAG))
        {
            Debug.LogError("AxeAttack hit enemy");
            // xu ly sat thuong o day
            GameManager.Instance.RangedEnemy.TakeDamage(_attackDamage);
            GameManager.Instance.Player.GainSoul(1);
        }
    }

    public void PlusDam()
    {
        if (GameManager.Instance.UpgradePoints <= 0)
            return;
        GameManager.Instance.AddUpgradePoint(-1);
        _attackDamage += GameManager.Instance.DamageUpgradeCost;
        UIManager.Instance.ShowAttackText(_attackDamage);
    }
}
