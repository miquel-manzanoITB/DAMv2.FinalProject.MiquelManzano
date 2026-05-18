using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
    /// <summary>
    /// Level selection menu. Instantiates a button per level defined in
    /// LevelMenuData and loads the corresponding scene via GameManager.
    ///
    /// Attach this to the LevelMenuPanel GameObject.
    /// </summary>
    public class LevelMenuUI : MonoBehaviour
    {
        [Header("Data")]
        public LevelMenuData levelMenuData;

        [Header("References")]
        public Transform buttonContainer;   // Vertical/Grid Layout Group
        public GameObject levelButtonPrefab; // Button with TextMeshProUGUI child

        void OnEnable()
        {
            BuildButtons();
        }

        void BuildButtons()
        {
            // Clear previous buttons
            foreach (Transform child in buttonContainer)
                Destroy(child.gameObject);

            if (levelMenuData == null) return;

            foreach (var level in levelMenuData.levels)
            {
                var go = Instantiate(levelButtonPrefab, buttonContainer);

                // Set label
                var label = go.GetComponentInChildren<TextMeshProUGUI>();
                if (label) label.text = level.displayName;

                // Wire click
                var btn = go.GetComponent<Button>();
                string sceneName = level.sceneName; // capture for lambda
                btn?.onClick.AddListener(() => LevelManager.Instance?.LoadLevel(sceneName));

                // Dim if locked
                if (!level.isUnlocked && btn)
                {
                    btn.interactable = false;
                    if (label) label.color = Color.gray;
                }
            }
        }

        public void Close()
        {
            Managers.UIManager.Instance?.ShowLevelMenu(false);
        }
    }
}