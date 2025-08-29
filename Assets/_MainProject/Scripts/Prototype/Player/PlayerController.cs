using UnityEngine;
using UnityEngine.InputSystem;

namespace Prototype.Player
{
    /// <summary>
    /// Contrôleur principal du joueur
    /// Mouvement de platformer 2D : X = gauche/droite, Y = saut, Z = échelles
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        [Header("Paramètres de mouvement")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float acceleration = 10f;
        [SerializeField] private float deceleration = 10f;
        
        [Header("Paramètres de saut")]
        [SerializeField] private float jumpForce = 7f;
        [SerializeField] private LayerMask groundLayerMask = 1; // Layer du sol
        
        [Header("Détection du sol")]
        [SerializeField] private Transform groundCheck;
        [SerializeField] private float groundCheckRadius = 0.3f;
        
        [Header("Debug")]
        [SerializeField] private bool showDebugInfo = true;
        
        // Composants
        private Rigidbody playerRigidbody;
        private PlayerInput playerInput;
        
        // Variables de mouvement
        private Vector2 moveInput;
        private Vector3 currentVelocity;
        private bool isGrounded;
        private bool jumpPressed;
        
        // Actions d'input
        private InputAction moveAction;
        private InputAction jumpAction;
        private InputAction interactAction;
        
        void Awake()
        {
            // Récupérer les composants
            playerRigidbody = GetComponent<Rigidbody>();
            playerInput = GetComponent<PlayerInput>();
            
            // if (playerRigidbody == null)
            // {
            //     Debug.LogError("PlayerController nécessite un Rigidbody !");
            // }
            
            // if (playerInput == null)
            // {
            //     Debug.LogError("PlayerController nécessite un PlayerInput !");
            // }
            
            // Créer le groundCheck si il n'existe pas
            if (groundCheck == null)
            {
                GameObject groundCheckObj = new GameObject("GroundCheck");
                groundCheckObj.transform.SetParent(transform);
                groundCheckObj.transform.localPosition = Vector3.down * 1f;
                groundCheck = groundCheckObj.transform;
            }
        }
        
        void Start()
        {
            // Configuration du Rigidbody
            if (playerRigidbody != null)
            {
                playerRigidbody.freezeRotation = true; // Empêcher la rotation
                playerRigidbody.linearDamping = 0f; // Pas de drag pour un contrôle précis
                
                // Bloquer uniquement la rotation, laisser X, Y, Z libres pour le mouvement
                playerRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            }
            
            // Récupérer les actions d'input
            if (playerInput != null)
            {
                moveAction = playerInput.actions["Move"];
                jumpAction = playerInput.actions["Jump"];
                interactAction = playerInput.actions["Interact"];
            }
        }
        
        void Update()
        {
            HandleInput();
            CheckGrounded();
        }
        
        void FixedUpdate()
        {
            HandleMovement();
            HandleJump();
        }
        
        /// <summary>
        /// Gère la lecture des inputs
        /// </summary>
        private void HandleInput()
        {
            if (moveAction != null)
            {
                moveInput = moveAction.ReadValue<Vector2>();
            }
            
            if (jumpAction != null && jumpAction.WasPressedThisFrame())
            {
                jumpPressed = true;
            }
        }
        
        /// <summary>
        /// Vérifie si le joueur est au sol
        /// </summary>
        private void CheckGrounded()
        {
            if (groundCheck == null) return;
            
            isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayerMask);
        }
        
        /// <summary>
        /// Gère le mouvement horizontal du joueur
        /// moveInput.x = gauche/droite (axe X)
        /// moveInput.y = sera utilisé plus tard pour les échelles (axe Z)
        /// </summary>
        private void HandleMovement()
        {
            if (playerRigidbody == null) return;
            
            // Mouvement horizontal uniquement (gauche/droite sur X)
            float targetXVelocity = moveInput.x * moveSpeed;
            
            // Appliquer l'accélération/décélération sur X
            float currentAcceleration = Mathf.Abs(moveInput.x) > 0 ? acceleration : deceleration;
            
            float newXVelocity = Mathf.MoveTowards(
                playerRigidbody.linearVelocity.x,
                targetXVelocity,
                currentAcceleration * Time.fixedDeltaTime
            );
            
            // Appliquer la vélocité en gardant Y (gravité/saut) et Z (échelles futures) intacts
            playerRigidbody.linearVelocity = new Vector3(
                newXVelocity, 
                playerRigidbody.linearVelocity.y, 
                playerRigidbody.linearVelocity.z
            );
        }
        
        /// <summary>
        /// Gère le saut du joueur sur l'axe Y
        /// </summary>
        private void HandleJump()
        {
            if (jumpPressed && isGrounded && playerRigidbody != null)
            {
                // Saut sur l'axe Y (vertical classique)
                playerRigidbody.linearVelocity = new Vector3(
                    playerRigidbody.linearVelocity.x, 
                    jumpForce,  // Y = saut vertical
                    playerRigidbody.linearVelocity.z
                );
            }
            
            jumpPressed = false;
        }
        
        /// <summary>
        /// Propriétés publiques
        /// </summary>
        public Vector2 MoveInput => moveInput;
        public bool IsGrounded => isGrounded;
        public Vector3 CurrentVelocity => playerRigidbody != null ? playerRigidbody.linearVelocity : Vector3.zero;
        public bool IsMoving => Mathf.Abs(moveInput.x) > 0.1f; // Seulement le mouvement horizontal
        public float VerticalInput => moveInput.y; // Pour les échelles futures
        
        // Événements d'input (appelés par le PlayerInput)
        public void OnMove(InputValue value)
        {
            moveInput = value.Get<Vector2>();
        }
        
        public void OnJump(InputValue value)
        {
            if (value.isPressed)
            {
                jumpPressed = true;
            }
        }
        
        public void OnInteract(InputValue value)
        {
            if (value.isPressed)
            {
                // Notifier le système d'interaction
                var interaction = GetComponent<PlayerEngineInteraction>();
                if (interaction != null)
                {
                    interaction.TriggerInteraction();
                }
            }
        }
        
        // Debug visuel
        void OnDrawGizmosSelected()
        {
            if (groundCheck != null)
            {
                Gizmos.color = isGrounded ? Color.green : Color.red;
                Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
            }
            
            if (showDebugInfo && Application.isPlaying)
            {
                // Mouvement horizontal
                Gizmos.color = Color.blue;
                Gizmos.DrawRay(transform.position, Vector3.right * moveInput.x * 2f);
                
                // Input vertical (pour les échelles futures)
                if (Mathf.Abs(moveInput.y) > 0.1f)
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawRay(transform.position, Vector3.forward * moveInput.y * 2f);
                }
            }
        }
    }
}
