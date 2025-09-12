using UnityEngine;

namespace Prototype.Engine
{
    /// <summary>
    /// Représente un étage d'un engin volant avec génération automatique des colliders
    /// Chaque étage peut avoir une largeur différente, des points d'accès, et génère ses murs/sols
    /// </summary>
    public class EngineLevel : MonoBehaviour
    {
        [Header("Configuration de l'étage")]
        [SerializeField] private int levelIndex = 0; // Index de l'étage (0 = rez-de-chaussée)
        [SerializeField] private float levelWidth = 5f; // Largeur de l'étage
        [SerializeField] private float levelHeight = 3f; // Hauteur de l'étage (utilisée aussi pour les murs)
        [SerializeField] private float floorYOffset = -1.5f; // Position du sol par rapport au center du transform
        
        [Header("Points d'accès")]
        [SerializeField] private Transform[] accessPoints; // Points où le joueur peut accéder à cet étage
        
        [Header("Génération des colliders")]
        [SerializeField] private float wallThickness = 0.2f; // Épaisseur des murs
        [SerializeField] private bool createFloor = true; // Créer un sol pour le niveau
        [SerializeField] private bool createCeiling = false; // Créer un plafond (désactivé par défaut)
        [SerializeField] private float floorThickness = 0.1f;
        // Variable d'ouverture d'échelle supprimée
        
        [Header("Visualisation")]
        [SerializeField] private bool showLevelBounds = true;
        [SerializeField] private bool showColliderBounds = true;
        [SerializeField] private Color levelColor = Color.blue;
        [SerializeField] private Color floorColor = Color.green;
        [SerializeField] private Color colliderColor = Color.red;
        
        // Variables privées pour la génération des colliders
        private GameObject boundsContainer;
        
        // Propriétés publiques
        public int LevelIndex => levelIndex;
        public float LevelWidth => levelWidth;
        public float LevelHeight => levelHeight;
        public float FloorYOffset => floorYOffset;
        public Transform[] AccessPoints => accessPoints;
        public float WallThickness => wallThickness;
        public float FloorThickness => floorThickness;
        // Propriété d'ouverture d'échelle supprimée
        
        void Awake()
        {
            // Si aucun point d'accès n'est défini, utiliser la position de l'étage
            if (accessPoints == null || accessPoints.Length == 0)
            {
                accessPoints = new Transform[] { transform };
            }
        }
        
