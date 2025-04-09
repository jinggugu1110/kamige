//using UnityEngine;

//public class PlayerController : MonoBehaviour
//{
//    public float pl_moveSpeed = 5f;
//    public float pl_jumpForce = 10f;

//    // 判定用Transform
//    public Transform groundCheck;
//    public Transform topGroundCheck; // 逆重力時の地面判定
//    public Transform leftCheck;
//    public Transform rightCheck;
//    public Transform topRightCheck;
//    public Transform bottomRightCheck;
//    public Transform topLeftCheck;
//    public Transform bottomLeftCheck;
//    public Transform rightMidCheck;
//    public Transform rightLowCheck;
//    public Transform leftMidCheck;
//    public Transform leftLowCheck;

//    // 90度回転した GroundCheck（左右重力用）
//    public Transform sideGroundCheck;
//    public Transform sideTopGroundCheck;
//    public Transform sideLeftCheck;
//    public Transform sideRightCheck;
//    public Transform sideTopLeftCheck;
//    public Transform sideTopRightCheck;
//    public Transform sideBottomLeftCheck;
//    public Transform sideBottomRightCheck;
//    public Transform sideRightMidCheck;
//    public Transform sideRightLowCheck;
//    public Transform sideLeftMidCheck;
//    public Transform sideLeftLowCheck;

//    private Rigidbody2D rb;
//    private bool isJumping = false;
//    private bool onGround = false;
//    private bool isBlockedLeft = false;
//    private bool isBlockedRight = false;
//    private bool isBlockedTopRight = false;
//    private bool isBlockedBottomRight = false;
//    private bool isBlockedTopLeft = false;
//    private bool isBlockedBottomLeft = false;
//    private bool isBlockedRightMid = false;
//    private bool isBlockedRightLow = false;
//    private bool isBlockedLeftMid = false;
//    private bool isBlockedLeftLow = false;
//    private Vector3 initialPosition;

//    void Start()
//    {
//        rb = GetComponent<Rigidbody2D>();
//        initialPosition = transform.position;
//    }

//    void Update()
//    {
//        CheckCollisions();
//        HandleMovement();
//        HandleJump();

//        if (transform.position.y < -10f || transform.position.x < -10f || transform.position.x > 10f)
//        {
//            Respawn();
//        }
//    }

//    private void HandleMovement()
//    {
//        float moveInput = Input.GetAxisRaw("Horizontal");
//        float verticalInput = Input.GetAxisRaw("Vertical");
//        Vector2 moveVelocity = rb.velocity;

//        if (rb.gravityScale == 1 || rb.gravityScale == -1) // 上下重力
//        {
//            moveVelocity.x = moveInput * pl_moveSpeed;

//            // 左右壁判定を適用
//            if ((isBlockedLeft || isBlockedLeftMid || isBlockedLeftLow) && moveInput < 0)
//            {
//                moveVelocity.x = 0;
//            }
//            if ((isBlockedRight || isBlockedRightMid || isBlockedRightLow) && moveInput > 0)
//            {
//                moveVelocity.x = 0;
//            }
//        }
//        else if (rb.gravityScale == 0) // 左右重力
//        {
//            moveVelocity.y = verticalInput * pl_moveSpeed;

//            // 上下壁判定を適用
//            if ((isBlockedTopLeft || isBlockedTopRight) && verticalInput > 0)
//            {
//                moveVelocity.y = 0;
//            }
//            if ((isBlockedBottomLeft || isBlockedBottomRight) && verticalInput < 0)
//            {
//                moveVelocity.y = 0;
//            }
//        }

//        rb.velocity = moveVelocity;
//    }

//    private void HandleJump()
//    {
//        if (onGround && Input.GetButtonDown("Jump"))
//        {
//            float rotationZ = transform.eulerAngles.z;
//            Vector2 jumpVelocity = rb.velocity;

//            if (rb.gravityScale == 1) // 通常のジャンプ（上方向）
//            {
//                jumpVelocity.y = pl_jumpForce;
//            }
//            else if (rb.gravityScale == -1) // 逆重力ジャンプ（下方向）
//            {
//                jumpVelocity.y = -pl_jumpForce;
//            }
//            else if (rb.gravityScale == 0) // 左右重力時
//            {
//                if (rotationZ == 90) // 右向き
//                {
//                    jumpVelocity.x = pl_jumpForce;
//                }
//                else if (rotationZ == 270) // 左向き
//                {
//                    jumpVelocity.x = -pl_jumpForce;
//                }
//            }

//            rb.velocity = jumpVelocity;
//            onGround = false;
//            isJumping = true;
//        }
//    }

//    private void CheckCollisions()
//    {
//        float rayDistance = 0.1f;
//        LayerMask groundLayer = LayerMask.GetMask("Ground");

