using UnityEngine;
using UnityEngine.InputSystem;
using Prototype.Player;

public class PlayerV2 : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 2.5f;
    [SerializeField]
    private float rotateSpeed = 12f;
    [SerializeField]
    float bodyHeight = .5f;
    [SerializeField]
    float bodyRadius = 1f;
    [SerializeField]
    private float jumpForce = 5f;

    float vertivalSpeed = 0;

    [SerializeField]
    float gravity = .5f;

    [SerializeField]
    private CameraFollow cameraFollow;

    [Header("Debug")]
    [SerializeField]
    bool debug = false;
    [SerializeField]
    bool debugCapsuleCast = false;
    [SerializeField]
    bool debugGroundCast = false;
    [SerializeField]
    bool debugFrontCast = false;

    [SerializeField]
    bool isOnLadder = false;

    [SerializeField]
    bool isOnActivable = false;
    [SerializeField]
    private LayerMask ladderMask;
    [SerializeField]
    private LayerMask groundMask;
    [SerializeField]
    private LayerMask activableMask;


    private Bounds ladderBounds;
    private Bounds ActivableBounds;

    bool onGround = false;

    [SerializeField]
    private Vector3 rotationDirection;

    [SerializeField]
    private Animator animator;

    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction interactAction;
    private InputAction jumpAction;
    private InputAction zoomAction;
    private Activable activable;

    private float rotateAngle = 0;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();

        moveAction = playerInput.actions["Move"];
        interactAction = playerInput.actions["Interact"];
        jumpAction = playerInput.actions["Jump"];
        zoomAction = playerInput.actions["Zoom"];
    }

    Vector3 horizontalDirection = new Vector3();
    Vector3 moveDirection;
    RaycastHit hitInfoFront;


    // Update is called once per frame
    void Update()
    {
        if (zoomAction.WasPressedThisFrame())
        {
            cameraFollow.macro = !cameraFollow.macro;
        }

        Vector3 verticanDirection = new Vector3();

        Vector3 moveInput = moveAction.ReadValue<Vector2>();
        horizontalDirection.x = moveInput.x;

        if (isOnLadder)
        {
            verticanDirection.y = moveInput.y;
        }
        else { 
            if (jumpAction.IsPressed())
            {
                verticanDirection.y = 1;
            }
        }
            

        if (horizontalDirection.x != 0)
        {
            rotationDirection = horizontalDirection;
        }

        if (interactAction.WasPressedThisFrame())
        {
            if (isOnActivable && activable != null)
            {
                activable.Release();
            }
        }
                    
            
        if (interactAction.IsPressed())
        {
            if (isOnActivable && activable != null)
            {
                activable.Activate();
            }
        }

        DetectLadder();

        DetectActivable();

        DetectGround();

        ApplyAnimation();


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


        float moveDistance = moveSpeed * Time.deltaTime;
        float jumpDistance = jumpForce * Time.deltaTime;

        if (isOnLadder)
        {
            vertivalSpeed = 0;
            jumpDistance = moveDistance;

        } else
        {
            vertivalSpeed += (vertivalSpeed + gravity) * Time.deltaTime;
            if(onGround)
            {
                if (verticanDirection.y < 0)
                { 
                    verticanDirection.y = 0;
                }
            }
        }

        // prend tous les masques sauf ladderMask, groundMask et activableMask
        LayerMask lm = ~(ladderMask | groundMask | activableMask);

        bool touch = Physics.CapsuleCast(transform.position, transform.position + Vector3.up * bodyHeight, bodyRadius, rotationDirection, out hitInfoFront, moveDistance, lm);

        transform.position += (horizontalDirection.normalized * moveDistance * (touch ? 0 : 1)) + (verticanDirection.normalized * jumpDistance);
        
        transform.position += (-transform.up * vertivalSpeed);
       

    }

    private void ApplyAnimation()
    {
        if (horizontalDirection != Vector3.zero)
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
            if (debugCapsuleCast)
            {
                Gizmos.DrawSphere(transform.position, bodyRadius);
                Gizmos.DrawSphere(transform.position + Vector3.up * bodyHeight, bodyRadius);

            }
            if (debugGroundCast)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawRay(transform.position + (transform.up * -bodyRadius), transform.up * bodyRadius);

                Gizmos.color = Color.green;
                Gizmos.DrawSphere(hitInfoGround.point, .01f);
            }

            if (debugFrontCast)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawRay(transform.position, rotationDirection * .5f);

                Vector3 start = transform.position + (transform.right * -.5f);
                Gizmos.color = Color.blue;
                Gizmos.DrawRay(start + (transform.up * bodyHeight), transform.right * .5f);



                Gizmos.color = Color.green;
                Gizmos.DrawSphere(hitInfoFront.point, .01f);
            }

            Gizmos.color = Color.red;

            Gizmos.DrawRay(transform.position + (transform.up * -bodyRadius), transform.up * -vertivalSpeed);
        }

    }

    RaycastHit hitInfoGround;

    private void DetectGround()
    {

        onGround = Physics.Raycast(transform.position, -transform.up, out hitInfoGround, vertivalSpeed + bodyRadius, groundMask);
        if (onGround && !isOnLadder)
        {
            vertivalSpeed = 0;
            transform.position = new Vector3 (transform.position.x, (hitInfoGround.point.y + bodyRadius), transform.position.z); 
        }
    }

    private void DetectLadder()
    {

        if (isOnLadder)
        {
            if (!ladderBounds.Contains(transform.position))
            {
                isOnLadder = false;
            }

        } else
        {
            
            RaycastHit hitInfo;

            Vector3 start = transform.position + (transform.right * -.5f);

            if (Physics.Raycast(start, transform.right, out hitInfo, .5f, ladderMask))
            {
                ladderBounds = hitInfo.collider.bounds;
                isOnLadder = true;
            }


        }
    }

    private void DetectActivable()
    {

        if (isOnActivable)
        {
            if (!ActivableBounds.Contains(transform.position))
            {
                isOnActivable = false;
            }

        }
        else
        {

            RaycastHit hitInfo;

            Vector3 start = transform.position + (transform.right * -.5f);

            if (Physics.Raycast(start, transform.right, out hitInfo, .5f, activableMask))
            {
                ActivableBounds = hitInfo.collider.bounds;
                activable = hitInfo.collider.GetComponent<Activable>();
                isOnActivable = true;
            }

        }

    }

    public void SetOnLadder(bool on)
    {
        isOnLadder = on;
    }
}
