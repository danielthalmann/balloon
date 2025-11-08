using UnityEngine;

public class Player : MonoBehaviour
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

    [SerializeField]
    bool debug = false;

    [SerializeField]
    bool isOnLadder = false;


    private Vector3 rotationDirection;

    [SerializeField]
    private Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 horizontalDirection = new Vector3();
        Vector3 verticanDirection = new Vector3();

        DetectLadder();

        if (!isOnLadder)
        {
            vertivalSpeed += (vertivalSpeed + gravity) * Time.deltaTime;
        } else
        {
            vertivalSpeed = 0;
        }


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


        if (horizontalDirection != Vector3.zero)
        {
            animator.SetBool("isWalking", true);
        } else
        {
            animator.SetBool("isWalking", false);
        }

        bool onGround = Physics.Raycast(transform.position + (transform.up * -bodyRadius), -transform.up, vertivalSpeed);
        if (onGround)
        {
            vertivalSpeed = 0;
        }


        Vector3 moveDirection = (horizontalDirection + verticanDirection).normalized;

        transform.right = Vector3.Slerp(transform.right, rotationDirection, rotateSpeed * Time.deltaTime);

        float moveDistance = moveSpeed * Time.deltaTime;

        RaycastHit hitInfo;

        bool touch = Physics.CapsuleCast(transform.position, transform.position + Vector3.up * bodyHeight, bodyRadius, moveDirection, out hitInfo, moveDistance);
        if (touch)
        {
            if (hitInfo.collider.isTrigger)
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

    private void OnDrawGizmos()
    {

        if(debug)
        {
            Gizmos.DrawSphere(transform.position, bodyRadius);
            Gizmos.DrawSphere(transform.position + Vector3.up * bodyHeight, bodyRadius);

            Gizmos.color = Color.red;

            Gizmos.DrawRay(transform.position + (transform.up * -bodyRadius), transform.up * -vertivalSpeed);
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
    }

    public void SetOnLadder(bool on)
    {
        isOnLadder = on;
    }
}
