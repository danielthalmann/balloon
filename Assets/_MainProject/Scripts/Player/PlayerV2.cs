using NUnit.Framework.Internal.Commands;
using UnityEditor.PackageManager;
using UnityEngine;

public class PlayerV2 : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 5f;
    [SerializeField]
    private float rotateSpeed = 12f;
    [SerializeField]
    float bodyHeight = .5f;
    [SerializeField]
    float bodyRadius = 1f;

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


    private Vector3 rotationDirection;

    [SerializeField]
    private Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    Vector3 horizontalDirection = new Vector3();
    Vector3 moveDirection;
    RaycastHit hitInfoFront;


    // Update is called once per frame
    void Update()
    {
        Vector3 verticanDirection = new Vector3();

        horizontalDirection.x = 0;

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

        DetectLadder();

        DetectGround();

        ApplyAnimation();


        // mise à jour de la rotation du personnage
        transform.right = Vector3.Slerp(transform.right, rotationDirection, rotateSpeed * Time.deltaTime);

        moveDirection = (horizontalDirection + verticanDirection).normalized;

        float moveDistance = moveSpeed * Time.deltaTime;


        bool touch = Physics.CapsuleCast(transform.position, transform.position + Vector3.up * bodyHeight, bodyRadius, moveDirection, out hitInfoFront, moveDistance);
        if (touch)
        {
            if (hitInfoFront.collider.isTrigger)
            {
                touch = false;
            }

        }

        if (!touch || isOnLadder)
        {
            transform.position += moveDirection * moveDistance;
        }

        
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
          

                Gizmos.DrawRay(transform.position, moveDirection * bodyRadius);

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

        bool onGround = Physics.Raycast(transform.position, -transform.up, out hitInfoGround, vertivalSpeed + bodyRadius);
        if (onGround && !isOnLadder)
        {
            vertivalSpeed = 0;
            transform.position = new Vector3 (transform.position.x, (hitInfoGround.point.y + bodyRadius), transform.position.z); 
        }
    }

    private void DetectLadder()
    {
        bool newStateLader = false;

        RaycastHit hitInfo;

        if (Physics.Raycast(transform.position, transform.right, out hitInfo, .1f))
        {
            Debug.Log(hitInfo);
            if (hitInfo.collider.gameObject.GetComponent<Ladder>())
            {
                newStateLader = true;
            }
        }

        if (isOnLadder != newStateLader)
        {
            isOnLadder = newStateLader;
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
