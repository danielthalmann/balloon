using UnityEngine;
using StateMachine;
using System.Collections;

public class GameWinState : State
{
    public override void Start()
    {

    }

    public override void Enter()
    {
        GameManager.instance.player.enabled = false;
        GameManager.instance.engineUpward.SetFallForce(0);
        GameManager.instance.winUI.SetActive(true);
        GameManager.instance.StartCoroutine(WaitAndFinish(3.0f));
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

        GameManager.instance.sceneLoader.LoadScene("Menu");
    }
}

