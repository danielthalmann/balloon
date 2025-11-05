using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class ProgressBar : ProgressBarAbstract
{

    private Slider slider;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        slider = GetComponent<Slider>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        slider.value = normalValue;
    }

}