//        if (rb.gravityScale == 1 || rb.gravityScale == -1) // 上下重力の時のみ
//        {
//            isBlockedLeft = Physics2D.Raycast(leftCheck.position, Vector2.left, rayDistance, groundLayer);
//            isBlockedRight = Physics2D.Raycast(rightCheck.position, Vector2.right, rayDistance, groundLayer);
//            isBlockedTopRight = Physics2D.Raycast(topRightCheck.position, Vector2.up, rayDistance, groundLayer);
//            isBlockedBottomRight = Physics2D.Raycast(bottomRightCheck.position, Vector2.down, rayDistance, groundLayer);
//            isBlockedTopLeft = Physics2D.Raycast(topLeftCheck.position, Vector2.up, rayDistance, groundLayer);
//            isBlockedBottomLeft = Physics2D.Raycast(bottomLeftCheck.position, Vector2.down, rayDistance, groundLayer);
//            isBlockedRightMid = Physics2D.Raycast(rightMidCheck.position, Vector2.right, rayDistance, groundLayer);
//            isBlockedRightLow = Physics2D.Raycast(rightLowCheck.position, Vector2.right, rayDistance, groundLayer);
//            isBlockedLeftMid = Physics2D.Raycast(leftMidCheck.position, Vector2.left, rayDistance, groundLayer);
//            isBlockedLeftLow = Physics2D.Raycast(leftLowCheck.position, Vector2.left, rayDistance, groundLayer);
//        }
//        else if (rb.gravityScale == 0) // 左右重力の時のみ
//        {
//            isBlockedTopLeft = Physics2D.Raycast(sideTopLeftCheck.position, Vector2.up, rayDistance, groundLayer);
//            isBlockedTopRight = Physics2D.Raycast(sideTopRightCheck.position, Vector2.up, rayDistance, groundLayer);
//            isBlockedBottomLeft = Physics2D.Raycast(sideBottomLeftCheck.position, Vector2.down, rayDistance, groundLayer);
//            isBlockedBottomRight = Physics2D.Raycast(sideBottomRightCheck.position, Vector2.down, rayDistance, groundLayer);
//        }
//    }


//    //private void HandleJump()
//    //{
//    //    if (onGround && Input.GetButtonDown("Jump"))
//    //    {
//    //        float rotationZ = transform.eulerAngles.z;
//    //        Vector2 jumpVelocity = rb.velocity;

//    //        if (rb.gravityScale == 1) // 通常のジャンプ（上方向）
//    //        {
//    //            jumpVelocity.y = pl_jumpForce;
//    //        }
//    //        else if (rb.gravityScale == -1) // 逆重力ジャンプ（下方向）
//    //        {
//    //            jumpVelocity.y = -pl_jumpForce;
//    //        }
//    //        else if (rb.gravityScale == 0) // 左右重力時
//    //        {
//    //            if (rotationZ == 90) // 右向き
//    //            {
//    //                jumpVelocity.x = pl_jumpForce;
//    //            }
//    //            else if (rotationZ == 270) // 左向き
//    //            {
//    //                jumpVelocity.x = -pl_jumpForce;
//    //            }
//    //        }

//    //        rb.velocity = jumpVelocity;
//    //        onGround = false;
//    //        isJumping = true;
//    //        Debug.Log("ジャンプ実行: " + jumpVelocity);
//    //    }
//    //}



//    //private void CheckCollisions()
//    //{
//    //    float rayDistance = 0.1f;
//    //    LayerMask groundLayer = LayerMask.GetMask("Ground");

//    //    if (rb.gravityScale == 1 || rb.gravityScale == -1) // 上下重力時
//    //    {
//    //        onGround = Physics2D.Raycast(groundCheck.position, Vector2.down, rayDistance, groundLayer) ||
//    //                   Physics2D.Raycast(topGroundCheck.position, Vector2.up, rayDistance, groundLayer);
//    //    }
//    //    else if (rb.gravityScale == 0) // 左右重力時
//    //    {
//    //        onGround = Physics2D.Raycast(sideGroundCheck.position, Vector2.left, rayDistance, groundLayer) ||
//    //                   Physics2D.Raycast(sideTopGroundCheck.position, Vector2.right, rayDistance, groundLayer);
//    //    }

//    //    isBlockedLeft = Physics2D.Raycast(leftCheck.position, Vector2.left, rayDistance, groundLayer);
//    //    isBlockedRight = Physics2D.Raycast(rightCheck.position, Vector2.right, rayDistance, groundLayer);
//    //    isBlockedRightMid = Physics2D.Raycast(rightMidCheck.position, Vector2.right, rayDistance, groundLayer);
//    //    isBlockedRightLow = Physics2D.Raycast(rightLowCheck.position, Vector2.right, rayDistance, groundLayer);
//    //    isBlockedLeftMid = Physics2D.Raycast(leftMidCheck.position, Vector2.left, rayDistance, groundLayer);
//    //    isBlockedLeftLow = Physics2D.Raycast(leftLowCheck.position, Vector2.left, rayDistance, groundLayer);

