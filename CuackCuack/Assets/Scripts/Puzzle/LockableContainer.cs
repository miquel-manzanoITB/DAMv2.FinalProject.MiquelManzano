using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A chest, drawer or door that requires the player to have the matching key.
/// Extend onUnlocked with animations, sounds, or item spawning via the Inspector.
///
/// Requires: Interactable component on the same GameObject.
/// </summary>
[RequireComponent(typeof(Interactable))]
public class LockableContainer : MonoBehaviour
{
    [Header("Lock")]
    [Tooltip("Must match the keyId on the KeyItem that opens this.")]
    public string keyId;

    [Tooltip("If true, the key is consumed (removed from KeyRing) when used.")]
    public bool consumeKey = true;

    [Header("Messages")]
    public string lockedMessage   = "It's locked. I need a key.";
    public string unlockedMessage = "Unlocked!";

    [Header("Task")]
    [Tooltip("Task index to complete when unlocked. -1 = none.")]
    public int taskIndexToComplete = -1;

    [Header("Events")]
    public UnityEvent onUnlocked;   // Hook up animations, SFX, item spawns here
    public UnityEvent onLocked;     // Shown when the player tries without the key

    // ── Internal ──────────────────────────────────────────────────────────────

    private Interactable _interactable;
    private bool _isUnlocked;

    void Awake()
    {
        _interactable = GetComponent<Interactable>();
        _interactable.hintText = lockedMessage;
    }

    void OnEnable()
    {
        _interactable.onInteract.AddListener(TryOpen);
    }

    void OnDisable()
    {
        _interactable.onInteract.RemoveListener(TryOpen);
    }

    void TryOpen()
    {
        if (_isUnlocked) return;

        var player = FindFirstObjectByType<KeyRing>();
        if (player == null) return;

        if (!player.HasKey(keyId))
        {
            Managers.UIManager.Instance?.ShowMessage(lockedMessage);
            onLocked?.Invoke();
            return;
        }

        // Unlock!
        _isUnlocked = true;
        if (consumeKey) player.RemoveKey(keyId);

        _interactable.hintText = "Open";
        Managers.UIManager.Instance?.ShowMessage(unlockedMessage);
        onUnlocked?.Invoke();

        // Complete task
        if (taskIndexToComplete >= 0)
        {
            var notebook = FindFirstObjectByType<Managers.NotebookUI>();
            notebook?.taskData?.CompleteTask(taskIndexToComplete);
            Managers.UIManager.Instance?.RefreshNotebook();
        }
    }
}
