using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float pl_moveSpeed = 5f;
    public float pl_jumpForce = 5f;

    // 判定用Transform
    public Transform groundCheck;
    public Transform topGroundCheck; // 逆重力時の地面判定
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
    private Vector3 initialPosition; // 初期位置を保存

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        initialPosition = transform.position; // ゲーム開始時の位置を保存
    }

    void Update()
    {
        CheckCollisions(); // 壁判定を先に行う
        HandleMovement();
        HandleJump();
        CheckGround();
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
            float jumpForce = pl_jumpForce;
            float rotationZ = transform.eulerAngles.z;
            Vector2 jumpVelocity = rb.velocity;

            if (rb.gravityScale == 1) // 通常のジャンプ（上方向）
            {
                if (!isBlockedTopLeft && !isBlockedTopRight)
                {
                    jumpVelocity.y = jumpForce;
                }
            }
            else if (rb.gravityScale == -1) // 逆重力ジャンプ（下方向）
            {
                if (!isBlockedBottomLeft && !isBlockedBottomRight)
                {
                    jumpVelocity.y = -jumpForce;
                }
            }
            else if (rb.gravityScale == 0) // 左右重力時（90° or 270°）
            {
                if (rotationZ == 90 && !isBlockedRight)
                {
                    jumpVelocity.x = jumpForce;
                }
                else if (rotationZ == 270 && !isBlockedLeft)
                {
                    jumpVelocity.x = -jumpForce;
                }
            }

            rb.velocity = jumpVelocity;
            onGround = false;
            isJumping = true;
        }
    }

    private void CheckGround()
    {
        float rotationZ = transform.eulerAngles.z;
        float rayDistance = 0.1f;
        LayerMask groundLayer = LayerMask.GetMask("Ground");

        if (rb.gravityScale > 0) // 通常の重力（下向き）
        {
            onGround = Physics2D.Raycast(groundCheck.position, Vector2.down, rayDistance, groundLayer) ||
                       Physics2D.Raycast(bottomRightCheck.position, Vector2.down, rayDistance, groundLayer) ||
                       Physics2D.Raycast(bottomLeftCheck.position, Vector2.down, rayDistance, groundLayer);
        }
        else if (rb.gravityScale < 0) // 逆重力（上向き）
        {
            onGround = Physics2D.Raycast(topGroundCheck.position, Vector2.up, rayDistance, groundLayer) ||
                       Physics2D.Raycast(topRightCheck.position, Vector2.up, rayDistance, groundLayer) ||
                       Physics2D.Raycast(topLeftCheck.position, Vector2.up, rayDistance, groundLayer);
        }
        else if (rb.gravityScale == 0) // 左右重力
        {
            if (rotationZ == 90)
            {
                onGround = isBlockedLeft;
            }
            else if (rotationZ == 270)
            {
                onGround = isBlockedRight;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            onGround = true;
            isJumping = false;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            onGround = false;
        }
    }

    private void Respawn()
    {
        transform.position = initialPosition;
        rb.velocity = Vector2.zero;
    }
}
