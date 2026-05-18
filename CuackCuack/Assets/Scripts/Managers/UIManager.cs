using TMPro;
using UnityEngine;

namespace Managers
{
    /// <summary>
    /// Central UI manager. Handles HUD, pause menu, loading screen,
    /// notebook, screen messages and level selection menu.
    /// Singleton — persists across scenes.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("Core Panels")]
        public GameObject hudPanel;
        public GameObject pausePanel;
        public GameObject loadingPanel;
        public GameObject levelMenuPanel;

        [Header("HUD Elements")]
        public TextMeshProUGUI interactionHintText;

        [Header("Sub-systems (auto-found if null)")]
        public NotebookUI notebookUI;
        public ScreenMessageUI screenMessageUI;

        // ── Singleton ─────────────────────────────────────────────────────────────

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Auto-find sub-systems on the same GameObject if not wired in Inspector
            if (notebookUI == null) notebookUI = GetComponentInChildren<NotebookUI>(true);
            if (screenMessageUI == null) screenMessageUI = GetComponentInChildren<ScreenMessageUI>(true);
        }

        void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        void OnEnable()
        {
            //GameManager.OnPauseChanged += OnPauseChanged;
            GameManager.OnGameStart    += OnGameStart;
        }

        void OnDisable()
        {
            //GameManager.OnPauseChanged -= OnPauseChanged;
            GameManager.OnGameStart    -= OnGameStart;
        }

        void Start()
        {
            SetPanel(hudPanel,       true);
            SetPanel(pausePanel,     false);
            SetPanel(loadingPanel,   false);
            SetPanel(levelMenuPanel, false);
        }

        // ── Panel control ─────────────────────────────────────────────────────────

        void OnGameStart()
        {
            SetPanel(hudPanel,       true);
            SetPanel(pausePanel,     false);
            SetPanel(levelMenuPanel, false);
        }

        void OnPauseChanged(bool paused)
        {
            SetPanel(pausePanel, paused);
            SetPanel(hudPanel,   !paused);
        }

        public void ShowLoadingScreen(bool show) => SetPanel(loadingPanel, show);

        public void ShowLevelMenu(bool show)
        {
            SetPanel(levelMenuPanel, show);
            SetPanel(hudPanel, !show);
            Cursor.lockState = show ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = show;
        }

        static void SetPanel(GameObject panel, bool active)
        {
            if (panel != null) panel.SetActive(active);
        }

        // ── HUD helpers ───────────────────────────────────────────────────────────

        public void ShowInteractionHint(string message)
        {
            if (interactionHintText == null) return;
            interactionHintText.text = message;
            interactionHintText.gameObject.SetActive(!string.IsNullOrEmpty(message));
        }

        public void HideInteractionHint() => ShowInteractionHint(string.Empty);

        /// <summary>Shows a timed on-screen message (e.g. "Glasses found!").</summary>
        public void ShowMessage(string message)
        {
            screenMessageUI?.Show(message);
        }

        /// <summary>Refreshes the notebook task list after a task is completed.</summary>
        public void RefreshNotebook()
        {
            notebookUI?.RefreshTasks();
        }
    }
}
