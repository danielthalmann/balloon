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

    public bool hover = false;

    bool activatedStarted = false;

    [SerializeField]
    RotateLinear rotateLinear;

    [SerializeField]
    EngineUpward engine;


    [SerializeField]
    ProgressBar progressBar;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        progressBar.min = 0f;
        progressBar.max = activationDuration;
        progressBar.SetValue(activationDurationTimer);
    }


    // Update is called once per frame
    void Update()
    {

        TreatActivation();

        if (hover)
        {
            if (!progressBar.gameObject.activeSelf)
            {
                progressBar.gameObject.SetActive(true);
            }
        } else
        {
            if (!activated && progressBar.gameObject.activeSelf)
            {
                progressBar.gameObject.SetActive(false);
            }
        }
    }

    public void Activate()
    {
        if (!activated || effectDurationTimer > reactivationDuration)
        {

            progressBar.min = 0f;
            progressBar.max = activationDuration;
            progressBar.SetValue(activationDurationTimer);

            if (rotateLinear != null)
                rotateLinear.value = progressBar.slideValue;

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
        if (!activated || effectDurationTimer > reactivationDuration)
        {
            activationDurationTimer = 0;
            if (!activated)
            {
                progressBar.SetValue(activationDurationTimer);
            }
            if (rotateLinear != null)
                rotateLinear.value = progressBar.slideValue;
        }
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
            progressBar.min = 0f;
            progressBar.max = effectDuration;
            progressBar.SetValue(effectDuration - effectDurationTimer);
            if (rotateLinear != null)
                rotateLinear.value = progressBar.slideValue;


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
