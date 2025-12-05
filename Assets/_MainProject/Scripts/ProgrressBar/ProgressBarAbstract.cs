
using UnityEngine;
using System;

public abstract class ProgressBarAbstract : MonoBehaviour, ProgressBarContract
{
    protected float min = 0;
    protected float max = 0;
    protected float value = 0;
    protected float normalValue;


    public void SetValue(float v)
    {
        this.value = v;
        normalValue = (value / (max - min));
    }

    public void SetMin(float v)
    {
        min = v;
    }

    public void SetMax(float v)
    {
        max = v;
    }

    public float GetValue()
    {
        return value;
    }

    public void SetNormalValue(float v)
    {
        this.value = v * (max - min);
        normalValue = v;
    }

    public float GetNormalValue()
    {
        return normalValue;
    }

    public void SetActive(bool v)
    {
        gameObject.SetActive(v);
    }

    public bool IsActive()
    {
        return gameObject.activeSelf;
    }

}
