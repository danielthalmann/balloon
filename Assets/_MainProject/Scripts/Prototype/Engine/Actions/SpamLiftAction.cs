using UnityEngine;

namespace Prototype.Engine.Actions
{
    /// <summary>
    /// Action: Chaque appui provoque un petit "boost" de remontée
    /// Doit être spammé rapidement pour avoir du effet
    /// </summary>
    public class SpamLiftAction : EngineAction
    {
        private float boostDuration;
        private float boostStrength;
        private float elapsedTime = 0f;
        private int spamCount = 0;

        public SpamLiftAction(
            EngineController engineController, 
            float boostDuration, 
            float boostStrength) 
            : base(engineController)
        {
            this.boostDuration = boostDuration;
            this.boostStrength = boostStrength;
        }

        public override void OnStart()
        {
            engine.StartLifting();
            spamCount = 1;
            elapsedTime = 0f;
        }

        public override void Update()
        {
            elapsedTime += Time.deltaTime;
            
            if (elapsedTime >= boostDuration)
            {
                Stop();
            }
        }

        public override void OnEnd()
        {
            engine.StopLifting();
            spamCount = 0;
        }

        public override bool IsFinished()
        {
            return elapsedTime >= boostDuration;
        }

        /// <summary>
        /// Appelé à chaque nouveau spam du joueur
        /// </summary>
        public void RegisterSpam()
        {
            if (isActive)
            {
                spamCount++;
                // Réinitialise le timer à chaque spam
                elapsedTime = 0f;
            }
        }

        public int SpamCount => spamCount;
    }
}