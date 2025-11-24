using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemy : MonoBehaviour
{
    [Header("Attack Parameters")]
    [SerializeField] private float attackCooldown;
    [SerializeField] private float _range;
    [SerializeField] private int damage;

    [Header("Collider Parameters")]
    [SerializeField] private float _colliderDistance;
    [SerializeField] private BoxCollider2D boxCollider;

    [Header("Player Layer")]
    [SerializeField] private LayerMask playerLayer;
    private float cooldownTimer = Mathf.Infinity;

    #region References
    [Header("References")]
    [SerializeField] private Animator _anim; // animator cua enemy
    [SerializeField] private EnemyPatrol enemyPatrol; // tham chieu toi script patrol
    #endregion

    private void Update()
    {
        cooldownTimer += Time.deltaTime;

        //Attack only when player in sight?
        if (PlayerInSight())
        {
            if (cooldownTimer >= attackCooldown)
            {
                cooldownTimer = 0;
                _anim.SetTrigger(CONSTANT.RANGED_ATTACK);
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

        //if (hit.collider != null)
        //{
        //    Debug.LogError("Player Detected!"); // co the hien debug khi phat hien player
        //}
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
}
