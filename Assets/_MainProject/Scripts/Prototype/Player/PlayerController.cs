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
        private Rigidbody rb;
        private float inputX;

        void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        void FixedUpdate()
        {
            if (rb == null) return;

            // déplacement horizontal
            rb.linearVelocity = new Vector3(inputX * moveSpeed, rb.linearVelocity.y, rb.linearVelocity.z);

            // orientation du visuel
            float vx = rb.linearVelocity.x;
            if (Mathf.Abs(vx) > 0.01f)
            {
                Transform t = model != null ? model : transform;
                float yaw = vx > 0f ? rightYaw : rightYaw + 180f; // ajuste selon l’orientation de ton modèle
                Quaternion targetRot = Quaternion.Euler(0f, yaw, 0f);
                t.localRotation = Quaternion.Slerp(t.localRotation, targetRot, turnSpeed * Time.fixedDeltaTime);
            }
        }

        // Player Input (Behavior = Send Messages) appelle automatiquement cette méthode
        public void OnMove(InputValue value)
        {
            Vector2 v = value.Get<Vector2>();
            inputX = v.x; // clavier: A/D, flèches ; gamepad: stick gauche X
        }
    }
}
