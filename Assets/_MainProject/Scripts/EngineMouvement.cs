using UnityEngine;

// Cette classe gère le mouvement d'un objet (ballon/cube) sous l'influence de forces et de zones spéciales
public class EngineMouvement : MonoBehaviour
{
    // Paramètres de base pour les forces physiques
    [Header("Forces")]
    [SerializeField] private float gravityForce = 10f;     // Contrôle la vitesse de chute (vers le bas)
    [SerializeField] private float propulsionForce = 20f;  // Contrôle la vitesse horizontale (déplacement latéral)
    
    // Références et états
    private Rigidbody rb;                 // Référence au composant Rigidbody qui gère la physique
    
    // États des zones
    private bool isInAscensionZone = false;  // Indique si l'objet est actuellement dans une zone d'ascension
    private bool isInSlowZone = false;       // Indique si l'objet est actuellement dans une zone de ralentissement
    
    // Valeurs dynamiques reçues des zones
    private float currentAscensionForce = 0f;  // Force d'ascension actuelle appliquée par une zone
    private float currentSlowFactor = 1f;      // Facteur de ralentissement actuel (1 = pas de ralentissement)

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
        
        // Vérification des zones au démarrage
        CheckInitialZones();
    }
    
    // Vérifie si l'objet démarre dans une zone
    void CheckInitialZones()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 0.1f);
        foreach (var hitCollider in hitColliders)
        {
            ZoneEffect zoneEffect = hitCollider.GetComponent<ZoneEffect>();
            if (zoneEffect != null)
            {
                // Applique immédiatement l'effet de la zone si l'objet démarre dedans
                ApplyZoneEffect(zoneEffect, true);
            }
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
            currentPropulsion *= currentSlowFactor;    // Réduit la force selon le facteur de la zone
            
        forceDirection.x = currentPropulsion;      // Applique la force horizontale (axe X)
        
        // Calcul de la force verticale
        if (isInAscensionZone)
            forceDirection.y = currentAscensionForce;  // Force vers le haut selon l'intensité de la zone
        else
            forceDirection.y = -gravityForce;       // Force vers le bas (gravité) ailleurs
            
        // Application de la force calculée au Rigidbody
        rb.AddForce(forceDirection);
        
        // Visualisation des forces (utile pour le débogage)
        Debug.DrawRay(transform.position, forceDirection.normalized * 2f, Color.red);
    }
    
    // Applique l'effet d'une zone
    void ApplyZoneEffect(ZoneEffect zoneEffect, bool isEntering)
    {
        if (zoneEffect == null) return;
        
        // Selon le type de zone
        if (zoneEffect.GetZoneType() == ZoneEffect.ZoneType.Ascension)
        {
            isInAscensionZone = isEntering;
            
            if (isEntering)
            {
                // Entrée dans une zone d'ascension
                currentAscensionForce = zoneEffect.GetEffectIntensity();
                Debug.Log("Entrée dans la zone d'ascension (force: " + currentAscensionForce + ")");
            }
            else
            {
                // Sortie d'une zone d'ascension
                currentAscensionForce = 0f;  // Réinitialise la force
                Debug.Log("Sortie de la zone d'ascension");
            }
        }
        else if (zoneEffect.GetZoneType() == ZoneEffect.ZoneType.Slow)
        {
            isInSlowZone = isEntering;
            
            if (isEntering)
            {
                // Entrée dans une zone lente
                currentSlowFactor = zoneEffect.GetEffectIntensity();
                Debug.Log("Entrée dans la zone lente (facteur: " + currentSlowFactor + ")");
            }
            else
            {
                // Sortie d'une zone lente
                currentSlowFactor = 1f;  // Réinitialise le facteur (1 = pas de ralentissement)
                Debug.Log("Sortie de la zone lente");
            }
        }
    }
    
    // Détecte l'entrée dans une zone (trigger)
    void OnTriggerEnter(Collider other)
    {
        // Recherche le composant ZoneEffect
        ZoneEffect zoneEffect = other.GetComponent<ZoneEffect>();
        if (zoneEffect != null)
        {
            // Applique l'effet de la zone (entrée)
            ApplyZoneEffect(zoneEffect, true);
        }
        else
        {
            // Compatibilité avec l'ancien système de tags
            if (other.CompareTag("AscensionZone"))
            {
                isInAscensionZone = true;
                currentAscensionForce = 30f;  // Valeur par défaut si pas de composant ZoneEffect
                Debug.Log("Entrée dans la zone d'ascension (système de tags)");
            }
                
            if (other.CompareTag("SlowZone"))
            {
                isInSlowZone = true;
                currentSlowFactor = 0.2f;  // Valeur par défaut si pas de composant ZoneEffect
                Debug.Log("Entrée dans la zone lente (système de tags)");
            }
        }
    }

    // Détecte la sortie d'une zone (trigger)
    void OnTriggerExit(Collider other)
    {
        // Recherche le composant ZoneEffect
        ZoneEffect zoneEffect = other.GetComponent<ZoneEffect>();
        if (zoneEffect != null)
        {
            // Applique l'effet de la zone (sortie)
            ApplyZoneEffect(zoneEffect, false);
        }
        else
        {
            // Compatibilité avec l'ancien système de tags
            if (other.CompareTag("AscensionZone"))
            {
                isInAscensionZone = false;
                currentAscensionForce = 0f;
                Debug.Log("Sortie de la zone d'ascension (système de tags)");
            }
                
            if (other.CompareTag("SlowZone"))
            {
                isInSlowZone = false;
                currentSlowFactor = 1f;
                Debug.Log("Sortie de la zone lente (système de tags)");
            }
        }
    }

    // Update est appelé à chaque frame
    void Update() {
        // Méthode vide mais peut être utilisée pour des actions non liées à la physique
    }
}
