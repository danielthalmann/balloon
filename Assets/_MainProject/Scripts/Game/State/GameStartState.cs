using UnityEngine;
using StateMachine;

public class GameStartState : State
{


    public override void Start()
    {
    }

    public override void Enter()
    {
        GameManager.instance.topography.gameObject.SetActive(true);
        GameManager.instance.topography.curve = GameManager.instance.parameter.groundCurve;
        GameManager.instance.topography.generateMesh();
        GameManager.instance.readyUI.SetActive(true);
        GameManager.instance.winUI.SetActive(false);
        GameManager.instance.loseUI.SetActive(false);
        GameManager.instance.player.enabled = false;
    }

    public override void Leave()
    {
        GameManager.instance.readyUI.gameObject.SetActive(false);
    }

    public override void Update()
    {
        if(GameManager.instance.playerInput.actions["Interact"].WasPressedThisFrame())
        {
            stateMachine.TransitionTo("normalFly");
        }
    }

    public override void FixedUpdate()
    {

    }
}

