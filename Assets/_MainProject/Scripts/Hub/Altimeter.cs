using UnityEngine;

public class Altimeter : MonoBehaviour
{
    [System.Serializable]
    public struct MinMax
    {
        public float min;
        public float max;
    };


    [SerializeField]
    GameObject cursor;

    [SerializeField]
    [Range(0, 1f)]
    public float value = 0.5f;

    [SerializeField]
    [Range(0, 10f)]
    float speed = 5f;

    [SerializeField]
    MinMax minMax;

    Vector3 min;

    float diff;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        diff = minMax.max - minMax.min;
        cursor.transform.localPosition = new Vector3(cursor.transform.localPosition.x, minMax.min + (diff * value), cursor.transform.localPosition.z);
    }

  
    // Update is called once per frame
    void Update()
    {
        diff = minMax.max - minMax.min;
        cursor.transform.localPosition = Vector3.Lerp(cursor.transform.localPosition, 
            new Vector3(cursor.transform.localPosition.x, minMax.min + (diff * value), cursor.transform.localPosition.z), 
            Time.deltaTime * speed);
    }
}
