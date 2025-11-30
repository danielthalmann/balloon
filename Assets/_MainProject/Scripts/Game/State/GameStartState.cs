using UnityEngine;
using StateMachine;

public class GameStartState : State
{
    TutorialManager tuto;

    public override void Start()
    {
        tuto = GameManager.instance.GetComponent<TutorialManager>();
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
        GameManager.instance.speedFactor = 0;
        if (tuto)
        {
            tuto.DisableAllTuto();
        }

    }

    public override void Leave()
    {
        GameManager.instance.readyUI.gameObject.SetActive(false);
    }

    public override void Update()
    {
        if(GameManager.instance.playerInput.actions["Interact"].WasPressedThisFrame())
        {
            if (tuto)
            {
                stateMachine.TransitionTo("tutorial");
            }
            else
            {
                stateMachine.TransitionTo("normalFly");
            }
        }
    }

    public override void FixedUpdate()
    {

    }
}

