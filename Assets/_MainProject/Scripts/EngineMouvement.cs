using UnityEngine;

// Cette classe gère le mouvement d'un objet (ballon/cube) sous l'influence de forces et de zones spéciales
public class EngineMouvement : MonoBehaviour
{
    // Paramètres de base pour les forces physiques
    [Header("Forces")]
    [SerializeField] private float gravityForce = 10f;     // Contrôle la vitesse de chute (vers le bas)
    [SerializeField] private float propulsionForce = 20f;  // Contrôle la vitesse horizontale (déplacement latéral)
    
    // Paramètres pour les effets spéciaux des zones
    [Header("Effets des zones")]
    [SerializeField] private float ascensionForce = 30f;   // Force vers le haut appliquée dans les zones d'ascension (contrebalance la gravité)
    [SerializeField] private float slowDownFactor = 0.2f;  // Multiplicateur pour ralentir le mouvement horizontal (plus petit = plus lent)
    
    // Références et états
    private Rigidbody rb;                // Référence au composant Rigidbody qui gère la physique
    private bool isInAscensionZone = false;  // Indique si l'objet est actuellement dans une zone d'ascension
    private bool isInSlowZone = false;       // Indique si l'objet est actuellement dans une zone de ralentissement

    // Initialisation au démarrage
    void Start()
    {
        // Récupère le composant Rigidbody attaché à cet objet
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;         // Désactive la gravité par défaut de Unity pour utiliser notre propre système
            rb.linearDamping = 0.5f;       // Ajoute un freinage aux mouvements pour plus de réalisme
        }
    }

    // FixedUpdate est appelé à intervalle fixe, idéal pour la physique
    void FixedUpdate()
    {
        if (rb == null) return;  // Sécurité pour éviter les erreurs si pas de Rigidbody
        
        // Initialise le vecteur de force à appliquer
        Vector3 forceDirection = Vector3.zero;
        
        // Calcul de la force horizontale
        float currentPropulsion = propulsionForce;  // Force de base
        if (isInSlowZone)
            currentPropulsion *= slowDownFactor;    // Réduit la force si dans une zone lente
            
        forceDirection.x = currentPropulsion;      // Applique la force horizontale (axe X)
        
        // Calcul de la force verticale
        if (isInAscensionZone)
            forceDirection.y = ascensionForce;      // Force vers le haut dans la zone d'ascension
        else
            forceDirection.y = -gravityForce;       // Force vers le bas (gravité) ailleurs
            
        // Application de la force calculée au Rigidbody
        rb.AddForce(forceDirection);
    }
    
    // Détecte l'entrée dans une zone (trigger)
    void OnTriggerEnter(Collider other)
    {
        // Vérifie si l'objet entre dans une zone d'ascension
        if (other.CompareTag("AscensionZone"))
        {
            isInAscensionZone = true;               // Active l'état "en zone d'ascension"
            Debug.Log("Entrée dans la zone d'ascension");
        }
            
        // Vérifie si l'objet entre dans une zone lente
        if (other.CompareTag("SlowZone"))
        {
            isInSlowZone = true;                    // Active l'état "en zone lente"
            Debug.Log("Entrée dans la zone lente");
        }
    }

    // Détecte la sortie d'une zone (trigger)
    void OnTriggerExit(Collider other)
    {
        // Vérifie si l'objet sort d'une zone d'ascension
        if (other.CompareTag("AscensionZone"))
        {
            isInAscensionZone = false;              // Désactive l'état "en zone d'ascension"
            Debug.Log("Sortie de la zone d'ascension");

            // Réinitialiser la vitesse verticale
            Vector3 velocity = rb.linearVelocity;
            velocity.y = 0;
            rb.linearVelocity = velocity;
        }
            
        // Vérifie si l'objet sort d'une zone lente
        if (other.CompareTag("SlowZone"))
        {
            isInSlowZone = false;                   // Désactive l'état "en zone lente"
            Debug.Log("Sortie de la zone lente");

            // Réinitialiser la vitesse horizontale ou l'ajuster à la valeur standard
            Vector3 velocity = rb.linearVelocity;
            velocity.x = propulsionForce * 0.1f; // Ajuste la vitesse initiale horizontale
            rb.linearVelocity = velocity;
        }
    }

    // Update est appelé à chaque frame
    void Update() {
        // Méthode vide mais peut être utilisée pour des actions non liées à la physique
    }
}
