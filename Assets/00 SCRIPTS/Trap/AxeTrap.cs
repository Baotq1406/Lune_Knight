using UnityEngine;
using System.Collections;

public class AxeTrap : MonoBehaviour
{
    [Header("Cài đặt Rìu")]
    [SerializeField] private int damage = 20;
    [SerializeField] private float activeDuration = 2.0f; // Rìu lắc lư trong bao lâu thì dừng

    private Animator anim;
    private bool isActive = false;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Hàm này được cái Nút gọi
    public void ActivateTrap()
    {
        // Nếu đang chạy thì không chạy lại
        if (isActive) return;

        StartCoroutine(ActivateRoutine());
    }

    IEnumerator ActivateRoutine()
    {
        isActive = true;

        // Kích hoạt: Bắt đầu lắc lư
        if (anim != null) anim.SetTrigger("Activate");

        // Chờ: Để rìu lắc lư một lúc (VD: 2 giây)
        yield return new WaitForSeconds(activeDuration);

        // Reset: Bắt buộc dừng lại và quay về Idle
        if (anim != null) anim.SetTrigger("Reset");

        isActive = false; // Cho phép kích hoạt lại lần sau
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isActive) return;

        PlayerController player = collision.GetComponentInParent<PlayerController>();
        if (player != null)
        {
            player.TakeDamage(damage, transform);
        }
    }
}