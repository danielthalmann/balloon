using UnityEngine;
using StateMachine;

public class TutorialState : State
{
    TutorialManager tuto;

    public override void Start()
    {
        tuto = GameManager.instance.GetComponent<TutorialManager>();
    }

    public override void Enter()
    {
        tuto.StartTuto();
    }

    public override void Leave()
    {
    }

    public override void Update()
    {
        if(GameManager.instance.playerInput.actions["Interact"].WasPressedThisFrame() || GameManager.instance.playerInput.actions["Jump"].WasPressedThisFrame())
        {
            if(!tuto.NextTuto())
            {
                stateMachine.TransitionTo("ready");
            }
        }
    }

    public override void FixedUpdate()
    {

    }
}

