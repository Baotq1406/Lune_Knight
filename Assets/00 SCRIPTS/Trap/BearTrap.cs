using UnityEngine;
using System.Collections;

public class BearTrap : MonoBehaviour
{
    [Header("Cấu hình Bẫy")]
    [SerializeField] private int damage = 1;          // Sát thương
    [SerializeField] private float clampDelay = 0.6f; // Thời gian chờ Animation đóng (Frame va chạm / Sample Rate)
    [SerializeField] private float resetTime = 2.0f;  // Thời gian bẫy giữ trạng thái đóng trước khi mở lại

    [Header("Âm thanh")] 
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip clampSound;    // Âm thanh bẫy đóng/kẹp

    // Các biến nội bộ
    private Animator anim;
    private bool isTriggered = false;

    private void Start()
    {
        anim = GetComponent<Animator>();

        // Tìm AudioSource nếu chưa được gán trong Inspector
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. Nếu bẫy đang hoạt động thì bỏ qua
        if (isTriggered) return;

        // 2. Tìm Player (Tìm ở cả cha để chắc chắn thấy)
        PlayerController player = collision.GetComponentInParent<PlayerController>();

        if (player != null)
        {
            StartCoroutine(TrapRoutine(player));
        }
    }

    // Coroutine xử lý toàn bộ quy trình
    IEnumerator TrapRoutine(PlayerController player)
    {
        isTriggered = true; // Khóa bẫy để không trigger nhiều lần

        // --- BƯỚC 1: BẮT GIỮ PLAYER ---
        // Tắt não (Script) của Player để ngắt input di chuyển ngay lập tức
        player.enabled = false;

        // Dừng vận tốc vật lý để Player không bị trượt đi
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }

        // (Mẹo Visual) Dịch chuyển Player vào chính giữa bẫy cho khớp hình ảnh
        // Giữ nguyên Y, chỉ chỉnh X theo vị trí của bẫy
        player.transform.position = new Vector3(transform.position.x, player.transform.position.y, player.transform.position.z);

        // --- BƯỚC 2: CHẠY ANIMATION ---
        if (anim != null) anim.SetTrigger("Activate");

        if (audioSource != null && clampSound != null)
        {
            audioSource.PlayOneShot(clampSound);
        }

        // --- BƯỚC 3: CHỜ KẸP ---
        // Đợi animation chạy đến frame đóng nắp (VD: 0.6 giây)
        yield return new WaitForSeconds(clampDelay);

        // --- BƯỚC 4: GÂY SÁT THƯƠNG ---
        if (player != null)
        {
            // Bật lại não cho Player trước khi gây damage
            // (Để Player có thể xử lý logic bị thương/Knockback trong hàm TakeDamage)
            player.enabled = true;

            // Gây sát thương + Đẩy lùi
            player.TakeDamage(damage, transform);

            Debug.Log("Player đã bị kẹp.");
        }

        // --- BƯỚC 5: CHỜ HỒI CHIÊU ---
        // Giữ trạng thái bẫy đóng một lúc cho ngầu
        yield return new WaitForSeconds(resetTime);

        // --- BƯỚC 6: RESET BẪY ---
        if (anim != null) anim.SetTrigger("Reset"); // Mở bẫy ra
        isTriggered = false; // Cho phép dẫm lại lần sau
    }
}