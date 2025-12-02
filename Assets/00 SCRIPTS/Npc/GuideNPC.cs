using UnityEngine;

public class GuideNPC : MonoBehaviour
{
    [Header("UI & Dialogue")]
    [SerializeField] private GameObject interactPrompt; // Nhắc nhở [Nhấn E]

    [TextArea(3, 10)] // Cho phép nhập lời thoại trực tiếp trong Inspector
    [SerializeField] private string npcDialogue = "Welcome to this journey! Let’s explore the world around us.";

    private bool playerInRange = false;

    void Update()
    {
        // Chỉ xử lý tương tác khi Player ở gần và nhấn E
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }

    private void Interact()
    {
        if (DialogueManager.Instance != null)
        {
            // 1. TẮT thông báo tương tác (Prompt)
            if (interactPrompt != null)
            {
                interactPrompt.SetActive(false);
            }

            // 2. Mở Panel Lời thoại
            DialogueManager.Instance.StartDialogue(npcDialogue);
        }
    }

    // --- Xử lý Player vào/ra vùng tương tác (Collider Is Trigger) ---

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // So sánh Tag của đối tượng va chạm là "Player"
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player ENTERED range: Prompt should be ON");
            playerInRange = true;
            // Hiển thị Prompt
            if (interactPrompt != null) interactPrompt.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;

            // Ẩn Prompt
            if (interactPrompt != null)
                interactPrompt.SetActive(false);

            // Tắt Panel Lời thoại (đề phòng Player đi ra trong khi đối thoại đang mở)
            if (DialogueManager.Instance != null)
                DialogueManager.Instance.EndDialogue();
        }
    }
}