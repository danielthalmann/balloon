using UnityEngine;

namespace Prototype.Engine
{
    /// <summary>
    /// Représente une échelle qui permet au joueur de passer d'un niveau à un autre
    /// </summary>
    public class EngineLadder : MonoBehaviour
    {
        [Header("Configuration de l'échelle")]
        [SerializeField] private int startLevelIndex; // Niveau de départ
        [SerializeField] private int targetLevelIndex; // Niveau d'arrivée
        [SerializeField] private float ladderWidth = 0.5f; // Largeur de l'échelle
        [SerializeField] private bool createTriggerArea = true; // Créer une zone de trigger

        [Header("Visualisation")]
        [SerializeField] private bool showDebugInfo = true;
        [SerializeField] private Color ladderColor = Color.yellow;

        // Référence aux niveaux
        private EngineLevel startLevel;
        private EngineLevel targetLevel;
        private BoxCollider triggerCollider;

        void Awake()
        {
            // Trouver les niveaux correspondants
            EngineLevel[] levels = GetComponentInParent<EngineController>()?.GetComponentsInChildren<EngineLevel>();
            
            if (levels != null)
            {
                foreach (EngineLevel level in levels)
                {
                    if (level.LevelIndex == startLevelIndex)
                    {
                        startLevel = level;
                    }
                    else if (level.LevelIndex == targetLevelIndex)
                    {
                        targetLevel = level;
                    }
                }
            }
            
            if (startLevel == null || targetLevel == null)
            {
                Debug.LogWarning($"Échelle {gameObject.name}: Niveaux de départ ou d'arrivée introuvables");
            }
        }

        void Start()
        {
            if (createTriggerArea)
            {
                CreateTriggerArea();
            }
        }

        /// <summary>
        /// Crée la zone de détection pour l'échelle
        /// </summary>
        private void CreateTriggerArea()
        {
            if (startLevel == null || targetLevel == null) return;
            
            triggerCollider = gameObject.AddComponent<BoxCollider>();
            triggerCollider.isTrigger = true;
            
            // Position verticale entre les deux niveaux
            float startY = startLevel.GetFloorWorldPosition().y;
            float targetY = targetLevel.GetFloorWorldPosition().y;
            float height = Mathf.Abs(targetY - startY);
            float centerY = (startY + targetY) / 2f;
            
            // Ajuster la position locale du GameObject pour centrer le trigger
            transform.position = new Vector3(
                transform.position.x,
                centerY,
                transform.position.z
            );
            
            // Définir la taille du trigger
            triggerCollider.size = new Vector3(ladderWidth, height, ladderWidth);
            
            // Ajouter un tag pour l'identification
            gameObject.tag = "Ladder";
        }

        /// <summary>
        /// Téléporte le joueur vers le niveau cible
        /// </summary>
        public void UseToClimb(Transform playerTransform)
        {
            if (targetLevel == null) return;
            
            // Trouver la position d'arrivée
            Vector3 targetPosition = targetLevel.GetFloorWorldPosition();
            // Ajuster la position X/Z pour correspondre à la position horizontale de l'échelle
            targetPosition.x = transform.position.x;
            targetPosition.z = transform.position.z;
            // Décaler vers le haut pour éviter la collision avec le sol
            targetPosition.y += 1f;
            
            // Téléporter le joueur
            playerTransform.position = targetPosition;
            
            if (showDebugInfo)
            {
                Debug.Log($"Joueur a utilisé l'échelle: {startLevelIndex} -> {targetLevelIndex}");
            }
        }

        /// <summary>
        /// Téléporte le joueur vers le niveau de départ (descendre)
        /// </summary>
        public void UseToDescend(Transform playerTransform)
        {
            if (startLevel == null) return;
            
            // Trouver la position d'arrivée
            Vector3 targetPosition = startLevel.GetFloorWorldPosition();
            // Ajuster la position X/Z pour correspondre à la position horizontale de l'échelle
            targetPosition.x = transform.position.x;
            targetPosition.z = transform.position.z;
            // Décaler vers le haut pour éviter la collision avec le sol
            targetPosition.y += 1f;
            
            // Téléporter le joueur
            playerTransform.position = targetPosition;
            
            if (showDebugInfo)
            {
                Debug.Log($"Joueur a utilisé l'échelle pour descendre: {targetLevelIndex} -> {startLevelIndex}");
            }
        }

        void OnDrawGizmosSelected()
        {
            if (!showDebugInfo) return;
            
            EngineLevel[] levels = GetComponentInParent<EngineController>()?.GetComponentsInChildren<EngineLevel>();
            if (levels == null || levels.Length == 0) return;
            
            EngineLevel start = null;
            EngineLevel target = null;
            
            foreach (EngineLevel level in levels)
            {
                if (level.LevelIndex == startLevelIndex)
                {
                    start = level;
                }
                else if (level.LevelIndex == targetLevelIndex)
                {
                    target = level;
                }
            }
            
            if (start == null || target == null) return;
            
            Vector3 startPos = start.GetFloorWorldPosition();
            Vector3 targetPos = target.GetFloorWorldPosition();
            startPos.y += 0.1f; // Légère élévation pour visibilité
            targetPos.y += 0.1f;
            
            // Position horizontale de l'échelle
            startPos.x = transform.position.x;
            startPos.z = transform.position.z;
            targetPos.x = transform.position.x;
            targetPos.z = transform.position.z;
            
            // Dessiner l'échelle
            Gizmos.color = ladderColor;
            Gizmos.DrawLine(startPos, targetPos);
            
            // Dessiner quelques barreaux
            float distance = Vector3.Distance(startPos, targetPos);
            int rungs = Mathf.Max(3, Mathf.FloorToInt(distance / 0.5f));
            
            for (int i = 0; i <= rungs; i++)
            {
                float t = i / (float)rungs;
                Vector3 pos = Vector3.Lerp(startPos, targetPos, t);
                Gizmos.DrawLine(
                    pos - new Vector3(ladderWidth/2, 0, 0), 
                    pos + new Vector3(ladderWidth/2, 0, 0)
                );
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                // Changer le message pour refléter la nouvelle méthode
                Debug.Log("Utilisez les flèches directionnelles pour monter/descendre");
            }
        }

        // void OnTriggerStay(Collider other)
        // {
        //     if (other.CompareTag("Player"))
        //     {
        //         // Le joueur quitte la zone de l'échelle
        //     }
        // }

        void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                // Le joueur quitte la zone de l'échelle
            }
        }

        // Obtenir la position du haut de l'échelle
        public Vector3 GetTopPosition()
        {
            if (targetLevel != null)
            {
                Vector3 topPos = targetLevel.GetFloorWorldPosition();
                topPos.x = transform.position.x;
                topPos.z = transform.position.z;
                return topPos;
            }
            return transform.position + Vector3.up * 3f; // Valeur par défaut
        }

        // Obtenir la position du bas de l'échelle
        public Vector3 GetBottomPosition()
        {
            if (startLevel != null)
            {
                Vector3 bottomPos = startLevel.GetFloorWorldPosition();
                bottomPos.x = transform.position.x;
                bottomPos.z = transform.position.z;
                return bottomPos;
            }
            return transform.position - Vector3.up * 3f; // Valeur par défaut
        }

        // Position de sortie au niveau supérieur
        public Vector3 GetTopExitPosition()
        {
            Vector3 topPos = GetTopPosition();
            topPos.y += 1f; // Léger décalage vers le haut
            return topPos;
        }
    }
}
