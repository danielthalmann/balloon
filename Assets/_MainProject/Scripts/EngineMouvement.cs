using UnityEngine;

public class EngineMouvement : MonoBehaviour
{
    [Header("Forces")]
    [SerializeField] private float gravityForce = 10f;     // Force de gravité
    [SerializeField] private float propulsionForce = 20f;  // Force de déplacement horizontal
    
    [Header("Effets des zones")]
    [SerializeField] private float ascensionForce = 15f;   // Force vers le haut dans la zone d'ascension
    [SerializeField] private float slowDownFactor = 0.5f;  // Facteur de ralentissement dans la zone lente
    
    private Rigidbody rb;
    private bool isInAscensionZone = false;
    private bool isInSlowZone = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;
            rb.linearDamping = 0.5f;  // Un peu de résistance pour plus de réalisme
        }
    }

    void FixedUpdate()
    {
        if (rb == null) return;
        
        // Application des forces
        Vector3 forceDirection = Vector3.zero;
        
        // Force horizontale (propulsion)
        float currentPropulsion = propulsionForce;
        if (isInSlowZone)
            currentPropulsion *= slowDownFactor;
            
        forceDirection.x = currentPropulsion;
        
        // Force verticale (gravité ou ascension)
        if (isInAscensionZone)
            forceDirection.y = ascensionForce;  // Force vers le haut
        else
            forceDirection.y = -gravityForce;   // Force vers le bas (gravité)
            
        // Application de la force résultante
        rb.AddForce(forceDirection);
    }
    
    // Détection des zones
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("AscensionZone"))
            isInAscensionZone = true;
            
        if (other.CompareTag("SlowZone"))
            isInSlowZone = true;
    }
    
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("AscensionZone"))
            isInAscensionZone = false;
            
        if (other.CompareTag("SlowZone"))
            isInSlowZone = false;
    }

    void Update() {
        Debug.Log("Zone ascension: " + isInAscensionZone + ", Zone lente: " + isInSlowZone);
    }
}
