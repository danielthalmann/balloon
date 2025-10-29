using UnityEngine;

public class BorderOfCamera : MonoBehaviour
{
    [SerializeField]
    private Camera mainCam;
    [SerializeField]
    private float x = 0;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (mainCam == null)
        {
            mainCam = Camera.main;
        }
    }

    private void FixedUpdate()
    {
        float distance = Mathf.Abs(mainCam.transform.localPosition.z - transform.localPosition.z);
        float frustumHeight = 2.0f * distance * Mathf.Tan(mainCam.fieldOfView * 0.5f * Mathf.Deg2Rad);
        float frustumWidth = frustumHeight * mainCam.aspect;

        transform.localPosition = new Vector3(-frustumWidth / 2 + x, transform.localPosition.y, transform.localPosition.z);
        /*
        Vector3 scale = transform.localScale;
        scale.x = frustumWidth;
        scale.y = frustumHeight;
        transform.localScale = scale;
        */

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
