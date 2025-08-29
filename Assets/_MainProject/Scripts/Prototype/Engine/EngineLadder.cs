using UnityEngine;

namespace Prototype.Engine
{
    /// <summary>
    /// Échelle permettant de se déplacer entre les niveaux d'un engin
    /// </summary>
    public class EngineLadder : MonoBehaviour
    {
        [Header("Configuration de l'échelle")]
        [SerializeField] private EngineLevel bottomLevel; // Niveau du bas
        [SerializeField] private EngineLevel topLevel; // Niveau du haut
        [SerializeField] private float climbSpeed = 3f; // Vitesse de montée/descente
        
        [Header("Points de transition")]
        [SerializeField] private Transform bottomPoint; // Point d'arrivée en bas
        [SerializeField] private Transform topPoint; // Point d'arrivée en haut
        
        [Header("Détection")]
        [SerializeField] private float interactionRange = 1f;
        [SerializeField] private LayerMask playerLayerMask = -1; // Tous les layers par défaut
        
        [Header("Visualisation")]
        [SerializeField] private bool showLadderGizmos = true;
        [SerializeField] private Color ladderColor = Color.green;
        
        // État de l'échelle
        private bool playerNearby = false;
        private GameObject currentPlayer;
        private bool playerIsClimbing = false;
        private bool debugLogged = false;
        private Transform playerOriginalParent; // Pour sauvegarder le parent
        
        void Start()
        {
            CreateTransitionPoints();
        }
        
        /// <summary>
        /// Crée les points de transition automatiquement BASÉS sur les niveaux
        /// </summary>
        private void CreateTransitionPoints()
        {
            // Créer les points basés sur les positions des niveaux
            if (bottomPoint == null)
            {
                GameObject bottomObj = new GameObject("BottomPoint");
                bottomObj.transform.SetParent(transform);
                bottomObj.transform.localPosition = Vector3.zero;
                bottomPoint = bottomObj.transform;
            }
            
            if (topPoint == null)
            {
                GameObject topObj = new GameObject("TopPoint");
                topObj.transform.SetParent(transform);
                topObj.transform.localPosition = Vector3.zero;
                topPoint = topObj.transform;
            }
            
            // CORRECTION : Positionner les points basés sur les niveaux
            PositionLadderPoints();
        }
        
        /// <summary>
        /// Positionne les points de l'échelle basés sur les niveaux
        /// </summary>
        private void PositionLadderPoints()
        {
            if (bottomLevel != null && bottomPoint != null)
            {
                // Position du sol du niveau du bas
                Vector3 bottomFloor = bottomLevel.GetFloorWorldPosition();
                bottomPoint.position = new Vector3(transform.position.x, bottomFloor.y, transform.position.z);
            }
            
            if (topLevel != null && topPoint != null)
            {
                // Position du sol du niveau du haut
                Vector3 topFloor = topLevel.GetFloorWorldPosition();
                topPoint.position = new Vector3(transform.position.x, topFloor.y, transform.position.z);
            }
        }
        
        void Update()
        {
            CheckForPlayer();
            HandleClimbing();
        }
        
        /// <summary>
        /// Vérifie si un joueur est à proximité de l'échelle
        /// </summary>
        private void CheckForPlayer()
        {
            Collider[] nearbyColliders = Physics.OverlapSphere(transform.position, interactionRange, playerLayerMask);
            
            bool playerFound = false;
            foreach (Collider collider in nearbyColliders)
            {
                if (collider.CompareTag("Player") || collider.name.Contains("Player"))
                {
                    currentPlayer = collider.gameObject;
                    playerFound = true;
                    
                    if (!debugLogged)
                    {
                        Debug.Log($"Joueur détecté près de l'échelle");
                        debugLogged = true;
                    }
                    break;
                }
            }
            
            if (playerNearby != playerFound)
            {
                playerNearby = playerFound;
                if (!playerNearby)
                {
                    currentPlayer = null;
                    debugLogged = false;
                    if (playerIsClimbing)
                    {
                        StopClimbing();
                    }
                }
            }
        }
        
        /// <summary>
        /// Gère la montée/descente de l'échelle
        /// </summary>
        private void HandleClimbing()
        {
            if (!playerNearby || currentPlayer == null) return;
            
            var playerController = currentPlayer.GetComponent<Prototype.Player.PlayerController>();
            if (playerController == null) return;
            
            float verticalInput = playerController.VerticalInput;
            
            if (Mathf.Abs(verticalInput) > 0.1f)
            {
                if (!playerIsClimbing)
                {
                    StartClimbing();
                }
                
                ClimbLadder(verticalInput);
            }
            else
            {
                if (playerIsClimbing)
                {
                    StopClimbing();
                }
            }
        }
        
        /// <summary>
        /// Commence l'escalade
        /// </summary>
        private void StartClimbing()
        {
            if (currentPlayer == null) return;
            
            playerIsClimbing = true;
            
            // CORRECTION : Sauvegarder le parent et détacher temporairement
            playerOriginalParent = currentPlayer.transform.parent;
            currentPlayer.transform.SetParent(null);
            
            Rigidbody playerRb = currentPlayer.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                playerRb.useGravity = false;
                playerRb.linearVelocity = Vector3.zero;
                // Pas de contraintes pour éviter les conflits
            }
            
