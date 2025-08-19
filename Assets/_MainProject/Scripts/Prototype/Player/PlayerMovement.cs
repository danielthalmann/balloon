using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

/// <summary>
/// Gère le mouvement du joueur dans la nacelle (axes X et Z) avec le nouveau Input System
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    [Header("Paramètres de mouvement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;
    
    [Header("Contraintes de mouvement")]
    [SerializeField] private float maxMoveDistance = 3f; // Distance max depuis le centre de la nacelle
    
    [Header("Références")]
    [SerializeField] private Transform basketCenter; // Centre de la nacelle actuelle
    [SerializeField] private Animator animator;
    
    [Header("Debug")]
    [SerializeField] private bool showDebugInfo = false;
    
    // Variables privées pour l'Input System
    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction climbAction;
    
    // Variables de mouvement
    private Vector2 moveInput;
    private Vector3 inputDirection;
    private Vector3 lastMoveDirection;
    private bool isMoving;
    
    // Ajouter ces variables dans PlayerMovement
    [Header("État d'escalade")]
    [SerializeField] private bool isClimbing = false;

    void Awake()
    {
        // Récupère le composant PlayerInput
        playerInput = GetComponent<PlayerInput>();
        
        // Récupère les actions spécifiques
        moveAction = playerInput.actions["Move"];
        climbAction = playerInput.actions["Jump"]; // Utilise Jump pour grimper temporairement
    }
    
    void Start()
    {
        // Se positionne automatiquement sur l'étage 1 au démarrage
        if (basketCenter == null)
        {
            BasketLevel[] allLevels = FindObjectsByType<BasketLevel>(FindObjectsSortMode.None);
            BasketLevel level1 = allLevels.FirstOrDefault(level => level.GetLevelNumber() == 1);
            
            if (level1 != null)
            {
                SetBasketCenter(level1.GetCenter());
                SetMaxMoveDistance(level1.GetMaxRadius());
                
                // Positionne le joueur au centre de l'étage 1
                Vector3 level1Pos = level1.GetCenter().position;
                float floorHeight = level1Pos.y + (level1.transform.localScale.y / 2f); // Surface du sol
                transform.position = new Vector3(level1Pos.x, floorHeight + 1f, level1Pos.z); // +1f = hauteur du capsule
            }
        }
    }
    
    void OnEnable()
    {
        // Active les actions
        moveAction.Enable();
        climbAction.Enable();
    }
    
    void OnDisable()
    {
        // Désactive les actions
        moveAction.Disable();
        climbAction.Disable();
    }
    
    void Update()
    {
        // Récupère les inputs
        GetInput();
        
        // Applique le mouvement
        ApplyMovement();
        
        // Gère la rotation
        HandleRotation();
        
        // Met à jour l'animator
        UpdateAnimator();
    }
    
    /// <summary>
    /// Récupère les inputs du joueur via le nouveau Input System
    /// </summary>
    private void GetInput()
    {
        // Récupère l'input de mouvement (Vector2)
        moveInput = moveAction.ReadValue<Vector2>();
        
        Debug.Log($"moveInput brut: {moveInput}");
        
        // Seulement mouvement sur X, pas de Z
        inputDirection = new Vector3(moveInput.x, 0f, 0f);
        
        Debug.Log($"inputDirection avant normalize: {inputDirection}, magnitude: {inputDirection.magnitude}");
        
        if (inputDirection.magnitude > 0.1f)
        {
            inputDirection = inputDirection.normalized;
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }
        
        Debug.Log($"inputDirection final: {inputDirection}, isMoving: {isMoving}");
        
        if (showDebugInfo && isMoving)
        {
            Debug.Log($"Input Direction: {inputDirection}");
        }
    }
    
    /// <summary>
    /// Applique le mouvement en respectant les contraintes de la nacelle
    /// </summary>
    private void ApplyMovement()
    {
        Debug.Log($"ApplyMovement - isMoving: {isMoving}, basketCenter: {basketCenter != null}, isClimbing: {isClimbing}");
        
        if (!isMoving || basketCenter == null || isClimbing) 
        {
            Debug.Log("ApplyMovement - SORTIE PRÉMATURÉE");
            return;
        }
        
        Vector3 desiredMovement = inputDirection * moveSpeed * Time.deltaTime;
        Vector3 newPosition = transform.position + desiredMovement;
        
        Debug.Log($"Position actuelle: {transform.position}");
        Debug.Log($"Nouvelle position désirée: {newPosition}");
        Debug.Log($"Mouvement désiré: {desiredMovement}");
        
        if (IsPositionValid(newPosition))
        {
            Debug.Log("Position VALIDE - Mouvement appliqué");
            transform.position = newPosition;
            lastMoveDirection = inputDirection;
        }
        else
        {
            Debug.Log("Position INVALIDE - Mouvement bloqué");
        }
    }
    
    /// <summary>
    /// Vérifie si une position est valide (dans les limites de la nacelle)
    /// </summary>
    /// <param name="position">Position à vérifier</param>
    /// <returns>True si la position est valide</returns>
    private bool IsPositionValid(Vector3 position)
    {
        if (basketCenter == null) return true;
        
        // Calcule la distance depuis le centre de la nacelle (seulement X et Z)
        Vector3 centerPos = basketCenter.position;
        Vector3 checkPos = position;
        
        // On garde la même hauteur Y que le centre pour le calcul
        centerPos.y = checkPos.y;
        
        float distance = Vector3.Distance(centerPos, checkPos);
        
        return distance <= maxMoveDistance;
    }
    
    /// <summary>
    /// Gère la rotation du joueur selon la direction de mouvement
    /// </summary>
    private void HandleRotation()
    {
        if (lastMoveDirection.magnitude > 0.1f)
        {
            // Calcule la rotation désirée
            Quaternion targetRotation = Quaternion.LookRotation(lastMoveDirection);
            
            // Applique la rotation progressivement
            transform.rotation = Quaternion.Slerp(
                transform.rotation, 
                targetRotation, 
                rotationSpeed * Time.deltaTime
            );
        }
    }
    
    /// <summary>
    /// Met à jour les paramètres de l'animator
    /// </summary>
    private void UpdateAnimator()
    {
        if (animator != null)
        {
            animator.SetBool("isWalking", isMoving);
        }
    }
    
    /// <summary>
    /// Définit le centre de la nacelle actuelle
    /// </summary>
    /// <param name="center">Transform du centre de la nacelle</param>
    public void SetBasketCenter(Transform center)
    {
        basketCenter = center;
        if (showDebugInfo)
        {
            Debug.Log($"Centre de nacelle défini : {center.name}");
        }
    }
    
    /// <summary>
    /// Définit la distance maximum de mouvement
    /// </summary>
    /// <param name="distance">Distance maximum</param>
    public void SetMaxMoveDistance(float distance)
    {
        maxMoveDistance = Mathf.Max(0f, distance);
        if (showDebugInfo)
        {
            Debug.Log($"Distance max de mouvement définie : {maxMoveDistance}");
        }
    }

    /// <summary>
    /// Active/désactive le mode escalade
    /// </summary>
    public void SetClimbingMode(bool climbing)
    {
        isClimbing = climbing;
    }
    
    /// <summary>
    /// Dessine les gizmos pour visualiser les limites de mouvement
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (basketCenter != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(basketCenter.position, maxMoveDistance);
        }
    }
}