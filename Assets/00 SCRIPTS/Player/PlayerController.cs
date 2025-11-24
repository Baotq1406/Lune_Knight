using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region === References ===
    [Header("References")]
    [SerializeField] private AnimationControllerBase _anim;  // Bộ điều khiển animation của player
    private Rigidbody2D _rigi;                               // Rigidbody để xử lý vật lý
    #endregion


    #region === Movement Settings ===
    [Header("Movement Settings")]
    [SerializeField] private float _speed;                    // Tốc độ di chuyển ngang
    [SerializeField] private float _jumpForce;                // Lực nhảy ban đầu
    [SerializeField] private float _fallMultiplier;           // Hệ số rơi nhanh khi thả phím nhảy
    [SerializeField] private float _lowJumpMultiplier;        // Hệ số giảm nhảy khi nhấn nhảy ngắn

    [Header("Jump Settings")]
    [SerializeField] private int _maxJumpCount = 2;           // Số lần nhảy tối đa (double jump)
    private int _currentJumpCount = 0;                        // Số lần nhảy hiện tại
    #endregion


    #region === Dash Settings ===
    [Header("Dash Settings")]
    [SerializeField] private float _dashSpeed = 10f;          // Tốc độ trong khi dash
    [SerializeField] private float _dashDuration = 0.2f;      // Thời gian duy trì tốc độ dash
    [SerializeField] private float _dashCooldown = 0.5f;      // Thời gian hồi dash

    private float _dashCooldownTimer = 0f;                    // Bộ đếm thời gian hồi dash
    private bool _isDashing = false;                          // Trạng thái đang dash hay không
    private float _normalGravity;                             // Lưu gravity ban đầu để reset sau khi dash
    #endregion


    #region === Attack Settings ===
    [Header("Attack Settings")]
    [SerializeField] private float _attackDuration = 0.25f;   // Thời gian thực hiện một đòn tấn công
    [SerializeField] private float _damage = 1f;                  // Lượng
    private bool _isAttacking = false;                        // Cờ kiểm tra player đang tấn công
    private Coroutine _attackCoroutine = null;                // Coroutine xử lý tấn công
    #endregion


    #region === Damage / Health ===
    [Header("Health Settings")]
    [SerializeField] private int _playerHealth = 5;           // Máu của người chơi
    [SerializeField] private float _knockbackForce = 8f;      // Lực đẩy lùi khi bị thương
    [SerializeField] private float _knockbackDuration = 0.15f;// Thời gian bị choáng/đẩy lùi

    private bool _isHurt = false;                             // Đang trong trạng thái bị thương
    private bool _isDead = false;                             // Player đã chết
    private bool _isKnockback = false;                        // Đang bị knockback
    #endregion


    #region === Player State ===
    [Header("Player State")]
    [SerializeField] private bool _isOnGrounded;              // Kiểm tra player đang chạm đất
    [SerializeField] private PlayerState _playerState = PlayerState.IDLE; // Trạng thái hiện tại
    public PlayerState playerState => _playerState;           // Getter cho trạng thái player
    #endregion


    void Start()
    {
        _rigi = GetComponent<Rigidbody2D>();
        _normalGravity = _rigi.gravityScale; // luu gravity ban dau
    }

    void Update()
    {
        if (!_isDashing) // khong di chuyen binh thuong khi dash
            Moving();

        JumpCheck();    // kiem tra nhay    
        TryDash();      // kiem tra nhan nut dash
        TryAttack(); // kiem tra tan cong


        UpdateState();  // cap nhat trang thai nhan vat
        _anim.UpdateAnimation(_playerState); // update animation


        Debug.DrawRay(transform.position, Vector2.down * 0.8f, Color.red); // ve ray kiem tra dat

        if (_isKnockback)
        {
            _anim.UpdateAnimation(_playerState);
            return; // không cho player tự điều khiển
        }

        if (_dashCooldownTimer > 0) // giam timer cooldown
            _dashCooldownTimer -= Time.deltaTime;
    }

    // ------------------- CAP NHAT TRANG THAI -------------------
    void UpdateState()
    {
        if (_isDashing) // neu dang dash
        {
            _playerState = PlayerState.DASH;
            return;
        }

        // kiem tra trang thai tan cong
        if (_isAttacking)
        {
            _playerState = PlayerState.ATTACK;
            return;
        }

        // kiem tra trang thai bi thuong
        if (_isHurt)
        {
            _playerState = PlayerState.HURT;
            return;
        }

        // kiem tra trang thai chet
        if (_isDead)
        {
            _playerState = PlayerState.DEATH;
            return;
        }

        // kiem tra trang thai nhan vat
        if (!_isOnGrounded) // neu o tren khong
        {
            if (_rigi.velocity.y > 0)
                _playerState = PlayerState.JUMP; // len cao
            else
                _playerState = PlayerState.FALL; // roi
        }
        else // neu tren dat
        {
            if (_rigi.velocity.x != 0)
                _playerState = PlayerState.RUN; // chay
            else
                _playerState = PlayerState.IDLE; // dung yen
        }
    }

    // -------------------- DI CHUYEN ----------------------
    void Moving()
    {
        Vector2 movement = _rigi.velocity;
        movement.x = Input.GetAxisRaw("Horizontal") * _speed; // lay input nguoi choi
        _rigi.velocity = movement;

        // doi huong nhan vat theo chieu di chuyen
        if (_rigi.velocity.x > 0)
        {
            Vector2 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
        else if (_rigi.velocity.x < 0)
        {
            Vector2 scale = transform.localScale;
            scale.x = -Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
    }

    // ----------------------- NHAY ------------------------
    void JumpCheck()
    {
        // Nhấn Space để nhảy 
        if (Input.GetKeyDown(KeyCode.Space) && _currentJumpCount < _maxJumpCount && !_isDashing)
        {
            _rigi.velocity = new Vector2(_rigi.velocity.x, 0); // reset lực rơi cho lần nhảy tiếp theo
            _rigi.AddForce(new Vector2(0, _jumpForce));

            _isOnGrounded = false;
            _currentJumpCount++;
        }

        // Điều chỉnh rơi nhanh hơn và nhảy thấp hơn
        if (_rigi.velocity.y < 0)
        {
            _rigi.velocity += Vector2.up * Physics2D.gravity.y * (_fallMultiplier - 1) * Time.deltaTime;
        }
        else if (_rigi.velocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            _rigi.velocity += Vector2.up * Physics2D.gravity.y * (_lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    // ----------------------- DASH ------------------------
    void TryDash()
    {
        // kiem tra nhan shift va cooldown
        if ((Input.GetKeyDown(KeyCode.LeftShift) || Input.GetMouseButtonDown(1)) && _dashCooldownTimer <= 0)
        {
            StartCoroutine(DashCoroutine());
        }
    }
    IEnumerator DashCoroutine()
    {
        _isDashing = true;
        _dashCooldownTimer = _dashCooldown; // reset cooldown

        float dashDirection = transform.localScale.x > 0 ? 1 : -1; // xac dinh huong dash theo huong nhan vat

        _rigi.gravityScale = 0;

        float dashTime = 0f;

        // thuc hien dash trong khoang thoi gian dashDuration
        while (dashTime < _dashDuration)
        {
            // lay input nguoi choi
            float horizontalInput = Input.GetAxisRaw("Horizontal");

            // neu nhan input nguoc chieu dash => ket thuc dash
            if ((dashDirection > 0 && horizontalInput < 0) || (dashDirection < 0 && horizontalInput > 0))
                break;

            // dat velocity dash
            _rigi.velocity = new Vector2(dashDirection * _dashSpeed, 0);

            dashTime += Time.deltaTime;
            yield return null;
        }

        _rigi.gravityScale = _normalGravity;
        _isDashing = false;
    }

    // ------------------- ATTACK -------------------
    void TryAttack()
    {
        if (_isDashing || _isAttacking)
            return;

        if (Input.GetKeyDown(KeyCode.J) || Input.GetMouseButtonDown(0))
        {
            _attackCoroutine = StartCoroutine(AttackCoroutine());
        }
    }
    IEnumerator AttackCoroutine()
    {
        _isAttacking = true;
        _rigi.velocity = Vector2.zero;

        yield return new WaitForSeconds(_attackDuration);

        _isAttacking = false;
        _attackCoroutine = null;
    }


    // ------------------- NHAN SAT THUONG -------------------
    public void TakeDamage(int damage, Transform enemyPos)
    {
        if (_isDead)
            return;

        _playerHealth -= damage;
        Debug.LogError(_playerHealth);

        // Hủy attack nếu đang bi attack
        if (_isAttacking)
        {
            _isAttacking = false;

            if (_attackCoroutine != null)
            {
                StopCoroutine(_attackCoroutine);
                _attackCoroutine = null;
            }
        }

        if (_playerHealth <= 0)
        {
            _isDead = true;
        }
        else
        {
            _isHurt = true;

            StartCoroutine(DoKnockback(enemyPos));
            StartCoroutine(ResetHurtState());
        }
    }


    private IEnumerator ResetHurtState()
    {
        // Thời gian bị thương (có thể chỉnh sửa nếu muốn)
        yield return new WaitForSeconds(0.2f);
        _isHurt = false;
    }

    private IEnumerator DoKnockback(Transform enemyPos)
    {
        _isKnockback = true;

        // hướng knockback: trái hoặc phải
        float dir = transform.position.x < enemyPos.position.x ? -1 : 1;

        float timer = 0f;

        while (timer < _knockbackDuration)
        {
            _rigi.velocity = new Vector2(dir * _knockbackForce, _rigi.velocity.y);
            timer += Time.deltaTime;
            yield return null;
        }

        _isKnockback = false;
    }



    // ------------------ KIEM TRA DAT --------------------
    private void OnCollisionEnter2D(Collision2D collision)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.8f);
        if (hit.collider != null)
        {
            _isOnGrounded = true; // dat dat
            _currentJumpCount = 0; // reset double jump
        }
    }

    // ---------------------- ENUM TRANG THAI ------------------
    public enum PlayerState
    {
        IDLE,
        RUN,
        JUMP,
        FALL,
        DASH,
        ATTACK,
        HURT,
        DEATH,
    }
}               