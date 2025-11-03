using UnityEngine;
using StateMachine;

public class GameStartState : State
{


    public override void Start()
    {
    }

    public override void Enter()
    {
        ((GameManager)stateMachine).ready.SetActive(true);
        ((GameManager)stateMachine).player.enabled = false;
    }

    public override void Leave()
    {
        ((GameManager)stateMachine).ready.gameObject.SetActive(false);
    }

    public override void Update()
    {
        if(((GameManager)stateMachine).playerInput.actions["Interact"].WasPressedThisFrame())
        {
            stateMachine.TransitionTo("NormalFly");
        }
    }

    public override void FixedUpdate()
    {

    }
}

