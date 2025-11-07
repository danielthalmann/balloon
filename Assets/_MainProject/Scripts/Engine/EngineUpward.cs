using UnityEngine;
using UnityEngine.UIElements;

public class EngineUpward : MonoBehaviour, ProgressBarContract
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

    GameManager game;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.position = new Vector3(transform.position.x, current, transform.position.z);
        game = UnityEngine.Object.FindAnyObjectByType<GameManager>();
        if (game == null)
        {
            Debug.LogError("GameManager not present");
        }
    }

    // Update is called once per frame
    void Update()
    {
        altimeter.value = GetNormalValue();

        nextcurrent = nextcurrent + ((GetActivableForce() - downwardVector) * Time.deltaTime);

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

    public float GetActivableForce()
    {
        return game.GetActivableForce();
    }

    public void SetFallForce(float force)
    {
        downwardVector = force;
    }

    public float GetValue()
    {
        return current;
    }

    public void SetMin(float v)
    {
        min = v;
    }

    public void SetMax(float v)
    {
        max = v;
    }

    public void SetValue(float v)
    {
        current = v;
    }

    public void SetActive(bool v)
    {
        gameObject.SetActive(v);
    }

    public bool IsActive()
    {
        return gameObject.activeSelf;
    }

    public float GetNormalValue()
    {
        return (current - min) / (max - min);
    }
}
