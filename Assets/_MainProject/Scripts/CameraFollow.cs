using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Camera camera;
    public float smoothSpeed = 0.125f;
    public Transform target;

    private Vector3 offset;

    /// <summary>
    /// Start est appel� avant la premi�re mise � jour d'image
    /// </summary>
    void Start()
    {
        // enregistre initialement la position de la cam�ra par rapport � l'objet observ�
        offset = camera.transform.position - target.transform.position;
    }


    /// <summary>
    /// Update est appel� une fois par rendu d'image
    /// </summary>
    void Update()
    {
        Vector3 desiredPosition = target.position + offset;
        // adouci le d�placement de la cam�ra avec Lerp
        Vector3 SmoothPosition = Vector3.Lerp(camera.transform.position, desiredPosition, smoothSpeed);
        camera.transform.position = SmoothPosition;
    }
}
