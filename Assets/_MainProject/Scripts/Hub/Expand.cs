using UnityEngine;

public class Expand : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {


    }

    private void FixedUpdate()
    {
        //Vector3 diff = (Camera.main.transform.position - transform.position);
        //float distance = diff.magnitude;
        float distance = Mathf.Abs(Camera.main.transform.position.z - transform.position.z);
        float frustumHeight = 2.0f * distance * Mathf.Tan(Camera.main.fieldOfView * 0.5f * Mathf.Deg2Rad);
        float frustumWidth = frustumHeight * Camera.main.aspect;

        Vector3 scale = transform.localScale;
        scale.x = frustumWidth;
        scale.y = frustumHeight;
        transform.localScale = scale;
      
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
