using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    // Bạn đã đặt giá trị này trong Inspector là "Level2"
    public string nextSceneName;

    // PHẢI LÀ OnTriggerEnter2D (chứ không phải OnTriggerEnter)
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Kiểm tra Tag. Đảm bảo Player có Tag là "Player" (viết hoa chữ P)
        if (other.CompareTag("Player"))
        {
            Debug.Log("Trigger đã kích hoạt! Chuyển sang Scene: " + nextSceneName);
            SceneManager.LoadScene(nextSceneName);
        }
    }
}