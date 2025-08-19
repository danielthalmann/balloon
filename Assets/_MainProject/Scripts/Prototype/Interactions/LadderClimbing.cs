using UnityEngine;

/// <summary>
/// Permet au joueur de grimper librement sur les échelles
/// </summary>
public class LadderClimbing : MonoBehaviour
{
    [Header("Configuration de l'échelle")]
    [SerializeField] private float climbSpeed = 3f;
    [SerializeField] private float snapDistance = 1f; // Distance pour "coller" à l'échelle
    
    [Header("Limites de l'échelle")]
    [SerializeField] private float bottomY = 0f; // Bas de l'échelle
    [SerializeField] private float topY = 8f;    // Haut de l'échelle
    
    [Header("UI")]
    [SerializeField] private GameObject interactionPopup;
    [SerializeField] private string interactionText = "Utilisez W/S pour monter/descendre";
    
    [Header("Debug")]
    [SerializeField] private bool showDebugInfo = true;
    [SerializeField] private bool showDebugGizmos = true;
    
    private bool playerOnLadder = false;
    private bool playerNearLadder = false;
    private PlayerMovement currentPlayer;
    private Vector3 originalPlayerPosition;
    
    void Start()
    {
        // Cache le popup au démarrage
        if (interactionPopup != null)
        {
            interactionPopup.SetActive(false);
        }
        
        // Calcule automatiquement les limites si pas définies
        if (bottomY == 0f && topY == 8f)
        {
            CalculateLadderLimits();
        }
    }
    
    void Update()
    {
        HandleLadderInteraction();
        HandleClimbing();
    }
    
    /// <summary>
    /// Gère l'entrée et sortie de l'échelle
    /// </summary>
    private void HandleLadderInteraction()
    {
        if (playerNearLadder && !playerOnLadder && Input.GetKeyDown(KeyCode.E))
        {
            StartClimbing();
        }
        else if (playerOnLadder && Input.GetKeyDown(KeyCode.E))
        {
            StopClimbing();
        }
    }
    
    /// <summary>
    /// Gère le mouvement sur l'échelle
    /// </summary>
    private void HandleClimbing()
    {
        if (!playerOnLadder || currentPlayer == null) return;
        
        // Récupère l'input vertical (W/S ou flèches haut/bas)
        float verticalInput = Input.GetAxisRaw("Vertical");
        
        if (Mathf.Abs(verticalInput) > 0.1f)
        {
            // Calcule le nouveau Y
            Vector3 currentPos = currentPlayer.transform.position;
            float newY = currentPos.y + (verticalInput * climbSpeed * Time.deltaTime);
            
            // Limite le mouvement aux bornes de l'échelle
            newY = Mathf.Clamp(newY, bottomY, topY);
            
            // Applique le mouvement (garde X et Z fixes sur l'échelle)
            currentPlayer.transform.position = new Vector3(
                transform.position.x, 
                newY, 
                currentPos.z
            );
            
            if (showDebugInfo)
            {
                Debug.Log($"Grimpe: Y={newY:F2}, Input={verticalInput:F2}");
            }
        }
    }
    
    /// <summary>
    /// Commence à grimper l'échelle
    /// </summary>
    private void StartClimbing()
    {
        if (currentPlayer == null) return;
        
        playerOnLadder = true;
        originalPlayerPosition = currentPlayer.transform.position;
        
        // Colle le joueur à l'échelle (même X)
        Vector3 snapPosition = new Vector3(
            transform.position.x,
            currentPlayer.transform.position.y,
            currentPlayer.transform.position.z
        );
        currentPlayer.transform.position = snapPosition;
        
        // Désactive le mouvement horizontal du joueur
        currentPlayer.SetClimbingMode(true);
        
        // Met à jour l'UI
        if (interactionPopup != null)
        {
            interactionPopup.SetActive(false);
        }
        
        if (showDebugInfo)
        {
            Debug.Log("Début de l'escalade");
        }
    }
    
    /// <summary>
    /// Arrête de grimper l'échelle
    /// </summary>
    private void StopClimbing()
    {
        if (currentPlayer == null) return;
        
        playerOnLadder = false;
        
        // Réactive le mouvement horizontal du joueur
        currentPlayer.SetClimbingMode(false);
        
        // Détecte sur quel étage le joueur se trouve
        DetectAndSetCurrentLevel();
        
        if (showDebugInfo)
        {
            Debug.Log("Fin de l'escalade");
        }
    }
    
    /// <summary>
    /// Détecte l'étage actuel du joueur et met à jour ses paramètres
    /// </summary>
    private void DetectAndSetCurrentLevel()
    {
        BasketLevel[] allLevels = FindObjectsOfType<BasketLevel>();
        BasketLevel closestLevel = null;
        float closestDistance = float.MaxValue;
        
        foreach (BasketLevel level in allLevels)
        {
            float distance = Mathf.Abs(currentPlayer.transform.position.y - level.transform.position.y);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestLevel = level;
            }
        }
        
        if (closestLevel != null)
        {
            currentPlayer.SetBasketCenter(closestLevel.GetCenter());
            currentPlayer.SetMaxMoveDistance(closestLevel.GetMaxRadius());
            
            if (showDebugInfo)
            {
                Debug.Log($"Joueur sur l'étage {closestLevel.GetLevelNumber()}");
            }
        }
    }
    
    /// <summary>
    /// Calcule automatiquement les limites de l'échelle
    /// </summary>
    private void CalculateLadderLimits()
    {
        bottomY = transform.position.y - (transform.localScale.y / 2f);
        topY = transform.position.y + (transform.localScale.y / 2f);
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearLadder = true;
            currentPlayer = other.GetComponent<PlayerMovement>();
            
            // Affiche le popup
            if (interactionPopup != null && !playerOnLadder)
            {
                interactionPopup.SetActive(true);
            }
            
            if (showDebugInfo)
            {
                Debug.Log("Joueur près de l'échelle");
            }
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Si le joueur s'éloigne trop, arrête l'escalade
            if (playerOnLadder)
            {
                StopClimbing();
            }
            
            playerNearLadder = false;
            currentPlayer = null;
            
            // Cache le popup
            if (interactionPopup != null)
            {
                interactionPopup.SetActive(false);
            }
            
            if (showDebugInfo)
            {
                Debug.Log("Joueur s'éloigne de l'échelle");
            }
        }
    }
    
    void OnDrawGizmosSelected()
    {
        if (!showDebugGizmos) return;
        
        // Dessine l'échelle
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, transform.localScale);
        
        // Dessine les limites
        Gizmos.color = Color.red;
        Vector3 bottomPos = new Vector3(transform.position.x, bottomY, transform.position.z);
        Vector3 topPos = new Vector3(transform.position.x, topY, transform.position.z);
        
        Gizmos.DrawWireSphere(bottomPos, 0.3f);
        Gizmos.DrawWireSphere(topPos, 0.3f);
        Gizmos.DrawLine(bottomPos, topPos);
    }
}