using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Gère les actions de contrôle de la montgolfière (montée, boost, etc.)
/// </summary>
public class BalloonActions : MonoBehaviour
{
    [Header("Paramètres de montée")]
    [SerializeField] private float simpleLiftForce = 3f;    // Force de montée simple
    [SerializeField] private float simpleLiftDuration = 2f; // Durée de montée simple
    
    [SerializeField] private float boostLiftForce = 2f;     // Force de montée par boost
    [SerializeField] private float boostLiftDuration = 1f;  // Durée de chaque boost
    [SerializeField] private int maxBoostCount = 3;         // Nombre max de boosts consécutifs
    
    [SerializeField] private float holdLiftForce = 1.5f;    // Force de montée continue
    [SerializeField] private float holdResourceMax = 10f;   // Resource max pour la montée continue
    [SerializeField] private float holdResourceRegenRate = 2f; // Vitesse de régénération
    
    [Header("Références")]
    [SerializeField] private BalloonController balloonController;
    
    [Header("Debug")]
    [SerializeField] private bool showDebugInfo = true;
    
    // Variables privées pour l'Input System
    private PlayerInput playerInput;
    private InputAction simpleLiftAction;
    private InputAction boostLiftAction;
    private InputAction holdLiftAction;
    
    // Variables de gameplay
    private int currentBoostCount = 0;
    private float currentHoldResource;
    private bool isHolding = false;
    
    void Awake()
    {
        // Récupère le composant PlayerInput
        playerInput = GetComponent<PlayerInput>();
        
        // Récupère les actions (utilise les actions existantes pour le moment)
        simpleLiftAction = playerInput.actions["Jump"];      // Espace
        boostLiftAction = playerInput.actions["Attack"];     // Clic gauche / Enter
        holdLiftAction = playerInput.actions["Interact"];    // E (avec Hold interaction)
        
        // Initialise la ressource
        currentHoldResource = holdResourceMax;
    }
    
    void OnEnable()
    {
        // Active les actions et abonne les callbacks
        simpleLiftAction.Enable();
        boostLiftAction.Enable();
        holdLiftAction.Enable();
        
        simpleLiftAction.performed += OnSimpleLift;
        boostLiftAction.performed += OnBoostLift;
        holdLiftAction.performed += OnHoldLiftStart;
        holdLiftAction.canceled += OnHoldLiftEnd;
    }
    
    void OnDisable()
    {
        // Désactive les actions et désabonne les callbacks
        simpleLiftAction.performed -= OnSimpleLift;
        boostLiftAction.performed -= OnBoostLift;
        holdLiftAction.performed -= OnHoldLiftStart;
        holdLiftAction.canceled -= OnHoldLiftEnd;
        
        simpleLiftAction.Disable();
        boostLiftAction.Disable();
        holdLiftAction.Disable();
    }
    
    void Update()
    {
        // Gère la montée continue
        HandleHoldLift();
        
        // Régénère la ressource de montée continue
        RegenerateHoldResource();
        
        // Reset du compteur de boost après un délai
        HandleBoostReset();
        
        // Affichage debug
        if (showDebugInfo)
        {
            DisplayDebugInfo();
        }
    }
    
    /// <summary>
    /// Callback pour la montée simple (une impulsion)
    /// </summary>
    private void OnSimpleLift(InputAction.CallbackContext context)
    {
        if (balloonController != null)
        {
            balloonController.ApplyLift(simpleLiftForce, simpleLiftDuration);
            
            if (showDebugInfo)
            {
                Debug.Log($"Montée simple appliquée : Force={simpleLiftForce}, Durée={simpleLiftDuration}s");
            }
        }
    }
    
    /// <summary>
    /// Callback pour la montée par boost (impulsions multiples)
    /// </summary>
    private void OnBoostLift(InputAction.CallbackContext context)
    {
        if (balloonController != null && currentBoostCount < maxBoostCount)
        {
            balloonController.ApplyLift(boostLiftForce, boostLiftDuration);
            currentBoostCount++;
            
            if (showDebugInfo)
            {
                Debug.Log($"Boost appliqué : {currentBoostCount}/{maxBoostCount}, Force={boostLiftForce}");
            }
        }
        else if (showDebugInfo && currentBoostCount >= maxBoostCount)
        {
            Debug.Log("Nombre maximum de boosts atteint !");
        }
    }
    
    /// <summary>
    /// Callback pour le début de la montée continue
    /// </summary>
    private void OnHoldLiftStart(InputAction.CallbackContext context)
    {
        if (currentHoldResource > 0)
        {
            isHolding = true;
            
            if (showDebugInfo)
            {
                Debug.Log("Début de la montée continue");
            }
        }
    }
    
    /// <summary>
    /// Callback pour la fin de la montée continue
    /// </summary>
    private void OnHoldLiftEnd(InputAction.CallbackContext context)
    {
        isHolding = false;
        
        // Restaure le modificateur de chute normal
        if (balloonController != null)
        {
            balloonController.FallModifier = 1f;
        }
        
        if (showDebugInfo)
        {
            Debug.Log("Fin de la montée continue");
        }
    }
    
    /// <summary>
    /// Gère la montée continue tant que le bouton est maintenu
    /// </summary>
    private void HandleHoldLift()
    {
        if (isHolding && currentHoldResource > 0 && balloonController != null)
        {
            // Consomme la ressource
            currentHoldResource -= Time.deltaTime;
            
            // Applique la force de montée
            balloonController.FallModifier = -holdLiftForce;
            
            // Arrête si la ressource est épuisée
            if (currentHoldResource <= 0)
            {
                currentHoldResource = 0;
                isHolding = false;
                balloonController.FallModifier = 1f;
                
                if (showDebugInfo)
                {
                    Debug.Log("Ressource de montée continue épuisée !");
                }
            }
        }
    }
    
    /// <summary>
    /// Régénère la ressource de montée continue
    /// </summary>
    private void RegenerateHoldResource()
    {
        if (!isHolding && currentHoldResource < holdResourceMax)
        {
            currentHoldResource += holdResourceRegenRate * Time.deltaTime;
            currentHoldResource = Mathf.Min(currentHoldResource, holdResourceMax);
        }
    }
    
    /// <summary>
    /// Reset du compteur de boost après un délai
    /// </summary>
    private void HandleBoostReset()
    {
        // Reset automatique après 3 secondes sans utilisation
        // (Logique simplifiée, pourrait être améliorée avec un timer)
        if (currentBoostCount > 0 && !boostLiftAction.WasPressedThisFrame())
        {
            // Cette logique pourrait être améliorée avec un vrai système de timer
        }
    }
    
    /// <summary>
    /// Affiche les informations de debug
    /// </summary>
    private void DisplayDebugInfo()
    {
        if (currentHoldResource < holdResourceMax || isHolding)
        {
            Debug.Log($"Ressource Hold: {currentHoldResource:F1}/{holdResourceMax} | Boosts: {currentBoostCount}/{maxBoostCount}");
        }
    }
    
    /// <summary>
    /// Reset manuel du compteur de boost (peut être appelé depuis l'extérieur)
    /// </summary>
    public void ResetBoostCount()
    {
        currentBoostCount = 0;
        
        if (showDebugInfo)
        {
            Debug.Log("Compteur de boost resetté");
        }
    }
    
    /// <summary>
    /// Propriété pour accéder à la ressource de montée continue
    /// </summary>
    public float HoldResource => currentHoldResource;
    
    /// <summary>
    /// Propriété pour accéder au nombre de boosts utilisés
    /// </summary>
    public int BoostCount => currentBoostCount;
}