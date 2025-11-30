using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMelee : MonoBehaviour
{
    #region Attack Parameters
    [Header("Attack Settings")]
    [SerializeField] private float _attackCooldown = 1.5f; // thoi gian cooldown giua cac dot tan cong (thay đổi từ 0 sang 1.5f)
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

    #region Knockback Settings
    [Header("Knockback Settings")]
    [SerializeField] private float _knockbackForce = 3f;
    [SerializeField] private float _knockbackDuration = 0.2f;
    private bool _isKnockback = false;
    #endregion

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
          if (_boxCollider == null)
          {
    //        Debug.LogWarning("BoxCollider is null on " + gameObject.name);
              return false;
          }

        Vector2 facingDir = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
        RaycastHit2D hit = Physics2D.BoxCast(
            _boxCollider.bounds.center + (Vector3)(facingDir * _range * _colliderDistance),
            new Vector3(_boxCollider.bounds.size.x * _range, _boxCollider.bounds.size.y, _boxCollider.bounds.size.z),
            0, Vector2.zero, 0, _playerLayer);

        // Debug để kiểm tra phát hiện
        //if (hit.collider != null)
        //{
        //    Debug.Log($"[{gameObject.name}] Detected: {hit.collider.name} on layer {LayerMask.LayerToName(hit.collider.gameObject.layer)}");
        //}

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
        Debug.LogError("Melee Enemy heal:" + _enemyHealth);
        _anim.SetTrigger(CONSTANT.MELEE_HURT);

        StartCoroutine(DoKnockback());

        if (_enemyHealth <= 0)
        {
            _isDead = true;
            GameManager.Instance.AddUpgradePoint(Random.Range(1, 3));
            StartCoroutine(Die());
        }
    }

    // Kiểm tra xem player có đang ở phía sau enemy không
    private bool IsPlayerBehind()
    {
        if (GameManager.Instance.Player == null) return false;

        float playerDirection = GameManager.Instance.Player.transform.position.x - transform.position.x;
        float enemyFacing = transform.localScale.x;

        // Player ở phía sau nếu:
        // - Enemy quay phải (scale.x > 0) và player ở bên trái (playerDirection < 0)
        // - Enemy quay trái (scale.x < 0) và player ở bên phải (playerDirection > 0)
        return (enemyFacing > 0 && playerDirection < 0) || (enemyFacing < 0 && playerDirection > 0);
    }

    // Knockback
    private IEnumerator DoKnockback()
    {
        _isKnockback = true;

        // Kiểm tra xem player có ở phía sau không trước khi knockback
        bool wasPlayerBehind = IsPlayerBehind();

        float dir = transform.position.x < GameManager.Instance.Player.transform.position.x ? -1 : 1;
        float timer = 0f;

        while (timer < _knockbackDuration)
        {
            _rigi.velocity = new Vector2(dir * _knockbackForce, _rigi.velocity.y);
            timer += Time.deltaTime;
            yield return null;
        }

        _rigi.velocity = new Vector2(0, _rigi.velocity.y);

        // CHỈ quay mặt về phía người chơi nếu bị tấn công từ phía sau
        if (wasPlayerBehind)
        {
            FacePlayer();
        }

        _isKnockback = false;
    }

    // Quay mặt enemy về phía người chơi
    private void FacePlayer()
    {
        if (GameManager.Instance.Player == null) return;

        float playerDirection = GameManager.Instance.Player.transform.position.x - transform.position.x;
        
        // Nếu player ở bên phải (playerDirection > 0) thì scale.x dương
        // Nếu player ở bên trái (playerDirection < 0) thì scale.x âm
        float newScaleX = playerDirection > 0 ? Mathf.Abs(transform.localScale.x) : -Mathf.Abs(transform.localScale.x);
        
        transform.localScale = new Vector3(newScaleX, transform.localScale.y, transform.localScale.z);
        
        // Cập nhật hướng trong EnemyPatrol để raycast bắn đúng hướng
        if (enemyPatrol != null)
        {
            enemyPatrol.UpdateFacingDirection();
        }
    }

    // Enemy chet
    private IEnumerator Die()
    {
        _anim.SetTrigger(CONSTANT.MELEE_DEATH);
        yield return new WaitForSeconds(0.8f);

        // tat doi tuong (uu tien tat parent neu co)
        if (this.transform.parent != null)
            this.transform.parent.gameObject.SetActive(false);
        else
            this.gameObject.SetActive(false);
    }
}
