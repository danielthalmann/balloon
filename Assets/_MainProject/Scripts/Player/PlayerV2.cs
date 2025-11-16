using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerV2 : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 2.5f;
    [SerializeField]
    private float rotateSpeed = 12f;
    [SerializeField]
    private float jumpForce = 5f;
    [SerializeField]
    private CameraFollow cameraFollow;

    [SerializeField]
    private float detectDistance = 1f;
    [SerializeField]
    private LayerMask ladderMask;
    [SerializeField]
    private LayerMask activableMask;
    [SerializeField]
    private string groundTag = "Ground";
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private float bodyHeight;

    [Header("Debug")]

    [SerializeField]
    bool debug = false;
    [SerializeField]
    bool isOnGround = false;
    [SerializeField]
    bool isOnLadder = false;
    [SerializeField]
    bool isOnActivable = false;
    [SerializeField]
    private Vector3 rotationDirection;

    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction interactAction;
    private InputAction jumpAction;
    private InputAction zoomAction;
    private Activable activable;

    private float rotateAngle = 0;

    private Rigidbody rb;

    float horizontalDirection;
    float verticanDirection;

    List<Collider> groundColliders = new List<Collider>();

    Collider playerCollider;
    Vector3 moveInput;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();

        moveAction = playerInput.actions["Move"];
        interactAction = playerInput.actions["Interact"];
        jumpAction = playerInput.actions["Jump"];
        zoomAction = playerInput.actions["Zoom"];

        rb = GetComponent<Rigidbody>();
        playerCollider = GetComponent<Collider>();
        
        GameObject[] grounds = GameObject.FindGameObjectsWithTag(groundTag);
        foreach (GameObject objet in grounds)
        {
            Collider collider = objet.GetComponent<Collider>();
            if (collider)
            {
                groundColliders.Add(collider);
            }
        }

    }

    private void FixedUpdate()
    {
        DetectLadder(moveInput.y);

        DetectActivable();

        DetectGround();

        ApplyAnimation();

    }


    // Update is called once per frame
    void Update()
    {
        if (zoomAction.WasPressedThisFrame())
        {
            cameraFollow.macro = !cameraFollow.macro;
        }

        moveInput = moveAction.ReadValue<Vector2>();
        horizontalDirection = moveInput.x;

        if (horizontalDirection != 0)
        {
            rotationDirection = new Vector3(horizontalDirection, 0, 0);
        }
            
        if (interactAction.IsPressed())
        {
            if (activable != null)
            {
                activable.Activate();
            }
        } else
        {
            if (activable != null)
            {
                activable.Release();
            }
        }


        // mise à jour de la rotation du personnage
        //transform.right = Vector3.Slerp(transform.right, rotationDirection, rotateSpeed * Time.deltaTime);
        if (rotationDirection.x > 0)
        {
            rotateAngle = Mathf.Lerp(rotateAngle, 0, rotateSpeed * Time.deltaTime);
            transform.rotation = Quaternion.AngleAxis(rotateAngle, Vector3.up);
        } else
        {
            rotateAngle = Mathf.Lerp(rotateAngle, 180, rotateSpeed * Time.deltaTime);
            transform.rotation = Quaternion.AngleAxis(rotateAngle, Vector3.up);

        }

        if (isOnLadder)
        {
            rb.useGravity = false;
            verticanDirection = moveInput.y * moveSpeed;
        }
        else
        {
            rb.useGravity = true;

            if (isOnGround && jumpAction.WasPressedThisFrame())
            {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                verticanDirection = rb.linearVelocity.y;
            }
            else
            {
                verticanDirection = rb.linearVelocity.y;
            }
        }

        rb.linearVelocity = new Vector3(horizontalDirection * moveSpeed, verticanDirection, rb.linearVelocity.z);

    }

    private void ApplyAnimation()
    {
        if (horizontalDirection != 0)
        {
            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }
    }

    private void OnDrawGizmos()
    {

        if(debug)
        {

            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, transform.right * detectDistance);

        }

    }
  
    RaycastHit hitInfoLadder;

    private void DetectLadder(float vertical)
    {
        
        if ((Physics.OverlapSphere(transform.position, 0.01f, ladderMask)).Length > 0)
        {
            if (!isOnLadder)
            {
                foreach (Collider collider in groundColliders)
                {
                    Physics.IgnoreCollision(playerCollider, collider, true);
                }
            }
            isOnLadder = true;
        }
        else
        {
            if (isOnLadder)
            {
                foreach (Collider collider in groundColliders)
                {
                    Physics.IgnoreCollision(playerCollider, collider, false);
                }
                isOnLadder = false;
            }
        }
       
    }

    private void DetectActivable()
    {
        Collider[] colliders;

        if ((colliders = Physics.OverlapSphere(transform.position, 0.01f, activableMask)).Length > 0)
        {
            Activable currentActivable = colliders[0].gameObject.GetComponent<Activable>();

            if (activable != currentActivable)
            {
                if(activable)
                {
                    activable.Release();
                }
                activable = currentActivable;
                activable.hover = true;
            }
            isOnActivable = true;
        } else
        {
            isOnActivable = false;
            if (activable)
            {
                activable.Release();
                activable.hover = false;
                activable = null;
            }
        }

    }

    public void SetOnLadder(bool on)
    {
        isOnLadder = on;
    }

    private void DetectGround()
    {   
        // Debug.DrawLine(transform.position, -transform.up * bodyHeight);
        if(Physics.Raycast(transform.position, -transform.up, bodyHeight))
        {
            isOnGround = true;
        } else
        {
            isOnGround = false;
        }
    }

}
