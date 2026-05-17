using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Player
{
    public class PlayerInteraction : MonoBehaviour
    {
        [Header("Raycast")]
        public Camera playerCamera;
        public float interactRange = 3f;
        public LayerMask interactableLayer;

        [Header("Drag")]
        public float scrollSpeed = 0.5f;
        public float minDragDistance = 1f;
        public float maxDragDistance = 4f;

        [Header("Object Rotation")]
        [Tooltip("Mouse sensitivity when rotating a held object with R + mouse.")]
        public float rotateSensitivity = 0.2f;

        [Header("Crosshair")]
        public Image crosshair;

        // ── Public state (used by PlayerMovement for the flying fix) ──────────────

        /// <summary>True while the player is holding (dragging) an object.</summary>
        public bool IsDragging => _isDragging && _dragging != null;

        /// <summary>The GameObject currently being dragged, or null.</summary>
        public GameObject DraggedObject => _dragging != null ? _dragging.gameObject : null;

        // ── Internal ──────────────────────────────────────────────────────────────

        private Interactable _hovered;
        private Interactable _dragging;
        private float _dragDistance;
        private bool _isDragging;
        private float _scrollDirection;
        private bool _isRotating;

        private PlayerInputController _input;

        // ── Unity lifecycle ───────────────────────────────────────────────────────

        void Awake()
        {
            _input = GetComponent<PlayerInputController>();
        }

        void OnEnable()
        {
            _input.OnInteractEvent  += OnInteract;
            _input.OnPickUpEvent    += OnPickUp;
            _input.OnDropEvent      += OnDrop;
            _input.OnScrollEvent    += OnScroll;
            _input.OnRotateObjectEvent += OnRotateObject;
        }

        void OnDisable()
        {
            _input.OnInteractEvent  -= OnInteract;
            _input.OnPickUpEvent    -= OnPickUp;
            _input.OnDropEvent      -= OnDrop;
            _input.OnScrollEvent    -= OnScroll;
            _input.OnRotateObjectEvent -= OnRotateObject;
        }

        void Update()
        {
            HandleHover();
            HandleDragInput();
            HandleScroll();
            HandleRotation();
        }

        // ── Input handlers ────────────────────────────────────────────────────────

        void OnInteract()
        {
            _hovered?.Interact();
        }

        void OnPickUp()
        {
            _isDragging = true;
            _hovered?.PickUp();
        }

        void OnDrop()
        {
            _isDragging = false;
        }

        void OnScroll(Vector2 dir)
        {
            _scrollDirection = dir.y;
        }

        void OnRotateObject(Vector2 signal)
        {
            _isRotating = signal != Vector2.zero;
        }

        // ── Private methods ───────────────────────────────────────────────────────

        void HandleHover()
        {
            Ray ray = playerCamera.ScreenPointToRay(new Vector2(Screen.width / 2f, Screen.height / 2f));

            if (Physics.Raycast(ray, out RaycastHit hit, interactRange, interactableLayer))
            {
                var interactable = hit.collider.GetComponent<Interactable>();
                if (interactable != _hovered)
                {
                    _hovered = interactable;
                    if (crosshair) crosshair.color = Color.green;

                    // Show interaction hint
                    Managers.UIManager.Instance?.ShowInteractionHint(_hovered?.GetHintText());
                }
            }
            else
            {
                if (_hovered != null)
                {
                    Managers.UIManager.Instance?.HideInteractionHint();
                }
                _hovered = null;
                if (crosshair) crosshair.color = Color.white;
            }

            Debug.DrawRay(ray.origin, ray.direction * interactRange, _hovered != null ? Color.green : Color.red);
        }

        void HandleDragInput()
        {
            // Start drag
            if (_isDragging && _hovered != null && _dragging == null)
            {
                Ray ray = playerCamera.ScreenPointToRay(new Vector2(Screen.width / 2f, Screen.height / 2f));
                if (Physics.Raycast(ray, out RaycastHit hit, interactRange, interactableLayer))
                {
                    _dragging = _hovered;
                    _dragDistance = hit.distance;
                    _dragging.StartDrag();
                }
            }

            // Drag each frame
            if (_dragging != null && _isDragging)
            {
                Ray ray = playerCamera.ScreenPointToRay(new Vector2(Screen.width / 2f, Screen.height / 2f));
                Vector3 targetPos = ray.GetPoint(_dragDistance);
                _dragging.DragTowards(targetPos);
            }

            // Release
            if (!_isDragging && _dragging != null)
            {
                _dragging.StopDrag();
                _dragging = null;
            }
        }

        void HandleScroll()
        {
            if (_dragging == null) return;
            _dragDistance += _scrollDirection * scrollSpeed * Time.deltaTime;
            _dragDistance = Mathf.Clamp(_dragDistance, minDragDistance, maxDragDistance);
        }

        void HandleRotation()
        {
            bool shouldRotate = _isRotating && _dragging != null;
            _input.SetCameraLocked(shouldRotate);

            if (!shouldRotate) return;

            Vector2 delta = Mouse.current.delta.ReadValue();
            if (delta != Vector2.zero)
                _dragging.ApplyRotation(delta, playerCamera.transform, rotateSensitivity);
        }
    }
}