        void Start()
        {
            CreateLevelBounds();
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
        
        /// <summary>
        /// Crée les murs et le sol invisibles autour du niveau
        /// </summary>
        private void CreateLevelBounds()
        {
            // Créer un conteneur pour tous les colliders
            boundsContainer = new GameObject("LevelBounds");
            boundsContainer.transform.SetParent(transform);
            boundsContainer.transform.localPosition = Vector3.zero;
            boundsContainer.transform.localRotation = Quaternion.identity;
            
            // Code des échelles supprimé
            // Plus d'échelles, on crée des sols normaux
            
            // Créer le sol normal
            if (createFloor)
            {
                CreateSolidFloor();
            }
            
            // Créer un plafond normal (optionnel)
            if (createCeiling)
            {
                // Plafond non implémenté
                Debug.Log("Plafond pas encore implémenté");
            }
            
            // Créer les 4 murs (du sol vers le haut)
            float wallYPosition = floorYOffset + levelHeight / 2f;
            
            CreateWall("LeftWall", new Vector3(-levelWidth/2f - wallThickness/2f, wallYPosition, 0), new Vector3(wallThickness, levelHeight, levelWidth));
            CreateWall("RightWall", new Vector3(levelWidth/2f + wallThickness/2f, wallYPosition, 0), new Vector3(wallThickness, levelHeight, levelWidth));
            CreateWall("FrontWall", new Vector3(0, wallYPosition, levelWidth/2f + wallThickness/2f), new Vector3(levelWidth, levelHeight, wallThickness));
            CreateWall("BackWall", new Vector3(0, wallYPosition, -levelWidth/2f - wallThickness/2f), new Vector3(levelWidth, levelHeight, wallThickness));
        }
        
        // Méthode FindLaddersForThisLevel supprimée
        
        // Méthode CreateFloorWithOpenings supprimée
        
        /// <summary>
        /// Crée un sol normal sans ouverture
        /// </summary>
        private void CreateSolidFloor()
        {
            GameObject floor = new GameObject("Floor");
            floor.transform.SetParent(boundsContainer.transform);
            floor.transform.localPosition = new Vector3(0, floorYOffset - floorThickness/2f, 0);
            floor.transform.localRotation = Quaternion.identity;
            
            BoxCollider floorCollider = floor.AddComponent<BoxCollider>();
            floorCollider.size = new Vector3(levelWidth, floorThickness, levelWidth);
            floorCollider.isTrigger = false;
        }
        
        // Méthode CreateFloorWithHole supprimée

        // Méthode CreateSmallFloor supprimée
        
        // Méthode CreateCeilingWithOpenings supprimée
        
        /// <summary>
        /// Crée un mur invisible
        /// </summary>
        private void CreateWall(string wallName, Vector3 position, Vector3 size)
        {
            GameObject wall = new GameObject(wallName);
            wall.transform.SetParent(boundsContainer.transform);
            wall.transform.localPosition = position;
            wall.transform.localRotation = Quaternion.identity;
            
            BoxCollider wallCollider = wall.AddComponent<BoxCollider>();
            wallCollider.size = size;
            wallCollider.isTrigger = false;
        }
        
        /// <summary>
        /// Recrée les limites si les paramètres du niveau changent
        /// </summary>
        [ContextMenu("Recreate Bounds")]
        public void RecreateBounds()
        {
            if (boundsContainer != null)
            {
                if (Application.isPlaying)
                {
                    Destroy(boundsContainer);
                }
                else
                {
                    DestroyImmediate(boundsContainer);
                }
            }
            
            CreateLevelBounds();
        }
        
        void OnDestroy()
        {
            if (boundsContainer != null && Application.isPlaying)
            {
                Destroy(boundsContainer);
            }
        }
        
        // Visualisation dans l'éditeur
        void OnDrawGizmosSelected()
        {
            if (!showLevelBounds && !showColliderBounds) return;
            
            Gizmos.matrix = transform.localToWorldMatrix;
            
            // Dessiner les limites de l'étage (zone logique)
            if (showLevelBounds)
            {
                Gizmos.color = levelColor;
                Vector3 levelCenter = new Vector3(0, floorYOffset + levelHeight / 2f, 0);
                Gizmos.DrawWireCube(levelCenter, new Vector3(levelWidth, levelHeight, levelWidth));
                
                // Dessiner le sol en vert
                Gizmos.color = floorColor;
                Vector3 floorCenter = new Vector3(0, floorYOffset, 0);
                Gizmos.DrawWireCube(floorCenter, new Vector3(levelWidth, 0.1f, levelWidth));
            }
            
            // Dessiner les colliders physiques (murs et sols)
            if (showColliderBounds)
            {
                float wallYPosition = floorYOffset + levelHeight / 2f;
                
                Gizmos.color = colliderColor;
                
                // Dessiner les murs
                Gizmos.DrawWireCube(new Vector3(-levelWidth/2f - wallThickness/2f, wallYPosition, 0), 
                                   new Vector3(wallThickness, levelHeight, levelWidth));
                Gizmos.DrawWireCube(new Vector3(levelWidth/2f + wallThickness/2f, wallYPosition, 0), 
                                   new Vector3(wallThickness, levelHeight, levelWidth));
                Gizmos.DrawWireCube(new Vector3(0, wallYPosition, levelWidth/2f + wallThickness/2f), 
                                   new Vector3(levelWidth, levelHeight, wallThickness));
                Gizmos.DrawWireCube(new Vector3(0, wallYPosition, -levelWidth/2f - wallThickness/2f), 
                                   new Vector3(levelWidth, levelHeight, wallThickness));
                
                // Dessiner le sol avec les ouvertures
                if (createFloor)
                {
                    Gizmos.color = new Color(colliderColor.r, colliderColor.g, colliderColor.b, 0.3f);
                    
                    // Visualisation des échelles supprimée
                    {
                        // Sol normal
                        Gizmos.DrawCube(new Vector3(0, floorYOffset - floorThickness/2f, 0), 
                                       new Vector3(levelWidth, floorThickness, levelWidth));
                    }
                }
            }
            
            // Dessiner les points d'accès
            if (showLevelBounds)
            {
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