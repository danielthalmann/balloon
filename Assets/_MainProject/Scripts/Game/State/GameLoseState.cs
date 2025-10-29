using UnityEngine;
using StateMachine;
using System.Collections;

public class GameLoseState : State
{
    public override void Start()
    {

    }

    public override void Enter()
    {
        ((GameManager)stateMachine).player.enabled = false;
        ((GameManager)stateMachine).engineUpward.SetFallForce(0);
        ((GameManager)stateMachine).StartCoroutine(WaitAndFinish(3.0f));
    }

    public override void Leave()
    {

    }

    public override void Update()
    {

    }

    public override void FixedUpdate()
    {

    }


    // every 2 seconds perform the print()
    private IEnumerator WaitAndFinish(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        ((GameManager)stateMachine).sceneLoader.LoadScene("Menu");
    }
}

