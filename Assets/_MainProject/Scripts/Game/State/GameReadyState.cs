using UnityEngine;
using StateMachine;

public class GameReadyState : State
{

    public override void Start()
    {
    }

    public override void Enter()
    {
        GameManager.instance.readyUI.SetActive(true);
    }

    public override void Leave()
    {
        GameManager.instance.readyUI.gameObject.SetActive(false);
    }

    public override void Update()
    {
        if(GameManager.instance.playerInput.actions["Interact"].WasPressedThisFrame() || GameManager.instance.playerInput.actions["Jump"].WasPressedThisFrame())
        {
            stateMachine.TransitionTo("normalFly");
        }
    }

}

