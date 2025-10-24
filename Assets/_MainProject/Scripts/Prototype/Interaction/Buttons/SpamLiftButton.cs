using UnityEngine;
using System.Linq;

namespace Prototype.Interaction.Buttons
{
    /// <summary>
    /// Bouton à "spammer" - chaque clic = un boost
    /// </summary>
    public class SpamLiftButton : MonoBehaviour, IInteractable
    {
        private Prototype.Engine.EngineController engineController;
        
        [SerializeField] private float boostDuration = 0.5f;
        [SerializeField] private float boostStrength = 1f;
        
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
            
            // Cherche si une action spam existe déjà
            var spamAction = engineController.ActiveActions
                .OfType<Prototype.Engine.Actions.SpamLiftAction>()
                .FirstOrDefault();
            
            if (spamAction != null && spamAction.IsActive)
            {
                // Enregistre un nouveau spam
                spamAction.RegisterSpam();
                Debug.Log($"🔄 Spam #{spamAction.SpamCount}");
            }
            else
            {
                // Crée une nouvelle action spam
                spamAction = new Prototype.Engine.Actions.SpamLiftAction(
                    engineController,
                    boostDuration,
                    boostStrength
                );
                engineController.ExecuteAction(spamAction);
                spamAction.RegisterSpam();
                Debug.Log($"🚀 SpamLift initiée!");
            }
        }
        
        public string GetDescription()
        {
            return "Bouton Spam Remontée";
        }
    }
}
