using UnityEngine;

namespace Prototype.Engine
{
    /// <summary>
    /// Génère automatiquement des murs invisibles autour d'un niveau d'engin
    /// pour empêcher le joueur de tomber, avec des ouvertures pour les échelles
    /// </summary>
    [RequireComponent(typeof(EngineLevel))]
    public class EngineLevelBounds : MonoBehaviour
    {
        [Header("Configuration des murs")]
        [SerializeField] private float wallHeight = 3f; // Hauteur des murs
        [SerializeField] private float wallThickness = 0.2f; // Épaisseur des murs
        [SerializeField] private bool createFloor = true; // Créer un sol pour le niveau
        [SerializeField] private bool createCeiling = false; // Créer un plafond (désactivé par défaut)
        [SerializeField] private float floorThickness = 0.1f;
        
        [Header("Ouvertures pour échelles")]
        [SerializeField] private float ladderOpeningSize = 1.5f; // Taille de l'ouverture pour les échelles
        
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
            float floorOffset = engineLevel.FloorYOffset;
            
            // Trouver les échelles qui traversent ce niveau
            EngineLadder[] ladders = FindLaddersForThisLevel();
            
            // Créer le sol avec des ouvertures pour les échelles
            if (createFloor)
            {
                CreateFloorWithOpenings(width, floorOffset, ladders);
            }
            
            // Créer un plafond avec des ouvertures pour les échelles (optionnel)
            if (createCeiling)
            {
                CreateCeilingWithOpenings(width, floorOffset + height, ladders);
            }
            
            // Créer les 4 murs (du sol vers le haut)
            float wallYPosition = floorOffset + wallHeight / 2f;
            
            CreateWall("LeftWall", new Vector3(-width/2f - wallThickness/2f, wallYPosition, 0), new Vector3(wallThickness, wallHeight, width));
            CreateWall("RightWall", new Vector3(width/2f + wallThickness/2f, wallYPosition, 0), new Vector3(wallThickness, wallHeight, width));
            CreateWall("FrontWall", new Vector3(0, wallYPosition, width/2f + wallThickness/2f), new Vector3(width, wallHeight, wallThickness));
            CreateWall("BackWall", new Vector3(0, wallYPosition, -width/2f - wallThickness/2f), new Vector3(width, wallHeight, wallThickness));
        }
        
        /// <summary>
        /// Trouve toutes les échelles qui concernent ce niveau
        /// </summary>
        private EngineLadder[] FindLaddersForThisLevel()
        {
            // Chercher toutes les échelles dans l'engin parent
            var engineController = GetComponentInParent<EngineController>();
            if (engineController == null) return new EngineLadder[0];
            
            EngineLadder[] allLadders = engineController.GetComponentsInChildren<EngineLadder>();
            System.Collections.Generic.List<EngineLadder> relevantLadders = new System.Collections.Generic.List<EngineLadder>();
            
            foreach (EngineLadder ladder in allLadders)
            {
                // Vérifier si cette échelle concerne ce niveau
                if (ladder.BottomLevel == engineLevel || ladder.TopLevel == engineLevel)
                {
                    relevantLadders.Add(ladder);
                }
            }
            
            return relevantLadders.ToArray();
        }
        
        /// <summary>
        /// Crée un sol avec des ouvertures pour les échelles
        /// </summary>
        private void CreateFloorWithOpenings(float width, float floorOffset, EngineLadder[] ladders)
        {
            if (ladders.Length == 0)
            {
                // Pas d'échelles, créer un sol normal
                CreateSolidFloor(width, floorOffset);
                return;
            }
            
            // Créer des segments de sol autour des ouvertures d'échelles
            foreach (EngineLadder ladder in ladders)
            {
                // Si c'est le niveau du haut de l'échelle, créer une ouverture
                if (ladder.TopLevel == engineLevel)
                {
                    CreateFloorWithHole(width, floorOffset, ladder.transform.position);
                    return; // Une seule ouverture par niveau pour l'instant
                }
            }
            
            // Si pas d'ouverture nécessaire, créer un sol normal
            CreateSolidFloor(width, floorOffset);
        }
        
        /// <summary>
        /// Crée un sol normal sans ouverture
        /// </summary>
        private void CreateSolidFloor(float width, float floorOffset)
        {
            GameObject floor = new GameObject("Floor");
            floor.transform.SetParent(boundsContainer.transform);
            floor.transform.localPosition = new Vector3(0, floorOffset - floorThickness/2f, 0);
            floor.transform.localRotation = Quaternion.identity;
            
            BoxCollider floorCollider = floor.AddComponent<BoxCollider>();
            floorCollider.size = new Vector3(width, floorThickness, width);
            floorCollider.isTrigger = false;
        }
        
