using UnityEngine;
using System.Collections.Generic;

namespace Prototype.Engine
{
    /// <summary>
    /// Contrôleur principal pour un engin volant
    /// Gère la chute permanente et les systèmes de remontée
    /// </summary>
    public class EngineController : MonoBehaviour
    {
        [Header("Paramètres de chute")]
        [SerializeField] private float fallSpeed = 2f; // Vitesse de chute en unités par seconde
        [SerializeField] private float groundCheckDistance = 0.5f; // Distance de détection du sol
        [SerializeField] private LayerMask groundLayer; // Layer du sol extérieur
        
        [Header("Paramètres de remontée")]
        [SerializeField] private float liftForce = 5f; // Force de remontée

        [Header("Inertie")]
        [SerializeField] private float inertiaMaxDuration = 3f; // Ajuste entre 1-3 secondes
        [SerializeField] private float inertiaDuration = 0f; 
        [SerializeField] private float inertiaForce = 0f; 
        [Header("Debug")]
        [SerializeField] private bool showDebugInfo = true;
        
        // Variables internes
        private Rigidbody EngineRigidbody;
        private bool isLifting = false;
        private float currentVerticalVelocity = 0f;
        // Système d'actions
        private List<EngineAction> _activeActions = new List<EngineAction>();
        public List<EngineAction> ActiveActions => _activeActions;
        
        void Awake()
        {
            // Récupérer le Rigidbody de l'engin
            EngineRigidbody = GetComponent<Rigidbody>();
        }
        
        void Start()
        {
            // Configuration initiale du Rigidbody
            if (EngineRigidbody != null)
            {
                // Empêcher la rotation automatique
                EngineRigidbody.freezeRotation = true;
                // Réduire le drag pour un mouvement plus fluide
                EngineRigidbody.linearDamping = 0.5f;
            }
        }
        
        void FixedUpdate()
        {
            // Mettre à jour toutes les actions actives
            UpdateActions();
            ApplyFalling();
        }

        /// <summary>
        /// Mettre à jour chaque action active
        /// </summary>
        private void UpdateActions()
        {
            for (int i = _activeActions.Count - 1; i >= 0; i--)
            {
                var action = _activeActions[i];
                
                if (action.IsActive)
                {
                    action.Update();
                    
                    if (action.IsFinished())
                    {
                        action.Stop();
                        _activeActions.RemoveAt(i);
                    }
                }
            }
        }
        
        /// <summary>
        /// Exécuter une nouvelle action
        /// </summary>
        public void ExecuteAction(EngineAction action)
        {
            // Arrêter les actions existantes (optionnel: tu peux aussi les laisser s'accumuler)
            StopAllActions();
            
            _activeActions.Add(action);
            action.Start();
        }

        /// <summary>
        /// Arrêter toutes les actions
        /// </summary>
        public void StopAllActions()
        {
            foreach (var action in _activeActions)
            {
                if (action.IsActive)
                {
                    action.Stop();
                }
            }
            _activeActions.Clear();
        }

        private void ApplyFalling()
        {
            if (EngineRigidbody == null) return;
            
            bool isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);
            
            // Gère la phase d'inertie
            if (!isLifting && inertiaDuration > 0f)
            {
                // L'engine continue de monter mais avec une force décroissante
                inertiaDuration -= Time.fixedDeltaTime;
                
                // Calcule la force d'inertie (décroît de liftForce à 0)
                float inertiaProgress = inertiaDuration / inertiaMaxDuration;
                inertiaForce = liftForce * inertiaProgress;
                
                if (inertiaForce > 0f && !isGrounded)
                {
                    Vector3 newPosition = EngineRigidbody.position + Vector3.up * inertiaForce * Time.fixedDeltaTime;
                    EngineRigidbody.MovePosition(newPosition);
                    currentVerticalVelocity = inertiaForce;
                }
                else if (inertiaDuration <= 0f)
                {
                    inertiaDuration = 0f;
                    inertiaForce = 0f;
                }
            }
            else if (!isLifting && !isGrounded && inertiaDuration <= 0f)
            {
                // Chute normale
                Vector3 newPosition = EngineRigidbody.position + Vector3.down * fallSpeed * Time.fixedDeltaTime;
                EngineRigidbody.MovePosition(newPosition);
                currentVerticalVelocity = -fallSpeed;
            }
            else if (isGrounded)
            {
                // Au sol
                currentVerticalVelocity = 0f;
                inertiaDuration = 0f;
            }
            else if (isLifting)
            {
                // Remontée active
                Vector3 newPosition = EngineRigidbody.position + Vector3.up * liftForce * Time.fixedDeltaTime;
                EngineRigidbody.MovePosition(newPosition);
                currentVerticalVelocity = liftForce;
            }
        }       
        /// <summary>
        /// Active la remontée de l'engin
        /// </summary>
        public void StartLifting()
        {
            if (EngineRigidbody == null) return;
            isLifting = true;
            // Pour un Rigidbody Kinematic, utiliser MovePosition
            Vector3 newPosition = EngineRigidbody.position + Vector3.up * liftForce * Time.fixedDeltaTime;
            EngineRigidbody.MovePosition(newPosition);
        }
        
        /// <summary>
        /// Arrête la remontée de l'engin
        /// </summary>
        public void StopLifting()
        {
            isLifting = false;
            inertiaDuration = inertiaMaxDuration; // Démarre la phase d'inertie
        }
        
        /// <summary>
        /// Propriétés publiques pour accéder aux informations
        /// </summary>
        public bool IsLifting => isLifting;
        public float CurrentVerticalVelocity => currentVerticalVelocity;
        public Vector3 CurrentPosition => transform.position;
        
        // Debug visuel dans l'éditeur
        void OnDrawGizmosSelected()
        {
            if (!showDebugInfo) return;
            
            // Afficher le raycast de détection du sol en jaune
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position, Vector3.down * groundCheckDistance);
            
            // Afficher la direction de chute en rouge
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, Vector3.down * fallSpeed);
            
            // Afficher la direction de remontée en vert si active
            if (isLifting)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawRay(transform.position, Vector3.up * liftForce);
            }
        }
    }
}
