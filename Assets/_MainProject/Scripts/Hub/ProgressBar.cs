using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class ProgressBar : MonoBehaviour
{
    [SerializeField]
    public float min = 0;
    public float max = 0;
    public float value = 0;

    public float slideValue;

    private Slider slider;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        slider = GetComponent<Slider>();

    }

    public void SetValue(float v)
    {
        this.value = v;
        slideValue = (value / (max - min));
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        // Debug.Log("max : " + max + " min : " + min + " value : " + value + " slideValue : " + slideValue);
        slider.value = slideValue;
    }
}
