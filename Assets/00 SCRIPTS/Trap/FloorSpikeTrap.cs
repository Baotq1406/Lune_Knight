using UnityEngine;

public class FloorSpikeTrap : MonoBehaviour
{
	[Header("Cài ??t")]
	[SerializeField] private int damage = 1; // L??ng máu m?t

	private void OnTriggerEnter2D(Collider2D collision)
	{
		// 1. Tìm Player (tìm ? c? cha ?? ch?c ch?n b?t ???c)
		PlayerController player = collision.GetComponentInParent<PlayerController>();

		// 2. N?u ?úng là Player
		if (player != null)
		{
			// 3. G?i hàm TakeDamage có s?n c?a Player
			// - damage: Tr? máu
			// - transform: Truy?n v? trí c?a b?y ?? Player tính h??ng b?t ng??c l?i (Knockback)
			player.TakeDamage(damage, transform);

			Debug.Log("?ã ch?m vào chông!");
		}
	}
}