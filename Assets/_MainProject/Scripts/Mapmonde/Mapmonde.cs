using UnityEngine;

public class Mapmonde : MonoBehaviour
{
    [SerializeField]
    private GameObject map;

    [SerializeField]
    private float speed = 1.0f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 rotation = map.transform.rotation.eulerAngles;
        rotation.y += speed * Time.deltaTime;
        map.transform.transform.eulerAngles = rotation;
    }
}
