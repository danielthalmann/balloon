using UnityEngine;
using FMOD.Studio;
using System.Collections.Generic;

public class ActivableSound : MonoBehaviour
{

    public FMODUnity.EventReference activeSound;
    
    private EventInstance soundInstance;

    private Activable activable;

    private List<EventInstance> eventInstances = new List<EventInstance>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        activable = GetComponent<Activable>();
    }

    /// <summary>
    /// Ajoute une nouvelle instance du son
    /// </summary>
    /// <param name="eventReference"></param>
    /// <returns></returns>
    public EventInstance CreateInstance(FMODUnity.EventReference eventReference)
    {
        EventInstance instance = FMODUnity.RuntimeManager.CreateInstance(eventReference);
        eventInstances.Add(instance);
        return instance;
    }

    private void OnEnable()
    {
        soundInstance = CreateInstance(activeSound);
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        if (activable.IsActivationInProgress())
        {
            FMOD.Studio.PLAYBACK_STATE state;
            soundInstance.getPlaybackState(out state);

            if (state == PLAYBACK_STATE.STOPPED)
            {
                soundInstance.start();
            }
        } else
        {
            soundInstance.stop(STOP_MODE.ALLOWFADEOUT);
        }
    }

    private void CleanUp()
    {
        foreach (EventInstance eventInstance in eventInstances)
        {
            eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            eventInstance.release();
        }
    }

    private void OnDisable()
    {
        CleanUp();
    }

}
