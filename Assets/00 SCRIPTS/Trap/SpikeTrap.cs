using UnityEngine;
using System.Collections;

public class SpikeTrap : MonoBehaviour
{
    [Header("Cấu hình Bẫy Chông")]
    [SerializeField] private int damage = 10;           
    [SerializeField] private float spikeUpDelay = 0.4f;// Thời gian chờ chông đâm lên
    [SerializeField] private float resetTime = 2.0f;   // Thời gian chông giữ trạng thái nhọn trước khi thụt xuống

    [Header("Âm thanh")] 
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip spikeUpSound;   

    private Animator anim;
    private bool isTriggered = false;

    private void Start()
    {
        anim = GetComponent<Animator>();

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isTriggered) return;

        PlayerController player = collision.GetComponentInParent<PlayerController>();

        if (player != null)
        {
            StartCoroutine(ActivateSpike(player));
        }
    }

    IEnumerator ActivateSpike(PlayerController player)
    {
        isTriggered = true;

        // Tắt script để player không đi được nữa
        player.enabled = false;

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }

        // chông đâm lên
        if (anim != null) anim.SetTrigger("Activate"); 

        if (audioSource != null && spikeUpSound != null)
        {
            audioSource.PlayOneShot(spikeUpSound);
        }

        // chờ chạm vào người
        yield return new WaitForSeconds(spikeUpDelay);

        // gây xác thương
        if (player != null)
        {
            // Bật lại não cho Player để xử lý Knockback
            player.enabled = true;

            // Truyền transform của bẫy để Player bị hất văng lên/ra xa
            player.TakeDamage(damage, transform);

            Debug.Log("Player đã bị chông đâm!");
        }

        // giữ gai ở trên cao
        yield return new WaitForSeconds(resetTime);

        // reset
        if (anim != null) anim.SetTrigger("Reset"); // Gai thụt xuống
        isTriggered = false;
    }
}