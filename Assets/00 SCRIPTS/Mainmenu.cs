using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class UIController : MonoBehaviour
{
    [Header("Containers")]
    public GameObject buttonsRoot;      // chứa 3 nút CHƠI / HƯỚNG DẪN / THOÁT
    public GameObject guidePanel;       // panel hướng dẫn (ban đầu inactive)

    [Header("Buttons")]
    public Button playButton;
    public Button guideButton;
    public Button exitButton;           // exit app (main menu)
    public Button guideCloseButton;     // nút đóng trên panel hướng dẫn

    [Header("Settings")]
    public string playSceneName = "Scene1"; // tên scene để load

    // Optional: canvas group for fade (nếu bạn muốn animation)
    public CanvasGroup guidePanelCanvasGroup;
    public float fadeDuration = 0.2f;

    void Start()
    {
        // đảm bảo trạng thái ban đầu
        if (guidePanel != null) guidePanel.SetActive(false);

        // register listeners
        if (playButton != null) playButton.onClick.AddListener(OnPlayClicked);
        if (guideButton != null) guideButton.onClick.AddListener(OnGuideClicked);
        if (exitButton != null) exitButton.onClick.AddListener(OnExitClicked);
        if (guideCloseButton != null) guideCloseButton.onClick.AddListener(OnCloseGuideClicked);
    }

    void OnPlayClicked()
    {
        // load scene - lưu ý: add scene vào Build Settings
        if (!string.IsNullOrEmpty(playSceneName))
            SceneManager.LoadScene(playSceneName);
        else
            Debug.LogWarning("Play scene name not set in UIController.");
    }

    void OnGuideClicked()
    {
        // Ẩn buttons root và hiện panel
        if (buttonsRoot != null) buttonsRoot.SetActive(false);

        if (guidePanel != null)
        {
            guidePanel.SetActive(true);

            // nếu có CanvasGroup, fade in
            if (guidePanelCanvasGroup != null)
            {
                guidePanelCanvasGroup.alpha = 0f;
                StartCoroutine(FadeCanvasGroup(guidePanelCanvasGroup, 0f, 1f, fadeDuration));
            }
        }

        // set focus sang nút đóng để điều khiển bàn phím/joystick tốt
        if (guideCloseButton != null)
            EventSystem.current.SetSelectedGameObject(guideCloseButton.gameObject);
    }

    void OnCloseGuideClicked()
    {
        // Ẩn panel, hiện lại buttons
        if (guidePanelCanvasGroup != null)
        {
            StartCoroutine(FadeOutAndHide());
        }
        else
        {
            if (guidePanel != null) guidePanel.SetActive(false);
            if (buttonsRoot != null) buttonsRoot.SetActive(true);
        }

        // set focus quay lại nút CHƠI (tùy)
        if (playButton != null)
            EventSystem.current.SetSelectedGameObject(playButton.gameObject);
    }

    void OnExitClicked()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // chỉ trong Editor
#endif
    }

    IEnumerator FadeCanvasGroup(CanvasGroup cg, float from, float to, float duration)
    {
        float elapsed = 0f;
        cg.alpha = from;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            cg.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }
        cg.alpha = to;
    }

    IEnumerator FadeOutAndHide()
    {
        if (guidePanelCanvasGroup != null)
        {
            yield return FadeCanvasGroup(guidePanelCanvasGroup, guidePanelCanvasGroup.alpha, 0f, fadeDuration);
        }
        if (guidePanel != null) guidePanel.SetActive(false);
        if (buttonsRoot != null) buttonsRoot.SetActive(true);
    }

}
