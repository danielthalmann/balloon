using UnityEngine;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;

namespace StateMachine
{

    public abstract class StateMachine : MonoBehaviour
    {
        protected Dictionary<string, State> states = new();
        protected Dictionary<string, object> datas = new();
        protected State currentState;

        // Init state machine
        public virtual void Init()
        {
            // Example 
            // AddState("Idle", new IdleState(), true);
            // AddState("Walk", new WalkState());
            // AddState("Run", new RunState());
            // AddState("Fight", new FightState());

        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            Init();

            foreach (State state in states.Values)
            {
                state.Start();
            }
            if (currentState != null)
                currentState.Enter();

        }

        // Update is called once per frame
        void Update()
        {
            if (currentState != null)
                currentState.Update();
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (currentState != null)
                currentState.FixedUpdate();
        }

        public void TransitionTo(string stateName)
        {

            State transitionState;

            if (states.TryGetValue(stateName, out transitionState))
            {
                if (currentState != null)
                    currentState.Leave();

                currentState = transitionState;
                currentState.Enter();
            } else
            {
                currentState = null;
                Debug.Log("Missing state : " + stateName);
            }
        }

        public void AddState(string stateName, State state, bool initialState = false)
        {
            state.SetStateMachine(this);
            states[stateName] = state;
            if (initialState)
                currentState = state;
        }

    }
}
