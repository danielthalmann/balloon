using UnityEngine;
using Prototype.Player;

namespace Prototype.Engine
{
    /// <summary>
    /// Échelle simple : Haut/Bas pour monter/descendre, Gauche/Droite pour sortir
    /// </summary>
    public class EngineLadder : MonoBehaviour
    {
        [Header("Configuration de l'échelle")]
        [SerializeField] private float climbSpeed = 3f;
        [SerializeField] private float exitDistance = 1.5f;
        [SerializeField] private bool autoCalculateLimits = true;
        [SerializeField] private float heightMargin = 0.5f;
        
        [Header("Limites manuelles (si auto désactivé)")]
        [SerializeField] private float topY = 5f; 
        [SerializeField] private float bottomY = 0f;
        
        [Header("Détection")]
        [SerializeField] private float interactionRange = 1f;
        [SerializeField] private LayerMask playerLayerMask = -1;
        [SerializeField] private LayerMask groundLayerMask = 1;
        
        [Header("Debug")]
        [SerializeField] private bool showDebugInfo = true;
        
        // État de l'échelle
        private GameObject currentPlayer;
        private bool playerIsClimbing = false;
        private Transform playerOriginalParent;
        
        // Variables pour les coordonnées locales (relatives à l'engin)
        private float localTopY;
        private float localBottomY;
        private Transform engineTransform;

        void Start()
        {
            if (autoCalculateLimits)
            {
                CalculateAutomaticLimits();
            }
        }
        
        void Update()
        {
            CheckForPlayer();
            HandleClimbing();
        }
        
        /// <summary>
        /// Calcule automatiquement les limites basées sur les niveaux au-dessus et en-dessous
        /// </summary>
        private void CalculateAutomaticLimits()
        {
            var engineController = GetComponentInParent<EngineController>();
            if (engineController == null) return;
            
            engineTransform = engineController.transform;
            EngineLevel[] allLevels = engineController.GetComponentsInChildren<EngineLevel>();
            
            EngineLevel levelBelow = null;
            EngineLevel levelAbove = null;
            float ladderLocalY = transform.localPosition.y;
            
            // Trouver les niveaux les plus proches
            foreach (EngineLevel level in allLevels)
            {
                float levelLocalY = level.transform.localPosition.y + level.FloorYOffset;
                
                if (levelLocalY < ladderLocalY)
                {
                    if (levelBelow == null || levelLocalY > levelBelow.transform.localPosition.y + levelBelow.FloorYOffset)
                    {
                        levelBelow = level;
                    }
                }
                else if (levelLocalY > ladderLocalY)
                {
                    if (levelAbove == null || levelLocalY < levelAbove.transform.localPosition.y + levelAbove.FloorYOffset)
                    {
                        levelAbove = level;
                    }
                }
            }
            
            // Calculer les limites locales
            localBottomY = levelBelow != null ? 
                levelBelow.transform.localPosition.y + levelBelow.FloorYOffset + heightMargin : 
                ladderLocalY - 2f;
                
            localTopY = levelAbove != null ? 
                levelAbove.transform.localPosition.y + levelAbove.FloorYOffset - heightMargin : 
                ladderLocalY + 2f;
        }
        
        /// <summary>
        /// Détecte si un joueur est proche
        /// </summary>
        private void CheckForPlayer()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, interactionRange, playerLayerMask);
            
            GameObject nearbyPlayer = null;
            foreach (Collider col in colliders)
            {
                if (col.CompareTag("Player") || col.name.Contains("Player"))
                {
                    nearbyPlayer = col.gameObject;
                    break;
                }
            }
            
