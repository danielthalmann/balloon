using UnityEngine;
using UnityEngine.InputSystem;
using Prototype.Engine;  // Ajouter cette ligne
using System.Collections.Generic; // Ajoutez cette ligne

namespace Prototype.Player
{
    /// <summary>
    /// Contrôleur principal du joueur
    /// Mouvement de platformer 2D : X = gauche/droite, Y = saut
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
        
        [Header("Paramètres d'échelle")]
        [SerializeField] private LayerMask ladderLayerMask = 1 << 0; // Ajuster selon votre configuration
        [SerializeField] private float ladderCheckDistance = 1f;
        [SerializeField] private KeyCode climbKey = KeyCode.E;
        [SerializeField] private float climbSpeed = 2f;
        
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
        
        private bool isNearLadder = false;
        private EngineLadder nearbyLadder;
        
        // Ajoutez cette propriété pour stocker l'échelle détectée par trigger
        private EngineLadder triggerDetectedLadder;
        
        // Variables pour l'escalade
        private bool isClimbing = false;
        private EngineLadder currentLadder;
        
        // Liste des colliders ignorés pendant l'escalade
        private List<Collider> ignoredColliders = new List<Collider>();

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
            CheckForLadder(); // Cette méthode contient maintenant la logique complète
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
            // Ne pas faire pivoter le joueur pendant l'escalade
            if (isClimbing) return;
            
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
        /// moveInput.y = non utilisé actuellement
        /// </summary>
        private void HandleMovement()
        {
            if (playerRigidbody == null) return;
            
            // Ne pas gérer le mouvement horizontal pendant l'escalade
            if (isClimbing) return;
            
            // Mouvement horizontal uniquement (gauche/droite sur X)
            float targetXVelocity = moveInput.x * moveSpeed;
            
            // Appliquer l'accélération/décélération sur X
            float currentAcceleration = Mathf.Abs(moveInput.x) > 0 ? acceleration : deceleration;
            
            float newXVelocity = Mathf.MoveTowards(
                playerRigidbody.linearVelocity.x,
                targetXVelocity,
                currentAcceleration * Time.fixedDeltaTime
            );
            
            // Appliquer la vélocité en gardant Y (gravité/saut) intact
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
            
            // Ne pas sauter pendant l'escalade
            if (isClimbing) return;
            
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
        public float VerticalInput => moveInput.y;
        
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
            }
        }

        /// <summary>
        /// Vérifie si le joueur est à proximité d'une échelle et gère l'escalade
        /// </summary>
        private void CheckForLadder()
        {
            // Si déjà en train de grimper, gérer le mouvement vertical
            if (isClimbing)
            {
                // Vérifier si le joueur pousse horizontalement pour quitter l'échelle
                if (Mathf.Abs(moveInput.x) > 0.7f) // Seuil à ajuster selon vos préférences
                {
                    // Quitter l'échelle avec une poussée dans la direction choisie
                    ExitLadderSideways(moveInput.x);
                    return;
                }
                
                // Mouvement vertical sur l'échelle
                float verticalMovement = moveInput.y * climbSpeed;
                playerRigidbody.linearVelocity = new Vector3(
                    0f,
                    verticalMovement,
                    0f
                );
                
                // Vérifier si le joueur a atteint le haut ou le bas de l'échelle
                if (currentLadder != null)
                {
                    if (transform.position.y >= currentLadder.GetTopPosition().y)
                    {
                        ExitLadder(true); // Sortir par le haut
                    }
                    else if (transform.position.y <= currentLadder.GetBottomPosition().y)
                    {
                        ExitLadder(false); // Sortir par le bas
                    }
                }
                
                // Sortir de l'échelle avec la touche de saut
                if (jumpPressed)
                {
                    ExitLadder(true);
                }
                
                return;
            }

            // Détection de l'échelle par trigger
            nearbyLadder = triggerDetectedLadder;
            isNearLadder = (nearbyLadder != null);
            
            // Si l'échelle est trouvée et que le joueur appuie sur haut ou bas
            if (isNearLadder && Mathf.Abs(moveInput.y) > 0.1f)
            {
                StartClimbing(nearbyLadder);
            }
        }
        
        /// <summary>
        /// Commence l'escalade de l'échelle
        /// </summary>
        private void StartClimbing(EngineLadder ladder)
        {
            isClimbing = true;
            currentLadder = ladder;
            
            // Désactiver la gravité pendant l'escalade
            playerRigidbody.useGravity = false;
            
            // Centrer le joueur sur l'échelle
            Vector3 newPos = transform.position;
            newPos.x = ladder.transform.position.x;
            newPos.z = ladder.transform.position.z;
            transform.position = newPos;
            
            // Ignorer les collisions avec les sols et plafonds pendant l'escalade
            Collider playerCollider = GetComponent<Collider>();
            if (playerCollider != null)
            {
                // Trouver tous les colliders des niveaux
                EngineLevel[] levels = FindObjectsOfType<EngineLevel>();
                foreach (EngineLevel level in levels)
                {
                    // Récupérer le GameObject contenant les colliders du niveau
                    if (level.transform.Find("LevelBounds") != null)
                    {
                        Transform boundsContainer = level.transform.Find("LevelBounds");
                        
                        // Trouver les colliders de sol
                        foreach (Transform child in boundsContainer)
                        {
                            if (child.name.Contains("Floor") || child.name.Contains("Ceiling"))
                            {
                                Collider floorCollider = child.GetComponent<Collider>();
                                if (floorCollider != null)
                                {
                                    Physics.IgnoreCollision(playerCollider, floorCollider, true);
                                    ignoredColliders.Add(floorCollider);
                                }
                            }
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// Quitte l'échelle et revient au mode normal
        /// </summary>
        private void ExitLadder(bool isTop)
        {
            isClimbing = false;
            
            // Réactiver la gravité
            playerRigidbody.useGravity = true;
            
            // Positionner correctement le joueur
            if (isTop && currentLadder != null)
            {
                // Placer le joueur au niveau supérieur
                Vector3 exitPos = currentLadder.GetTopExitPosition();
                transform.position = exitPos;
            }
            
            // Restaurer les collisions
            Collider playerCollider = GetComponent<Collider>();
            if (playerCollider != null)
            {
                foreach (Collider col in ignoredColliders)
                {
                    if (col != null)
                        Physics.IgnoreCollision(playerCollider, col, false);
                }
            }
            ignoredColliders.Clear();
            
            currentLadder = null;
        }
        
        /// <summary>
        /// Quitte l'échelle par le côté (tombe)
        /// </summary>
        private void ExitLadderSideways(float direction)
        {
            isClimbing = false;
            
            // Réactiver la gravité
            playerRigidbody.useGravity = true;
            
            // Donner une petite impulsion horizontale dans la direction choisie
            float exitForce = 3f;
            playerRigidbody.linearVelocity = new Vector3(
                direction * exitForce,
                0f,
                0f
            );
            
            // Restaurer les collisions
            Collider playerCollider = GetComponent<Collider>();
            if (playerCollider != null)
            {
                foreach (Collider col in ignoredColliders)
                {
                    if (col != null)
                        Physics.IgnoreCollision(playerCollider, col, false);
                }
            }
            ignoredColliders.Clear();
            
            currentLadder = null;
            
            if (showDebugInfo)
            {
                Debug.Log("Joueur tombé de l'échelle");
            }
        }
        
        /// <summary>
        /// Détecte la présence d'une échelle par trigger
        /// </summary>
        void OnTriggerEnter(Collider other)
        {
            EngineLadder ladder = other.GetComponent<EngineLadder>();
            if (ladder != null)
            {
                triggerDetectedLadder = ladder;
            }
        }
        
        /// <summary>
        /// Quand le joueur quitte la zone de trigger d'une échelle
        /// </summary>
        void OnTriggerExit(Collider other)
        {
            EngineLadder ladder = other.GetComponent<EngineLadder>();
            if (ladder != null && ladder == triggerDetectedLadder)
            {
                triggerDetectedLadder = null;
            }
        }
    }
}