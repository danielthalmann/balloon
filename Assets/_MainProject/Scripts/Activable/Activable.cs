using System;
using UnityEngine;

public class Activable : MonoBehaviour
{
    [SerializeField]
    float activationDuration = 4.0f;
    [SerializeField]
    float ReactivationDuration = 6.0f;
    [SerializeField]
    float effectDuration = 10.0f;
    [SerializeField]
    float effectStrength = 5.0f;


    float activatedTimer = 0;

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

        Activation();
    }


    void Activation()
    {
        if (activated)
        {
            activatedTimer += Time.deltaTime;

            if (!activatedStarted)
            {
                activatedStarted = true;
                engine.addForce(effectStrength);
                activatedTimer = 0;
            }

            if (activatedTimer > effectDuration)
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
