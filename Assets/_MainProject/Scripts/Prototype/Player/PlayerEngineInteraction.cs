using UnityEngine;
using Prototype.Engine;


namespace Prototype.Player
{
    /// <summary>
    /// Gère l'interaction entre le joueur et les engins volants
    /// </summary>
    public class PlayerEngineInteraction : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private float interactionRange = 2f;
        [SerializeField] private LayerMask engineLayerMask = -1; // Par défaut, tous les layers
        
        [Header("Debug")]
        [SerializeField] private bool showDebugInfo = true;
        
        // Composants
        private PlayerController playerController;
        
        // État actuel
        private EngineController currentEngine;
        private EngineLevel currentLevel;
        private bool isOnEngine = false;
        
        // Transform original du joueur
        private Transform originalParent;
        
        void Awake()
        {
            playerController = GetComponent<PlayerController>();
            originalParent = transform.parent;
            
            // if (playerController == null)
            // {
            //     Debug.LogError("PlayerEngineInteraction nécessite un PlayerController !");
            // }
        }
        
        void Update()
        {
            if (!isOnEngine)
            {
                CheckForNearbyEngine();
            }
        }
        
        /// <summary>
        /// Méthode publique appelée par le PlayerController quand E est pressé
        /// </summary>
        public void TriggerInteraction()
        {
            // Debug.Log("TriggerInteraction appelé !"); // DEBUG
            
            if (!isOnEngine && currentEngine != null)
            {
                // Debug.Log($"Tentative de montée sur l'engin : {currentEngine.name}"); // DEBUG
                BoardEngine();
            }
            else if (isOnEngine)
            {
                // Debug.Log("Tentative de descente de l'engin"); // DEBUG
                LeaveEngine();
            }
            else
            {
                Debug.Log($"Pas d'engin détecté. isOnEngine: {isOnEngine}, currentEngine: {(currentEngine != null ? currentEngine.name : "null")}"); // DEBUG
            }
        }
        
        /// <summary>
        /// Cherche les engins volants à proximité
        /// </summary>
        private void CheckForNearbyEngine()
        {
            Collider[] nearbyColliders = Physics.OverlapSphere(transform.position, interactionRange, engineLayerMask);
            
            // DEBUG : Afficher combien de colliders sont détectés
            // if (nearbyColliders.Length > 0 && showDebugInfo)
            // {
            //     Debug.Log($"Colliders détectés : {nearbyColliders.Length}");
            // }
            
            EngineController nearestEngine = null;
            float nearestDistance = float.MaxValue;
            
            foreach (Collider collider in nearbyColliders)
            {
                EngineController engine = collider.GetComponentInParent<EngineController>();
                if (engine != null)
                {
                    float distance = Vector3.Distance(transform.position, engine.transform.position);
                    if (distance < nearestDistance)
                    {
                        nearestDistance = distance;
                        nearestEngine = engine;
                    }
                    
                    // DEBUG
                    // if (showDebugInfo)
                    // {
                    //     Debug.Log($"Engin trouvé : {engine.name} à distance {distance:F2}");
                    // }
                }
            }
            
            currentEngine = nearestEngine;
        }
        
        /// <summary>
        /// Monte sur l'engin volant
        /// </summary>
        private void BoardEngine()
        {
            if (currentEngine == null) return;
            
            // Trouver le niveau le plus proche
            EngineLevel[] levels = currentEngine.GetComponentsInChildren<EngineLevel>();
            EngineLevel closestLevel = null;
            float closestDistance = float.MaxValue;
            
            // Debug.Log($"Niveaux trouvés : {levels.Length}"); // DEBUG
            
            foreach (EngineLevel level in levels)
            {
                float distance = Vector3.Distance(transform.position, level.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestLevel = level;
                }
            }
            
            if (closestLevel != null)
            {
                // Attacher le joueur à l'engin
                transform.SetParent(currentEngine.transform);
                currentLevel = closestLevel;
                isOnEngine = true;
                
                // Positionner le joueur sur le niveau
                Transform accessPoint = closestLevel.GetClosestAccessPoint(transform.position);
                transform.position = accessPoint.position + Vector3.up * 1f;
                
                // Debug.Log($"Joueur monté sur l'engin au niveau {closestLevel.LevelIndex}");
            }
            else
            {
                Debug.LogError("Aucun niveau trouvé sur l'engin !"); // DEBUG
            }
        }
        
        /// <summary>
        /// Descend de l'engin volant
        /// </summary>
        private void LeaveEngine()
        {
            if (!isOnEngine) return;
            
            // Détacher le joueur de l'engin
            transform.SetParent(originalParent);
            
            // Réinitialiser l'état
            currentEngine = null;
            currentLevel = null;
            isOnEngine = false;
            
            // Debug.Log("Joueur descendu de l'engin");
        }
        
        /// <summary>
        /// Propriétés publiques
        /// </summary>
        public bool IsOnEngine => isOnEngine;
        public EngineController CurrentEngine => currentEngine;
        public EngineLevel CurrentLevel => currentLevel;
        
        // Debug visuel
        void OnDrawGizmosSelected()
        {
            if (!showDebugInfo) return;
            
            // Afficher la portée d'interaction
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, interactionRange);
            
            // Afficher l'engin détecté
            if (currentEngine != null && !isOnEngine)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position, currentEngine.transform.position);
            }
        }
    }
}