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
        void Start()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        // ── Button callbacks ──────────────────────────────────────────────────────

        /// <summary>Empieza desde el primer nivel.</summary>
        public void OnPlayClicked()
        {
            LevelManager.Instance?.LoadLevel("Level01");
        }

        /// <summary>Abre el panel de selección de niveles.</summary>

        /// <summary>Vuelve al panel principal desde el selector de niveles.</summary>

        public void OnQuitClicked() => GameManager.Instance?.QuitGame();

        // ── Internal ──────────────────────────────────────────────────────────────

    }
}