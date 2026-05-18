using UnityEngine;

namespace Managers
{
    /// <summary>
    /// Attach to the PausePanel. Wire the buttons in the Inspector.
    /// </summary>
    public class PauseMenuUI : MonoBehaviour
    {
        public void OnResumeClicked()
        {
            GameManager.Instance?.SetPause(false);
        }

        /// <summary>
        /// Lleva al jugador al menú principal. Desde allí puede seleccionar nivel.
        /// </summary>
        public void OnMainMenu()
        {
            GameManager.Instance?.SetPause(false);
            LevelManager.Instance?.LoadMainMenu();
        }

        public void OnQuitClicked()
        {
            GameManager.Instance?.QuitGame();
        }
    }
}