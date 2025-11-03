using UnityEngine;
using StateMachine;

public class GameNormalFlyState : State
{

    protected EngineUpward engineUpward;

    public override void Start()
    {

    }

    public override void Enter()
    {
        ((GameManager)stateMachine).player.enabled = true;
        ((GameManager)stateMachine).engineUpward.SetFallForce(((GameManager)stateMachine).fallForce);
    }

    public override void Leave()
    {
        ((GameManager)stateMachine).engineUpward.SetFallForce(0);
    }

    public override void Update()
    {

    }

    public override void FixedUpdate()
    {
        if (((GameManager)stateMachine).engineUpward.GetValue() < .1f)
        {
            stateMachine.TransitionTo("Lose");
        }
    }
}

