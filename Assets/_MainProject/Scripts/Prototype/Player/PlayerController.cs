using UnityEngine;
using UnityEngine.InputSystem;

namespace Prototype.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 6f;
        [SerializeField] private Transform model; // enfant visuel
        [SerializeField] private float turnSpeed = 12f; // lissage de rotation
        [SerializeField] private float rightYaw = 0f;
        [SerializeField] private float jumpForce = 8f; // Force du saut
        [SerializeField] private LayerMask groundLayer; // Layer du sol
        [SerializeField] private float groundCheckDistance = 0.1f; // Distance de vérification du sol
        
        private Rigidbody rb;
        // Parent retiré: le joueur n'est plus enfant de l'engin, donc pas de référence parent
        private float inputX;
        private bool isGrounded;

        private bool isJumping = false;
        private float jumpStartTime = 0f;
        private float jumpDuration = 0.3f; // Durée pendant laquelle on compense

        void Awake()
        {
            rb = GetComponent<Rigidbody>();
            // Plus de gestion de parent ici
        }

        void FixedUpdate()
        {
            if (rb == null) return;

            // Vérifier si le joueur est au sol
            CheckGrounded();
            
            // Si on vient d'atterrir, arrêter le jump
            if (isGrounded && isJumping && Time.time > jumpStartTime + jumpDuration)
            {
                isJumping = false;
            }

            // déplacement horizontal
            rb.linearVelocity = new Vector3(inputX * moveSpeed, rb.linearVelocity.y, rb.linearVelocity.z);

            // orientation du visuel
            float vx = rb.linearVelocity.x;
            if (Mathf.Abs(vx) > 0.01f)
            {
                Transform t = model != null ? model : transform;
                float yaw = vx > 0f ? rightYaw : rightYaw + 180f; // ajuste selon l'orientation de ton modèle
                Quaternion targetRot = Quaternion.Euler(0f, yaw, 0f);
                t.localRotation = Quaternion.Slerp(t.localRotation, targetRot, turnSpeed * Time.fixedDeltaTime);
            }
        }

        private void CheckGrounded()
        {
            // Raycast vers le bas pour vérifier si on touche le sol
            isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance + 0.5f, groundLayer);
        }

        private float GetGroundVerticalVelocity()
        {
            // Raycast sur tout pour récupérer la plateforme sous le joueur
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, groundCheckDistance + 0.6f, ~0))
            {
                // 1) Cas de l'engin
                var engine = hit.collider.GetComponentInParent<Prototype.Engine.EngineController>();
                if (engine != null) return engine.CurrentVerticalVelocity;

                // 2) Cas d'une autre plateforme mobile avec Rigidbody
                var groundRb = hit.collider.attachedRigidbody;
                if (groundRb != null) return groundRb.linearVelocity.y;
            }
            return 0f;
        }

        // Player Input (Behavior = Send Messages) appelle automatiquement cette méthode
        public void OnMove(InputValue value)
        {
            Vector2 v = value.Get<Vector2>();
            inputX = v.x; // clavier: A/D, flèches ; gamepad: stick gauche X
        }

        // Player Input appelle cette méthode lors de l'appui sur le bouton de saut
        public void OnJump(InputValue value)
        {
            if (value.isPressed && isGrounded)
            {
                float groundVelY = GetGroundVerticalVelocity(); // vitesse verticale de la plateforme/engin
                isJumping = true;
                jumpStartTime = Time.time;

                // Saut constant relatif à l'engin / plateforme
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce + groundVelY, rb.linearVelocity.z);
            }
        }
    }
}
