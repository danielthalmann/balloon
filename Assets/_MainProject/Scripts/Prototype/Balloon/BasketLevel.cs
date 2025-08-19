using UnityEngine;

/// <summary>
/// Représente un étage de la nacelle avec ses propriétés
/// </summary>
public class BasketLevel : MonoBehaviour
{
    [Header("Propriétés de l'étage")]
    [SerializeField] private int levelNumber = 1;
    [SerializeField] private float maxMoveRadius = 3f;
    [SerializeField] private Transform centerPoint;
    
    [Header("Points d'intérêt")]
    [SerializeField] private Transform[] actionButtonPositions;
    [SerializeField] private Transform[] ladderPositions;
    
    [Header("Debug")]
    [SerializeField] private bool showDebugGizmos = true;
    
    void Start()
    {
        // Si pas de centre défini, utilise la position de cet objet
        if (centerPoint == null)
        {
            centerPoint = transform;
        }
    }
    
    /// <summary>
    /// Vérifie si une position est valide sur cet étage
    /// </summary>
    public bool IsPositionValid(Vector3 position)
    {
        Vector3 centerPos = centerPoint.position;
        Vector3 checkPos = position;
        
        // Projette sur le même plan Y
        centerPos.y = checkPos.y;
        
        float distance = Vector3.Distance(centerPos, checkPos);
        return distance <= maxMoveRadius;
    }
    
    /// <summary>
    /// Obtient le centre de cet étage
    /// </summary>
    public Transform GetCenter()
    {
        return centerPoint;
    }
    
    /// <summary>
    /// Obtient le rayon maximum de mouvement
    /// </summary>
    public float GetMaxRadius()
    {
        return maxMoveRadius;
    }
    
    /// <summary>
    /// Définit le numéro de l'étage
    /// </summary>
    public int GetLevelNumber()
    {
        return levelNumber;
    }
    
    void OnDrawGizmosSelected()
    {
        if (showDebugGizmos && centerPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(centerPoint.position, maxMoveRadius);
            
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(centerPoint.position, Vector3.one * 0.5f);
        }
    }
}
