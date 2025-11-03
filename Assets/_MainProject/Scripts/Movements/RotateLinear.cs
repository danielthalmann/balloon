using UnityEngine;

public class RotateLinear : MonoBehaviour
{

    [SerializeField]
    public float min = 0;
    [SerializeField]
    public float max = 0;
    [SerializeField]
    public float value = 0;

    [SerializeField]
    public Vector3 orientation;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        orientation.Normalize();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float rotateValue = Mathf.Lerp(min, max, value);

        Vector3 rotate = orientation * rotateValue;

        transform.eulerAngles = rotate;

    }

}
