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
    private LayerMask ladderMask;
    [SerializeField]
    private LayerMask groundMask;

    private Bounds ladderBounds;

    bool onGround = false;

        [SerializeField]
    private Vector3 rotationDirection;

    [SerializeField]
    private Animator animator;

    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction interactAction;
    private PlayerEngineInteraction playerInteraction;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerInteraction = GetComponent<PlayerEngineInteraction>();
        playerInput = GetComponent<PlayerInput>();

        moveAction = playerInput.actions["Move"];
        interactAction = playerInput.actions["Interact"];

    }

    Vector3 horizontalDirection = new Vector3();
    Vector3 moveDirection;
    RaycastHit hitInfoFront;


    // Update is called once per frame
    void Update()
    {
        Vector3 verticanDirection = new Vector3();

        Vector3 moveInput = moveAction.ReadValue<Vector2>();
        horizontalDirection.x = moveInput.x;
        verticanDirection.y = moveInput.y;

        if (horizontalDirection.x != 0)
        {
            rotationDirection = horizontalDirection;
        }

        if(interactAction.WasPressedThisFrame())
        {
            if(playerInteraction)
                playerInteraction.TriggerInteraction();
        }

        /*
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            horizontalDirection.x = -1.0f;
            rotationDirection = horizontalDirection;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            horizontalDirection.x = 1.0f;
            rotationDirection = horizontalDirection;
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            verticanDirection.y = 1.0f;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            verticanDirection.y = -1.0f;
        }
        */

        DetectLadder();

        DetectGround();

        ApplyAnimation();


        // mise à jour de la rotation du personnage
        transform.right = Vector3.Slerp(transform.right, rotationDirection, rotateSpeed * Time.deltaTime);

        moveDirection = (horizontalDirection + verticanDirection).normalized;

        float moveDistance = moveSpeed * Time.deltaTime;
        float jumpDistance = jumpForce * Time.deltaTime;
        if (isOnLadder)
        {
            jumpDistance = moveDistance;
        } else
        {
            if(onGround)
            {
                if (verticanDirection.y < 0)
                { 
                    verticanDirection.y = 0;
                }
            }
        }


        bool touch = Physics.CapsuleCast(transform.position, transform.position + Vector3.up * bodyHeight, bodyRadius, rotationDirection, out hitInfoFront, moveDistance);
        if (touch)
        {
            if (hitInfoFront.collider.isTrigger)
            {
                touch = false;
            }

        }

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

        if (!isOnLadder)
        {
            vertivalSpeed += (vertivalSpeed + gravity) * Time.deltaTime;
        }
        else
        {
            vertivalSpeed = 0;
        }

    }

    public void SetOnLadder(bool on)
    {
        isOnLadder = on;
    }
}
