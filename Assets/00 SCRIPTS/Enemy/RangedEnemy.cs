using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemy : MonoBehaviour
{
    #region Attack Parameters
    [Header("Attack Parameters")]
    [SerializeField] private float _attackCooldown; // thoi gian cooldown giua cac dot tan cong
    [SerializeField] private float _range; // khoang cach tan cong
    [SerializeField] private Transform _firePoint; // diem ban mui ten
    [SerializeField] private ArrowBase _arrowPrefab; // prefab mui ten
    #endregion

    #region Collider Parameters
    [Header("Collider Parameters")]
    [SerializeField] private float _colliderDistance; // khoang cach collider so voi enemy
    [SerializeField] private BoxCollider2D boxCollider; // collider cua enemy
    #endregion

    #region Player Layer
    [Header("Player Layer")]
    [SerializeField] private LayerMask playerLayer; // layer cua player
    private float cooldownTimer = Mathf.Infinity; // bo dem cooldown
    #endregion

    #region References
    [Header("References")]
    [SerializeField] private Animator _anim; // animator cua enemy
    [SerializeField] private EnemyPatrol enemyPatrol; // tham chieu toi script patrol
    Rigidbody2D _rigi; // rigidbody cua enemy
    #endregion

    [Header("Knockback Settings")]
    [SerializeField] private float _knockbackForce = 3f;
    [SerializeField] private float _knockbackDuration = 0.2f;
    private bool _isKnockback = false;

    [SerializeField] private int _enemyHealth = 100;
    private bool _isDead = false;

    private void Start()
    {
        _rigi = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        // dung het logic neu enemy da chet
        if (_isDead) return;

        cooldownTimer += Time.deltaTime;

        if (GameManager.Instance.Player.isDeadPlayer)
            return;

        // Keep fire point aligned with facing
        AlignFirePointToFacing();

        //Attack only when player in sight and not knockback
        if (PlayerInSight() && !_isKnockback)
        {
            if (cooldownTimer >= _attackCooldown)
            {
                cooldownTimer = 0;
                _anim.SetTrigger(CONSTANT.RANGED_ATTACK);
            }
        }

        // tat/enemy patrol khi player o trong tam nhin
        if (enemyPatrol != null)
            enemyPatrol.enabled = !PlayerInSight() && !_isKnockback && !_isDead;
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
        if (boxCollider == null) return;
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

    // === DAMAGE / KNOCKBACK / DEATH ===
    public void TakeDamage(int damage)
    {
        if (_isDead) return;

        _enemyHealth -= damage;
        _anim.SetTrigger(CONSTANT.RANGED_HURT);

        StartCoroutine(DoKnockback());

        if (_enemyHealth <= 0)
        {
            _isDead = true;
            // ngat tan cong ngay lap tuc
            _anim.ResetTrigger(CONSTANT.RANGED_ATTACK);
            StartCoroutine(Die());
        }
    }

    private IEnumerator DoKnockback()
    {
        _isKnockback = true;

        // day ra xa player
        Transform player = GameManager.Instance.Player.transform;
        float dir = transform.position.x < player.position.x ? -1f : 1f;
        float timer = 0f;

        while (timer < _knockbackDuration)
        {
            if (_rigi != null)
                _rigi.velocity = new Vector2(dir * _knockbackForce, _rigi.velocity.y);
            timer += Time.deltaTime;
            yield return null;
        }

        if (_rigi != null)
            _rigi.velocity = new Vector2(0, _rigi.velocity.y);
        _isKnockback = false;
    }

    private IEnumerator Die()
    {
        _anim.SetTrigger(CONSTANT.RANGED_DEATH);
        yield return new WaitForSeconds(0.8f);
        if (this.transform.parent != null)
            this.transform.parent.gameObject.SetActive(false);
        else
            this.gameObject.SetActive(false);
    }
}
                            