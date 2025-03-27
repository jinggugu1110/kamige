using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float pl_moveSpeed = 5f;
    public float pl_jumpForce = 10f;

    // 通常の GroundCheck
    public Transform groundCheck;
    public Transform topGroundCheck;
    public Transform leftCheck;
    public Transform rightCheck;
    public Transform topRightCheck;
    public Transform bottomRightCheck;
    public Transform topLeftCheck;
    public Transform bottomLeftCheck;
    public Transform rightMidCheck;
    public Transform rightLowCheck;
    public Transform leftMidCheck;
    public Transform leftLowCheck;

    // 90度回転した GroundCheck（左右重力用）
    public Transform sideGroundCheck;
    public Transform sideTopGroundCheck;
    public Transform sideLeftCheck;
    public Transform sideRightCheck;
    public Transform sideTopLeftCheck;
    public Transform sideTopRightCheck;
    public Transform sideBottomLeftCheck;
    public Transform sideBottomRightCheck;
    public Transform sideRightMidCheck;
    public Transform sideRightLowCheck;
    public Transform sideLeftMidCheck;
    public Transform sideLeftLowCheck;

    private Rigidbody2D rb;
    private bool isJumping = false;
    private bool onGround = false;
    private bool isBlockedLeft = false;
    private bool isBlockedRight = false;
    private bool isBlockedTopRight = false;
    private bool isBlockedBottomRight = false;
    private bool isBlockedTopLeft = false;
    private bool isBlockedBottomLeft = false;
    private bool isBlockedRightMid = false;
    private bool isBlockedRightLow = false;
    private bool isBlockedLeftMid = false;
    private bool isBlockedLeftLow = false;
    private Vector3 initialPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        initialPosition = transform.position;
    }

    void Update()
    {
        CheckCollisions();
        CheckGround();
        HandleMovement();
        HandleJump();
    }

    private void CheckCollisions()
    {
        float rayDistance = 0.1f;
        LayerMask groundLayer = LayerMask.GetMask("Ground");

        isBlockedLeft = Physics2D.Raycast(leftCheck.position, Vector2.left, rayDistance, groundLayer);
        isBlockedRight = Physics2D.Raycast(rightCheck.position, Vector2.right, rayDistance, groundLayer);
        isBlockedTopRight = Physics2D.Raycast(topRightCheck.position, Vector2.up, rayDistance, groundLayer);
        isBlockedBottomRight = Physics2D.Raycast(bottomRightCheck.position, Vector2.down, rayDistance, groundLayer);
        isBlockedTopLeft = Physics2D.Raycast(topLeftCheck.position, Vector2.up, rayDistance, groundLayer);
        isBlockedBottomLeft = Physics2D.Raycast(bottomLeftCheck.position, Vector2.down, rayDistance, groundLayer);
        isBlockedRightMid = Physics2D.Raycast(rightMidCheck.position, Vector2.right, rayDistance, groundLayer);
        isBlockedRightLow = Physics2D.Raycast(rightLowCheck.position, Vector2.right, rayDistance, groundLayer);
        isBlockedLeftMid = Physics2D.Raycast(leftMidCheck.position, Vector2.left, rayDistance, groundLayer);
        isBlockedLeftLow = Physics2D.Raycast(leftLowCheck.position, Vector2.left, rayDistance, groundLayer);
    }

    private void CheckGround()
    {
        float rayDistance = 0.1f;
        LayerMask groundLayer = LayerMask.GetMask("Ground");

        if (rb.gravityScale > 0)
        {
            onGround = Physics2D.Raycast(groundCheck.position, Vector2.down, rayDistance, groundLayer) ||
                       Physics2D.Raycast(bottomRightCheck.position, Vector2.down, rayDistance, groundLayer) ||
                       Physics2D.Raycast(bottomLeftCheck.position, Vector2.down, rayDistance, groundLayer);
        }
        else if (rb.gravityScale < 0)
        {
            onGround = Physics2D.Raycast(topGroundCheck.position, Vector2.up, rayDistance, groundLayer) ||
                       Physics2D.Raycast(topRightCheck.position, Vector2.up, rayDistance, groundLayer) ||
                       Physics2D.Raycast(topLeftCheck.position, Vector2.up, rayDistance, groundLayer);
        }
        else if (rb.gravityScale == 0)
        {
            if (transform.eulerAngles.z == 90)
            {
                onGround = Physics2D.Raycast(sideLeftCheck.position, Vector2.left, rayDistance, groundLayer) ||
                           Physics2D.Raycast(sideBottomLeftCheck.position, Vector2.left, rayDistance, groundLayer) ||
                           Physics2D.Raycast(sideTopLeftCheck.position, Vector2.left, rayDistance, groundLayer) ||
                           Physics2D.Raycast(sideLeftMidCheck.position, Vector2.left, rayDistance, groundLayer) ||
                           Physics2D.Raycast(sideLeftLowCheck.position, Vector2.left, rayDistance, groundLayer);
            }
            else if (transform.eulerAngles.z == 270)
            {
                onGround = Physics2D.Raycast(sideRightCheck.position, Vector2.right, rayDistance, groundLayer) ||
                           Physics2D.Raycast(sideBottomRightCheck.position, Vector2.right, rayDistance, groundLayer) ||
                           Physics2D.Raycast(sideTopRightCheck.position, Vector2.right, rayDistance, groundLayer) ||
                           Physics2D.Raycast(sideRightMidCheck.position, Vector2.right, rayDistance, groundLayer) ||
                           Physics2D.Raycast(sideRightLowCheck.position, Vector2.right, rayDistance, groundLayer);
            }
        }
    }

    private void HandleMovement()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        Vector2 moveVelocity = rb.velocity;

        if (rb.gravityScale == 1 || rb.gravityScale == -1)
        {
            if ((isBlockedLeft && moveInput < 0) || (isBlockedRight && moveInput > 0))
            {
                moveVelocity.x = 0;
            }
            else
            {
                moveVelocity.x = moveInput * pl_moveSpeed;
            }
        }
        else if (rb.gravityScale == 0)
        {
            if ((isBlockedTopLeft || isBlockedTopRight) && verticalInput > 0)
            {
                moveVelocity.y = 0;
            }
            else if ((isBlockedBottomLeft || isBlockedBottomRight) && verticalInput < 0)
            {
                moveVelocity.y = 0;
            }
            else
            {
                moveVelocity.y = verticalInput * pl_moveSpeed;
            }
        }

        rb.velocity = moveVelocity;
    }

    private void HandleJump()
    {
        if (onGround && Input.GetButtonDown("Jump"))
        {
            float rotationZ = transform.eulerAngles.z;
            Vector2 jumpVelocity = rb.velocity;

            if (rb.gravityScale == 1)
            {
                jumpVelocity.y = pl_jumpForce;
            }
            else if (rb.gravityScale == -1)
            {
                jumpVelocity.y = -pl_jumpForce;
            }
            else if (rb.gravityScale == 0)
            {
                if (rotationZ == 90 && !isBlockedRight)
                {
                    jumpVelocity.x = pl_jumpForce;
                }
                else if (rotationZ == 270 && !isBlockedLeft)
                {
                    jumpVelocity.x = -pl_jumpForce;
                }
            }

            rb.velocity = jumpVelocity;
            onGround = false;
            isJumping = true;
        }
    }

    private void Respawn()
    {
        transform.position = initialPosition;
        rb.velocity = Vector2.zero;
    }
}
