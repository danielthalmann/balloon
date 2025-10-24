using UnityEngine;

namespace Prototype.Interaction.Buttons
{
    /// <summary>
    /// Bouton qui déclenche une remontée temporaire (2 secondes)
    /// </summary>
    public class TimedLiftButton : MonoBehaviour, IInteractable
    {
        private Prototype.Engine.EngineController engineController;
        
        [SerializeField] private float liftDuration = 2f;
        
        void Start()
        {
            engineController = FindFirstObjectByType<Prototype.Engine.EngineController>();
            
            if (engineController == null)
            {
                Debug.LogError($"❌ {gameObject.name}: Aucun EngineController trouvé!");
            }
        }
        
        public void OnClicked()
        {
            if (engineController == null) return;
            
            var timedLift = new Prototype.Engine.Actions.TimedLiftAction(
                engineController, 
                liftDuration
            );
            engineController.ExecuteAction(timedLift);
            
            Debug.Log($"⏱️ TimedLift: {liftDuration}s");
        }
        
        public string GetDescription()
        {
            return "Bouton Remontée Temporaire";
        }
    }
}