using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InputEvent : MonoBehaviour
{
    public UnityEvent onSomeAction;

    public InputActionReference actionReference;


    void OnEnable()
    {
        actionReference.action.performed += OnAction;
    }

    void OnDisable()
    {
        actionReference.action.performed -= OnAction;
    }

    void OnAction(InputAction.CallbackContext context)
    {
        onSomeAction.Invoke();
    }
}
