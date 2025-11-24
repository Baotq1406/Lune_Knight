using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemy : MonoBehaviour
{
    #region Attack Parameters
    [Header("Attack Parameters")]
    [SerializeField] private float _attackCooldown;
    [SerializeField] private float _range;
    [SerializeField] private int _damage;
    [SerializeField] private Transform _firePoint;
    [SerializeField] private ArrowBase _arrowPrefab;
    #endregion

    #region Collider Parameters
    [Header("Collider Parameters")]
    [SerializeField] private float _colliderDistance;
    [SerializeField] private BoxCollider2D boxCollider;
    #endregion

    #region Player Layer
    [Header("Player Layer")]
    [SerializeField] private LayerMask playerLayer;
    private float cooldownTimer = Mathf.Infinity;
    #endregion

    #region References
    [Header("References")]
    [SerializeField] private Animator _anim; // animator cua enemy
    [SerializeField] private EnemyPatrol enemyPatrol; // tham chieu toi script patrol
    #endregion

    private void Update()
    {
        cooldownTimer += Time.deltaTime;

        // Keep fire point aligned with facing
        AlignFirePointToFacing();

        //Attack only when player in sight?
        if (PlayerInSight())
        {
            if (cooldownTimer >= _attackCooldown)
            {
                cooldownTimer = 0;
                _anim.SetTrigger(CONSTANT.RANGED_ATTACK);
            }
        }

        // tat/enemy patrol khi player o trong tam nhin
        if (enemyPatrol != null)
            enemyPatrol.enabled = !PlayerInSight();
    }

    // Make _firePoint face the same direction as the enemy (based on localScale.x)
    private void AlignFirePointToFacing()
    {
        if (_firePoint == null) return;

        // Right when scale.x > 0, Left when scale.x < 0
        // Setting the 'right' vector directly adjusts the rotation accordingly.
        _firePoint.right = Vector3.right * Mathf.Sign(transform.localScale.x);
    }

    // kiem tra xem player co trong tam nhin hay khong
    private bool PlayerInSight()
    {
        RaycastHit2D hit =
            Physics2D.BoxCast(
                boxCollider.bounds.center + transform.right * _range * this.transform.localScale.x * _colliderDistance,
                new Vector3(boxCollider.bounds.size.x * _range, boxCollider.bounds.size.y, boxCollider.bounds.size.z),
                0, Vector2.left, 0, playerLayer);

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

    public void ShootArrow()
    {
        if (_arrowPrefab == null || _firePoint == null) return;

        // Spawn arrow aligned with fire point's rotation (which we keep in sync with facing)
        //Instantiate(_arrowPrefab, _firePoint.position, _firePoint.rotation);

        // Su dung Object Pooling
        GameObject arrows = ObjectPoolingX.Instance.GetObject(_arrowPrefab.gameObject);
        arrows.transform.position = _firePoint.position; 
        arrows.transform.rotation = _firePoint.rotation;    
        arrows.SetActive(true);

        //Debug.Log("Shoot Arrow");
    }
}
