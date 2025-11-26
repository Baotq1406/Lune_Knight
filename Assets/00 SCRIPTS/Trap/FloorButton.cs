using UnityEngine;
using System.Collections;

public class FloorButton : MonoBehaviour
{
    [Header("Kết nối")]
    [SerializeField] private AxeTrap axeTrap; // Kéo script cái Rìu vào đây

    [Header("Cài đặt")]
    [SerializeField] private float cooldown = 2.5f; // Thời gian chờ để được dẫm lần tiếp theo

    private bool canPress = true; // Biến kiểm tra xem có được dẫm không

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Nếu đang trong thời gian chờ (cooldown) thì không làm gì
        if (!canPress) return;

        // Kiểm tra Player
        if (collision.GetComponentInParent<PlayerController>() != null)
        {
            StartCoroutine(TriggerProcess());
        }
    }

    IEnumerator TriggerProcess()
    {
        canPress = false; // Khóa nút lại ngay lập tức
        Debug.Log("Đã dẫm vào nút!");

        // 1. Kích hoạt bẫy Rìu
        if (axeTrap != null)
        {
            axeTrap.ActivateTrap();
        }

        // 2. Chờ thời gian hồi chiêu (ví dụ 2.5 giây)
        // (Thời gian này nên lâu hơn thời gian Rìu lắc lư một chút)
        yield return new WaitForSeconds(cooldown);

        // 3. Mở khóa để dẫm lại được
        canPress = true;
    }
}