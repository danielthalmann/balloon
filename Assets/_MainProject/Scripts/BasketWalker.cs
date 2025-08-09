using UnityEngine;
using UnityEngine.InputSystem;

// Perso 2.5D: marche sur un niveau courant, grimpe/descend via échelles (triggers) entre niveaux multiples.
[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Rigidbody))]
public class BasketWalker : MonoBehaviour
{
    [Header("Déplacement")]
    [SerializeField] float speedX = 3f;       // vitesse latérale au niveau
    [SerializeField] float speedZ = 3f;       // vitesse avant/arrière dans la nacelle
    [SerializeField] float climbSpeed = 2f;   // vitesse sur l’échelle
    [SerializeField] float paddingX = 0.02f;  // marge X avec les bords de la nacelle
    [SerializeField] float paddingY = 0.02f;  // marge Y avec les surfaces
    [SerializeField] float paddingZ = 0.02f;  // marge Z avec les bords de la nacelle

    [Header("Références nacelle")]
    [SerializeField] Transform basket;            // Transform de la nacelle (souvent parent)
    [SerializeField] BoxCollider basketCollider;  // BoxCollider de la nacelle (calcule les limites)

    [Header("Entrées (Nouveau Input System)")]
    // Action "Move" (Vector2) → X: gauche/droite, Y: haut/bas
    [SerializeField] InputActionReference moveAction;

    [Header("Niveaux (multi-étages)")]
    // Place 0..N repères dans la nacelle (du bas vers le haut). Chaque repère est la hauteur "sol" de l’étage.
    // Si vide: fallback à 2 niveaux (sol et plafond intérieur de la nacelle).
    [SerializeField] Transform[] levelMarkers;

    Renderer rend;        // taille visuelle du perso
    Rigidbody rb;

    // État escalade
    bool atLadder = false;     // dans une zone d’échelle ?
    float ladderX = 0f;        // X de l’échelle active (on colle X pendant l’escalade)
    Bounds ladderBounds;       // hauteur couverte par l’échelle active
    Collider activeLadder;     // référence dynamique à l’échelle (utile si la nacelle bouge)
    bool climbing = false;     // en train de grimper/descendre ?
    int currentLevel = 0;      // index du niveau où on “marche”
    int targetLevel = -1;      // index vers lequel on grimpe

    void Awake()
    {
        // Empêche le perso de pousser la nacelle
        var myCol = GetComponent<Collider>();
        if (basketCollider && myCol)
            Physics.IgnoreCollision(myCol, basketCollider, true);
            
        if (!basket) basket = transform.parent;
        if (basket && !basketCollider) basketCollider = basket.GetComponent<BoxCollider>();
        rend = GetComponent<Renderer>();
        rb = GetComponent<Rigidbody>();

        rb.useGravity = false;
        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        if (moveAction && moveAction.action != null)
            moveAction.action.Enable();

        // Choisit le niveau initial: le plus bas (0) par défaut
        currentLevel = 0;
    }

    void OnDestroy()
    {
        if (moveAction && moveAction.action != null)
            moveAction.action.Disable();
    }

