using UnityEngine;

namespace Prototype.Engine
{
    /// <summary>
    /// Génère automatiquement des murs invisibles autour d'un niveau d'engin
    /// pour empêcher le joueur de tomber
    /// </summary>
    [RequireComponent(typeof(EngineLevel))]
    public class EngineLevelBounds : MonoBehaviour
    {
        [Header("Configuration des murs")]
        [SerializeField] private float wallHeight = 3f; // Hauteur des murs
        [SerializeField] private float wallThickness = 0.2f; // Épaisseur des murs
        [SerializeField] private bool createFloor = true; // Créer un sol pour le niveau
        [SerializeField] private float floorThickness = 0.1f;
        
        [Header("Debug")]
        [SerializeField] private bool showBounds = true;
        [SerializeField] private Color boundsColor = Color.red;
        
        private EngineLevel engineLevel;
        private GameObject boundsContainer;
        
        void Awake()
        {
            engineLevel = GetComponent<EngineLevel>();
        }
        
        void Start()
        {
            CreateLevelBounds();
        }
        
        /// <summary>
        /// Crée les murs et le sol invisibles autour du niveau
        /// </summary>
        private void CreateLevelBounds()
        {
            if (engineLevel == null) return;
            
            // Créer un conteneur pour tous les colliders
            boundsContainer = new GameObject("LevelBounds");
            boundsContainer.transform.SetParent(transform);
            boundsContainer.transform.localPosition = Vector3.zero;
            boundsContainer.transform.localRotation = Quaternion.identity;
            
            float width = engineLevel.LevelWidth;
            float height = engineLevel.LevelHeight;
            float floorOffset = engineLevel.FloorYOffset; // Utiliser l'offset du EngineLevel
            
            // Créer le sol si demandé
            if (createFloor)
            {
                CreateFloor(width, floorOffset);
            }
            
            // Créer les 4 murs (du sol vers le haut)
            float wallYPosition = floorOffset + wallHeight / 2f;
            
            CreateWall("LeftWall", new Vector3(-width/2f - wallThickness/2f, wallYPosition, 0), new Vector3(wallThickness, wallHeight, width));
            CreateWall("RightWall", new Vector3(width/2f + wallThickness/2f, wallYPosition, 0), new Vector3(wallThickness, wallHeight, width));
            CreateWall("FrontWall", new Vector3(0, wallYPosition, width/2f + wallThickness/2f), new Vector3(width, wallHeight, wallThickness));
            CreateWall("BackWall", new Vector3(0, wallYPosition, -width/2f - wallThickness/2f), new Vector3(width, wallHeight, wallThickness));
        }
        
        /// <summary>
        /// Crée un mur invisible
        /// </summary>
        private void CreateWall(string wallName, Vector3 position, Vector3 size)
        {
            GameObject wall = new GameObject(wallName);
            wall.transform.SetParent(boundsContainer.transform);
            wall.transform.localPosition = position;
            wall.transform.localRotation = Quaternion.identity;
            
            // Ajouter un Box Collider
            BoxCollider wallCollider = wall.AddComponent<BoxCollider>();
            wallCollider.size = size;
            wallCollider.isTrigger = false; // Collider physique, pas trigger
            
            // Pas de tag personnalisé pour éviter les erreurs
        }
        
        /// <summary>
        /// Crée le sol du niveau
        /// </summary>
        private void CreateFloor(float width, float floorOffset)
        {
            GameObject floor = new GameObject("Floor");
            floor.transform.SetParent(boundsContainer.transform);
            floor.transform.localPosition = new Vector3(0, floorOffset - floorThickness/2f, 0);
            floor.transform.localRotation = Quaternion.identity;
            
            // Ajouter un Box Collider pour le sol
            BoxCollider floorCollider = floor.AddComponent<BoxCollider>();
            floorCollider.size = new Vector3(width, floorThickness, width);
            floorCollider.isTrigger = false;
            
            // Pas de tag personnalisé pour éviter les erreurs
        }
        
        /// <summary>
        /// Recrée les limites si les paramètres du niveau changent
        /// </summary>
        [ContextMenu("Recreate Bounds")]
        public void RecreateBounds()
        {
            // Détruire l'ancien conteneur s'il existe
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
            
            // Recréer les limites
            CreateLevelBounds();
        }
        
        /// <summary>
        /// Nettoyage quand l'objet est détruit
        /// </summary>
        void OnDestroy()
        {
            if (boundsContainer != null && Application.isPlaying)
            {
                Destroy(boundsContainer);
            }
        }
        
        // Debug visuel
        void OnDrawGizmosSelected()
        {
            if (!showBounds || engineLevel == null) return;
            
            float width = engineLevel.LevelWidth;
            float height = engineLevel.LevelHeight;
            float floorOffset = engineLevel.FloorYOffset;
            float wallYPosition = floorOffset + wallHeight / 2f;
            
            Gizmos.color = boundsColor;
            Gizmos.matrix = transform.localToWorldMatrix;
            
            // Dessiner les murs
            // Mur gauche
            Gizmos.DrawWireCube(new Vector3(-width/2f - wallThickness/2f, wallYPosition, 0), 
                               new Vector3(wallThickness, wallHeight, width));
            
            // Mur droit
            Gizmos.DrawWireCube(new Vector3(width/2f + wallThickness/2f, wallYPosition, 0), 
                               new Vector3(wallThickness, wallHeight, width));
            
            // Mur avant
            Gizmos.DrawWireCube(new Vector3(0, wallYPosition, width/2f + wallThickness/2f), 
                               new Vector3(width, wallHeight, wallThickness));
            
            // Mur arrière
            Gizmos.DrawWireCube(new Vector3(0, wallYPosition, -width/2f - wallThickness/2f), 
                               new Vector3(width, wallHeight, wallThickness));
            
            // Dessiner le sol si activé
            if (createFloor)
            {
                Gizmos.color = new Color(boundsColor.r, boundsColor.g, boundsColor.b, 0.3f);
                Gizmos.DrawCube(new Vector3(0, floorOffset - floorThickness/2f, 0), 
                               new Vector3(width, floorThickness, width));
            }
        }
    }
}
