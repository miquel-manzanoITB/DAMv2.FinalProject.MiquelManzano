using Player;
using UnityEngine;
using UnityEngine.Events;

namespace Managers
{
    /// <summary>
    /// Handles game state: pause and global events.
    /// Scene transitions have moved to LevelManager.
    /// Singleton — persists across scenes.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        // ── Events ────────────────────────────────────────────────────────────────

        public static event UnityAction<bool> OnPauseChanged;
        public static event UnityAction OnGameStart;

        // ── State ─────────────────────────────────────────────────────────────────

        public bool IsPaused { get; private set; }

        // ── Singleton ─────────────────────────────────────────────────────────────

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        void OnEnable()
        {
            PlayerInputController.OnPauseEvent += GoMainMenu;
            LevelManager.OnSceneLoaded += OnSceneLoaded;
        }

        void OnDisable()
        {
            PlayerInputController.OnPauseEvent -= GoMainMenu;
            LevelManager.OnSceneLoaded -= OnSceneLoaded;
        }

        void GoMainMenu()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            LevelManager.Instance?.LoadMainMenu();
        } 

        // ── Pause ─────────────────────────────────────────────────────────────────

        public void TogglePause() => SetPause(!IsPaused);

        public void SetPause(bool paused)
        {
            if (IsPaused == paused) return;

            IsPaused = paused;
            Cursor.lockState = paused ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = paused;

            OnPauseChanged?.Invoke(paused);
        }

        public void LoadScene(string sceneName) => LevelManager.Instance?.LoadLevel(sceneName);

        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        // ── Internal ──────────────────────────────────────────────────────────────

        void OnSceneLoaded(string sceneName)
        {
            bool isGameScene = !LevelManager.Instance.IsMainMenu();
            if (isGameScene) OnGameStart?.Invoke();
        }
    }
}