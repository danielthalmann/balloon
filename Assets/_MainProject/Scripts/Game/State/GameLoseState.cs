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
        GameManager.instance.player.enabled = false;
        GameManager.instance.engineUpward.SetFallForce(0);
        GameManager.instance.loseUI.SetActive(true);
        GameManager.instance.StartCoroutine(WaitAndFinish(3.0f));
        foreach (FMODUnity.StudioEventEmitter fm in Object.FindObjectsByType<FMODUnity.StudioEventEmitter>(FindObjectsSortMode.None))
        {
            fm.Stop();
        }

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

