using System;
using UnityEngine;

public class Activable : MonoBehaviour
{
    [SerializeField]
    float activationDuration = 4.0f;
    [SerializeField]
    float reactivationDuration = 6.0f;
    [SerializeField]
    float effectDuration = 10.0f;
    [SerializeField]
    float effectStrength = 5.0f;


    public float activationDurationTimer = 0;
    public float effectDurationTimer = 0;

    public bool activated = false;
    bool activatedStarted = false;

    [SerializeField]
    EngineUpward engine;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
                
    }

    // Update is called once per frame
    void Update()
    {

        TreatActivation();
    }

    public void Activate()
    {
        if (!activated || effectDurationTimer > reactivationDuration)
        {
            activationDurationTimer += Time.deltaTime;
            if (activationDurationTimer > activationDuration)
            {
                activated = true;
                effectDurationTimer = 0;
            }
        }
    }

    public void Release()
    {
        activationDurationTimer = 0;
    }

    void TreatActivation()
    {
        if (activated)
        {
            effectDurationTimer += Time.deltaTime;

            if (!activatedStarted)
            {
                activatedStarted = true;
                engine.addForce(effectStrength);
                activationDurationTimer = 0;
            }

            if (effectDurationTimer > effectDuration)
            {
                activated = false;
            }
        }
        else
        {
            if (activatedStarted)
            {
                activatedStarted = false;
                engine.addForce(-effectStrength);
            }
        }
    }
}
