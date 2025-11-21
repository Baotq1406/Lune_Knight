using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMelee : MonoBehaviour
{
    #region Attack Parameters
    [Header("Attack Settings")]
    private float _attackCooldown; // thoi gian cooldown giua cac dot tan cong
    [SerializeField] private float _range; // khoang cach tan cong
    [SerializeField] private int _damage; // luong sat thuong
    #endregion

    #region Collider Parameters
    [Header("Collider Settings")]
    [SerializeField] private float _colliderDistance; // khoang cach collider so voi enemy
    [SerializeField] private BoxCollider2D boxCollider; // collider cua enemy
    [SerializeField] private LayerMask playerLayer; // layer cua player
    private float cooldownTimer = Mathf.Infinity; // bo dem cooldown
    #endregion

    #region References
    [Header("References")]
    [SerializeField] private Animator _anim; // animator cua enemy
    [SerializeField] private EnemyPatrol enemyPatrol; // tham chieu toi script patrol
    #endregion

    private void Update()
    {
        cooldownTimer += Time.deltaTime; // tang bo dem theo thoi gian

        // Tan cong khi player o trong tam nhin
        if (PlayerInSight())
        {
            if (cooldownTimer >= _attackCooldown)
            {
                cooldownTimer = 0; // reset bo dem
                _anim.SetTrigger("meleeAttack"); // kich hoat animation tan cong
            }
        }

        // tat/enemy patrol khi player o trong tam nhin
        if (enemyPatrol != null)
            enemyPatrol.enabled = !PlayerInSight();
    }

    // kiem tra xem player co trong tam nhin hay khong
    private bool PlayerInSight()
    {
        RaycastHit2D hit =
            Physics2D.BoxCast(
                boxCollider.bounds.center + transform.right * _range * this.transform.localScale.x * _colliderDistance,
                new Vector3(boxCollider.bounds.size.x * _range, boxCollider.bounds.size.y, boxCollider.bounds.size.z),
                0, Vector2.left, 0, playerLayer);

        if (hit.collider != null)
        {
            //Debug.LogError("Player Detected!"); // co the hien debug khi phat hien player
        }

        return hit.collider != null;
    }

    // Ve hinh hinh chu nhat cua tam nhin tren editor de de quan sat
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(
            boxCollider.bounds.center + transform.right * _range * this.transform.localScale.x * _colliderDistance,
            new Vector3(boxCollider.bounds.size.x * _range, boxCollider.bounds.size.y, boxCollider.bounds.size.z));
    }

    // Giam mau player neu player o trong tam nhin
    public void DamagePlayer()
    {
        if (PlayerInSight())
        {
            Debug.LogError("Player Damaged!"); // co the thay bang code giam mau thuc te
        }
    }
}
