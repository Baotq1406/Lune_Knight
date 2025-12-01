using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    // Singleton Instance
    public static DialogueManager Instance;

    [Header("UI References")]
    [SerializeField] private GameObject dialogueBackground; // Dùng để làm mờ UI nền
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueText;

    private void Awake()
    {
        // Khởi tạo Singleton: đảm bảo chỉ có một thể hiện
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void StartDialogue(string text)
    {
        // 1. Bật Background mờ (che thanh máu, v.v.)
        if (dialogueBackground != null) dialogueBackground.SetActive(true);

        // 2. Bật Panel và hiển thị nội dung
        if (dialoguePanel != null) dialoguePanel.SetActive(true);
        if (dialogueText != null) dialogueText.text = text;

        // *Tùy chọn: Thêm logic DỪNG Player input ở đây*
    }

    public void EndDialogue()
    {
        // 1. Tắt Panel
        if (dialoguePanel != null) dialoguePanel.SetActive(false);

        // 2. Tắt Background mờ
        if (dialogueBackground != null) dialogueBackground.SetActive(false);

        // *Tùy chọn: Thêm logic CHO PHÉP Player di chuyển ở đây*
    }
}