        /// <summary>
        /// Crée un sol avec un trou pour l'échelle
        /// </summary>
        private void CreateFloorWithHole(float width, float floorOffset, Vector3 ladderPosition)
        {
            // Convertir la position de l'échelle en coordonnées locales
            Vector3 localLadderPos = transform.InverseTransformPoint(ladderPosition);
            
            // Créer 4 segments de sol autour du trou
            float halfWidth = width / 2f;
            float halfOpening = ladderOpeningSize / 2f;
            
            // Sol avant (devant l'échelle)
            if (localLadderPos.z + halfOpening < halfWidth)
            {
                GameObject frontFloor = new GameObject("FloorFront");
                frontFloor.transform.SetParent(boundsContainer.transform);
                
                float frontZ = (localLadderPos.z + halfOpening + halfWidth) / 2f;
                float frontDepth = halfWidth - (localLadderPos.z + halfOpening);
                
                frontFloor.transform.localPosition = new Vector3(0, floorOffset - floorThickness/2f, frontZ);
                
                BoxCollider frontCollider = frontFloor.AddComponent<BoxCollider>();
                frontCollider.size = new Vector3(width, floorThickness, frontDepth);
            }
            
            // Sol arrière (derrière l'échelle)
            if (localLadderPos.z - halfOpening > -halfWidth)
            {
                GameObject backFloor = new GameObject("FloorBack");
                backFloor.transform.SetParent(boundsContainer.transform);
                
                float backZ = (localLadderPos.z - halfOpening - halfWidth) / 2f;
                float backDepth = (localLadderPos.z - halfOpening) + halfWidth;
                
                backFloor.transform.localPosition = new Vector3(0, floorOffset - floorThickness/2f, backZ);
                
                BoxCollider backCollider = backFloor.AddComponent<BoxCollider>();
                backCollider.size = new Vector3(width, floorThickness, backDepth);
            }
            
            // Sol gauche
            if (localLadderPos.x - halfOpening > -halfWidth)
            {
                GameObject leftFloor = new GameObject("FloorLeft");
                leftFloor.transform.SetParent(boundsContainer.transform);
                
                float leftX = (localLadderPos.x - halfOpening - halfWidth) / 2f;
                float leftWidth = (localLadderPos.x - halfOpening) + halfWidth;
                float leftZ = localLadderPos.z;
                float leftDepth = ladderOpeningSize;
                
                leftFloor.transform.localPosition = new Vector3(leftX, floorOffset - floorThickness/2f, leftZ);
                
                BoxCollider leftCollider = leftFloor.AddComponent<BoxCollider>();
                leftCollider.size = new Vector3(leftWidth, floorThickness, leftDepth);
            }
            
            // Sol droite
            if (localLadderPos.x + halfOpening < halfWidth)
            {
                GameObject rightFloor = new GameObject("FloorRight");
                rightFloor.transform.SetParent(boundsContainer.transform);
                
                float rightX = (localLadderPos.x + halfOpening + halfWidth) / 2f;
                float rightWidth = halfWidth - (localLadderPos.x + halfOpening);
                float rightZ = localLadderPos.z;
                float rightDepth = ladderOpeningSize;
                
                rightFloor.transform.localPosition = new Vector3(rightX, floorOffset - floorThickness/2f, rightZ);
                
                BoxCollider rightCollider = rightFloor.AddComponent<BoxCollider>();
                rightCollider.size = new Vector3(rightWidth, floorThickness, rightDepth);
            }
        }
        
        /// <summary>
        /// Crée un plafond avec des ouvertures pour les échelles
        /// </summary>
        private void CreateCeilingWithOpenings(float width, float ceilingOffset, EngineLadder[] ladders)
        {
            // Similaire à CreateFloorWithOpenings mais pour le plafond
            // Pour l'instant, on ne crée pas de plafond pour éviter les complications
            Debug.Log("Plafond avec ouvertures pas encore implémenté");
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
            Gizmos.DrawWireCube(new Vector3(-width/2f - wallThickness/2f, wallYPosition, 0), 
                               new Vector3(wallThickness, wallHeight, width));
            Gizmos.DrawWireCube(new Vector3(width/2f + wallThickness/2f, wallYPosition, 0), 
                               new Vector3(wallThickness, wallHeight, width));
            Gizmos.DrawWireCube(new Vector3(0, wallYPosition, width/2f + wallThickness/2f), 
                               new Vector3(width, wallHeight, wallThickness));
            Gizmos.DrawWireCube(new Vector3(0, wallYPosition, -width/2f - wallThickness/2f), 
                               new Vector3(width, wallHeight, wallThickness));
            
            // Dessiner le sol avec les ouvertures
            if (createFloor)
            {
                Gizmos.color = new Color(boundsColor.r, boundsColor.g, boundsColor.b, 0.3f);
                
                // Trouver les échelles pour visualiser les ouvertures
                EngineLadder[] ladders = FindLaddersForThisLevel();
                
                if (ladders.Length > 0)
                {
                    foreach (EngineLadder ladder in ladders)
                    {
                        if (ladder.TopLevel == engineLevel)
                        {
                            // Dessiner l'ouverture
                            Vector3 localLadderPos = transform.InverseTransformPoint(ladder.transform.position);
                            Gizmos.color = Color.green;
                            Gizmos.DrawWireCube(new Vector3(localLadderPos.x, floorOffset, localLadderPos.z), 
                                               new Vector3(ladderOpeningSize, floorThickness, ladderOpeningSize));
                        }
                    }
                }
                else
                {
                    // Sol normal
                    Gizmos.DrawCube(new Vector3(0, floorOffset - floorThickness/2f, 0), 
                                   new Vector3(width, floorThickness, width));
                }
            }
        }
    }
}
