using UnityEngine;

/// <summary>
/// A key the player can pick up. When interacted with it registers itself
/// with the player's KeyRing. Place on an Interactable GameObject.
///
/// Requires: Interactable component on the same GameObject.
/// </summary>
[RequireComponent(typeof(Interactable))]
public class KeyItem : MonoBehaviour
{
    [Header("Key Identity")]
    [Tooltip("Must match the keyId on the LockableContainer this key opens.")]
    public string keyId;

    [Header("Feedback")]
    public string pickupMessage = "You picked up a key.";
    public int taskIndexToComplete = -1;   // -1 = no task to complete

    // ── Internal ──────────────────────────────────────────────────────────────

    private Interactable _interactable;

    void Awake()
    {
        _interactable = GetComponent<Interactable>();
    }

    void OnEnable()
    {
        _interactable.onInteract.AddListener(OnPickedUp);
    }

    void OnDisable()
    {
        _interactable.onInteract.RemoveListener(OnPickedUp);
    }

    void OnPickedUp()
    {
        // Add to the player's key ring
        var keyRing = FindFirstObjectByType<KeyRing>();
        if (keyRing == null)
        {
            Debug.LogWarning("[KeyItem] No KeyRing found in scene.");
            return;
        }

        keyRing.AddKey(keyId);

        // Show feedback
        Managers.UIManager.Instance?.ShowMessage(pickupMessage);

        // Complete associated task
        if (taskIndexToComplete >= 0)
        {
            var notebook = FindFirstObjectByType<Managers.NotebookUI>();
            notebook?.taskData?.CompleteTask(taskIndexToComplete);
            Managers.UIManager.Instance?.RefreshNotebook();
        }

        // Deactivate the key object
        gameObject.SetActive(false);
    }
}
