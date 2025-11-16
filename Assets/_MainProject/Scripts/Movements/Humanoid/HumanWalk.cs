using UnityEngine;

public class HumanWalk : MonoBehaviour
{
    [SerializeField]
    WalkAnimation walk;
    [SerializeField]
    GameObject target;

    [SerializeField]
    float startPosition = 0f;

    [SerializeField]
    LayerMask groundLayer;
    [SerializeField]
    float groundDistance = 0;
    [SerializeField]
    bool hitLayer = false;

    float elapseTime = 0f;

    Vector3 initPosition;
    Vector3 forward;
    Vector3 up;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        elapseTime = startPosition;
        initPosition = target.transform.localPosition;
        forward = transform.forward;
        up = transform.up;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        elapseTime += Time.deltaTime * walk.speed;
        Vector3 pos;

        float horizontalValue = walk.amplitude * walk.horizontalCurve.Evaluate(elapseTime);
        float verticalValue = walk.amplitude * walk.verticalCurve.Evaluate(elapseTime);
        pos = (forward * horizontalValue) + (up * verticalValue);

        target.transform.localPosition = initPosition + pos;

        if (hitLayer)
        {
            RaycastHit hit;
            if (Physics.Raycast(target.transform.position + up, -up, out hit, 1f + groundDistance, groundLayer))
            {
                target.transform.position = hit.point + (up * groundDistance);
            }

        }

        

    }
}
