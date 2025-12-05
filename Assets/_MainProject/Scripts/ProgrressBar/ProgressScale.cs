using UnityEngine;

public class ProgressScale : ProgressBarAbstract
{
    public float min_scale = 0;
    public float max_scale = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
           
    }

    void FixedUpdate()
    {
        SetMin(min_scale);
        SetMax(max_scale);
        SetNormalValue(GameManager.instance.speedFactor);
        transform.localScale = new Vector3(transform.localScale.x, GetValue(), transform.localScale.z);
    }
}
