using UnityEngine;
using System.Collections;

public class StatueTrap : MonoBehaviour
{
    [Header("Cấu hình Khoảng Cách")]
    [Tooltip("Khoảng cách phát hiện bên TRÁI (Đi từ xa tới)")]
    [SerializeField] private float rangeLeft = 3.0f;

    [Tooltip("Khoảng cách phát hiện bên PHẢI (Đi sát lưng)")]
    [SerializeField] private float rangeRight = 1.0f;

    [Header("Cấu hình Sát thương")]
    [SerializeField] private int damage = 20;
    [SerializeField] private float attackDelay = 0.4f; // Canh cho khớp lúc kiếm chạm đất
    [SerializeField] private float cooldown = 2.0f;

    private Animator anim;
    private Transform playerTransform;
    private bool isAttacking = false;

    // Biến khóa: Đảm bảo chỉ chém 1 lần mỗi khi Player bước vào
    private bool hasAttackedSession = false;

    private void Start()
    {
        anim = GetComponent<Animator>();
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null) playerTransform = player.transform;
    }

    private void Update()
    {
        if (playerTransform == null || isAttacking) return;

        // 1. Tính toán vị trí
        float distanceX = playerTransform.position.x - transform.position.x;
        float distanceY = Mathf.Abs(playerTransform.position.y - transform.position.y);
        float absDistX = Mathf.Abs(distanceX);

        // 2. Kiểm tra xem có đang ở trong vùng kích hoạt không
        bool inRange = false;

        // Chỉ chém nếu cùng độ cao (lệch không quá 1.5m)
        if (distanceY < 1.5f)
        {
            if (distanceX < 0) // Bên Trái
            {
                if (absDistX <= rangeLeft) inRange = true;
            }
            else // Bên Phải
            {
                if (absDistX <= rangeRight) inRange = true;
            }
        }

        // 3. Xử lý Logic Chém
        if (inRange)
        {
            // Nếu chưa chém lần nào trong đợt này thì mới chém
            if (!hasAttackedSession)
            {
                // Quay mặt đúng hướng
                if (distanceX < 0) FaceLeft();
                else FaceRight();

                StartCoroutine(AttackRoutine());
            }
        }
        else
        {
            // 4. Logic Reset (Vùng đệm)
            // Chỉ khi Player đi ra xa hơn tầm đánh một chút (buffer 0.5m) thì mới reset
            // Để tránh bị lỗi chém lặp lại khi đứng ngay mép vạch
            float currentLimit = (distanceX < 0) ? rangeLeft : rangeRight;

            if (absDistX > currentLimit + 0.5f)
            {
                hasAttackedSession = false; // Reset để lần sau vào lại sẽ bị chém tiếp
            }
        }
    }

    void FaceRight() { transform.localScale = new Vector3(1, 1, 1); }
    void FaceLeft() { transform.localScale = new Vector3(-1, 1, 1); }

    IEnumerator AttackRoutine()
    {
        isAttacking = true;
        hasAttackedSession = true; // Đánh dấu là ĐÃ CHÉM

        if (anim != null) anim.SetTrigger("Attack");

        // Chờ kiếm vung xuống
        yield return new WaitForSeconds(attackDelay);

        // Gây sát thương
        if (playerTransform != null)
        {
            // Tính lại khoảng cách lúc kiếm chạm đất (để xem player chạy thoát chưa)
            float distX = playerTransform.position.x - transform.position.x;
            float absDist = Mathf.Abs(distX);

            // Kiểm tra kỹ: Đang chém bên nào thì check tầm bên đó
            // (Tránh lỗi chém bên phải mà trúng bên trái)
            bool hit = false;

            // Nếu đang quay trái (scale.x < 0)
            if (transform.localScale.x < 0 && distX < 0 && absDist <= rangeLeft + 0.5f) hit = true;

            // Nếu đang quay phải (scale.x > 0)
            if (transform.localScale.x > 0 && distX > 0 && absDist <= rangeRight + 0.5f) hit = true;

            if (hit)
            {
                playerTransform.GetComponent<PlayerController>().TakeDamage(damage, transform);
                Debug.Log("Chém trúng!");
            }
        }

        yield return new WaitForSeconds(cooldown);
        isAttacking = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red; // Trái
        Gizmos.DrawLine(transform.position, transform.position + Vector3.left * rangeLeft);
        Gizmos.DrawWireSphere(transform.position + Vector3.left * rangeLeft, 0.2f);

        Gizmos.color = Color.blue; // Phải
        Gizmos.DrawLine(transform.position, transform.position + Vector3.right * rangeRight);
        Gizmos.DrawWireSphere(transform.position + Vector3.right * rangeRight, 0.2f);
    }
}