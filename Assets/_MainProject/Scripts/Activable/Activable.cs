using System;
using UnityEngine;

public class Activable : MonoBehaviour, ActivableContract
{
    [SerializeField]
    float activationDuration = 4.0f;
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
    ProgressBarAbstract progressBar;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        progressBar.SetMin(0f);
        progressBar.SetMax(activationDuration);
        progressBar.SetValue(activationDurationTimer);

        GameManager game = UnityEngine.Object.FindAnyObjectByType<GameManager>();
        if (game == null)
        {
            Debug.LogError("GameManager not present");
        } else
        {
            game.RegisterActivable(this);
        }
    }


    // Update is called once per frame
    void Update()
    {

        TreatActivation();

        if (hover)
        {
            if (!progressBar.IsActive())
            {
                progressBar.SetActive(true);
            }
        } else
        {
            if (!activated && progressBar.IsActive())
            {
                progressBar.SetActive(false);
            }
        }
    }

    public void Activate()
    {
        if (!activated)
        {

            progressBar.SetMin(0f);
            progressBar.SetMax(activationDuration);
            progressBar.SetValue(activationDurationTimer);

            if (rotateLinear != null)
                rotateLinear.value = progressBar.GetNormalValue();

            activationDurationTimer += Time.deltaTime;
            if (activationDurationTimer > activationDuration)
            {
                activated = true;
                activationDurationTimer = 0;
                effectDurationTimer = 0;
            }
        }
    }

    public void Release()
    {
        if (!activated)
        {
            activationDurationTimer = 0;
            if (!activated)
            {
                progressBar.SetValue(activationDurationTimer);
            }
            if (rotateLinear != null)
                rotateLinear.value = progressBar.GetNormalValue();
        }
    }

    void TreatActivation()
    {
        if (activated)
        {
            effectDurationTimer += Time.deltaTime;

            if (effectDurationTimer > effectDuration)
            {
                activated = false;
            }
            progressBar.SetMin(0f);
            progressBar.SetMax(effectDuration);
            progressBar.SetValue(effectDuration - effectDurationTimer);
            if (rotateLinear != null)
                rotateLinear.value = progressBar.GetNormalValue();

        }
    }

    public float GetForce()
    {
        if (activated)
        {
            return effectStrength;
        }
        return 0;
    }

    public bool IsActivated()
    {
        return activated;
    }
}
