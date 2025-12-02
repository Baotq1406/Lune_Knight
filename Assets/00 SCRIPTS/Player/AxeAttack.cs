using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeAttack : MonoBehaviour
{
    [SerializeField] private int _attackDamage = 25;

    private void Start()
    {
        _attackDamage = PlayerPrefs.GetInt(CONSTANT.ATTACK_DAMAGE, _attackDamage);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(CONSTANT.ENEMY_ORC_TAG))
        {
            Debug.LogError("AxeAttack hit enemy");
            // Lay component truc tiep tu collision object
            EnemyMelee enemyMelee = collision.GetComponentInParent<EnemyMelee>();
            if (enemyMelee != null)
            {
                enemyMelee.TakeDamage(_attackDamage);
                GameManager.Instance.Player.GainSoul(1);
            }
        }

        if (collision.CompareTag(CONSTANT.ENEMY_ARCHER_TAG))
        {
            Debug.LogError("AxeAttack hit enemy");
            // Lay component truc tiep tu collision object (tim ca parent va children)
            RangedEnemy rangedEnemy = collision.GetComponentInParent<RangedEnemy>();
            if (rangedEnemy != null)
            {
                rangedEnemy.TakeDamage(_attackDamage);
                GameManager.Instance.Player.GainSoul(1);
            }
        }

        if (collision.CompareTag(CONSTANT.ENEMY_BOSS_TAG))
        {
            Debug.LogError("AxeAttack hit BOSS");
            // xu ly sat thuong o day
            BossController.Instance.TakeDamage(_attackDamage, GameManager.Instance.Player.transform);
            GameManager.Instance.Player.GainSoul(1);
        }
    }

    public void PlusDam()
    {
        if (GameManager.Instance.UpgradePoints <= 0)
            return;
        GameManager.Instance.AddUpgradePoint(-1);
        _attackDamage += GameManager.Instance.DamageUpgradeCost;
        PlayerPrefs.SetInt(CONSTANT.ATTACK_DAMAGE, _attackDamage);
        UIManager.Instance.ShowAttackText(_attackDamage);
    }
}
