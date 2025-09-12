using UnityEngine;
using Prototype.Engine;

namespace Prototype.Player
{
    /// <summary>
    /// Gère l'interaction simple entre le joueur et les engins volants
    /// E = monter, Q = descendre
    /// </summary>
    public class PlayerEngineInteraction : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private float interactionRange = 2f;
        [SerializeField] private LayerMask engineLayerMask = -1;
        
        [Header("Debug")]
        [SerializeField] private bool showDebugInfo = true;
        
        // État simple
        private EngineController nearbyEngine;
        private bool isOnEngine = false;
        private Transform originalParent;
        
        // Propriétés publiques
        public bool IsOnEngine => isOnEngine;
        public EngineController CurrentEngine => isOnEngine ? nearbyEngine : null;
        
        void Awake()
        {
            originalParent = transform.parent;
        }
        
        void Update()
        {
            // Chercher un engin proche seulement si on n'est pas déjà sur un engin
            // ET seulement de temps en temps pour éviter les performances
            if (!isOnEngine && Time.fixedTime % 0.1f < Time.fixedDeltaTime)
            {
                FindNearbyEngine();
            }
        }
        
        /// <summary>
        /// Cherche l'engin le plus proche (version simplifiée)
        /// </summary>
        private void FindNearbyEngine()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, interactionRange, engineLayerMask);
            
            nearbyEngine = null;
            float closestDistance = float.MaxValue;
            
            foreach (Collider collider in colliders)
            {
                EngineController engine = collider.GetComponentInParent<EngineController>();
                if (engine != null)
                {
                    float distance = Vector3.Distance(transform.position, engine.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        nearbyEngine = engine;
                    }
                }
            }
        }
        
        /// <summary>
        /// Appelé par PlayerController quand E est pressé
        /// </summary>
        public void TriggerInteraction()
        {
            if (!isOnEngine && nearbyEngine != null)
            {
                BoardEngine();
            }
        }
        
        /// <summary>
        /// Appelé par PlayerController quand C est pressé (descendre uniquement)
        /// </summary>
        public void TriggerExit()
        {
            Debug.Log("TriggerExit appelé par la touche C !"); // Debug plus précis
            if (isOnEngine)
            {
                LeaveEngine();
            }
        }
        
        /// <summary>
        /// Monte sur l'engin avec positionnement précis
        /// </summary>
        private void BoardEngine()
        {
            if (nearbyEngine == null) return;
            
            // Attacher le joueur à l'engin
            transform.SetParent(nearbyEngine.transform);
            isOnEngine = true;
            
            // Chercher un point d'accès ou utiliser le centre
            Transform accessPoint = FindAccessPoint();
            if (accessPoint != null)
            {
                transform.position = accessPoint.position + Vector3.up * 1f;
            }
            else
            {
                // Par défaut : centre de l'engin + hauteur
                Vector3 engineCenter = nearbyEngine.transform.position;
                transform.position = engineCenter + Vector3.up * 1.5f;
            }
            
            if (showDebugInfo)
            {
                Debug.Log($"Monté sur l'engin : {nearbyEngine.name}");
            }
        }

        /// <summary>
        /// Trouve un point d'accès sur l'engin (optionnel)
        /// </summary>
        private Transform FindAccessPoint()
        {
            // Chercher un enfant nommé "AccessPoint" ou similaire
            Transform accessPoint = nearbyEngine.transform.Find("AccessPoint");
            if (accessPoint != null) return accessPoint;
            
            // Ou chercher le premier EngineLevel
            EngineLevel level = nearbyEngine.GetComponentInChildren<EngineLevel>();
            if (level != null) return level.transform;
            
            return null;
        }
        
        /// <summary>
        /// Descend de l'engin (version corrigée)
        /// </summary>
        private void LeaveEngine()
        {
            if (!isOnEngine) return;
            
            Debug.Log("LeaveEngine exécuté !"); // Debug pour voir d'où ça vient
            
            // Téléporter le joueur à côté de l'engin avant de détacher
            Vector3 exitPosition = nearbyEngine.transform.position + Vector3.right * 3f + Vector3.up * 1f;
            transform.position = exitPosition;
            
            // Détacher le joueur
            transform.SetParent(originalParent);
            isOnEngine = false;
            
            // IMPORTANT : Garder la référence de l'engin pour pouvoir remonter
            // nearbyEngine reste disponible pour une nouvelle interaction
            
            if (showDebugInfo)
            {
                Debug.Log("Descendu de l'engin");
            }
        }
        
        // Debug visuel simplifié
        void OnDrawGizmosSelected()
        {
            if (!showDebugInfo) return;
            
            // Portée d'interaction
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, interactionRange);
            
            // Engin détectable
            if (nearbyEngine != null && !isOnEngine)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, nearbyEngine.transform.position);
            }
        }
    }
}