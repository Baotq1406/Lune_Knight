using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMelee : MonoBehaviour
{
    #region Attack Parameters
    [Header("Attack Settings")]
    private float _attackCooldown = 0; // thoi gian cooldown giua cac dot tan cong
    [SerializeField] private float _range = 2f; // khoang cach tan cong
    [SerializeField] private int _damage = 10; // luong sat thuong
    //private bool _isAttacking = false; // trang thai dang attack
    #endregion

    #region Collider Parameters
    [Header("Collider Settings")]
    [SerializeField] private float _colliderDistance = 0.5f;
    [SerializeField] private BoxCollider2D _boxCollider;
    [SerializeField] private LayerMask _playerLayer;
    private float _cooldownTimer = Mathf.Infinity;
    #endregion

    #region References
    [Header("References")]
    [SerializeField] private Animator _anim;
    [SerializeField] private EnemyPatrol enemyPatrol;
    private Rigidbody2D _rigi;
    #endregion

    [Header("Knockback Settings")]
    [SerializeField] private float _knockbackForce = 3f;
    [SerializeField] private float _knockbackDuration = 0.2f;
    private bool _isKnockback = false;

    [SerializeField] private int _enemyHealth = 100;
    private bool _isDead = false;

    private void Awake()
    {
        _rigi = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        //Debug.Log("Player dead: " + GameManager.Instance.Player.isDeadPlayer);
        if (_isDead) return;

        if (GameManager.Instance.Player.isDeadPlayer)
            return;

        _cooldownTimer += Time.deltaTime;

        // Chi tan cong khi player o trong tam nhin va khong dang attack hoac knockback
        if (PlayerInSight() && !_isKnockback)
        {
            if (_cooldownTimer >= _attackCooldown)
            {
                _cooldownTimer = 0;
                _anim.SetTrigger(CONSTANT.MELEE_ATTACK); // kich hoat animation attack
            }
        }

        // Tat patrol khi player o trong tam nhin hoac knockback hoac da chet
        if (enemyPatrol != null)
            enemyPatrol.enabled = !PlayerInSight() && !_isKnockback && !_isDead;
    }

    // Kiem tra player co trong tam nhin khong (chi tinh theo facing)
    private bool PlayerInSight()
    {
        Vector2 facingDir = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
        RaycastHit2D hit = Physics2D.BoxCast(
            _boxCollider.bounds.center + (Vector3)(facingDir * _range * _colliderDistance),
            new Vector3(_boxCollider.bounds.size.x * _range, _boxCollider.bounds.size.y, _boxCollider.bounds.size.z),
            0, Vector2.zero, 0, _playerLayer);

        return hit.collider != null;
    }

    // Ve hinh hinh chu nhat cua tam nhin tren editor de de quan sat
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(
            _boxCollider.bounds.center + transform.right * _range * this.transform.localScale.x * _colliderDistance,
            new Vector3(_boxCollider.bounds.size.x * _range, _boxCollider.bounds.size.y, _boxCollider.bounds.size.z));
    }

    // Giam mau player
    public void DamagePlayer()
    {
        if (PlayerInSight())
        {
            GameManager.Instance.Player.TakeDamage(_damage, this.transform);
        }
    }

    // Nhận sát thương
    public void TakeDamage(int damage)
    {
        if (_isDead) return;

        _enemyHealth -= damage;
        _anim.SetTrigger(CONSTANT.MELEE_HURT);

        StartCoroutine(DoKnockback());

        if (_enemyHealth <= 0)
        {
            _isDead = true;
            StartCoroutine(Die());
        }
    }

    // Knockback
    private IEnumerator DoKnockback()
    {
        _isKnockback = true;

        float dir = transform.position.x < GameManager.Instance.Player.transform.position.x ? -1 : 1;
        float timer = 0f;

        while (timer < _knockbackDuration)
        {
            _rigi.velocity = new Vector2(dir * _knockbackForce, _rigi.velocity.y);
            timer += Time.deltaTime;
            yield return null;
        }

        _rigi.velocity = new Vector2(0, _rigi.velocity.y);
        _isKnockback = false;
    }
    // Enemy chet
    private IEnumerator Die()
    {
        _anim.SetTrigger(CONSTANT.MELEE_DEATH);
        yield return new WaitForSeconds(0.8f);
        this.transform.parent.gameObject.SetActive(false);
    }
}
