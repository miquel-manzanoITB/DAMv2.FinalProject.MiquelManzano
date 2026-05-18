using System.Collections;
using TMPro;
using UnityEngine;

namespace Managers
{
    /// <summary>
    /// Shows a brief text message on screen (e.g. "You found the glasses!").
    /// Call UIManager.Instance.ShowMessage("text") from anywhere.
    /// Attach this to a child of the HUD canvas.
    /// </summary>
    public class ScreenMessageUI : MonoBehaviour
    {
        [Header("References")]
        public TextMeshProUGUI messageText;

        [Header("Settings")]
        public float displayDuration = 3f;
        public float fadeDuration = 0.5f;

        // ── Internal ──────────────────────────────────────────────────────────────

        private Coroutine _showCoroutine;

        void Awake()
        {
            if (messageText) messageText.alpha = 0f;
        }

        // ── Public API ────────────────────────────────────────────────────────────

        public void Show(string message)
        {
            if (_showCoroutine != null) StopCoroutine(_showCoroutine);
            _showCoroutine = StartCoroutine(ShowRoutine(message));
        }

        // ── Private ───────────────────────────────────────────────────────────────

        IEnumerator ShowRoutine(string message)
        {
            messageText.text = message;

            // Fade in
            yield return Fade(0f, 1f, fadeDuration);

            // Hold
            yield return new WaitForSeconds(displayDuration);

            // Fade out
            yield return Fade(1f, 0f, fadeDuration);
        }

        IEnumerator Fade(float from, float to, float duration)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                messageText.alpha = Mathf.Lerp(from, to, elapsed / duration);
                yield return null;
            }
            messageText.alpha = to;
        }
    }
}
