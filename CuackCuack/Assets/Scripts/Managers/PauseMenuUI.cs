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

        public void OnLevelMenuClicked()
        {
            GameManager.Instance?.SetPause(false);
            UIManager.Instance?.ShowLevelMenu(true);
        }

        public void OnMainMenuClicked()
        {
            GameManager.Instance?.LoadMainMenu();
        }

        public void OnQuitClicked()
        {
            GameManager.Instance?.QuitGame();
        }
    }
}
