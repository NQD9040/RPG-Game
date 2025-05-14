using UnityEngine;
using System.Collections;

public class SceneFadeIn : MonoBehaviour
{
    [SerializeField] private CanvasGroup fadeCanvasGroup; // Tham chiếu đến CanvasGroup của FadeImage
    [SerializeField] private float fadeDuration = 1f; // Thời gian fade (giây)

    void Start()
    {
        // Kiểm tra CanvasGroup
        if (fadeCanvasGroup == null)
        {
            Debug.LogWarning("Fade CanvasGroup is not assigned in the Inspector!");
            return;
        }

        // Đảm bảo alpha ban đầu là 1 (màn hình đen)
        fadeCanvasGroup.alpha = 1f;

        // Bắt đầu fade in
        StartCoroutine(FadeIn());
    }

    // Coroutine để fade in (từ đen sang trong suốt)
    private IEnumerator FadeIn()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            yield return null;
        }
        fadeCanvasGroup.alpha = 0f; // Đảm bảo alpha là 0 khi hoàn tất
    }
}