using UnityEngine;

namespace Prototype.Engine.Actions
{
    /// <summary>
    /// Action: Fait remonter l'engin pendant une durée fixe
    /// </summary>
    public class TimedLiftAction : EngineAction
    {
        private float duration;
        private float elapsedTime = 0f;

        public TimedLiftAction(EngineController engineController, float duration) 
            : base(engineController)
        {
            this.duration = duration;
        }

        public override void OnStart()
        {
            elapsedTime = 0f;
            engine.StartLifting();
        }

        public override void Update()
        {
            elapsedTime += Time.deltaTime;
            
            if (IsFinished())
            {
                Stop();
            }
        }

        public override void OnEnd()
        {
            engine.StopLifting();
        }

        public override bool IsFinished()
        {
            return elapsedTime >= duration;
        }
    }
}