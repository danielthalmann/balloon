using UnityEngine;

/// <summary>
/// Contrôle le comportement principal de la montgolfière (chute, montée, modificateurs)
/// </summary>
public class BalloonController : MonoBehaviour
{
    [Header("Paramètres de chute")]
    [SerializeField] private float fallSpeed = 2f; // Vitesse de chute constante
    [SerializeField] private float currentFallModifier = 1f; // Modificateur actuel de chute
    
    [Header("Debug")]
    [SerializeField] private bool showDebugInfo = true;
    
    // Variables privées
    private Vector3 velocity;
    
    /// <summary>
    /// Propriété pour accéder au modificateur de chute depuis l'extérieur
    /// </summary>
    public float FallModifier 
    { 
        get => currentFallModifier; 
        set => currentFallModifier = Mathf.Max(0f, value); // Ne peut pas être négatif
    }
    
    void Start()
    {
        // Initialisation
        velocity = Vector3.zero;
    }
    
    void Update()
    {
        // Calcule la chute avec le modificateur actuel
        ApplyFall();
        
        // Applique le mouvement
        transform.position += velocity * Time.deltaTime;
        
        // Affichage debug
        if (showDebugInfo)
        {
            Debug.Log($"Vitesse de chute: {velocity.y:F2} | Modificateur: {currentFallModifier:F2}");
        }
    }
    
    /// <summary>
    /// Applique la chute constante avec les modificateurs
    /// </summary>
    private void ApplyFall()
    {
        // La chute est toujours vers le bas (axe Y négatif)
        velocity.y = -fallSpeed * currentFallModifier;
    }
    
    /// <summary>
    /// Applique une force de montée temporaire
    /// </summary>
    /// <param name="force">Force de montée (positive = vers le haut)</param>
    /// <param name="duration">Durée en secondes</param>
    public void ApplyLift(float force, float duration)
    {
        StartCoroutine(LiftCoroutine(force, duration));
    }
    
    /// <summary>
    /// Coroutine pour gérer la montée temporaire
    /// </summary>
    private System.Collections.IEnumerator LiftCoroutine(float force, float duration)
    {
        float originalModifier = currentFallModifier;
        
        // Applique la force de montée (valeur négative pour aller vers le haut)
        currentFallModifier = -force / fallSpeed;
        
        // Attend la durée spécifiée
        yield return new WaitForSeconds(duration);
        
        // Restaure le modificateur original
        currentFallModifier = originalModifier;
    }
    
    /// <summary>
    /// Modifie temporairement le modificateur de chute
    /// </summary>
    /// <param name="modifier">Nouveau modificateur</param>
    /// <param name="duration">Durée en secondes</param>
    public void ModifyFallSpeed(float modifier, float duration)
    {
        StartCoroutine(ModifyFallSpeedCoroutine(modifier, duration));
    }
    
    /// <summary>
    /// Coroutine pour modifier temporairement la vitesse de chute
    /// </summary>
    private System.Collections.IEnumerator ModifyFallSpeedCoroutine(float modifier, float duration)
    {
        float originalModifier = currentFallModifier;
        currentFallModifier = modifier;
        
        yield return new WaitForSeconds(duration);
        
        currentFallModifier = originalModifier;
    }
}