using UnityEngine;

// S'assure qu'un Rigidbody est présent sur l'objet qui porte ce script
[RequireComponent(typeof(Rigidbody))]
public class ControlledFall : MonoBehaviour
{
    // Vitesse de chute en m/s (réglable dans l'Inspector)
    [SerializeField, Min(0f)] float fallSpeed = 2f;
    // Si coché: descend (−Y). Si décoché: monte (+Y).
    [SerializeField] bool fallDown = true;
    [SerializeField, Min(0f)] float upSpeed = 4f;     // vitesse de montée pendant le boost
    [SerializeField, Min(0f)] float boostDuration = 1.5f; // durée du boost en secondes

    // Référence cache du Rigidbody pour éviter GetComponent à chaque frame
    Rigidbody rb;
    Coroutine boostCo;

    void Awake()
    {
        
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false; // on gère nous-mêmes la “gravité”
        rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    void FixedUpdate()
    {
        var v = rb.linearVelocity;

        // Direction contrôlée par un switch pour éviter les inversions si la scène/caméra est retournée
        v.y = fallDown ? -fallSpeed : +fallSpeed;

        v.z = 0f;
        rb.linearVelocity = v;
    }

    // Appelé par le bouton quand on a atteint le nombre d'appuis
    public void TriggerBoostUp()
    {
        if (boostCo != null) StopCoroutine(boostCo);
        boostCo = StartCoroutine(BoostUpRoutine());
    }

    System.Collections.IEnumerator BoostUpRoutine()
    {
        // Mémorise l'état actuel
        bool prevFallDown = fallDown;
        float prevSpeed = fallSpeed;

        // Monte: signe positif → fallDown = false
        fallDown = false;
        fallSpeed = Mathf.Max(0.01f, upSpeed);

        yield return new WaitForSeconds(boostDuration);

        // Restaure l'état précédent (redescend comme avant)
        fallDown = prevFallDown;
        fallSpeed = prevSpeed;
    }
}
