using UnityEngine;

public class AnimatorTarget 
{
    public AnimCurve animationCurve;
    public GameObject targetObject;

    public bool hitLayer = false;
    public LayerMask groundLayer;
    public float groundDistance;
    public float startPosition;

    float elapseTime;
    Vector3 initPosition;
    Vector3 forward;
    Vector3 up;


    public void Init(Vector3 forward, Vector3 up)
    {
        elapseTime = 0;
        this.initPosition = targetObject.transform.localPosition;
        this.forward = forward;
        this.up = up;

    }

    public void Update(float deltaTime, float speed = 1.0f)
    {
        elapseTime += deltaTime * (animationCurve.speed * speed);
        Vector3 pos;

        float horizontalValue = animationCurve.amplitude * animationCurve.xCurve.Evaluate(elapseTime + startPosition);
        float verticalValue = animationCurve.amplitude * animationCurve.yCurve.Evaluate(elapseTime + startPosition);
        pos = (forward * horizontalValue) + (up * verticalValue);

        targetObject.transform.localPosition = initPosition + pos;

        if (hitLayer)
        {
            RaycastHit hit;
            if (Physics.Raycast(targetObject.transform.position + up, -up, out hit, 1f + groundDistance, groundLayer))
            {
                targetObject.transform.position = hit.point + (up * groundDistance);
            }

        }

    }

}
