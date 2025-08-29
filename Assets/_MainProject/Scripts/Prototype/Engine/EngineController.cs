using UnityEngine;

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
        
        [Header("Paramètres de remontée")]
        [SerializeField] private float liftForce = 5f; // Force de remontée
        
        [Header("Debug")]
        [SerializeField] private bool showDebugInfo = true;
        
        // Variables internes
        private Rigidbody EngineRigidbody;
        private bool isLifting = false;
        private float currentVerticalVelocity = 0f;
        
        void Awake()
        {
            // Récupérer le Rigidbody de l'engin
            EngineRigidbody = GetComponent<Rigidbody>();
            
            // if (EngineRigidbody == null)
            // {
            //     Debug.LogError("EngineController nécessite un Rigidbody sur le GameObject !");
            // }
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
            ApplyFalling();
        }
        
        /// <summary>
        /// Applique la force de chute permanente
        /// </summary>
        private void ApplyFalling()
        {
            if (EngineRigidbody == null) return;
            
            if (!isLifting)
            {
                // Appliquer la chute
                EngineRigidbody.linearVelocity = new Vector3(
                    EngineRigidbody.linearVelocity.x,
                    -fallSpeed,
                    EngineRigidbody.linearVelocity.z
                );
            }
            
            currentVerticalVelocity = EngineRigidbody.linearVelocity.y;
        }
        
        /// <summary>
        /// Active la remontée de l'engin
        /// </summary>
        public void StartLifting()
        {
            if (EngineRigidbody == null) return;
            
            isLifting = true;
            
            // Appliquer la force de remontée
            EngineRigidbody.linearVelocity = new Vector3(
                EngineRigidbody.linearVelocity.x,
                liftForce,
                EngineRigidbody.linearVelocity.z
            );
        }
        
        /// <summary>
        /// Arrête la remontée de l'engin
        /// </summary>
        public void StopLifting()
        {
            isLifting = false;
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
        
        // // Debug dans l'interface
        // void OnGUI()
        // {
        //     if (!showDebugInfo) return;
            
        //     GUILayout.BeginArea(new Rect(10, 10, 300, 100));
        //     GUILayout.Label($"Engin Volant - Debug");
        //     GUILayout.Label($"Position Y: {transform.position.y:F2}");
        //     GUILayout.Label($"Vitesse verticale: {currentVerticalVelocity:F2}");
        //     GUILayout.Label($"En remontée: {(isLifting ? "OUI" : "NON")}");
        //     GUILayout.EndArea();
        // }
    }
}
