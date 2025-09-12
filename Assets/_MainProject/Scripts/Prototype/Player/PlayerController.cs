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
        [SerializeField] private float rotateSpeed = 12f;
        
        [Header("Paramètres de saut")]
        [SerializeField] private float jumpForce = 4f;
        [SerializeField] private float fallMultiplier = 1.5f;
        [SerializeField] private LayerMask groundLayerMask = 1; // Layer du sol
        
        [Header("Détection du sol")]
        [SerializeField] private Transform groundCheck;
        [SerializeField] private float groundCheckRadius = 0.3f;
        
        [Header("Debug")]
        [SerializeField] private bool showDebugInfo = true;


        
        // Composants
        private Rigidbody playerRigidbody;
        
        // Variables de mouvement
        private Vector2 moveInput;
        private Vector3 rotationDirection;
        private bool isGrounded;
        private bool jumpPressed;
        
        void Awake()
        {
            playerRigidbody = GetComponent<Rigidbody>();
            
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
                playerRigidbody.freezeRotation = false;
                playerRigidbody.linearDamping = 0f;
                playerRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            }
        }
        
        void Update()
        {
            CheckGrounded();
            HandleRotation();
        }
        
        void FixedUpdate()
        {
            HandleMovement();
            HandleJump();
        }

        /// <summary>
        /// Gère la rotation du personnage selon la direction de mouvement
        /// </summary>
        private void HandleRotation()
        {
            // Si le joueur se déplace horizontalement, mettre à jour la direction de rotation
            if (Mathf.Abs(moveInput.x) > 0.1f)
            {
                rotationDirection = new Vector3(moveInput.x, 0, 0);
                
                // Appliquer la rotation en douceur
                transform.right = Vector3.Slerp(transform.right, rotationDirection, rotateSpeed * Time.deltaTime);
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
        /// Gère le saut du joueur : déclenchement et physique améliorée
        /// </summary>
        private void HandleJump()
        {
            if (playerRigidbody == null) return;
            
            // Déclenchement du saut
            if (jumpPressed && isGrounded)
            {
                // Saut sur l'axe Y (vertical classique)
                playerRigidbody.linearVelocity = new Vector3(
                    playerRigidbody.linearVelocity.x, 
                    jumpForce,  // Y = saut vertical
                    playerRigidbody.linearVelocity.z
                );
            }
            
            // Physique améliorée : gravité renforcée en chute
            if (playerRigidbody.linearVelocity.y < 0)
            {
                playerRigidbody.linearVelocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
            }
            
            jumpPressed = false;
        }
        
        /// <summary>
        /// Propriétés publiques
        /// </summary>
        public Vector2 MoveInput => moveInput;
        public bool IsGrounded => isGrounded;
        public Vector3 CurrentVelocity => playerRigidbody.linearVelocity;
        public bool IsMoving => Mathf.Abs(moveInput.x) > 0.1f; // Seulement le mouvement horizontal
        public float VerticalInput => moveInput.y; // Pour les échelles futures
        
        // Événements d'input (seul système d'input nécessaire)
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
                var interaction = GetComponent<PlayerEngineInteraction>();
                interaction?.TriggerInteraction();
            }
        }

        // public void OnExit(InputValue value)
        // {
        //     if (value.isPressed)
        //     {
        //         var interaction = GetComponent<PlayerEngineInteraction>();
        //         interaction?.TriggerExit();
        //     }
        // }

        // AJOUTER cette méthode à la place (utilise la touche C) :
        public void OnCrouch(InputValue value)
        {
            if (value.isPressed)
            {
                var interaction = GetComponent<PlayerEngineInteraction>();
                interaction?.TriggerExit();
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
