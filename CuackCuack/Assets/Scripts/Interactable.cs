using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Base interactable component. Attach to any physics object the player can
/// pick up, drag or interact with. Extend via the UnityEvent hooks or subclass.
/// </summary>
public class Interactable : MonoBehaviour
{
    [Header("Settings")]
    public bool isDraggable = true;

    [Header("Interaction Hint")]
    [Tooltip("Text shown on the crosshair when the player looks at this object.")]
    public string hintText = "Press E to interact";

    [Header("Drag Physics")]
    public float dragForce = 50f;

    [Header("Events")]
    public UnityEvent onInteract;
    public UnityEvent onPickUp;

    // ── Internal ──────────────────────────────────────────────────────────────

    private Rigidbody _rb;

    void Awake() => _rb = GetComponent<Rigidbody>();

    // ── Public API ────────────────────────────────────────────────────────────

    public string GetHintText() => hintText;

    public virtual void Interact() => onInteract?.Invoke();
    public virtual void PickUp()   => onPickUp?.Invoke();

    public void StartDrag()
    {
        if (_rb) _rb.linearDamping = 10f;
    }

    public void DragTowards(Vector3 targetPos)
    {
        if (!isDraggable || _rb == null) return;
        Vector3 dir = targetPos - transform.position;
        _rb.AddForce(dir * dragForce, ForceMode.Force);
    }

    public void ApplyRotation(Vector2 mouseDelta, Transform cameraTransform, float sensitivity)
    {
        if (_rb == null) return;
        _rb.MoveRotation(_rb.rotation
                         * Quaternion.AngleAxis(mouseDelta.x * sensitivity, cameraTransform.up)
                         * Quaternion.AngleAxis(-mouseDelta.y * sensitivity, cameraTransform.right));
    }

    public void StopDrag()
    {
        if (_rb) _rb.linearDamping = 1f;
    }
}