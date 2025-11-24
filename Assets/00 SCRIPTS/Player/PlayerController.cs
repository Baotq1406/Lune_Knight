using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Movement
    [Header("Movement Settings")]
    [SerializeField] private float _speed;             // toc do di chuyen         
    [SerializeField] private float _jumpForce;         // luc nhay
    [SerializeField] private float _fallMultiplier;    // dieu chinh roi nhanh hon
    [SerializeField] private float _lowJumpMultiplier; // dieu chinh nhay thap hon
    #endregion

    #region Dash Settings
    [Header("Dash Settings")]
    [SerializeField] private float _dashSpeed = 10f;     // toc do dash
    [SerializeField] private float _dashDuration = 0.2f; // thoi gian giu toc do dash
    [SerializeField] private float _dashCooldown = 0.5f; // thoi gian hoi dash
    private float _dashCooldownTimer = 0f;               // timer hoi dash
    private bool _isDashing = false;                     // trang thai dash dang dien ra
    private float _normalGravity;                        // luu gravity goc de reset sau dash
    #endregion

    #region Attack Settings
    [Header("Attack Settings")]
    [SerializeField] private float _attackDuration = 0.25f; // thoi gian tan cong
    private bool _isAttacking = false;                     // trang thai tan cong
    #endregion

    #region Player State
    [Header("Player State")]
    [SerializeField] private bool _isOnGrounded;         // kiem tra cham dat
    [SerializeField] private int _maxJumpCount = 2;      // so lan nhay toi da
    private int _currentJumpCount = 0;                   // so lan nhay hien tai
    [SerializeField] PlayerState _playerState = PlayerState.IDLE; // trang thai hien tai
    public PlayerState playerState => _playerState;      // getter trang thai nhan vat
    [SerializeField] AnimationControllerBase _anim;    // reference animation controller
    #endregion



    private Rigidbody2D _rigi;

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

        if (_isAttacking)
        {
            _playerState = PlayerState.ATTACK;
            return;
        }

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
            StartCoroutine(AttackCoroutine());
        }
    }

    IEnumerator AttackCoroutine()
    {
        _isAttacking = true;

        _rigi.velocity = Vector2.zero;

        yield return new WaitForSeconds(_attackDuration); // cho den khi ket thuc attack

        _isAttacking = false;
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
    }
}               