    void Update()
    {
        if (!basket || !basketCollider) return;

        // Lecture des entrées
        Vector2 move = ReadMove();
        float inputX = move.x;
        float inputY = move.y;

        // Infos tailles et limites
        Bounds b = basketCollider.bounds;
        float halfCharX = rend ? rend.bounds.extents.x : 0f;
        float halfCharY = rend ? rend.bounds.extents.y : 0f;
        float halfCharZ = rend ? rend.bounds.extents.z : 0f;

        // Mise à jour dynamique des infos d’échelle (si la nacelle/l’échelle bouge)
        if (atLadder && activeLadder)
        {
            ladderBounds = activeLadder.bounds;
            ladderX = ladderBounds.center.x;
        }

        // Liste triée des hauteurs "où poser le perso" (Y cibles)
        var levels = GetLevelsY(b, halfCharY);
        if (levels.Length == 0) return;

        currentLevel = Mathf.Clamp(currentLevel, 0, levels.Length - 1);

        // Limites X dans la nacelle
        float halfBasketX = b.extents.x;
        float limitX = Mathf.Max(0f, halfBasketX - halfCharX - paddingX);
        // Limites Z dans la nacelle
        float halfBasketZ = b.extents.z;
        float limitZ = Mathf.Max(0f, halfBasketZ - halfCharZ - paddingZ);

        Vector3 pos = transform.position;

        if (climbing)
        {
            // Coller à l’échelle en X
            pos.x = ladderX;

            // Contrôle libre vertical sur l’échelle
            float minY = ladderBounds.min.y + halfCharY + paddingY;
            float maxY = ladderBounds.max.y - halfCharY - paddingY;
            float deltaY = inputY * climbSpeed * Time.deltaTime;
            pos.y = Mathf.Clamp(pos.y + deltaY, minY, maxY);

            // Sortie volontaire de l’échelle si on pousse à l’horizontale près d’un palier
            if (Mathf.Abs(inputX) > 0.1f)
            {
                // Cherche le palier le plus proche
                int closest = 0;
                float best = Mathf.Infinity;
                for (int i = 0; i < levels.Length; i++)
                {
                    float d = Mathf.Abs(pos.y - levels[i]);
                    if (d < best) { best = d; closest = i; }
                }
                const float detachSnap = 0.05f;
                if (best <= detachSnap)
                {
                    currentLevel = closest;
                    climbing = false;
                    targetLevel = -1;
                    pos.y = levels[currentLevel];
                }
            }
        }
        else
        {
            // Marche latérale au niveau courant
            pos.x += inputX * speedX * Time.deltaTime;
            pos.x = Mathf.Clamp(pos.x, basket.position.x - limitX, basket.position.x + limitX);

            // Coller au niveau courant en Y
            pos.y = levels[currentLevel];

            // Début d’escalade si dans une échelle + entrée verticale
            if (atLadder && Mathf.Abs(inputY) > 0.1f)
            {
                climbing = true;
                targetLevel = -1;
                pos.x = ladderX; // snap X dès le départ
            }

            // Si pas d’escalade en cours, utiliser l’axe vertical d’entrée pour avancer/reculer (Z)
            if (!climbing)
            {
                pos.z += inputY * speedZ * Time.deltaTime;
            }
        }

        // Rester dans les limites Z de la nacelle
        pos.z = Mathf.Clamp(pos.z, basket.position.z - limitZ, basket.position.z + limitZ);

        transform.position = pos;
    }

    Vector2 ReadMove()
    {
        if (moveAction && moveAction.action != null)
            return moveAction.action.ReadValue<Vector2>();

        // Fallback (ancien Input Manager si “Both”)
        return new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        );
    }

    // Construit la liste des hauteurs de niveaux (du bas vers le haut)
    // - Si des markers sont fournis: Y = marker.position.y + demi-hauteur perso + paddingY
    // - Sinon: 2 niveaux = sol et plafond intérieur
    float[] GetLevelsY(Bounds basketBounds, float halfCharY)
    {
        if (levelMarkers != null && levelMarkers.Length > 0)
        {
            var ys = new float[levelMarkers.Length];
            for (int i = 0; i < levelMarkers.Length; i++)
                ys[i] = levelMarkers[i].position.y + halfCharY + paddingY;

            System.Array.Sort(ys);
            return ys;
        }
        else
        {
            float floorY = basketBounds.min.y + halfCharY + paddingY;
            float upperY = basketBounds.max.y - halfCharY - paddingY;
            return new float[] { floorY, upperY };
        }
    }

    // Gestion des zones d’échelle (cubes fins en Trigger, Tag "Ladder")
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ladder"))
        {
            atLadder = true;
            activeLadder = other;
            ladderBounds = other.bounds; // init, mais sera mis à jour dynamiquement
            ladderX = ladderBounds.center.x;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ladder"))
        {
            atLadder = false;
            activeLadder = null;

            // Si on sort en cours d’escalade, on choisit le niveau le plus proche
            if (climbing)
            {
                climbing = false;
                targetLevel = -1;

                // Recalage sur le niveau le plus proche
                float y = transform.position.y;
                Bounds b = basketCollider.bounds;
                float halfCharY = rend ? rend.bounds.extents.y : 0f;
                var levels = GetLevelsY(b, halfCharY);

                int closest = 0;
                float best = Mathf.Infinity;
                for (int i = 0; i < levels.Length; i++)
                {
                    float d = Mathf.Abs(y - levels[i]);
                    if (d < best) { best = d; closest = i; }
                }
                currentLevel = closest;
            }
        }
    }
}