//    //    Debug.Log("onGround: " + onGround);
//    //}

//    private void OnCollisionEnter2D(Collision2D collision)
//    {
//        if (collision.gameObject.CompareTag("Ground"))
//        {
//            onGround = true;
//            isJumping = false;
//        }
//        else if (collision.gameObject.CompareTag("Spikes"))
//        {
//            Respawn();
//        }
//    }

//    private void OnCollisionExit2D(Collision2D collision)
//    {
//        if (collision.gameObject.CompareTag("Ground"))
//        {
//            onGround = false;
//        }
//    }

//    private void Respawn()
//    {
//        transform.position = initialPosition;
//        rb.velocity = Vector2.zero;
//    }
//}

using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float pl_moveSpeed = 5f;
    public float pl_jumpForce = 10f;

    // 判定用Transform（上下重力用）
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

    // 90度回転したTransform（左右重力用）
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
        HandleMovement();
        HandleJump();

        if (transform.position.y < -10f || transform.position.x < -10f || transform.position.x > 10f)
        {
            Respawn();
        }
    }

    private void HandleMovement()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        Vector2 moveVelocity = rb.velocity;

        if (rb.gravityScale == 1 || rb.gravityScale == -1) // 上下重力
        {
            moveVelocity.x = moveInput * pl_moveSpeed;

            if ((isBlockedLeft || isBlockedLeftMid || isBlockedLeftLow) && moveInput < 0)
                moveVelocity.x = 0;
            if ((isBlockedRight || isBlockedRightMid || isBlockedRightLow) && moveInput > 0)
                moveVelocity.x = 0;
        }
        else if (rb.gravityScale == 0) // 左右重力
        {
            moveVelocity.y = verticalInput * pl_moveSpeed;

            if ((isBlockedLeft || isBlockedLeftMid || isBlockedLeftLow) && verticalInput < 0)
                moveVelocity.y = 0;
            if ((isBlockedRight || isBlockedRightMid || isBlockedRightLow) && verticalInput > 0)
                moveVelocity.y = 0;
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
                jumpVelocity.y = pl_jumpForce;
            else if (rb.gravityScale == -1)
                jumpVelocity.y = -pl_jumpForce;
            else if (rb.gravityScale == 0) // 左右重力時
            {
                if (Mathf.Abs(rotationZ - 90f) < 1f) // 約90度（右向き）
                    jumpVelocity.x = pl_jumpForce;
                else if (Mathf.Abs(rotationZ - 270f) < 1f) // 約270度（左向き）
                    jumpVelocity.x = -pl_jumpForce;
            }


            rb.velocity = jumpVelocity;
            onGround = false;
            isJumping = true;
        }
    }

    private void CheckCollisions()
    {
        float rayDistance = 0.1f;
        LayerMask groundLayer = LayerMask.GetMask("Ground");

        bool useSide = rb.gravityScale == 0;

        Transform lCheck = useSide ? sideLeftCheck : leftCheck;
        Transform rCheck = useSide ? sideRightCheck : rightCheck;
        Transform rMid = useSide ? sideRightMidCheck : rightMidCheck;
        Transform rLow = useSide ? sideRightLowCheck : rightLowCheck;
        Transform lMid = useSide ? sideLeftMidCheck : leftMidCheck;
        Transform lLow = useSide ? sideLeftLowCheck : leftLowCheck;
        Transform gCheck = useSide ? sideGroundCheck : groundCheck;
        Transform topGCheck = useSide ? sideTopGroundCheck : topGroundCheck;

        isBlockedLeft = Physics2D.Raycast(lCheck.position, useSide ? Vector2.down : Vector2.left, rayDistance, groundLayer);
        isBlockedRight = Physics2D.Raycast(rCheck.position, useSide ? Vector2.up : Vector2.right, rayDistance, groundLayer);
        isBlockedRightMid = Physics2D.Raycast(rMid.position, useSide ? Vector2.up : Vector2.right, rayDistance, groundLayer);
        isBlockedRightLow = Physics2D.Raycast(rLow.position, useSide ? Vector2.up : Vector2.right, rayDistance, groundLayer);
        isBlockedLeftMid = Physics2D.Raycast(lMid.position, useSide ? Vector2.down : Vector2.left, rayDistance, groundLayer);
        isBlockedLeftLow = Physics2D.Raycast(lLow.position, useSide ? Vector2.down : Vector2.left, rayDistance, groundLayer);

        // 地面判定（左右 or 上下で切り替え）
        onGround = Physics2D.Raycast(gCheck.position, useSide ? Vector2.left : Vector2.down, rayDistance, groundLayer)
                || Physics2D.Raycast(topGCheck.position, useSide ? Vector2.right : Vector2.up, rayDistance, groundLayer);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            onGround = true;
            isJumping = false;
        }
        else if (collision.gameObject.CompareTag("Spikes"))
        {
            Respawn();
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
