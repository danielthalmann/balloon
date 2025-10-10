using UnityEngine;
using UnityEngine.UIElements;

public class EngineUpward : MonoBehaviour
{

    [SerializeField]
    float min;
    [SerializeField]
    float max;
    [SerializeField]
    float current;
    float nextcurrent;
    [SerializeField]
    float upwardVector;
    [SerializeField]
    float downwardVector;
    [SerializeField]
    float speed = 5.0f;
    [SerializeField]
    Altimeter altimeter;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.position = new Vector3(transform.position.x, current, transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {

        altimeter.value = (current - min) / (max - min);

        nextcurrent = nextcurrent + ((upwardVector - downwardVector) * Time.deltaTime);

        if (nextcurrent > max)
        {
            nextcurrent = max;
        }
        if (nextcurrent < min)
        {
            nextcurrent = min;
        }

        current = Mathf.Lerp(current, nextcurrent, speed * Time.deltaTime);

        transform.position = new Vector3(transform.position.x, current, transform.position.z);

    }

    public void addForce(float force)
    {
        upwardVector += force;
    }
}
