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

        GameManager.instance.ground.curve = GameManager.instance.parameter.groundCurve;
        GameManager.instance.ground.width = GameManager.instance.parameter.levelDistance * 1.5f;
        GameManager.instance.ground.height = GameManager.instance.engineUpward.GetDeltaValue();
        GameManager.instance.ground.generateMesh();

        GameManager.instance.readyUI.SetActive(false);
        GameManager.instance.winUI.SetActive(false);
        GameManager.instance.loseUI.SetActive(false);
        GameManager.instance.player.enabled = false;
        if (GameManager.instance.cameraFollow)
        {
            GameManager.instance.cameraFollow.macro = false;
        }
        GameManager.instance.speedFactor = 0;
        GameManager.instance.rain.SetActive(false);

        if (tuto)
        {
            tuto.DisableAllTuto();
        }
    }

    public override void Leave()
    {

    }

    public override void Update()
    {
        if (tuto)
        {
            stateMachine.TransitionTo("tutorial");
        }
        else
        {
            stateMachine.TransitionTo("ready");
        }
    }

    public override void FixedUpdate()
    {

    }
}

