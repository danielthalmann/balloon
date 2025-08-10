using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InteractableButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IDragHandler
{
    public enum Mode { SinglePress, MultiPress, Hold, CircleGesture }

    [Header("Mode")]
    public Mode interactionMode = Mode.SinglePress;
    public UnityEvent onActivated;

    [Header("MultiPress")]
    public int requiredPresses = 3;
    public float multiPressWindow = 1.0f;
    int presses; float windowEnd;

    [Header("Hold")]
    public float holdDuration = 1.5f;
    bool holding; float holdStart;

    [Header("Circle Gesture")]
    public int requiredTurns = 1;      // 1=360°, 2=720°…
    public bool clockwiseOnly = false;
    public float minRadiusPixels = 40f;
    public float maxIdleTime = 0.5f;
    Vector2 centerScreen, lastPointer; float accumulatedAngle; float lastMoveTime;

    [Header("Activation clavier/manette (optionnel)")]
    public InputActionReference interactAction; // ex: Player/Interact (E, Gamepad South)
    public bool requireProximity = false;
    public string playerTag = "Player";
    bool inRange;

    Camera cam;
    Camera Cam => cam ? cam : (cam = Camera.main);

    void OnEnable()
    {
        if (interactAction && interactAction.action != null)
        {
            var a = interactAction.action;
            a.Enable();
            a.performed += OnInteractPerformed;  // clic / multi-press
            a.started   += OnInteractStarted;    // hold start
            a.canceled  += OnInteractCanceled;   // hold end
        }
    }
    void OnDisable()
    {
        if (interactAction && interactAction.action != null)
        {
            var a = interactAction.action;
            a.performed -= OnInteractPerformed;
            a.started   -= OnInteractStarted;
            a.canceled  -= OnInteractCanceled;
            a.Disable();
        }
    }

    bool Allowed() => !requireProximity || inRange;

    // Souris/tactile
    public void OnPointerDown(PointerEventData e)
    {
        if (interactionMode == Mode.Hold && Allowed())
        { holding = true; holdStart = Time.unscaledTime; }

        if (interactionMode == Mode.CircleGesture)
        {
            centerScreen = WorldOrUIToScreenPoint();
            lastPointer = e.position; lastMoveTime = Time.unscaledTime;
        }
    }
    public void OnPointerUp(PointerEventData e)
    {
        if (interactionMode == Mode.Hold && holding)
        {
            holding = false;
            if (Allowed() && Time.unscaledTime - holdStart >= holdDuration) Activate();
        }
    }
    public void OnPointerClick(PointerEventData e)
    {
        if (!Allowed()) return;

        if (interactionMode == Mode.SinglePress) Activate();
        else if (interactionMode == Mode.MultiPress)
        {
            float now = Time.unscaledTime;
            if (now > windowEnd) presses = 0;
            presses++; windowEnd = now + multiPressWindow;
            if (presses >= requiredPresses) { Activate(); presses = 0; }
        }
    }
    public void OnDrag(PointerEventData e)
    {
        if (interactionMode != Mode.CircleGesture) return;

        Vector2 prev = lastPointer - centerScreen;
        Vector2 cur  = e.position - centerScreen;
        if (prev.magnitude < minRadiusPixels || cur.magnitude < minRadiusPixels) { lastPointer = e.position; return; }

        float delta = Vector2.SignedAngle(prev, cur);
        accumulatedAngle += delta; lastPointer = e.position; lastMoveTime = Time.unscaledTime;

        float turns = Mathf.Abs(accumulatedAngle) / 360f;
        bool senseOk = !clockwiseOnly || accumulatedAngle > 0f;
        if (Allowed() && turns >= requiredTurns && senseOk)
        { Activate(); accumulatedAngle = 0f; }
    }

    // Clavier/manette (E, bouton A, etc.)
    void OnInteractStarted(InputAction.CallbackContext ctx)
    {
        if (interactionMode == Mode.Hold && Allowed())
        { holding = true; holdStart = Time.unscaledTime; }
    }
    void OnInteractCanceled(InputAction.CallbackContext ctx)
    {
        if (interactionMode == Mode.Hold && holding)
        {
            holding = false;
            if (Allowed() && Time.unscaledTime - holdStart >= holdDuration) Activate();
        }
    }
    void OnInteractPerformed(InputAction.CallbackContext ctx)
    {
        if (!Allowed()) return;
        if (interactionMode == Mode.SinglePress) Activate();
        else if (interactionMode == Mode.MultiPress)
        {
            float now = Time.unscaledTime;
            if (now > windowEnd) presses = 0;
            presses++; windowEnd = now + multiPressWindow;
            if (presses >= requiredPresses) { Activate(); presses = 0; }
        }
        // CircleGesture: rester pointer/drag
    }

    void Update()
    {
        if (interactionMode == Mode.CircleGesture && Time.unscaledTime - lastMoveTime > maxIdleTime)
            accumulatedAngle = 0f;

        // Option “hold auto” (activer sans relâcher) :
        // if (interactionMode == Mode.Hold && holding && Time.unscaledTime - holdStart >= holdDuration) { holding = false; Activate(); }
    }

    void Activate() => onActivated?.Invoke();

    Vector2 WorldOrUIToScreenPoint()
    {
        var rt = GetComponent<RectTransform>();
        if (rt && rt.gameObject.activeInHierarchy)
            return RectTransformUtility.WorldToScreenPoint(Cam, rt.position);
        return Cam ? (Vector2)Cam.WorldToScreenPoint(transform.position) : (Vector2)transform.position;
    }

    // Zone de proximité optionnelle (ajoute un Trigger autour du bouton)
    void OnTriggerEnter(Collider other) { if (requireProximity && other.CompareTag(playerTag)) inRange = true; }
    void OnTriggerExit (Collider other) { if (requireProximity && other.CompareTag(playerTag)) inRange = false; }
}
