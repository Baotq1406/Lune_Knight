using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : Singleton<BossController>
{
    #region Movement Settings
    [Header("Movement Settings")]
    [SerializeField] private float _moveSpeed = 3.0f;
    [SerializeField] private float _stopDistance = 2f; // khoang cach dung lai gan player
    #endregion

    #region Detection Settings
    [Header("Detection Settings")]
    [SerializeField] private float _detectionRange = 10f; // tam phat hien player
    [SerializeField] private BoxCollider2D _boxCollider; // collider de BoxCast
    [SerializeField] private LayerMask _playerLayer; // layer cua player
    [SerializeField] private float _colliderDistance = 0.5f; // khoang cach offset BoxCast
    #endregion

    #region Melee Attack Settings
    [Header("Melee Attack Settings")]
    [SerializeField] private float _attackRange = 2f; // tam danh cua melee attack
    [SerializeField] private float _attackColliderDistance = 0.5f; // khoang cach offset BoxCast tan cong
    [SerializeField] private float _attackCooldown = 2f; // thoi gian cooldown giua cac dot tan cong
    [SerializeField] private float _attackDuration = 1f; // thoi gian animation tan cong
    [SerializeField] private int _damage = 20; // luong sat thuong
    private float _attackCooldownTimer = 0f; // bo dem cooldown
    private bool _isAttackingMelee = false; // co khoa state khi dang tan cong
    #endregion

    #region Melee Second Attack Settings
    [Header("Melee Second Attack Settings")]
    [SerializeField] private float _attackRange_2 = 2f; // tam danh cua melee attack
    [SerializeField] private float _attackColliderDistance_2 = 0.5f; // khoang cach offset BoxCast tan cong
    [SerializeField] private float _attackCooldown_2 = 2f; // thoi gian cooldown giua cac dot tan cong
    [SerializeField] private float _attackDuration_2 = 1f; // thoi gian animation tan cong
    [SerializeField] private int _damage_2 = 20; // luong sat thuong
    private float _attackCooldownTimer_2 = 0f; // bo dem cooldown
    private bool _isAttackingMelee_2 = false; // co khoa state khi dang tan cong
    #endregion

    #region Ranged Attack Settings
    [Header("Ranged Attack Settings")]
    [SerializeField] private float _rangedAttackRange = 6f; // tam tan cong tam xa
    [SerializeField] private float _rangedAttackColliderDistance = 0.5f; // khoang cach offset BoxCast tan cong tam xa
    [SerializeField] private float _rangedAttackCooldown = 3f; // thoi gian cooldown cho ranged attack
    [SerializeField] private float _rangedAttackDuration = 1.5f; // thoi gian animation ranged attack
    private float _rangedAttackCooldownTimer = 0f; // bo dem cooldown ranged attack
    private bool _isAttackingRange = false;
    [SerializeField] private GameObject _laserPrefab; // prefab laser
    [SerializeField] private Transform _laserFirePoint; // diem ban laser
    #endregion

    #region Hurt and Knockback Settings
    [Header("Hurt and Knockback Settings")]
    [SerializeField] private float _knockbackForce = 5f; // luc day lui khi bi tan cong
    [SerializeField] private float _knockbackDuration = 0.3f; // thoi gian knockback
    [SerializeField] private float _hurtDuration = 0.5f; // thoi gian animation hurt
    #endregion

    #region Death Settings
    [Header("Death Settings")]
    [SerializeField] private float _deathDuration = 2f; // thoi gian animation chet
    #endregion

    [SerializeField] private int _healthBoss = 1000;
    private bool _isHurt = false;
    [SerializeField] private bool _isDead = false;

    #region State
    [Header("State")]
    [SerializeField] private BossState _currentBossState = BossState.IDLE;
    public BossState currentBossState => _currentBossState;
    #endregion

    #region References
    [Header("References")]
    [SerializeField] private AnimationControllerBase _anim;
    [SerializeField] private Animator _animator;
    private Rigidbody2D _rigi;
    private Transform _player;
    private bool _playerDetected = false; // co danh dau da phat hien player
    #endregion

    void Start()
    {
        _rigi = this.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Giam cooldown timer
        if (_attackCooldownTimer > 0)
            _attackCooldownTimer -= Time.deltaTime;
        
        if (_attackCooldownTimer_2 > 0)
            _attackCooldownTimer_2 -= Time.deltaTime;
        
        if (_rangedAttackCooldownTimer > 0)
            _rangedAttackCooldownTimer -= Time.deltaTime;


        if (GameManager.Instance.Player.isDeadPlayer)
            return;

        UpdateState(); // cap nhat trang thai boss

        if (_isDead) return; // neu da chet thi khong lam gi nua

        DetectPlayer(); // phat hien player
        CheckPlayerInRangedRange(); // kiem tra player trong tam tan cong tam xa
        AlignFirePointToFacing();

        // Chi xu ly logic khi KHONG dang tan cong VA KHONG bi hurt
        if (!_isAttackingMelee && !_isAttackingRange && !_isAttackingMelee_2 && !_isHurt)
        {
            if (_playerDetected)
            {
                // UU TIEN 1: Melee attack neu player gan va cooldown ready
                if (PlayerInAttackMelee() && _attackCooldownTimer <= 0)
                {
                    TryAttack(); // thu tan cong melee
                } 
                // UU TIEN 2: Melee attack 2 neu player trong tam va cooldown ready
                else if (PlayerInAttackMelee_2() && _attackCooldownTimer_2 <= 0)
                {
                    TryAttack_2(); // thu tan cong melee 2
                }
                // UU TIEN 3: Ranged attack neu player trong tam xa va cooldown ready
                else if (CheckPlayerInRangedRange() && _rangedAttackCooldownTimer <= 0)
                {
                    TryRangedAttack(); // thu tan cong tam xa
                }
                // Neu khong tan cong duoc thi di chuyen
                else
                {
                    Moving(); // di chuyen khi chua den tam tan cong
                }
            }
            else
            {
                _rigi.velocity = new Vector2(0, _rigi.velocity.y); // dung lai neu khong phat hien
            }
        }
        _anim.UpdateAnimationBoss(_currentBossState);
    }

    // Phat hien player bang BoxCast
    private void DetectPlayer()
    {
        if (_boxCollider == null)
        {
            //Debug.LogWarning("BoxCollider chua duoc gan cho Boss!");
            return;
        }

        // Tinh huong BoxCast (ca 2 phia)
        Vector2 facingDir = transform.localScale.x > 0 ? Vector2.right : Vector2.left;

        // Thuc hien BoxCast de phat hien player
        RaycastHit2D hit = Physics2D.BoxCast(
            _boxCollider.bounds.center + (Vector3)(facingDir * _detectionRange * _colliderDistance),
            new Vector3(_boxCollider.bounds.size.x * _detectionRange, _boxCollider.bounds.size.y, _boxCollider.bounds.size.z),
            0,
            Vector2.zero,
            0,
            _playerLayer
        );

        _playerDetected = hit.collider != null;
    }

    // Kiem tra player co Dalam tam tan cong tam xa khong bang BoxCast
    private bool CheckPlayerInRangedRange()
    {
        if (_boxCollider == null || GameManager.Instance.Player == null)
            return false;
 
        // Tinh huong tan cong
        Vector2 facingDir = transform.localScale.x > 0 ? Vector2.right : Vector2.left;

        // Thuc hien BoxCast de kiem tra tam tan cong tam xa
        RaycastHit2D hit = Physics2D.BoxCast(
            _boxCollider.bounds.center + (Vector3)(facingDir * _rangedAttackRange * _rangedAttackColliderDistance),
            new Vector3(_boxCollider.bounds.size.x * _rangedAttackRange, _boxCollider.bounds.size.y, _boxCollider.bounds.size.z),
            0,
            Vector2.zero,
            0,
            _playerLayer
        );

        // Debug log de kiem tra
        if (hit.collider != null)
        {
            //Debug.Log("Player in Ranged Range! Hit: " + hit.collider.name + " | Distance: " + Vector2.Distance(transform.position, hit.point));
        }

        return hit.collider != null;
    }

    // Kiem tra player co trong tam tan cong khong
    private bool PlayerInAttackMelee()
    {
        if (_boxCollider == null || GameManager.Instance.Player == null) return false;

        // Tinh huong tan cong
        Vector2 facingDir = transform.localScale.x > 0 ? Vector2.right : Vector2.left;

        // Thuc hien BoxCast de kiem tra tam tan cong (su dung _attackColliderDistance)
        RaycastHit2D hit = Physics2D.BoxCast(
            _boxCollider.bounds.center + (Vector3)(facingDir * _attackRange * _attackColliderDistance),
            new Vector3(_boxCollider.bounds.size.x * _attackRange, _boxCollider.bounds.size.y, _boxCollider.bounds.size.z),
            0,
            Vector2.zero,
            0,
            _playerLayer
        );

        return hit.collider != null;
    }

    // Kiem tra player co trong tam tan cong melee thu 2 khong
    private bool PlayerInAttackMelee_2()
    {
        if (_boxCollider == null || GameManager.Instance.Player == null) return false;

        // Tinh huong tan cong
        Vector2 facingDir = transform.localScale.x > 0 ? Vector2.right : Vector2.left;

        // Thuc hien BoxCast de kiem tra tam tan cong thu 2 (su dung _attackColliderDistance_2)
        RaycastHit2D hit = Physics2D.BoxCast(
            _boxCollider.bounds.center + (Vector3)(facingDir * _attackRange_2 * _attackColliderDistance_2),
            new Vector3(_boxCollider.bounds.size.x * _attackRange_2, _boxCollider.bounds.size.y, _boxCollider.bounds.size.z),
            0,
            Vector2.zero,
            0,
            _playerLayer
        );

        return hit.collider != null;
    }

    // Thu tan cong player
    private void TryAttack()
    {
        // Kiem tra player da chet chua
        if (GameManager.Instance.Player.isDeadPlayer) return;
        
        // Kiem tra cooldown va trang thai
        if (_attackCooldownTimer > 0 || _isAttackingMelee) return;

        // Dung lai de tan cong
        _rigi.velocity = Vector2.zero;

        // Bat dau tan cong
        StartCoroutine(AttackCoroutine());
    }

    // Coroutine xu ly tan cong
    private IEnumerator AttackCoroutine()
    {
        _isAttackingMelee = true; // KHOA state transition
        _attackCooldownTimer = _attackCooldown;

        // Trigger animation tan cong
        if (_animator != null)
        {
            _animator.SetTrigger(CONSTANT.BOSS_MELEE_ATK);
        }
        
        // CHO animation choi het (quan trọng!)
        float elapsed = 0f;
        while (elapsed < _attackDuration)
        {
            // Kiem tra player co chet giua chung khong
            if (GameManager.Instance.Player.isDeadPlayer)
            {
                _isAttackingMelee = false;
                // Reset animator ve IDLE de dung animation
                if (_animator != null)
                {
                    _animator.ResetTrigger(CONSTANT.BOSS_MELEE_ATK);
                }
                yield break; // Thoat coroutine ngay lap tuc
            }
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        _isAttackingMelee = false; // MO KHOA state transition
    }

    // Thu tan cong melee thu 2 player
    private void TryAttack_2()
    {
        // Kiem tra player da chet chua
        if (GameManager.Instance.Player.isDeadPlayer) return;
        
        // Kiem tra cooldown va trang thai
        if (_attackCooldownTimer_2 > 0 || _isAttackingMelee_2) return;

        // Dung lai de tan cong
        _rigi.velocity = Vector2.zero;

        // Bat dau tan cong melee 2
        StartCoroutine(AttackCoroutine_2());
    }

    // Coroutine xu ly tan cong melee 2
    private IEnumerator AttackCoroutine_2()
    {
        _isAttackingMelee_2 = true; // KHOA state transition
        _attackCooldownTimer_2 = _attackCooldown_2;

        // Trigger animation tan cong melee 2
        if (_animator != null)
        {
            _animator.SetTrigger(CONSTANT.BOSS_MELEE_ATK_2);
        }
        
        // CHO animation choi het (quan trọng!)
        float elapsed = 0f;
        while (elapsed < _attackDuration_2)
        {
            // Kiem tra player co chet giua chung khong
            if (GameManager.Instance.Player.isDeadPlayer)
            {
                _isAttackingMelee_2 = false;
                // Reset animator ve IDLE de dung animation
                if (_animator != null)
                {
                    _animator.ResetTrigger(CONSTANT.BOSS_MELEE_ATK_2);
                }
                yield break; // Thoat coroutine ngay lap tuc
            }
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        _isAttackingMelee_2 = false; // MO KHOA state transition
    }

    // Thu tan cong tam xa player
    private void TryRangedAttack()
    {
        // Kiem tra player da chet chua
        if (GameManager.Instance.Player.isDeadPlayer) return;
        
        // Kiem tra cooldown va trang thai
        if (_rangedAttackCooldownTimer > 0 || _isAttackingRange) return;

        // Dung lai de tan cong
        _rigi.velocity = Vector2.zero;

        // Bat dau tan cong tam xa
        StartCoroutine(RangedAttackCoroutine());
    }

    // Coroutine xu ly tan cong tam xa
    private IEnumerator RangedAttackCoroutine()
    {
        _isAttackingRange = true; // KHOA state transition
        _currentBossState = BossState.RANGEDATK; // SET state la RANGEDATK
        _rangedAttackCooldownTimer = _rangedAttackCooldown;

        // Trigger animation tan cong tam xa
        if (_animator != null)
        {
            _animator.SetTrigger(CONSTANT.BOSS_RANGED_ATK);
        }

        // CHO animation choi het
        float elapsed = 0f;
        while (elapsed < _rangedAttackDuration)
        {
            // Kiem tra player co chet giua chung khong
            if (GameManager.Instance.Player.isDeadPlayer)
            {
                _isAttackingRange = false;
                // Reset animator ve IDLE de dung animation
                if (_animator != null)
                {
                    _animator.ResetTrigger(CONSTANT.BOSS_RANGED_ATK);
                }
                yield break; // Thoat coroutine ngay lap tuc
            }
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        _isAttackingRange = false; // MO KHOA state transition
    }

    // Ham goi tu animation event de gai sat thuong melee attack 1
    public void DealDamageToPlayer()
    {
        // Khong gay dame neu player da chet
        if (GameManager.Instance.Player.isDeadPlayer) return;
        
        if (PlayerInAttackMelee())
        {
            GameManager.Instance.Player.TakeDamage(_damage, this.transform);
        }

        if (PlayerInAttackMelee_2())
        {
            GameManager.Instance.Player.TakeDamage(_damage_2, this.transform);
        }
    }
    public void ShootLaser()
    {
        if (_laserPrefab == null || _laserFirePoint == null)
            return;

        // Khoi tao laser
        //GameObject laser = Instantiate(_laserPrefab, _laserFirePoint.position, _laserFirePoint.rotation);

        GameObject laser = ObjectPoolingX.Instance.GetObject(_laserPrefab);
        laser.transform.position = _laserFirePoint.position;
        laser.transform.rotation = _laserFirePoint.rotation;
        laser.SetActive(true);
    }

    // ham giup firePoint luon quay cung huong voi enemy
    private void AlignFirePointToFacing()
    {
        if (_laserFirePoint == null) return;

        // neu enemy quay phai => scale.x > 0
        // quay trai => scale.x < 0
        _laserFirePoint.right = Vector3.right * Mathf.Sign(transform.localScale.x);
    }

    // Cap nhat trang thai Boss dua vao hanh dong hien tai
    void UpdateState()
    {
        if(_isDead)
        {
            _currentBossState = BossState.DEAD;
            return;
        }

        // UU TIEN cao nhat: Neu dang hurt
        if (_isHurt)
        {
            _currentBossState = BossState.HURT;
            return;
        }

        // UU TIEN: Neu dang tan cong thi giu nguyen state tuong ung
        if (_isAttackingMelee)
        {
            _currentBossState = BossState.MELEEATK;
            return;
        }

        if (_isAttackingMelee_2)
        {
            _currentBossState = BossState.MELEEATK_2;
            return;
        }

        if (_isAttackingRange)
        {
            _currentBossState = BossState.RANGEDATK;
            return;
        }

        // Kiem tra co dang di chuyen khong (velocity.x khac 0)
        if (Mathf.Abs(_rigi.velocity.x) > 0.1f)
        {
            _currentBossState = BossState.WALK;
        }
        else
        {
            _currentBossState = BossState.IDLE;
        }
    }

    // Ham di chuyen Boss ve phia nguoi choi
    private void Moving()
    {
        // Lay vi tri player tu GameManager
        if (GameManager.Instance.Player == null)
        {
            _rigi.velocity = Vector2.zero;
            return;
        }

        _player = GameManager.Instance.Player.transform;

        // Tinh khoang cach den player
        float distanceToPlayer = Vector2.Distance(transform.position, _player.position);

        // Neu da du gan thi dung lai
        if (distanceToPlayer <= _stopDistance)
        {
            _rigi.velocity = new Vector2(0, _rigi.velocity.y);
            return;
        }

        // Tinh huong di chuyen ve phia player
        float direction = _player.position.x > transform.position.x ? 1 : -1;

        // Di chuyen theo huong player
        _rigi.velocity = new Vector2(direction * _moveSpeed, _rigi.velocity.y);

        // Quay mat Boss ve phia player
        FlipTowardsPlayer(direction);
    }

    public void TakeDamage(int damage, Transform attackerTransform)
    {
        if (_isDead) return; // Khong nhan dam neu da chet

        _healthBoss -= damage;
        Debug.Log("Boss Health: " + _healthBoss);

        if (_healthBoss <= 0)
        {
            _healthBoss = 0; // Dam bao health khong am
            StartCoroutine(DeathCoroutine());
        }
        else
        {
            // Bat dau hurt va knockback
            StartCoroutine(HurtCoroutine(attackerTransform));
        }
    }

    // Coroutine xu ly hurt va knockback
    private IEnumerator HurtCoroutine(Transform attackerTransform)
    {
        _isHurt = true; // KHOA state

        // Set Bool animation hurt
        if (_animator != null)
        {
            _animator.SetBool(CONSTANT.BOSS_HURT, true);
        }

        // Tinh huong day lui (nguoc voi vi tri attacker)
        float knockbackDirection = transform.position.x < attackerTransform.position.x ? -1f : 1f;
        
        // Ap dung luc knockback
        float timer = 0f;
        while (timer < _knockbackDuration)
        {
            if (_rigi != null)
            {
                _rigi.velocity = new Vector2(knockbackDirection * _knockbackForce, _rigi.velocity.y);
            }
            timer += Time.deltaTime;
            yield return null;
        }

        // Dung lai sau knockback
        if (_rigi != null)
        {
            _rigi.velocity = new Vector2(0, _rigi.velocity.y);
        }

        // Cho animation hurt choi xong
        yield return new WaitForSeconds(_hurtDuration - _knockbackDuration);

        // Tat Bool animation hurt
        if (_animator != null)
        {
            _animator.SetBool(CONSTANT.BOSS_HURT, false);
        }

        _isHurt = false; // MO KHOA state
    }

    // Coroutine xu ly chet
    private IEnumerator DeathCoroutine()
    {
        _isDead = true;
        _currentBossState = BossState.DEAD;

        // Set Bool animation chet
        if (_animator != null)
        {
            _animator.SetBool(CONSTANT.BOSS_DEAD, true);
        }

        // Dung lai
        if (_rigi != null)
        {
            _rigi.velocity = Vector2.zero;
            _rigi.isKinematic = true; // Vo hieu hoa physics
        }

        Debug.Log("Boss defeated! Playing death animation...");

        // CHO animation chet choi het
        yield return new WaitForSeconds(_deathDuration);

        Debug.Log("Destroying Boss GameObject...");
        
        // Destroy game object
        Destroy(gameObject);
    }

    // Quay mat Boss ve phia player
    private void FlipTowardsPlayer(float direction)
    {
        Vector3 scale = transform.localScale;
        scale.x = direction > 0 ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        transform.localScale = scale;
    }

    // Ve Gizmos de debug BoxCast trong Scene view
    private void OnDrawGizmos()
    {
        if (_boxCollider == null) return;

        Vector2 facingDir = transform.localScale.x > 0 ? Vector2.right : Vector2.left;

        // Ve BoxCast detection range (mau do/xanh la)
        Gizmos.color = _playerDetected ? Color.green : Color.red;
        Vector3 boxCenter = _boxCollider.bounds.center + (Vector3)(facingDir * _detectionRange * _colliderDistance);
        Vector3 boxSize = new Vector3(_boxCollider.bounds.size.x * _detectionRange, _boxCollider.bounds.size.y, _boxCollider.bounds.size.z);
        Gizmos.DrawWireCube(boxCenter, boxSize);

        // Ve BoxCast attack range (mau vang) - su dung _attackColliderDistance
        Gizmos.color = Color.yellow;
        Vector3 attackBoxCenter = _boxCollider.bounds.center + (Vector3)(facingDir * _attackRange * _attackColliderDistance);
        Vector3 attackBoxSize = new Vector3(_boxCollider.bounds.size.x * _attackRange, _boxCollider.bounds.size.y, _boxCollider.bounds.size.z);
        Gizmos.DrawWireCube(attackBoxCenter, attackBoxSize);

        // Ve BoxCast melee attack 2 range (mau cam) - su dung _attackColliderDistance_2
        Gizmos.color = Color.Lerp(Color.yellow, Color.red, 0.5f); // Mau cam
        Vector3 attack2BoxCenter = _boxCollider.bounds.center + (Vector3)(facingDir * _attackRange_2 * _attackColliderDistance_2);
        Vector3 attack2BoxSize = new Vector3(_boxCollider.bounds.size.x * _attackRange_2, _boxCollider.bounds.size.y, _boxCollider.bounds.size.z);
        Gizmos.DrawWireCube(attack2BoxCenter, attack2BoxSize);

        // Ve BoxCast ranged attack range (mau cyan/magenta)
        // Kiem tra player co trong tam ranged attack khong
        bool playerInRanged = false;
        if (GameManager.Instance != null && GameManager.Instance.Player != null)
        {
            Vector2 facingDirection = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
            RaycastHit2D hit = Physics2D.BoxCast(
                _boxCollider.bounds.center + (Vector3)(facingDirection * _rangedAttackRange * _rangedAttackColliderDistance),
                new Vector3(_boxCollider.bounds.size.x * _rangedAttackRange, _boxCollider.bounds.size.y, _boxCollider.bounds.size.z),
                0,
                Vector2.zero,
                0,
                _playerLayer
            );
            playerInRanged = hit.collider != null;
        }
        
        Gizmos.color = playerInRanged ? Color.cyan : Color.magenta;
        Vector3 rangedBoxCenter = _boxCollider.bounds.center + (Vector3)(facingDir * _rangedAttackRange * _rangedAttackColliderDistance);
        Vector3 rangedBoxSize = new Vector3(_boxCollider.bounds.size.x * _rangedAttackRange, _boxCollider.bounds.size.y, _boxCollider.bounds.size.z);
        Gizmos.DrawWireCube(rangedBoxCenter, rangedBoxSize);
    }

    public enum BossState
    {
        IDLE,
        WALK,
        HURT,
        DEAD,
        MELEEATK,
        RANGEDATK,
        MELEEATK_2,
    }
}