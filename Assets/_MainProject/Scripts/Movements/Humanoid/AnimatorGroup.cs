using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class AnimatorGroup : MonoBehaviour
{
    public AnimatorTarget[] animators;
    public float speed = 1.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach(AnimatorTarget animator in animators)
        {
            animator.Init(transform.forward, transform.up);
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach(AnimatorTarget animator in animators)
        {
            animator.Update(Time.deltaTime, speed);
        }
        
    }
}
