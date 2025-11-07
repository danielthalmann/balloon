using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class GameManager : StateMachine.StateMachine
{
    public PlayerInput playerInput;

    public PlayerV2 player;

    public Topography topography;

    [Header("UI")]
    public GameObject readyUI;
    public GameObject loseUI;
    public GameObject winUI;

    public EngineUpward engineUpward;

    public SceneLoaderManager sceneLoader;

    [Header("Game parameters")]
    public GameParameter parameter;

    private List<ActivableContract> activables = new List<ActivableContract>();

    private float flyDurationTime = 0;

    public static GameManager instance { get; protected set; } = null;

    /// <summary>
    /// add fly duration
    /// </summary>
    /// <param name="time"></param>
    public void AddFlyTime(float time)
    {
        flyDurationTime += time;
    }

    /// <summary>
    /// return the force of all activable object
    /// </summary>
    /// <returns></returns>
    public float GetActivableForce()
    {
        float force = 0;

        foreach(Activable activable in activables)
        {
            force += activable.GetForce();
        }
        return force;
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

    /// <summary>
    /// Init state machine
    /// </summary>
    public override void Init()
    {
        if (GameManager.instance == null)
        {
            GameManager.instance = this;
        } else
        {
            throw (new System.Exception("GameManager is soon instanciate"));
        }

        playerInput = GetFirstObjectByType<PlayerInput>();
        if (engineUpward == null)
            engineUpward = GetFirstObjectByType<EngineUpward>();
        if (engineUpward == null)
            Debug.LogError("the engineUpward cannot be found in scene");
        if (sceneLoader == null)
            sceneLoader = GetFirstObjectByType<SceneLoaderManager>();

        // Example 
        AddState("start", new GameStartState(), true);
        AddState("normalFly", new GameNormalFlyState());
        AddState("lose", new GameLoseState());
        AddState("win", new GameWinState());

        flyDurationTime = 0;

    }

    /// <summary>
    /// Register a activable object
    /// </summary>
    /// <param name="activable"></param>
    public void RegisterActivable(ActivableContract activable)
    {
        activables.Add(activable);
    }

    /// <summary>
    /// call this function after every frame 
    /// </summary>
    public override void StateMachineFixedUpdate()
    {
        UpdateTopography();
    }

    /// <summary>
    /// Test if the balloon has reach the duration time of fly
    /// </summary>
    public void TestFlyFinish()
    {
        TestFlyLose();
        TestFlyWin();
    }

    /// <summary>
    /// Test if the balloon has reach the duration time of fly
    /// </summary>
    public void TestFlyLose()
    {
        float height = parameter.groundCurve.Evaluate(flyDurationTime / parameter.levelTimeDuration);
        float balloonHeight = GetEngineUpwardValue();
        Debug.Log("height:" + height + " balloonHeight:" + balloonHeight);

        if (balloonHeight < height)
        {
            TransitionTo("lose");
        }
    }

    /// <summary>
    /// Test if the balloon has reach the duration time of fly
    /// </summary>
    public void TestFlyWin()
    {
        if (flyDurationTime > parameter.levelTimeDuration)
        {
            TransitionTo("win");
        }
    }

    public float GetEngineUpwardValue()
    {
        return engineUpward.GetNormalValue();
    }

    public void UpdateTopography()
    {
        float h = GetEngineUpwardValue();
        float t = (flyDurationTime / parameter.levelTimeDuration);

        topography.h = h;
        topography.t = t;

    }
}
