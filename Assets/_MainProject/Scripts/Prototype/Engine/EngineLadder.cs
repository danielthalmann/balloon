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
        
        void Start() // Changé de Awake à Start
        {
            CreateTransitionPoints();
        }
        
        /// <summary>
        /// Crée les points de transition automatiquement
        /// </summary>
        private void CreateTransitionPoints()
        {
            // Créer les points de transition automatiquement si ils n'existent pas
            if (bottomPoint == null)
            {
                GameObject bottomObj = new GameObject("BottomPoint");
                bottomObj.transform.SetParent(transform);
                bottomObj.transform.localPosition = Vector3.zero;
                bottomPoint = bottomObj.transform;
                Debug.Log("BottomPoint créé automatiquement");
            }
            
            if (topPoint == null)
            {
                GameObject topObj = new GameObject("TopPoint");
                topObj.transform.SetParent(transform);
                topObj.transform.localPosition = Vector3.forward * 3f; // 3 unités vers l'avant (Z)
                topPoint = topObj.transform;
                Debug.Log("TopPoint créé automatiquement");
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
                // Vérifier par tag ET par nom si le tag ne marche pas
                if (collider.CompareTag("Player") || collider.name.Contains("Player"))
                {
                    currentPlayer = collider.gameObject;
                    playerFound = true;
                    Debug.Log($"Joueur détecté près de l'échelle : {collider.name}");
                    break;
                }
            }
            
            if (playerNearby != playerFound)
            {
                playerNearby = playerFound;
                if (!playerNearby)
                {
                    currentPlayer = null;
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
            
            // Récupérer le contrôleur du joueur
            var playerController = currentPlayer.GetComponent<Prototype.Player.PlayerController>();
            if (playerController == null)
            {
                Debug.LogError("PlayerController non trouvé sur le joueur !");
                return;
            }
            
            // Vérifier l'input vertical (W/S ou flèches haut/bas)
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
            
            // Désactiver la gravité pendant l'escalade
            Rigidbody playerRb = currentPlayer.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                playerRb.useGravity = false;
                playerRb.linearVelocity = Vector3.zero; // Arrêter tout mouvement
            }
            
            Debug.Log("Escalade commencée !");
        }
        
        /// <summary>
        /// Fait monter/descendre le joueur sur l'échelle
        /// </summary>
        private void ClimbLadder(float input)
        {
            if (currentPlayer == null || bottomPoint == null || topPoint == null) return;
            
            // Mouvement sur l'axe Z (profondeur) pour changer de niveau
            Vector3 climbDirection = Vector3.forward * input * climbSpeed * Time.deltaTime;
            currentPlayer.transform.Translate(climbDirection);
            
            // Maintenir le joueur aligné sur l'échelle (axe X)
            Vector3 playerPos = currentPlayer.transform.position;
            currentPlayer.transform.position = new Vector3(
                transform.position.x, // Aligner sur X de l'échelle
                playerPos.y,
                playerPos.z
            );
            
            // Vérifier si le joueur a atteint un niveau
            CheckLevelTransition();
        }
        
        /// <summary>
        /// Vérifie si le joueur doit changer de niveau
        /// </summary>
        private void CheckLevelTransition()
        {
            if (currentPlayer == null || bottomPoint == null || topPoint == null) return;
            
            Vector3 playerPos = currentPlayer.transform.position;
            
            // Si le joueur monte et atteint le point haut
            if (Vector3.Distance(playerPos, topPoint.position) < 0.8f)
            {
                TransitionToLevel(topLevel, topPoint.position);
            }
            // Si le joueur descend et atteint le point bas
            else if (Vector3.Distance(playerPos, bottomPoint.position) < 0.8f)
            {
                TransitionToLevel(bottomLevel, bottomPoint.position);
            }
        }
        
        /// <summary>
        /// Effectue la transition vers un niveau
        /// </summary>
        private void TransitionToLevel(EngineLevel targetLevel, Vector3 targetPosition)
        {
            if (targetLevel == null || currentPlayer == null) return;
            
            // Positionner le joueur sur le niveau
            currentPlayer.transform.position = targetPosition + Vector3.right * 1f; // Un peu à côté de l'échelle
            
            StopClimbing();
            
            Debug.Log($"Transition vers le niveau {targetLevel.LevelIndex}");
        }
        
        /// <summary>
        /// Arrête l'escalade
        /// </summary>
        private void StopClimbing()
        {
            playerIsClimbing = false;
            
            if (currentPlayer != null)
            {
                // Réactiver la gravité
                Rigidbody playerRb = currentPlayer.GetComponent<Rigidbody>();
                if (playerRb != null)
                {
                    playerRb.useGravity = true;
                }
            }
            
            Debug.Log("Escalade terminée !");
        }
        
        // Debug visuel
        void OnDrawGizmosSelected()
        {
            if (!showLadderGizmos) return;
            
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
                Gizmos.DrawWireSphere(bottomPoint.position, 0.3f);
                Gizmos.DrawWireSphere(topPoint.position, 0.3f);
            }
        }
    }
}