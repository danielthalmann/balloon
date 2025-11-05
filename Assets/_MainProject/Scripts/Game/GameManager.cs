using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class GameManager : StateMachine.StateMachine
{
    public PlayerInput playerInput;

    public PlayerV2 player;

    [Header("UI")]
    public GameObject ready;

    public EngineUpward engineUpward;

    public SceneLoaderManager sceneLoader;

    public float fallForce = 0.5f;

    private List<ActivableContract> activables = new List<ActivableContract>();


    // Init state machine
    public override void Init()
    {
        playerInput = GetFirstObjectByType<PlayerInput>();
        if (engineUpward == null)
            engineUpward = GetFirstObjectByType<EngineUpward>();
        if (engineUpward == null)
            Debug.LogError("the engineUpward cannot be found in scene");
        if (sceneLoader == null)
            sceneLoader = GetFirstObjectByType<SceneLoaderManager>();

        // Example 
        AddState("Start", new GameStartState(), true);
        AddState("NormalFly", new GameNormalFlyState());
        AddState("Lose", new GameLoseState());
        AddState("Win", new GameWinState());

    }

    /// <summary>
    /// retourne le premier objet selon le type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T GetFirstObjectByType<T>()
    {
        System.Type typeDeT = typeof(T);

        Object[] objs = Object.FindObjectsByType(typeDeT, FindObjectsSortMode.InstanceID);
        if (objs.Length > 0)
        {
            return (T)(object)objs[0];
        }
        return default(T);
    }


    public void RegisterActivable(ActivableContract activable)
    {
        activables.Add(activable);
    }

    public float GetActivableForce()
    {
        float force = 0;

        foreach(Activable activable in activables)
        {
            force += activable.GetForce();
        }
        Debug.Log("Force : " + force);
        return force;
    }



}
