using UnityEngine;

public class EnginePositionStart : MonoBehaviour
{
    public GameObject plane; // Glissez votre plan ici dans l'Inspector
    public float startHeight = 15f; // Hauteur initiale au-dessus du plan
    
    void Start()
    {
        if (plane != null)
        {
            // Récupère la taille du plan et du cube
            Renderer planeRenderer = plane.GetComponent<Renderer>();
            Renderer cubeRenderer = GetComponent<Renderer>();
            
            // Calcule la position à gauche du plan
            float leftEdge = plane.transform.position.x - planeRenderer.bounds.size.x/2;
            float cubeHalfWidth = cubeRenderer.bounds.size.x/2;
            
            // Calcule la hauteur initiale (plan + demi-hauteur du cube + hauteur supplémentaire)
            float yPos = plane.transform.position.y + cubeRenderer.bounds.size.y/2 + startHeight;
            
            // Place le cube
            transform.position = new Vector3(leftEdge + cubeHalfWidth, yPos, plane.transform.position.z);
        }
    }
}
