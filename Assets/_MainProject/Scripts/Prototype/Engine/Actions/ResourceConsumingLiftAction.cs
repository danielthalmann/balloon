using UnityEngine;

namespace Prototype.Engine.Actions
{
    /// <summary>
    /// Action: Remontée active tant qu'on l'active, consomme une ressource
    /// </summary>
    public class ResourceConsumingLiftAction : EngineAction
    {
        private float resourceCostPerSecond;
        private float currentResource;
        private float maxResource;
        private bool shouldLift = true; // Contrôlé par le player via UpdateInput

        public ResourceConsumingLiftAction(
            EngineController engineController, 
            float maxResource, 
            float resourceCostPerSecond) 
            : base(engineController)
        {
            this.maxResource = maxResource;
            this.currentResource = maxResource;
            this.resourceCostPerSecond = resourceCostPerSecond;
        }

        public override void OnStart()
        {
            engine.StartLifting();
        }

        public override void Update()
        {
            if (shouldLift && currentResource > 0)
            {
                currentResource -= resourceCostPerSecond * Time.deltaTime;
                
                if (currentResource <= 0)
                {
                    currentResource = 0;
                    Stop();
                }
            }
            else
            {
                Stop();
            }
        }

        public override void OnEnd()
        {
            engine.StopLifting();
            shouldLift = false;
        }

        public override bool IsFinished()
        {
            return currentResource <= 0 || !shouldLift;
        }

        // Accesseurs
        public float CurrentResource => currentResource;
        public float MaxResource => maxResource;
        
        /// <summary>
        /// Le player appelle ça pour maintenir la remontée
        /// </summary>
        public void SetLiftActive(bool active)
        {
            shouldLift = active;
            
            if (active && currentResource > 0 && !isActive)
            {
                Start();
            }
        }

        /// <summary>
        /// Recharge la ressource
        /// </summary>
        public void Recharge(float amount)
        {
            currentResource = Mathf.Min(currentResource + amount, maxResource);
        }
    }
}