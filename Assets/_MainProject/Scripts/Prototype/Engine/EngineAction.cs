using UnityEngine;

namespace Prototype.Engine
{
    /// <summary>
    /// Classe abstraite représentant une action possible sur l'engin
    /// Chaque action gère sa propre logique et timing
    /// </summary>
    public abstract class EngineAction
    {
        protected EngineController engine;
        protected bool isActive = false;

        public EngineAction(EngineController engineController)
        {
            engine = engineController;
        }

        /// <summary>
        /// Appelée chaque frame si l'action est active
        /// </summary>
        public abstract void Update();

        /// <summary>
        /// Appelée au démarrage de l'action
        /// </summary>
        public abstract void OnStart();

        /// <summary>
        /// Appelée à la fin de l'action
        /// </summary>
        public abstract void OnEnd();

        /// <summary>
        /// Indique si l'action est encore active
        /// </summary>
        public abstract bool IsFinished();

        public bool IsActive => isActive;

        /// <summary>
        /// Démarre l'action
        /// </summary>
        public virtual void Start()
        {
            isActive = true;
            OnStart();
        }

        /// <summary>
        /// Arrête l'action
        /// </summary>
        public virtual void Stop()
        {
            isActive = false;
            OnEnd();
        }
    }
}