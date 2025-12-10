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
        GameManager.instance.player.enabled = true;
        GameManager.instance.topography.gameObject.SetActive(true);
    }

    public override void Leave()
    {
        GameManager.instance.engineUpward.SetFallForce(0);
    }

    public override void Update()
    {

    }

    public override void FixedUpdate()
    {
        GameManager.instance.AddFlyTime(Time.fixedDeltaTime);

        GameManager.instance.engineUpward.SetFallForce(GameManager.instance.parameter.fallForce);

        GameManager.instance.TestFlyFinish();

        // calcul le facteur de vitesse
        GameManager.instance.CalculateSpeedFactor();

    }
}

