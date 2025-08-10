using UnityEngine;

public class ForwardLinear : MonoBehaviour
{

    public Vector3 vector;

    public float speed;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        vector.Normalize();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += (vector * (speed * Time.deltaTime));
    }
}
