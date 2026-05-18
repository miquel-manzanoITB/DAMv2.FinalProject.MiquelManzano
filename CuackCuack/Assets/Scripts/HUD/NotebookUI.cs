using Player;
using TMPro;
using UnityEngine;

namespace Managers
{
    /// <summary>
    /// Notebook HUD toggled with Tab. Displays the task list styled like
    /// the Untitled Goose Game checklist — tasks cross out when completed.
    /// Attach to a Canvas child GameObject.
    /// </summary>
    public class NotebookUI : MonoBehaviour
    {
        [Header("Data")]
        [Tooltip("The TaskData ScriptableObject for this level.")]
        public TaskData taskData;

        [Header("References")]
        public GameObject notebookPanel;
        public Transform taskContainer;   // Vertical Layout Group parent
        public GameObject taskRowPrefab;  // Prefab with a TextMeshProUGUI child

        // ── Internal ──────────────────────────────────────────────────────────────

        private bool _isOpen;
        private Player.PlayerInputController _input;

        // ── Unity lifecycle ───────────────────────────────────────────────────────

        void Awake()
        {
            if (notebookPanel != null)
                notebookPanel.SetActive(false);

            // Awake is the right place: both this and PlayerInputController run Awake
            // before any Start, so FindFirstObjectByType is safe here.
            _input = FindFirstObjectByType<Player.PlayerInputController>();

            if (_input == null)
                Debug.LogWarning("[NotebookUI] PlayerInputController not found in scene. " +
                                 "Make sure the Player is in the same scene.");
        }

        // OnEnable/OnDisable now safely subscribe because _input is set in Awake,
        // which always runs before the first OnEnable.
        void OnEnable()
        {
            if (_input != null) _input.OnNotebookEvent += ToggleNotebook;
            GameManager.OnPauseChanged += OnPauseChanged;
        }

        void OnDisable()
        {
            if (_input != null) _input.OnNotebookEvent -= ToggleNotebook;
            GameManager.OnPauseChanged -= OnPauseChanged;
        }

        // ── Public API ────────────────────────────────────────────────────────────

        /// <summary>Call after completing a task to refresh the notebook display.</summary>
        public void RefreshTasks()
        {
            if (taskData == null) return;

            foreach (Transform child in taskContainer)
                Destroy(child.gameObject);

            foreach (var task in taskData.tasks)
            {
                var row = Instantiate(taskRowPrefab, taskContainer);
                var tmp = row.GetComponentInChildren<TextMeshProUGUI>();
                if (tmp == null) continue;

                tmp.text = task.description;

                if (task.isCompleted)
                {
                    tmp.fontStyle = FontStyles.Strikethrough;
                    tmp.color = new Color(0.4f, 0.4f, 0.4f);
                }
                else
                {
                    tmp.fontStyle = FontStyles.Normal;
                    tmp.color = Color.black;
                }
            }
        }

        public void CompleteTask(string description)
        {
            global::AudioManager.Instance?.PlaySFX("TaskDone");
            taskData.CompleteTask(description);
        }

        // ── Private methods ───────────────────────────────────────────────────────

        void ToggleNotebook()
        {
            Debug.Log("Toggle Notebook");
            global::AudioManager.Instance?.PlaySFX(_isOpen ? "CloseNotebook" : "OpenNotebook");
            if (GameManager.Instance != null && GameManager.Instance.IsPaused) return;
            SetOpen(!_isOpen);
        }

        void OnPauseChanged(bool paused)
        {
            if (paused && _isOpen) SetOpen(false);
        }

        void SetOpen(bool open)
        {
            _isOpen = open;

            notebookPanel.SetActive(open);
            Cursor.lockState = open ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = open;

            if (open) RefreshTasks();
        }
    }
}