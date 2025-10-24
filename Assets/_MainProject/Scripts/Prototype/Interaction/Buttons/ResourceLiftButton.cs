using UnityEngine;

namespace Prototype.Interaction.Buttons
{
    /// <summary>
    /// Bouton qui consomme une ressource pour remonter
    /// Maintient la remontée pendant 3 secondes
    /// </summary>
    public class ResourceLiftButton : MonoBehaviour, IInteractable
    {
        private Prototype.Engine.EngineController engineController;
        private Prototype.Engine.Actions.ResourceConsumingLiftAction currentAction;
        
        [SerializeField] private float maxResource = 100f;
        [SerializeField] private float resourceCostPerSecond = 20f;
        [SerializeField] private float holdDuration = 3f;
        
        private float pressTime = 0f;
        private bool isPressed = false;
        
        void Start()
        {
            engineController = FindFirstObjectByType<Prototype.Engine.EngineController>();
            
            if (engineController == null)
            {
                Debug.LogError($"❌ {gameObject.name}: Aucun EngineController trouvé!");
            }
        }
        
        void Update()
        {
            // Auto-release après holdDuration
            if (isPressed)
            {
                pressTime += Time.deltaTime;
                if (pressTime >= holdDuration)
                {
                    Release();
                }
            }
        }
        
        public void OnClicked()
        {
            if (engineController == null || isPressed) return;
            
            isPressed = true;
            pressTime = 0f;
            
            currentAction = new Prototype.Engine.Actions.ResourceConsumingLiftAction(
                engineController,
                maxResource,
                resourceCostPerSecond
            );
            engineController.ExecuteAction(currentAction);
            currentAction.SetLiftActive(true);
            
            Debug.Log($"💧 ResourceLift: Démarrage (ressource: {maxResource})");
        }
        
        public void Release()
        {
            if (currentAction != null)
            {
                currentAction.SetLiftActive(false);
            }
            isPressed = false;
            Debug.Log($"💧 ResourceLift: Arrêt");
        }
        
        public string GetDescription()
        {
            return "Bouton Remontée Ressource";
        }
    }
}
