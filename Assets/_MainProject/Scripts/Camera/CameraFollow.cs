using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollow : MonoBehaviour
{
    public Camera currentCamera;
    public float smoothSpeed = 0.125f;
    public Transform target;
    private Vector3 offset;
    public Vector3 macroZoom;

    public bool macro;


    /// <summary>
    /// Start est appelé avant la première mise à jour d'image
    /// </summary>
    void Start()
    {
        // enregistre initialement la position de la caméra par rapport à l'objet observé
        offset = currentCamera.transform.position - target.transform.position;
        if (macro)
        {
            currentCamera.transform.position = target.position + offset; 
        }
    }


    /// <summary>
    /// Update est appelé une fois par rendu d'image
    /// </summary>
    void Update()
    {
        Vector3 desiredPosition = target.position + offset;
        if (macro)
        {
            desiredPosition += macroZoom;
        }

        // adouci le déplacement de la caméra avec Lerp
        Vector3 SmoothPosition = Vector3.Lerp(currentCamera.transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        currentCamera.transform.position = SmoothPosition;

    }
}