            if (nearbyPlayer != null && currentPlayer == null)
            {
                currentPlayer = nearbyPlayer;
            }
            else if (nearbyPlayer == null && currentPlayer != null && !playerIsClimbing)
            {
                currentPlayer = null;
            }
        }
        
        /// <summary>
        /// Gère l'escalade
        /// </summary>
        private void HandleClimbing()
        {
            if (currentPlayer == null) return;
            
            var playerController = currentPlayer.GetComponent<PlayerController>();
            if (playerController == null) return;
            
            Vector2 input = playerController.MoveInput;
            
            // Commencer à grimper
            if (!playerIsClimbing && Mathf.Abs(input.y) > 0.1f)
            {
                StartClimbing();
            }
            
            if (playerIsClimbing)
            {
                // Mouvement vertical
                if (Mathf.Abs(input.y) > 0.1f)
                {
                    ClimbVertical(input.y);
                }
                
                // Sortir horizontalement
                if (Mathf.Abs(input.x) > 0.1f)
                {
                    ExitLadder(input.x);
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
            playerOriginalParent = currentPlayer.transform.parent;
            
            // PAS de téléportation ! Le joueur reste où il est
            // On aligne juste progressivement sur l'axe X de l'échelle
            
            // Désactiver la gravité
            Rigidbody playerRb = currentPlayer.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                playerRb.useGravity = false;
                playerRb.linearVelocity = Vector3.zero;
            }
        }
        
        /// <summary>
        /// Propriétés pour les limites en coordonnées monde
        /// </summary>
        private float TopLimit => autoCalculateLimits && engineTransform != null ? 
            engineTransform.position.y + localTopY : topY;
            
        private float BottomLimit => autoCalculateLimits && engineTransform != null ? 
            engineTransform.position.y + localBottomY : bottomY;
        
        /// <summary>
        /// Mouvement vertical sur l'échelle avec alignement progressif
        /// </summary>
        private void ClimbVertical(float input)
        {
            if (currentPlayer == null) return;
            
            Vector3 currentPos = currentPlayer.transform.position;
            
            // Mouvement vertical
            Vector3 verticalMovement = Vector3.up * input * climbSpeed * Time.deltaTime;
            Vector3 newPosition = currentPos + verticalMovement;
            
            // Alignement progressif sur l'échelle (pas instantané)
            float alignSpeed = 2f; // Vitesse d'alignement
            newPosition.x = Mathf.MoveTowards(currentPos.x, transform.position.x, alignSpeed * Time.deltaTime);
            
            // Limiter la hauteur
            newPosition.y = Mathf.Clamp(newPosition.y, BottomLimit, TopLimit);
            
            currentPlayer.transform.position = newPosition;
        }
        
        /// <summary>
        /// Sortir de l'échelle avec mouvement fluide
        /// </summary>
        private void ExitLadder(float horizontalInput)
        {
            if (currentPlayer == null) return;
            
            // Mouvement fluide vers la sortie au lieu de téléportation
            Vector3 currentPos = currentPlayer.transform.position;
            float exitSpeed = 3f; // Vitesse de sortie
            
            Vector3 exitMovement = Vector3.right * horizontalInput * exitSpeed * Time.deltaTime;
            Vector3 newPosition = currentPos + exitMovement;
            
            // Sortir quand on est assez loin de l'échelle
            float distanceFromLadder = Mathf.Abs(newPosition.x - transform.position.x);
            if (distanceFromLadder > 1f)
            {
                StopClimbing();
            }
            else
            {
                currentPlayer.transform.position = newPosition;
            }
        }
        
        /// <summary>
        /// Arrête l'escalade
        /// </summary>
        private void StopClimbing()
        {
            playerIsClimbing = false;
            
            if (currentPlayer != null)
            {
                // Remettre la gravité
                Rigidbody playerRb = currentPlayer.GetComponent<Rigidbody>();
                if (playerRb != null)
                {
                    playerRb.useGravity = true;
                    playerRb.constraints = RigidbodyConstraints.FreezeRotation;
                }
                
                // Remettre le parent si sur un engin
                var playerInteraction = currentPlayer.GetComponent<PlayerEngineInteraction>();
                if (playerInteraction != null && playerInteraction.IsOnEngine)
                {
                    // Garder attaché à l'engin
                }
                else if (playerOriginalParent != null)
                {
                    currentPlayer.transform.SetParent(playerOriginalParent);
                }
            }
            
            playerOriginalParent = null;
        }
        
        // Debug visuel
        void OnDrawGizmosSelected()
        {
            // Portée d'interaction
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, interactionRange);
            
            // Échelle
            Gizmos.color = Color.green;
            Vector3 bottom = new Vector3(transform.position.x, BottomLimit, transform.position.z);
            Vector3 top = new Vector3(transform.position.x, TopLimit, transform.position.z);
            Gizmos.DrawLine(bottom, top);
            
            // Points de sortie
            Gizmos.color = Color.red;
            Vector3 leftExit = transform.position + Vector3.left * exitDistance;
            Vector3 rightExit = transform.position + Vector3.right * exitDistance;
            Gizmos.DrawWireSphere(leftExit, 0.3f);
            Gizmos.DrawWireSphere(rightExit, 0.3f);
        }
        
        // Propriétés pour compatibilité avec EngineLevel
        public EngineLevel BottomLevel => null;
        public EngineLevel TopLevel => null;

        /// <summary>
        /// Vérifie si un joueur spécifique est en train de grimper cette échelle
        /// </summary>
        public bool IsPlayerClimbing(GameObject player)
        {
            return playerIsClimbing && currentPlayer == player;
        }
    }
}