            // Positionner le joueur sur l'échelle
            Vector3 playerPos = currentPlayer.transform.position;
            currentPlayer.transform.position = new Vector3(
                transform.position.x,
                playerPos.y,
                transform.position.z
            );
            
            Debug.Log("Escalade commencée !");
        }
        
        /// <summary>
        /// Fait monter/descendre le joueur sur l'échelle
        /// </summary>
        private void ClimbLadder(float input)
        {
            if (currentPlayer == null || bottomPoint == null || topPoint == null) return;
            
            // CORRECTION : Utiliser transform.Translate au lieu de MovePosition
            Vector3 climbDirection = Vector3.up * input * climbSpeed * Time.deltaTime;
            currentPlayer.transform.Translate(climbDirection, Space.World);
            
            // Maintenir l'alignement sur l'échelle
            Vector3 playerPos = currentPlayer.transform.position;
            currentPlayer.transform.position = new Vector3(
                transform.position.x,
                Mathf.Clamp(playerPos.y, bottomPoint.position.y - 0.5f, topPoint.position.y + 0.5f),
                transform.position.z
            );
            
            // Vérifier les transitions
            CheckLevelTransition();
        }
        
        /// <summary>
        /// Vérifie si le joueur doit changer de niveau
        /// </summary>
        private void CheckLevelTransition()
        {
            if (currentPlayer == null || bottomPoint == null || topPoint == null) return;
            
            Vector3 playerPos = currentPlayer.transform.position;
            
            // CORRECTION : Distances plus précises
            float distanceToTop = Mathf.Abs(playerPos.y - topPoint.position.y);
            float distanceToBottom = Mathf.Abs(playerPos.y - bottomPoint.position.y);
            
            Debug.Log($"Player Y: {playerPos.y:F2}, Top Y: {topPoint.position.y:F2}, Bottom Y: {bottomPoint.position.y:F2}");
            Debug.Log($"Distance to top: {distanceToTop:F2}, Distance to bottom: {distanceToBottom:F2}");
            
            // Si le joueur atteint le niveau du haut
            if (distanceToTop < 1f)
            {
                TransitionToLevel(topLevel, topPoint.position, "haut");
            }
            // Si le joueur atteint le niveau du bas
            else if (distanceToBottom < 1f)
            {
                TransitionToLevel(bottomLevel, bottomPoint.position, "bas");
            }
        }
        
        /// <summary>
        /// Effectue la transition vers un niveau
        /// </summary>
        private void TransitionToLevel(EngineLevel targetLevel, Vector3 targetPosition, string direction)
        {
            if (targetLevel == null || currentPlayer == null) return;
            
            // Positionner le joueur à côté de l'échelle sur le niveau
            Vector3 finalPosition = new Vector3(
                targetPosition.x + 1.5f, // À côté de l'échelle
                targetPosition.y + 1f,   // Un peu au-dessus du sol
                targetPosition.z
            );
            
            currentPlayer.transform.position = finalPosition;
            
            // CORRECTION : Remettre le joueur comme enfant du parent original
            if (playerOriginalParent != null)
            {
                currentPlayer.transform.SetParent(playerOriginalParent);
            }
            
            StopClimbing();
            
            Debug.Log($"Transition vers le niveau {targetLevel.LevelIndex} ({direction})");
        }
        
        /// <summary>
        /// Arrête l'escalade
        /// </summary>
        private void StopClimbing()
        {
            playerIsClimbing = false;
            
            if (currentPlayer != null)
            {
                // Remettre le parent si pas encore fait
                if (playerOriginalParent != null && currentPlayer.transform.parent == null)
                {
                    currentPlayer.transform.SetParent(playerOriginalParent);
                }
                
                Rigidbody playerRb = currentPlayer.GetComponent<Rigidbody>();
                if (playerRb != null)
                {
                    playerRb.useGravity = true;
                    playerRb.constraints = RigidbodyConstraints.FreezeRotation;
                }
            }
            
            playerOriginalParent = null; // Reset
            Debug.Log("Escalade terminée !");
        }
        
        // Debug visuel
        void OnDrawGizmosSelected()
        {
            if (!showLadderGizmos) return;
            
            // Mettre à jour les positions des points
            PositionLadderPoints();
            
            // Dessiner la portée d'interaction
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, interactionRange);
            
            // Dessiner l'échelle
            Gizmos.color = ladderColor;
            if (bottomPoint != null && topPoint != null)
            {
                Gizmos.DrawLine(bottomPoint.position, topPoint.position);
                
                // Points de transition
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(bottomPoint.position, 0.5f);
                Gizmos.DrawWireSphere(topPoint.position, 0.5f);
                
                // Labels
                #if UNITY_EDITOR
                UnityEditor.Handles.Label(bottomPoint.position, $"Bottom: Y={bottomPoint.position.y:F1}");
                UnityEditor.Handles.Label(topPoint.position, $"Top: Y={topPoint.position.y:F1}");
                #endif
            }
        }

        /// <summary>
        /// Propriétés publiques pour accès externe
        /// </summary>
        public EngineLevel BottomLevel => bottomLevel;
        public EngineLevel TopLevel => topLevel;
    }
}