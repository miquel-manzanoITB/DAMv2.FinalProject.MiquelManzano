using UnityEngine;

namespace Managers
{
    /// <summary>
    /// Main menu UI. Attach to the MainMenu canvas root.
    /// Wire buttons in the Inspector to the public methods below.
    ///
    /// The main menu scene should have its own Canvas — no HUD, no pause panel.
    /// </summary>
    public class MainMenuUI : MonoBehaviour
    {
        [Header("Panels")]
        public GameObject mainPanel;    // Botones: Jugar, Niveles, Salir
        public GameObject levelPanel;   // LevelMenuUI vive aquí

        void Start()
        {
            ShowMain();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        // ── Button callbacks ──────────────────────────────────────────────────────

        /// <summary>Empieza desde el primer nivel.</summary>
        public void OnPlayClicked()
        {
            // LevelMenuData define cuál es el primer nivel.
            // Alternativamente hardcodea el nombre de escena aquí.
            var levelMenu = levelPanel.GetComponent<LevelMenuUI>();
            if (levelMenu != null && levelMenu.levelMenuData?.levels.Count > 0)
                LevelManager.Instance?.LoadLevel(levelMenu.levelMenuData.levels[0].sceneName);
        }

        /// <summary>Abre el panel de selección de niveles.</summary>
        public void OnLevelsClicked()
        {
            mainPanel.SetActive(false);
            levelPanel.SetActive(true);
        }

        /// <summary>Vuelve al panel principal desde el selector de niveles.</summary>
        public void OnBackClicked() => ShowMain();

        public void OnQuitClicked() => GameManager.Instance?.QuitGame();

        // ── Internal ──────────────────────────────────────────────────────────────

        void ShowMain()
        {
            mainPanel.SetActive(true);
            levelPanel.SetActive(false);
        }
    }
}