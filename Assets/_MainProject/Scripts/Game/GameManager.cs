using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class GameManager : StateMachine.StateMachine
{
    public PlayerInput playerInput;

    public Player player;

    [Header("Topography")]
    public Topography topography;
    public Topography ground;

    [Header("UI")]
    public GameObject readyUI;
    public GameObject loseUI;
    public GameObject winUI;

    [Header("Engine")]
    public EngineUpward engineUpward;

    [Header("Camera")]
    public CameraFollow cameraFollow;

    [Header("Scene")]
    public SceneLoaderManager sceneLoader;

    [Header("Instant value")]
    [Range(0,2.0f)]
    public float speedFactor = 1.0f;


    [Header("Game parameters")]
    public GameParameter parameter;

    private List<ActivableContract> activables = new List<ActivableContract>();

    private float flyDistance = 0;

    public static GameManager instance { get; protected set; } = null;

    private float diffHeightFly = 1f;

    /// <summary>
    /// add fly duration
    /// </summary>
    /// <param name="time"></param>
    public void AddFlyTime(float time)
    {
        flyDistance += (time * speedFactor);
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
        AddState("tutorial", new TutorialState());
        AddState("ready", new GameReadyState());
        AddState("normalFly", new GameNormalFlyState());
        AddState("lose", new GameLoseState());
        AddState("win", new GameWinState());

        flyDistance = 0;

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
    /// Return the diff of elevation of balloon and the elevation of ground
    /// </summary>
    /// <returns></returns>
    public float GetElevationFly()
    {
        return diffHeightFly;
    }

    /// <summary>
    /// Calculate the speed of engine from the height of the ground
    /// </summary>
    public void CalculateSpeedFactor()
    {
        float height_engine = GetEngineUpwardValue();
        float height_ground = parameter.groundCurve.Evaluate(flyDistance / parameter.levelDistance);
        diffHeightFly = height_engine - height_ground;

        if(diffHeightFly < 0)
        {
            speedFactor = 1.0f;
            return;
        }

        if (diffHeightFly > parameter.limitOfBestFly)
        {
            speedFactor = 0;
        } else 
        {
            speedFactor = 2.0f - (diffHeightFly / (parameter.limitOfBestFly / 2));
        }

    }

    /// <summary>
    /// Test if the balloon has reach the duration time of fly
    /// </summary>
    public void TestFlyLose()
    {
        float height = parameter.groundCurve.Evaluate(flyDistance / parameter.levelDistance);
        float balloonHeight = GetEngineUpwardValue();
        //Debug.Log("height:" + height + " balloonHeight:" + balloonHeight);

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
        if (flyDistance > parameter.levelDistance)
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
        float t = (flyDistance / parameter.levelDistance);

        topography.h = h;
        topography.t = t;

        ground.h = h;
        ground.t = t;

    }
}
