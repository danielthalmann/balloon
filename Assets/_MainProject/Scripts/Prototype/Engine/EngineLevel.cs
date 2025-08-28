using UnityEngine;

namespace Prototype.Engine
{
    /// <summary>
    /// Représente un étage d'un engin volant
    /// Chaque étage peut avoir une largeur différente et des points d'accès
    /// </summary>
    public class EngineLevel : MonoBehaviour
    {
        [Header("Configuration de l'étage")]
        [SerializeField] private int levelIndex = 0; // Index de l'étage (0 = rez-de-chaussée)
        [SerializeField] private float levelWidth = 5f; // Largeur de l'étage
        [SerializeField] private float levelHeight = 3f; // Hauteur de l'étage
        [SerializeField] private float floorYOffset = -1.5f; // Position du sol par rapport au center du transform
        
        [Header("Points d'accès")]
        [SerializeField] private Transform[] accessPoints; // Points où le joueur peut accéder à cet étage
        
        [Header("Visualisation")]
        [SerializeField] private bool showLevelBounds = true;
        [SerializeField] private Color levelColor = Color.blue;
        [SerializeField] private Color floorColor = Color.green;
        
        // Propriétés publiques
        public int LevelIndex => levelIndex;
        public float LevelWidth => levelWidth;
        public float LevelHeight => levelHeight;
        public float FloorYOffset => floorYOffset;
        public Transform[] AccessPoints => accessPoints;
        
        void Awake()
        {
            // Si aucun point d'accès n'est défini, utiliser la position de l'étage
            if (accessPoints == null || accessPoints.Length == 0)
            {
                accessPoints = new Transform[] { transform };
            }
        }
        
        /// <summary>
        /// Vérifie si une position est dans les limites de cet étage
        /// Prend en compte que le sol est décalé vers le bas
        /// </summary>
        public bool IsPositionInLevel(Vector3 position)
        {
            Vector3 localPosition = transform.InverseTransformPoint(position);
            
            // Vérifier les limites X et Z (largeur)
            bool inXZBounds = Mathf.Abs(localPosition.x) <= levelWidth / 2f &&
                             Mathf.Abs(localPosition.z) <= levelWidth / 2f;
            
            // Vérifier la hauteur Y (du sol vers le haut)
            bool inYBounds = localPosition.y >= floorYOffset && 
                            localPosition.y <= floorYOffset + levelHeight;
            
            return inXZBounds && inYBounds;
        }
        
        /// <summary>
        /// Trouve le point d'accès le plus proche d'une position donnée
        /// </summary>
        public Transform GetClosestAccessPoint(Vector3 position)
        {
            if (accessPoints == null || accessPoints.Length == 0)
                return transform;
            
            Transform closest = accessPoints[0];
            float closestDistance = Vector3.Distance(position, closest.position);
            
            for (int i = 1; i < accessPoints.Length; i++)
            {
                float distance = Vector3.Distance(position, accessPoints[i].position);
                if (distance < closestDistance)
                {
                    closest = accessPoints[i];
                    closestDistance = distance;
                }
            }
            
            return closest;
        }
        
        /// <summary>
        /// Obtient la position du sol dans l'espace monde
        /// </summary>
        public Vector3 GetFloorWorldPosition()
        {
            return transform.TransformPoint(new Vector3(0, floorYOffset, 0));
        }
        
        /// <summary>
        /// Obtient la hauteur du plafond dans l'espace monde
        /// </summary>
        public Vector3 GetCeilingWorldPosition()
        {
            return transform.TransformPoint(new Vector3(0, floorYOffset + levelHeight, 0));
        }
        
        // Visualisation dans l'éditeur
        void OnDrawGizmosSelected()
        {
            if (!showLevelBounds) return;
            
            Gizmos.color = levelColor;
            Gizmos.matrix = transform.localToWorldMatrix;
            
            // Dessiner les limites de l'étage (du sol vers le haut)
            Vector3 levelCenter = new Vector3(0, floorYOffset + levelHeight / 2f, 0);
            Gizmos.DrawWireCube(levelCenter, new Vector3(levelWidth, levelHeight, levelWidth));
            
            // Dessiner le sol en vert
            Gizmos.color = floorColor;
            Vector3 floorCenter = new Vector3(0, floorYOffset, 0);
            Gizmos.DrawWireCube(floorCenter, new Vector3(levelWidth, 0.1f, levelWidth));
            
            // Dessiner les points d'accès
            Gizmos.color = Color.yellow;
            if (accessPoints != null)
            {
                foreach (Transform accessPoint in accessPoints)
                {
                    if (accessPoint != null)
                    {
                        Gizmos.matrix = Matrix4x4.identity;
                        Gizmos.DrawWireSphere(accessPoint.position, 0.5f);
                        Gizmos.matrix = transform.localToWorldMatrix;
                    }
                }
            }
            
            // Afficher des infos textuelles dans l'éditeur
            Gizmos.matrix = Matrix4x4.identity;
            Gizmos.color = Color.white;
            #if UNITY_EDITOR
            UnityEditor.Handles.Label(transform.position + Vector3.up * 2f, $"Level {levelIndex}");
            #endif
        }
    }
}