using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static InputSystem_Actions;

namespace Player
{
    public class PlayerInputController : MonoBehaviour, IPlayerActions
    {
        // ── Events ────────────────────────────────────────────────────────────────

        public event UnityAction<Vector2> OnMoveEvent         = delegate { };
        public event UnityAction<Vector2> OnLookEvent         = delegate { };
        public event UnityAction<Vector2> OnScrollEvent       = delegate { };
        public event UnityAction          OnJumpEvent         = delegate { };
        public event UnityAction          OnInteractEvent     = delegate { };
        public event UnityAction          OnPickUpEvent       = delegate { };
        public event UnityAction          OnDropEvent         = delegate { };
        public event UnityAction<Vector2> OnRotateObjectEvent = delegate { };
        public event UnityAction          OnNotebookEvent     = delegate { };

        public static event UnityAction OnPauseEvent;

        // ── Internal ──────────────────────────────────────────────────────────────

        private InputSystem_Actions _inputActions;

        // Dos flags independientes — la cámara se bloquea si cualquiera es true.
        private bool _lockedByRotation; // PlayerInteraction lo controla cada frame
        private bool _lockedByUI;       // NotebookUI / menús lo controlan por eventos

        private bool IsCameraLocked => _lockedByRotation || _lockedByUI;

        // ── Unity lifecycle ───────────────────────────────────────────────────────

        void Awake()
        {
            _inputActions = new InputSystem_Actions();
            _inputActions.Player.SetCallbacks(this);
        }

        void OnEnable()
        {
            _inputActions.Enable();
            //Managers.GameManager.OnPauseChanged += OnPauseChanged;
        }

        void OnDisable()
        {
            _inputActions.Disable();
            //Managers.GameManager.OnPauseChanged -= OnPauseChanged;
        }

        /*
        void OnPauseChanged(bool paused)
        {
            SetCameraLockedByUI(paused);
            if (paused)
            {
                _inputActions.Player.Move.Disable();
                _inputActions.Player.Look.Disable();
                _inputActions.Player.Jump.Disable();
                _inputActions.Player.Interact.Disable();
                _inputActions.Player.PickUp.Disable();
                _inputActions.Player.Scroll.Disable();
                _inputActions.Player.RotateObject.Disable();
                _inputActions.Player.Notebook.Disable();
            }
            else
            {
                _inputActions.Player.Move.Enable();
                _inputActions.Player.Look.Enable();
                _inputActions.Player.Jump.Enable();
                _inputActions.Player.Interact.Enable();
                _inputActions.Player.PickUp.Enable();
                _inputActions.Player.Scroll.Enable();
                _inputActions.Player.RotateObject.Enable();
                _inputActions.Player.Notebook.Enable();
            }
        }
        */

        // ── Public API ────────────────────────────────────────────────────────────

        /// <summary>Llamado cada frame por PlayerInteraction al rotar un objeto.</summary>
        public void SetCameraLocked(bool locked) => _lockedByRotation = locked;

        /// <summary>Llamado por UI (notebook, menús). No interfiere con la rotación de objetos.</summary>
        public void SetCameraLockedByUI(bool locked) => _lockedByUI = locked;

        // ── IPlayerActions callbacks ──────────────────────────────────────────────

        public void OnMove(InputAction.CallbackContext context)
            => OnMoveEvent.Invoke(context.ReadValue<Vector2>());

        public void OnLook(InputAction.CallbackContext context)
        {
            var value = IsCameraLocked ? Vector2.zero : context.ReadValue<Vector2>();
            OnLookEvent.Invoke(value);
        }

        public void OnScroll(InputAction.CallbackContext context)
            => OnScrollEvent.Invoke(context.ReadValue<Vector2>());

        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.started) OnJumpEvent.Invoke();
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            if (context.started) OnInteractEvent.Invoke();
        }

        public void OnPauseGame(InputAction.CallbackContext context)
        {
            if (context.performed) OnPauseEvent?.Invoke();
        }

        public void OnPickUp(InputAction.CallbackContext context)
        {
            if (context.started)  OnPickUpEvent.Invoke();
            if (context.canceled) OnDropEvent.Invoke();
        }

        public void OnRotateObject(InputAction.CallbackContext context)
        {
            if (context.started)  OnRotateObjectEvent.Invoke(Vector2.one);
            if (context.canceled) OnRotateObjectEvent.Invoke(Vector2.zero);
        }

        public void OnNotebook(InputAction.CallbackContext context)
        {
            if (context.performed) OnNotebookEvent.Invoke();
        }
    }
}