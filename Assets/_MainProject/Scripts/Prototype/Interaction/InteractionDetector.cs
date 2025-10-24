using UnityEngine;
using UnityEngine.InputSystem;

namespace Prototype.Interaction
{
    /// <summary>
    /// Détecte les clics sur les objets interactifs
    /// </summary>
    public class InteractionDetector : MonoBehaviour
    {
        private Camera mainCamera;
        [SerializeField] private float clickDistance = 100f;
        private InputAction clickAction;
        private InputAction interactAction;

        void Start()
        {
            mainCamera = Camera.main;
            
            // Crée une InputAction pour le clic souris gauche
            clickAction = new InputAction("Click", binding: "<Mouse>/leftButton");
            clickAction.Enable();

                // Touche E
            interactAction = new InputAction("Interact", binding: "<Keyboard>/e");
            interactAction.Enable();
            
            if (mainCamera == null)
            {
                Debug.LogError("❌ Aucune caméra Main trouvée!");
            }
        }

        void Update()
        {
            if ((clickAction != null && clickAction.WasPressedThisFrame()) || 
                (interactAction != null && interactAction.WasPressedThisFrame()))
            {
                HandleClick();
            }
        }

        private void HandleClick()
        {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            
            if (Physics.Raycast(ray, out RaycastHit hit, clickDistance))
            {
                IInteractable interactable = hit.collider.GetComponent<IInteractable>();
                
                if (interactable != null)
                {
                    Debug.Log($"✓ Interaction: {interactable.GetDescription()}");
                    interactable.OnClicked();
                }
            }
        }

        void OnDisable()
        {
            clickAction?.Disable();
            interactAction?.Disable();
        }
    }
}