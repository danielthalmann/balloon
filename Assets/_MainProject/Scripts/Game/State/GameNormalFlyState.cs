using UnityEngine;
using StateMachine;

public class GameNormalFlyState : State
{

    protected EngineUpward engineUpward;
    protected bool tooHeight = false;

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


        GameManager.instance.TestFlyFinish();

        // calcul le facteur de vitesse
        GameManager.instance.CalculateSpeedFactor();


        if (GameManager.instance.topography.t > .6f && !GameManager.instance.rain.activeSelf)
        {
            GameManager.instance.rain.SetActive(true);
        }


        if (GameManager.instance.GetElevationFly() > .5f)
        {
            tooHeight = true;
        } 

        if (tooHeight)
        {
            if (GameManager.instance.GetElevationFly() < .3f)
            {
                tooHeight = false;
            }
            GameManager.instance.engineUpward.SetFallForce(GameManager.instance.parameter.fallForce * 2);
        }
        else
        {
            GameManager.instance.engineUpward.SetFallForce(GameManager.instance.parameter.fallForce);
        }



    }
}

