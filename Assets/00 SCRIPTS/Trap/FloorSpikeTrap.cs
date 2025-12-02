using UnityEngine;

public class FloorSpikeTrap : MonoBehaviour
{
	[Header("Settings")]
	[SerializeField] private int damage = 1; // L??ng máu m?t

    [Header("Âm thanh")] 
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip activateSound;

    private void Start()
    {
        // Khởi tạo/Tìm kiếm AudioSource
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
	{
		// 1. Tìm Player (tìm ? c? cha ?? ch?c ch?n b?t ???c)
		PlayerController player = collision.GetComponentInParent<PlayerController>();

		// 2. N?u ?úng là Player
		if (player != null)
		{
            if (audioSource != null && activateSound != null)
            {
                audioSource.PlayOneShot(activateSound);
            }

            // 3. G?i hàm TakeDamage có s?n c?a Player
            // - damage: Tr? máu
            // - transform: Truy?n v? trí c?a b?y ?? Player tính h??ng b?t ng??c l?i (Knockback)
            player.TakeDamage(damage, transform);
		}
	}
}