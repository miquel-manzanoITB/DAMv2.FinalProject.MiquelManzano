using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Managers
{
    /// <summary>
    /// Handles all level/scene transitions. Keeps GameManager focused on game state only.
    /// Singleton — persists across scenes. Place on the same persistent GameObject as GameManager.
    /// </summary>
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager Instance { get; private set; }

        // ── Events ────────────────────────────────────────────────────────────────

        /// <summary>Fires before a scene is loaded. Useful to show a loading screen.</summary>
        public static event UnityAction OnBeforeSceneLoad;

        /// <summary>Fires after a scene finishes loading.</summary>
        public static event UnityAction<string> OnSceneLoaded;

        // ── Config ────────────────────────────────────────────────────────────────

        [Header("Scene Names")]
        public string mainMenuScene = "MainMenu";

        // ── Singleton ─────────────────────────────────────────────────────────────

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        void OnEnable()  => SceneManager.sceneLoaded += HandleSceneLoaded;
        void OnDisable() => SceneManager.sceneLoaded -= HandleSceneLoaded;

        // ── Public API ────────────────────────────────────────────────────────────

        public void LoadLevel(string sceneName)
        {
            GameManager.Instance?.SetPause(false);
            OnBeforeSceneLoad?.Invoke();
            SceneManager.LoadScene(sceneName);
        }

        public void LoadMainMenu() => LoadLevel(mainMenuScene);

        public void ReloadCurrentLevel() => LoadLevel(SceneManager.GetActiveScene().name);

        public bool IsMainMenu()
            => SceneManager.GetActiveScene().name == mainMenuScene;

        // ── Internal ──────────────────────────────────────────────────────────────

        void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            OnSceneLoaded?.Invoke(scene.name);
        }
    }
}