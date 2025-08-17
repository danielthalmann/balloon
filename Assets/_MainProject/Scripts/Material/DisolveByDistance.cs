using UnityEngine;

public class DisolveByDistance : MonoBehaviour
{
    public Transform target;

    /// <summary>
    /// Distance à laquelle le matériel se dissoud
    /// </summary>
    public float distanceToDisolve;

    public float distanceToSolve;

    public float distance;

    public float smoothSpeed = 0.125f;

    public Material material;

    private float cutOff;
    private float currentCutOff;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (distanceToSolve < distanceToDisolve)
        {
            float temp = distanceToSolve;
            distanceToSolve = distanceToDisolve;
            distanceToDisolve = temp;
        }

        distance = Vector3.Distance(transform.position, target.position);
        if (distance > distanceToSolve)
        {
            cutOff = 0.0f;
        }
        if (distance < distanceToDisolve)
        {
            cutOff = 1.0f;
        }
        currentCutOff = cutOff;
        material.SetFloat("_Cutoff_Height", currentCutOff);
    }

    void Update()
    {

        if (currentCutOff != cutOff)
        {
            Debug.Log(currentCutOff != cutOff);


            if (cutOff == 1.0f)
            {
                currentCutOff += smoothSpeed * Time.deltaTime;

            } else
            {
                currentCutOff -= smoothSpeed * Time.deltaTime;
            }


            if (currentCutOff > 1.0f)
            {
                currentCutOff = 1.0f;
            }
            if (currentCutOff < 0.0f)
            {
                currentCutOff = 0.0f;
            }
            material.SetFloat("_Cutoff_Height", currentCutOff);
        }

        distance = Vector3.Distance(transform.position, target.position);

        if (distance > distanceToSolve)
        {
            cutOff = 0.0f;
        }
        if (distance < distanceToDisolve)
        {
            cutOff = 1.0f;
        }

    }
}
