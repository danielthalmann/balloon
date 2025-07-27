using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Camera camera;
    public float smoothSpeed = 0.125f;
    public Transform target;

    private Vector3 offset;

    /// <summary>
    /// Start est appelé avant la première mise à jour d'image
    /// </summary>
    void Start()
    {
        // enregistre initialement la position de la caméra par rapport à l'objet observé
        offset = camera.transform.position - target.transform.position;
    }


    /// <summary>
    /// Update est appelé une fois par rendu d'image
    /// </summary>
    void Update()
    {
        Vector3 desiredPosition = target.position + offset;
        // adouci le déplacement de la caméra avec Lerp
        Vector3 SmoothPosition = Vector3.Lerp(camera.transform.position, desiredPosition, smoothSpeed);
        camera.transform.position = SmoothPosition;
    }
}
