using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region === References ===
    [Header("References")]
    [SerializeField] private AnimationControllerBase _anim;
    private Rigidbody2D _rigi;
    private CameraController _camera; // cache cho shake camera
    #endregion

    #region === Movement Settings ===
    [Header("Movement Settings")]
    [SerializeField] private float _speed; // toc do di chuyen
    [SerializeField] private float _jumpForce; // luc nhay
    [SerializeField] private float _fallMultiplier; // he so dieu chinh toc do roi
    [SerializeField] private float _lowJumpMultiplier; // he so dieu chinh nhay thap

    [Header("Jump Settings")]
    [SerializeField] private int _maxJumpCount = 2; // so lan nhay toi da
    private int _currentJumpCount = 0; // so lan nhay hien tai
    #endregion

    #region === Dash Settings ===
    [Header("Dash Settings")]
    [SerializeField] private float _dashSpeed = 10f; // toc do dash
    [SerializeField] private float _dashDuration = 0.2f; // thoi gian dash
    [SerializeField] private float _dashCooldown = 0.5f; // thoi gian hoi dash
    private float _dashCooldownTimer = 0f; // bo dem hoi dash
    private bool _isDashing = false; // trang thai dang dash
    private float _normalGravity; // luu gia tri gravity binh thuong
    [SerializeField] TrailRenderer _dashTrail; // trail dash
    #endregion

    #region === Attack Settings ===
    [Header("Attack Settings")]
    [SerializeField] private float _attackDuration = 0.25f; // thoi gian tan cong
    [SerializeField] private float _attackCooldown = 0.5f; // thoi gian cooldown giua cac lan tan cong
    [SerializeField] GameObject _axeAttack; // hitbox Axe Attack
    private bool _isAttacking = false; // trang thai dang tan cong
    private float _attackCooldownTimer = 0f; // bo dem cooldown tan cong
    private Coroutine _attackCoroutine = null;  // tham chieu toi coroutine tan cong
    #endregion

    #region === Damage / Health/ Knockback===
    [Header("Healing / Soul Settings")]
    [SerializeField] private int _maxHealth = 100;           // Máu tối đa
    public int maxHealth => _maxHealth;               // Ham get mau toi da
    [SerializeField] private int _currentHealth;           // Máu hiện tại
    [SerializeField] private int _healAmount = 1;          // Số máu hồi mỗi lần
    public int healAmount => _healAmount;               // Ham get so mau hoi
    [SerializeField] private int _maxSoul = 5;             // Năng lượng tối đa
    [SerializeField] private int _currentSoul = 0;         // Năng lượng hiện tại
    [SerializeField] private float _healCooldown = 0.5f;  // Cooldown hồi máu
    [SerializeField] private GameObject _healVFX;          // Hiệu ứng hồi máu
    private bool _isHealing = false;

    [Header("Knockback Settings")]
    [SerializeField] private float _knockbackForce = 8f; // luc knockback khi bi danh
    [SerializeField] private float _knockbackDuration = 0.15f; // thoi gian knockback

    [Header("Invincibility (I-Frame) Settings")]
    [SerializeField] private float _invincibleDuration = 0.8f;    // thoi gian vo hieu hoa nhan sat thuong
    private bool _isInvincible = false;                           // dang vo hieu hoa
    [SerializeField] private float _cameraShakeDuration = 0.2f;    // thoi gian shake camera khi bi thuong
    [SerializeField] private float _cameraShakeMagnitude = 0.1f;  // do manh shake camera
    [SerializeField] CameraController _cameraController; // tham chieu toi camera controller
    [SerializeField] private bool _isHurt = false; // trang thai bi thuong
    [SerializeField] private bool _isDead = false; // trang thai chet
    public bool isDeadPlayer => _isDead; // ham get trang thai chet
    private bool _isKnockback = false; // trang thai bi knockback
    #endregion

    #region === Player State ===
    [Header("Player State")]
    [SerializeField] private bool _isOnGrounded; // trang thai cham dat
    [SerializeField] private PlayerState _playerState = PlayerState.IDLE; // trang thai hien tai cua nhan vat
    public PlayerState playerState => _playerState; // ham get trang thai nhan vat
    #endregion

    void Start()
    {
        _rigi = GetComponent<Rigidbody2D>(); // lay component Rigidbody2D
        _normalGravity = _rigi.gravityScale; // luu lai gia tri gravity ban dau
        if (_axeAttack != null)
            _axeAttack.SetActive(false); // tat hitbox ri truoc khi choi

        _maxHealth = PlayerPrefs.GetInt(CONSTANT.MAX_HEALTH, _maxHealth); // lay max HP tu player prefs
        if (PlayerPrefs.GetInt(CONSTANT.LAST_CHECKPOINT_HEALTH, _currentHealth) > 0)
        {
            _currentHealth = PlayerPrefs.GetInt(CONSTANT.LAST_CHECKPOINT_HEALTH, _currentHealth); // lay mau hien tai o checkpoint
        } else
        {
            _currentHealth = _maxHealth; // khoi tao mau
        }
        _healAmount = PlayerPrefs.GetInt(CONSTANT.HEAL_AMOUNT, _healAmount); // lay luc hoi mau tu player prefs
        //_currentHealth = _maxHealth; // khoi tao mau
        //_currentHealth = PlayerPrefs.GetInt(CONSTANT.LAST_CHECKPOINT_HEALTH, _currentHealth); // lay mau hien tai o checkpoint
        UIManager.Instance.UpdateHealthSlider(_currentHealth, _maxHealth); // cap nhat UI mau

        _currentSoul = 0; // khoi tao nang luong
        UIManager.Instance.UpdateSoulSlider(_currentSoul, _maxSoul); // cap nhat UI soul

        //UIManager.Instance.UpdatePointText(_upgradePoints); // cap nhat UI diem nang cap
        //_camera = FindObjectOfType<CameraController>(); // cache camera controller

        UIManager.Instance.ShowHPText(_maxHealth); // hien thi text mau luc dau
        UIManager.Instance.ShowHealText(_healAmount); // hien thi text heal luc dau
    }

    void Update()
    {
        if (UIManager.Instance._statsPanel.activeSelf)
            return; // neu dang mo panel thong so thi khong cho dieu khien nhan vat

        if (!_isDead)
        {
            // neu khong dang dash thi cho phep di chuyen
            if (!_isDashing)
                Moving();

            JumpCheck(); // kiem tra nhay
            TryDash();   // kiem tra dash
            TryAttack(); // kiem tra tan cong

        }
        UpdateState(); // cap nhap trang thai nhan vat
        _anim.UpdateAnimation(_playerState); // cap nhat animation

        Debug.DrawRay(transform.position, Vector2.down * 0.8f, Color.red); // ve ray kiem tra dat

        // neu dang bi knockback thi khong cho dieu khien
        if (_isKnockback)
        {
            _anim.UpdateAnimation(_playerState);
            return;
        }

        // neu dang bi thuong, knockback, dash hoac tan cong thi khong cho di chuyen
        if (_isHurt || _isKnockback || _isDashing || _isAttacking)
        {
            _rigi.velocity = new Vector2(0, _rigi.velocity.y);
            // dam bao hitbox ri luon tat neu khong con tan cong
            if (!_isAttacking && _axeAttack != null && _axeAttack.activeSelf)
                _axeAttack.SetActive(false);
            return;
        }

        if (_dashCooldownTimer > 0)
            _dashCooldownTimer -= Time.deltaTime; // giam thoi gian hoi dash

        if (_attackCooldownTimer > 0)
            _attackCooldownTimer -= Time.deltaTime; // giam thoi gian hoi attack

        // dam bao hitbox ri tat neu mat trang thai tan cong
        if (!_isAttacking && _axeAttack != null && _axeAttack.activeSelf)
            _axeAttack.SetActive(false);

        if (!_isHurt && !_isDead && !_isDashing && !_isAttacking)
        {
            if (Input.GetKeyDown(KeyCode.K) || Input.GetMouseButtonDown(2)) // phím hồi máu
                HealWithSoul();
        }
    }

    // cap nhat trang thai nhan vat dua vao cac co
    void UpdateState()
    {
        if (_isDashing)
        {
            _playerState = PlayerState.DASH;
            return;
        }
        if (_isAttacking)
        {
            _playerState = PlayerState.ATTACK;
            return;
        }
        if (_isHurt)
        {
            _playerState = PlayerState.HURT;
            return;
        }
        if (_isDead)
        {
            _playerState = PlayerState.DEATH;
            return;
        }
        if (!_isOnGrounded)
        {
            _playerState = _rigi.velocity.y > 0 ? PlayerState.JUMP : PlayerState.FALL;
        }
        else
        {
            _playerState = _rigi.velocity.x != 0 ? PlayerState.RUN : PlayerState.IDLE;
        }
    }

    // xu ly di chuyen ngang va quay mat nhan vat
    void Moving()
    {
        Vector2 movement = _rigi.velocity;
        movement.x = Input.GetAxisRaw("Horizontal") * _speed; // lay input ngang
        _rigi.velocity = movement;

        // quay mat nhan vat theo huong di chuyen
        if (_rigi.velocity.x > 0)
        {
            var scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
        else if (_rigi.velocity.x < 0)
        {
            var scale = transform.localScale;
            scale.x = -Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
    }

    // kiem tra va xu ly nhay
    void JumpCheck()
    {
        // nhan space de nhay, chi duoc nhay toi da _maxJumpCount lan
        if (Input.GetKeyDown(KeyCode.Space) && _currentJumpCount < _maxJumpCount && !_isDashing)
        {
            // Nếu đang rơi → chỉ cho nhảy 1 lần
            if (_rigi.velocity.y < 0)
                _currentJumpCount = _maxJumpCount - 1; // nghĩa là chỉ còn 1 jump cuối
            else if (_isOnGrounded)
                _currentJumpCount = 0;  // trên đất thì reset
                                        // nếu đang bay lên thì giữ nguyên currentJumpCount

            _rigi.velocity = new Vector2(_rigi.velocity.x, 0); // reset van toc Y
            _rigi.AddForce(new Vector2(0, _jumpForce)); // them luc nhay
            _isOnGrounded = false;
            _currentJumpCount++;
            //Debug.LogError("Jump Count: " + _currentJumpCount); 
        }

        // dieu chinh toc do roi va nhay thap
        if (_rigi.velocity.y < 0)
        {
            _rigi.velocity += Vector2.up * Physics2D.gravity.y * (_fallMultiplier - 1) * Time.deltaTime;
        }
        else if (_rigi.velocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            _rigi.velocity += Vector2.up * Physics2D.gravity.y * (_lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    // kiem tra va xu ly dash
    void TryDash()
    {
        if ((Input.GetKeyDown(KeyCode.LeftShift) || Input.GetMouseButtonDown(1)) && _dashCooldownTimer <= 0)
        {
            // neu dang tan cong thi huy tan cong khi bat dau dash
            if (_isAttacking)
                CancelAttack();
            StartCoroutine(DashCoroutine());
        }
    }

    // coroutine xu ly dash
    IEnumerator DashCoroutine()
    {
        _isDashing = true;
        _dashCooldownTimer = _dashCooldown;

        // bat trail khi dash
        if (_dashTrail != null)
            _dashTrail.emitting = true;

        float dashDirection = transform.localScale.x > 0 ? 1 : -1;
        _rigi.gravityScale = 0; // tat gravity khi dash

        float dashTime = 0f;
        while (dashTime < _dashDuration)
        {
            float horizontalInput = Input.GetAxisRaw("Horizontal");
            // dung dash neu nguoi choi thay doi huong
            if ((dashDirection > 0 && horizontalInput < 0) || (dashDirection < 0 && horizontalInput > 0))
                break;

            _rigi.velocity = new Vector2(dashDirection * _dashSpeed, 0);
            dashTime += Time.deltaTime;
            yield return null;
        }

        // khoi phuc gravity sau khi dash
        _rigi.gravityScale = _normalGravity;
        _isDashing = false;

        // tat trail khi ket thuc dash
        if (_dashTrail != null)
            _dashTrail.emitting = false;
    }

    // kiem tra va xu ly tan cong
    void TryAttack()
    {
        if (_isDashing || _isAttacking || _attackCooldownTimer > 0)
            return;

        // nhan input tan cong
        if (Input.GetKeyDown(KeyCode.J) || Input.GetMouseButtonDown(0))
        {
            _attackCoroutine = StartCoroutine(AttackCoroutine());
        }
    }

    // coroutine xu ly tan cong
    IEnumerator AttackCoroutine()
    {
        _isAttacking = true;
        _attackCooldownTimer = _attackCooldown; // bat dau cooldown
        _rigi.velocity = Vector2.zero; // dung nhan vat lai khi tan cong
        yield return new WaitForSeconds(_attackDuration);
        // ket thuc tan cong
        CancelAttack();
    }

    // ham huy tan cong, tat hitbox
    private void CancelAttack()
    {
        _isAttacking = false;
        if (_attackCoroutine != null)
        {
            StopCoroutine(_attackCoroutine);
            _attackCoroutine = null;
        }
        if (_axeAttack != null && _axeAttack.activeSelf)
            _axeAttack.SetActive(false);
    }

    // bat hitbox, goi tu animation event
    public void EnableAxeAttack()
    {
        if (_axeAttack != null)
            _axeAttack.SetActive(true);
    }

    // tat hitbox, goi tu animation event
    public void DisableAxeAttack()
    {
        if (_axeAttack != null)
            _axeAttack.SetActive(false);
    }

    // nhan sat thuong, xu ly I-frame va shake camera
    public void TakeDamage(int damage, Transform enemyPos)
    {
        if (_isDead || _isInvincible)
            return;

        _currentHealth -= damage;
        UIManager.Instance.UpdateHealthSlider(_currentHealth, _maxHealth); // cap nhat UI mau
        //Debug.LogError("Player Health: " + _currentHealth);

        if (_isAttacking)
        {
            // huy tan cong ngay lap tuc neu dang tan cong
            CancelAttack();
        }

        if (_currentHealth <= 0)
        {
            _isDead = true;
            _rigi.velocity = Vector2.zero; // dung nhan vat lai khi chet
            CancelAttack(); // tat hitbox khi chet
            return;
        }

        _isHurt = true;
        StartCoroutine(DoKnockback(enemyPos)); // xu ly knockback
        StartCoroutine(ResetHurtState()); // reset trang thai hurt
        StartCoroutine(InvincibilityCoroutine()); // bat invincibility tam thoi
        _cameraController?.Shake(_cameraShakeDuration, _cameraShakeMagnitude); // shake camera

        PlayerPrefs.SetInt(CONSTANT.LAST_CHECKPOINT_HEALTH, _currentHealth); // luu mau hien tai o checkpoint
    }

    // coroutine vo hieu hoa nhan sat thuong tam thoi
    private IEnumerator InvincibilityCoroutine()
    {
        _isInvincible = true;
        yield return new WaitForSeconds(_invincibleDuration);
        _isInvincible = false;
    }

    // coroutine reset trang thai bi thuong
    private IEnumerator ResetHurtState()
    {
        yield return new WaitForSeconds(0.4f);
        _isHurt = false;
    }

    // coroutine xu ly knockback khi bi danh
    private IEnumerator DoKnockback(Transform enemyPos)
    {
        _isKnockback = true;
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

    // kiem tra cham dat de reset so lan nhay
    private void OnCollisionEnter2D(Collision2D collision)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.8f);
        if (hit.collider != null)
        {
            _isOnGrounded = true;
            _currentJumpCount = 0;
        }
    }

    // ham hoi mau, goi tu item heal
    public void GainSoul(int amount)
    {
        _currentSoul += amount;
        if (_currentSoul > _maxSoul)
            _currentSoul = _maxSoul;

        UIManager.Instance.UpdateSoulSlider(_currentSoul, _maxSoul); // cap nhat UI soul

        //Debug.Log("Soul: " + _currentSoul + "/" + _maxSoul);
    }

    // ham hoi mau bang nang luong
    public void HealWithSoul()
    {
        // dieu kien de hoi mau
        if (_isDead || _isHealing || _currentSoul < _maxSoul || _currentHealth >= _maxHealth)
            return;

        _isHealing = true;

        if (_currentSoul == _maxSoul)
        {
            _currentHealth += _healAmount;
        }

        if (_currentHealth > _maxHealth)
        {
            _currentHealth = _maxHealth;
        }

        // hien thi hieu ung hoi mau
        if (_healVFX != null)
        {
            Instantiate(_healVFX, transform.position, Quaternion.identity);
        }

        _currentSoul = 0;
        UIManager.Instance.UpdateSoulSlider(_currentSoul, _maxSoul);
        UIManager.Instance.UpdateHealthSlider(_currentHealth, _maxHealth);
        //Debug.Log("Healed! Health: " + _currentHealth + ", Soul: " + _currentSoul);

        StartCoroutine(ResetHealState());
    }

    private IEnumerator ResetHealState()
    {
        yield return new WaitForSeconds(_healCooldown);
        _isHealing = false;
    }

    // ham tang max HP
    public void PlusMaxHP()
    {
        // kiem tra diem nang cap
        if (GameManager.Instance.UpgradePoints <= 0)
            return;

        GameManager.Instance.AddUpgradePoint(-1); // tru diem nang cap
        _maxHealth +=GameManager.Instance.HpUpgradeCost; // tang max HP
        PlayerPrefs.SetInt(CONSTANT.MAX_HEALTH, _maxHealth); // luu max HP
        UIManager.Instance.UpdateHealthSlider(_currentHealth, _maxHealth); // cap nhat UI mau
        UIManager.Instance.ShowHPText(_maxHealth); // hien thi text max HP
    }

    // ham tang luc hoi mau
    public void PlusHealAmount()
    {
        // kiem tra diem nang cap
        if (GameManager.Instance.UpgradePoints <= 0)
            return;

        // tru diem nang cap
        GameManager.Instance.AddUpgradePoint(-1);
        _healAmount += GameManager.Instance.HealUpgradeCost; // tang luc hoi mau
        PlayerPrefs.SetInt(CONSTANT.HEAL_AMOUNT, _healAmount); // luu luc hoi mau
        UIManager.Instance.ShowHealText(_healAmount); // hien thi text luc hoi mau
    }


    // enum trang thai nhan vat
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