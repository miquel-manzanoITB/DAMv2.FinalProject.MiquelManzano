using UnityEngine;

namespace Player
{
    /// <summary>
    /// Handles player movement: walking, jumping and drag.
    /// Attach to the Player root GameObject.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Walk")]
        public float moveSpeed = 4f;

        [Header("Jump")]
        public float jumpForce = 5f;

        [Header("Ground Check")]
        public float rayLength = 1.1f;
        public LayerMask groundLayer;

        // ── Internal ──────────────────────────────────────────────────────────────

        private Rigidbody _rb;
        private PlayerInputController _input;
        private PlayerCamera _playerCamera;
        private PlayerInteraction _interaction;
        private Vector2 _moveInput;
        private bool _isGrounded;
        private bool _isPaused;

        // ── Unity lifecycle ───────────────────────────────────────────────────────

        void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _input = GetComponent<PlayerInputController>();
            _playerCamera = GetComponent<PlayerCamera>();
            _interaction = GetComponent<PlayerInteraction>();

            _rb.freezeRotation = true;
        }

        void OnEnable()
        {
            _input.OnMoveEvent += OnMove;
            _input.OnJumpEvent += OnJump;
            Managers.GameManager.OnPauseChanged += OnPauseChanged;
        }

        void OnDisable()
        {
            _input.OnMoveEvent -= OnMove;
            _input.OnJumpEvent -= OnJump;
            Managers.GameManager.OnPauseChanged -= OnPauseChanged;
        }

        void Update()
        {
            if (_isPaused) return;
            CheckGround();
            _playerCamera.SetMoving(_moveInput != Vector2.zero && _isGrounded);
        }

        void FixedUpdate()
        {
            if (_isPaused) return;
            Move();
        }

        // ── Input handlers ────────────────────────────────────────────────────────

        void OnPauseChanged(bool paused)
        {
            _isPaused = paused;
            _moveInput = Vector2.zero;
            _rb.isKinematic = paused;
        }

        void OnMove(Vector2 input) => _moveInput = input;

        void OnJump()
        {
            // FIX: Do not jump if the player is standing on a held object.
            // This prevents the "flying" exploit where you place an object under
            // yourself and keep jumping off it.
            if (_isGrounded && !IsStandingOnHeldObject())
                Jump();
        }

        // ── Private methods ───────────────────────────────────────────────────────

        void Move()
        {
            Vector3 direction = transform.right * _moveInput.x
                                + transform.forward * _moveInput.y;

            _rb.AddForce(direction * moveSpeed, ForceMode.VelocityChange);

            // Cap horizontal speed so the player doesn't accelerate forever
            Vector3 flatVelocity = new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z);
            if (flatVelocity.magnitude > moveSpeed)
            {
                Vector3 capped = flatVelocity.normalized * moveSpeed;
                _rb.linearVelocity = new Vector3(capped.x, _rb.linearVelocity.y, capped.z);
            }
        }

        void Jump()
        {
            _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z);
            _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        void CheckGround()
        {
            Vector3 origin = transform.position + Vector3.up * 0.1f;
            _isGrounded = Physics.Raycast(origin, Vector3.down, rayLength, groundLayer);
            Debug.DrawRay(origin, Vector3.down * rayLength, _isGrounded ? Color.green : Color.red);
        }

        /// <summary>
        /// Returns true if the object directly beneath the player is the one
        /// currently being held by PlayerInteraction. This prevents the flying exploit.
        /// </summary>
        bool IsStandingOnHeldObject()
        {
            if (_interaction == null || !_interaction.IsDragging) return false;

            Vector3 origin = transform.position + Vector3.up * 0.1f;
            if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, rayLength))
            {
                return hit.collider.gameObject == _interaction.DraggedObject;
            }
            return false;
        }
    }
}