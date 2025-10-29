using UnityEditor;

namespace StateMachine
{
    public class State
    {
        protected StateMachine stateMachine;

        public virtual void Start()
        {

        }
                
        public virtual void Enter()
        {
            
        }

        public virtual void Leave()
        {

        }

        public virtual void Update()
        {

        }

        public virtual void FixedUpdate()
        {

        }

        public void SetStateMachine(StateMachine machine)
        {
            stateMachine = machine;
        }

    }
}