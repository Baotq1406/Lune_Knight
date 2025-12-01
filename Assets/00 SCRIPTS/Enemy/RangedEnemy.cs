using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemy : MonoBehaviour
{
    #region Attack Parameters
    [Header("Attack Parameters")]
    [SerializeField] private float _attackCooldown; // thoi gian cho giua cac lan tan cong
    [SerializeField] private float _range;           // tam nhin de tan cong
    [SerializeField] private Transform _firePoint;   // vi tri ban mui ten
    [SerializeField] private ArrowBase _arrowPrefab; // prefab mui ten
    #endregion

    #region Collider Parameters
    [Header("Collider Parameters")]
    [SerializeField] private float _colliderDistance; // khoang cach khoi collider ban dau
    [SerializeField] private BoxCollider2D boxCollider; // collider cua enemy
    #endregion

    #region Player Layer
    [Header("Player Layer")]
    [SerializeField] private LayerMask playerLayer; // layer cua player
    private float cooldownTimer = Mathf.Infinity;   // bo dem cooldown tan cong
    #endregion

    #region References
    [Header("References")]
    [SerializeField] private Animator _anim;         // animator enemy
    [SerializeField] private EnemyPatrol enemyPatrol; // script di chuyen cua enemy
    Rigidbody2D _rigi;                                // rigidbody enemy
    #endregion

    [Header("Knockback Settings")]
    [SerializeField] private float _knockbackForce = 3f;      // luc day lui khi bi tan cong
    [SerializeField] private float _knockbackDuration = 0.2f; // thoi gian knockback
    private bool _isKnockback = false;                        // trang thai dang bi knockback

    [SerializeField] private int _enemyHealth = 100; // mau enemy
    private bool _isDead = false;                   // trang thai chet

    private void Awake()
    {
        _rigi = GetComponent<Rigidbody2D>(); // lay component rigidbody
    }

    private void Update()
    {
        // dung tat ca logic neu enemy da chet
        if (_isDead) return;

        cooldownTimer += Time.deltaTime; // tang cooldown theo thoi gian

        if (GameManager.Instance.Player.isDeadPlayer)
            return;

        // can chinh firePoint theo huong ma enemy dang quay
        AlignFirePointToFacing();

        // neu player trong tam va khong bi knockback thi tan cong
        if (PlayerInSight() && !_isKnockback)
        {
            if (cooldownTimer >= _attackCooldown)
            {
                cooldownTimer = 0; // reset cooldown
                _anim.SetTrigger(CONSTANT.RANGED_ATTACK); // animation ban mui ten
            }
        }

        // tat patrol khi nhin thay player hoac dang knockback
        if (enemyPatrol != null)
            enemyPatrol.enabled = !PlayerInSight() && !_isKnockback && !_isDead;
    }

    // ham giup firePoint luon quay cung huong voi enemy
    private void AlignFirePointToFacing()
    {
        if (_firePoint == null) return;

        // neu enemy quay phai => scale.x > 0
        // quay trai => scale.x < 0
        _firePoint.right = Vector3.right * Mathf.Sign(transform.localScale.x);
    }

    // kiem tra xem player co trong tam tan cong hay khong
    private bool PlayerInSight()
    {
        if (boxCollider == null) return false;

        RaycastHit2D hit =
            Physics2D.BoxCast(
                boxCollider.bounds.center + transform.right * _range * this.transform.localScale.x * _colliderDistance,
                new Vector3(boxCollider.bounds.size.x * _range, boxCollider.bounds.size.y, boxCollider.bounds.size.z),
                0,
                Vector2.left,
                0,
                playerLayer);

        return hit.collider != null;
    }

    // ve hinh chu nhat de debug tam nhin tren editor
    private void OnDrawGizmos()
    {
        if (boxCollider == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(
            boxCollider.bounds.center + transform.right * _range * this.transform.localScale.x * _colliderDistance,
            new Vector3(boxCollider.bounds.size.x * _range, boxCollider.bounds.size.y, boxCollider.bounds.size.z));
    }

    // ham duoc goi boi animation event de ban mui ten
    public void ShootArrow()
    {
        if (_arrowPrefab == null || _firePoint == null) return;

        // lay doi tuong tu object pooling
        GameObject arrows = ObjectPoolingX.Instance.GetObject(_arrowPrefab.gameObject);

        arrows.transform.position = _firePoint.position; // dat vi tri
        arrows.transform.rotation = _firePoint.rotation; // dat huong ban
        arrows.SetActive(true);                          // kich hoat mui ten
    }

    // ham enemy nhan sat thuong
    public void TakeDamage(int damage)
    {
        if (_isDead) return;

        _enemyHealth -= damage;
        //Debug.LogError("Ranged Health enemy:" + _enemyHealth);
        _anim.SetTrigger(CONSTANT.RANGED_HURT);

        StartCoroutine(DoKnockback()); // bat dau knockback

        if (_enemyHealth <= 0)
        {
            _isDead = true;
            //Debug.LogError("Ranged Enemy Dead");
            GameManager.Instance.AddUpgradePoint(Random.Range(1, 3));
            _anim.ResetTrigger(CONSTANT.RANGED_ATTACK); // reset trigger tan cong
            StartCoroutine(Die()); // bat dau animation chet
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

    // coroutine knockback - day enemy ra xa player
    private IEnumerator DoKnockback()
    {
        _isKnockback = true;

        // Kiểm tra xem player có ở phía sau không trước khi knockback
        bool wasPlayerBehind = IsPlayerBehind();

        // tinh huong day lui (nguoc voi huong player)
        Transform player = GameManager.Instance.Player.transform;
        float dir = transform.position.x < player.position.x ? -1f : 1f;
        float timer = 0f;

        // ap dung luc knockback trong khoang thoi gian
        while (timer < _knockbackDuration)
        {
            if (_rigi != null)
                _rigi.velocity = new Vector2(dir * _knockbackForce, _rigi.velocity.y);
            timer += Time.deltaTime;
            yield return null;
        }

        // dung lai sau khi het thoi gian knockback
        if (_rigi != null)
            _rigi.velocity = new Vector2(0, _rigi.velocity.y);

        // Chỉ quay mặt về phía người chơi nếu bị tấn công từ phía sau
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

    // coroutine chet - choi animation roi tat doi tuong
    private IEnumerator Die()
    {
        _anim.SetTrigger(CONSTANT.RANGED_DEATH);
        yield return new WaitForSeconds(0.8f); // doi animation chet choi xong

        // tat doi tuong (uu tien tat parent neu co)
        if (this.transform.parent != null)
            this.transform.parent.gameObject.SetActive(false);
        else
            this.gameObject.SetActive(false);
    }
}
