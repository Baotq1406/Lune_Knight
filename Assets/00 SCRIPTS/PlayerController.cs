using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    # region Movement
    [SerializeField] private float _speed = 5f;           // toc do di chuyen
    [SerializeField] private float _jumpForce = 400f;    // luc nhay
    #endregion

    #region Dash Settings
    [SerializeField] private float _dashSpeed = 10f;     // toc do dash
    [SerializeField] private float _dashDuration = 0.2f; // thoi gian giu toc do dash
    [SerializeField] private float _dashCooldown = 0.5f; // thoi gian hoi dash
    private float _dashCooldownTimer = 0f;               // timer hoi dash
    private bool _isDashing = false;                     // trang thai dash dang dien ra

    private float _normalGravity;                        // luu gravity goc de reset sau dash
    #endregion

    # region State
    [SerializeField] private bool _isOnGrounded;         // kiem tra cham dat
    [SerializeField] PlayerState _playerState = PlayerState.IDLE; // trang thai hien tai
    [SerializeField] AnimationControllerBase _anim;    // reference animation controller
    # endregion

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

        UpdateState();  // cap nhat trang thai nhan vat
        _anim.UpdateAnimation(_playerState); // update animation

        Debug.DrawRay(transform.position, Vector2.down * 0.7f, Color.red); // ve ray kiem tra dat

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
        if (Input.GetKeyDown(KeyCode.Space) && _isOnGrounded && !_isDashing)
        {
            _isOnGrounded = false;                // tat kiem tra dat khi nhay
            _rigi.AddForce(new Vector2(0, _jumpForce)); // them luc nhay
        }
    }

    // ----------------------- DASH ------------------------
    void TryDash()
    {
        // kiem tra nhan shift va cooldown
        if (Input.GetKeyDown(KeyCode.LeftShift) && _dashCooldownTimer <= 0)
        {
            StartCoroutine(DashCoroutine());
        }
    }
    IEnumerator DashCoroutine()
    {
        _isDashing = true;
        _dashCooldownTimer = _dashCooldown;

        float dashDirection = transform.localScale.x > 0 ? 1 : -1;

        _rigi.gravityScale = 0;

        float dashTime = 0f;

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


    // ------------------ KIEM TRA DAT --------------------
    private void OnCollisionEnter2D(Collision2D collision)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.7f);
        if (hit.collider != null)
        {
            _isOnGrounded = true; // dat dat
        }
    }

    // ---------------------- ENUM TRANG THAI ------------------
    public enum PlayerState
    {
        IDLE,
        RUN,
        JUMP,
        FALL,
        DASH
    }
}
