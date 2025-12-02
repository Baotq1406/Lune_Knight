using UnityEngine;

public class FloorSpikeTrap : MonoBehaviour
{
	[Header("Settings")]
	[SerializeField] private int damage = 100; 

    [Header("Âm thanh")] 
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip activateSound;

    private void Start()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
	{
		// Tìm Player 
		PlayerController player = collision.GetComponentInParent<PlayerController>();

		if (player != null)
		{
            if (audioSource != null && activateSound != null)
            {
                audioSource.PlayOneShot(activateSound);
            }

            player.TakeDamage(damage, transform);
		}
	}
}