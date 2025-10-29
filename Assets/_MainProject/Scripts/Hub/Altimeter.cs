using UnityEngine;

public class Altimeter : MonoBehaviour
{
    public enum Orientation
    {
        Horizontal,
        Vertival,
        Depth
    }

    [System.Serializable]
    public struct MinMax
    {
        public float min;
        public float max;
    };

    [SerializeField]
    Orientation orientation = Orientation.Vertival;

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
        if (orientation == Orientation.Vertival)
        {
            cursor.transform.localPosition = new Vector3(cursor.transform.localPosition.x, minMax.min + (diff * value), cursor.transform.localPosition.z);
        }
        if (orientation == Orientation.Horizontal)
        {
            cursor.transform.localPosition = new Vector3(minMax.min + (diff * value), cursor.transform.localPosition.y, cursor.transform.localPosition.z);
        }
        if (orientation == Orientation.Depth)
        {
            cursor.transform.localPosition = new Vector3(cursor.transform.localPosition.x, cursor.transform.localPosition.y, minMax.min + (diff * value));
        }

    }

  
    // Update is called once per frame
    void Update()
    {
        diff = minMax.max - minMax.min;
        Vector3 b = new Vector3();

        if (orientation == Orientation.Vertival)
        {
            b = new Vector3(cursor.transform.localPosition.x, minMax.min + (diff * value), cursor.transform.localPosition.z);
        }
        if (orientation == Orientation.Horizontal)
        {
            b = new Vector3(minMax.min + (diff * value), cursor.transform.localPosition.y, cursor.transform.localPosition.z);
        }
        if (orientation == Orientation.Depth)
        {
            b = new Vector3(cursor.transform.localPosition.x, cursor.transform.localPosition.y, minMax.min + (diff * value));
        }

        cursor.transform.localPosition = Vector3.Lerp(cursor.transform.localPosition, b, Time.deltaTime * speed);
        
    }
}
