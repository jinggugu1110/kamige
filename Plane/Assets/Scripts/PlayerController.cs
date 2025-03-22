//using UnityEngine;

//public class PlayerController : MonoBehaviour
//{
//    public float moveSpeed = 5f;
//    public float jumpForce = 10f;
//    public Transform groundCheck; // 地面判定用のTransform（プレイヤーの足元に設置）
//    public Transform leftCheck;   // 左側の壁判定用
//    public Transform rightCheck;  // 右側の壁判定用

//    private Rigidbody2D rb;
//    private bool isJumping = false;
//    private bool onGround = false;
//    private bool isBlockedLeft = false;
//    private bool isBlockedRight = false;
//    private Vector3 initialPosition; // 初期位置を保存

//    void Start()
//    {
//        rb = GetComponent<Rigidbody2D>();
//        initialPosition = transform.position; // ゲーム開始時の位置を保存
//    }

//    void Update()
//    {
//        float moveInput = Input.GetAxisRaw("Horizontal");
//        Vector2 moveVelocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

//        // 壁の判定（左右の Raycast）
//        isBlockedLeft = Physics2D.Raycast(leftCheck.position, Vector2.left, 0.1f, LayerMask.GetMask("Ground"));
//        isBlockedRight = Physics2D.Raycast(rightCheck.position, Vector2.right, 0.1f, LayerMask.GetMask("Ground"));

//        // 左右の壁がある場合は移動を防ぐ
//        if ((isBlockedLeft && moveInput < 0) || (isBlockedRight && moveInput > 0))
//        {
//            moveVelocity.x = 0;
//        }

//        rb.velocity = moveVelocity;

//        // 地面判定（レイキャストを使用）
//        onGround = Physics2D.Raycast(groundCheck.position, Vector2.down, 0.1f, LayerMask.GetMask("Ground"));

//        // ジャンプ処理
//        if (Input.GetButtonDown("Jump") && onGround)
//        {
//            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
//            isJumping = true;
//            onGround = false; // 空中にいるので false
//        }

//        // 画面外に出たらリスポーン
//        if (transform.position.y < -10f) // 例: Y座標が -10 以下になったら初期位置に戻す
//        {
//            Respawn();
//        }
//    }

//    private void OnCollisionEnter2D(Collision2D collision)
//    {
//        if (collision.gameObject.CompareTag("Ground"))
//        {
//            foreach (ContactPoint2D contact in collision.contacts)
//            {
//                if (contact.normal.y > 0.5f)
//                {
//                    onGround = true;
//                    isJumping = false;
//                }
//            }
//        }

//        // Spike に当たったらリスポーン
//        if (collision.gameObject.CompareTag("Spikes"))
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

//    // 初期位置に戻す処理
//    private void Respawn()
//    {
//        transform.position = initialPosition; // 初期位置に戻す
//        rb.velocity = Vector2.zero; // 動きをリセット
//    }
//}


using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 10f;

    // 判定用Transform
    public Transform groundCheck;
    public Transform leftCheck;
    public Transform rightCheck;
    public Transform topRightCheck;
    public Transform bottomRightCheck;
    public Transform topLeftCheck;
    public Transform bottomLeftCheck;

    private Rigidbody2D rb;
    private bool isJumping = false;
    private bool onGround = false;
    private bool isBlockedLeft = false;
    private bool isBlockedRight = false;
    private bool isBlockedTopRight = false;
    private bool isBlockedBottomRight = false;
    private bool isBlockedTopLeft = false;
    private bool isBlockedBottomLeft = false;
    private Vector3 initialPosition; // 初期位置を保存

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        initialPosition = transform.position; // ゲーム開始時の位置を保存
    }

    void Update()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");
        Vector2 moveVelocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        // 壁の判定（左右）
        isBlockedLeft = Physics2D.Raycast(leftCheck.position, Vector2.left, 0.1f, LayerMask.GetMask("Ground"));
        isBlockedRight = Physics2D.Raycast(rightCheck.position, Vector2.right, 0.1f, LayerMask.GetMask("Ground"));

        // 角の判定（四隅）
        isBlockedTopRight = Physics2D.Raycast(topRightCheck.position, Vector2.right, 0.1f, LayerMask.GetMask("Ground"));
        isBlockedBottomRight = Physics2D.Raycast(bottomRightCheck.position, Vector2.right, 0.1f, LayerMask.GetMask("Ground"));
        isBlockedTopLeft = Physics2D.Raycast(topLeftCheck.position, Vector2.left, 0.1f, LayerMask.GetMask("Ground"));
        isBlockedBottomLeft = Physics2D.Raycast(bottomLeftCheck.position, Vector2.left, 0.1f, LayerMask.GetMask("Ground"));

        // 左右の壁がある場合は移動を防ぐ
        if ((isBlockedLeft && moveInput < 0) || (isBlockedRight && moveInput > 0))
        {
            moveVelocity.x = 0;
        }

        rb.velocity = moveVelocity;

        // 地面判定（レイキャストを使用）
        onGround = Physics2D.Raycast(groundCheck.position, Vector2.down, 0.1f, LayerMask.GetMask("Ground"));

        // ジャンプ処理
        if (Input.GetButtonDown("Jump") && onGround)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            isJumping = true;
            onGround = false; // 空中にいるので false
        }

        // 画面外に出たらリスポーン
        if (transform.position.y < -10f) // 例: Y座標が -10 以下になったら初期位置に戻す
        {
            Respawn();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (contact.normal.y > 0.5f)
                {
                    onGround = true;
                    isJumping = false;
                }
            }
        }

        // Spike に当たったらリスポーン
        if (collision.gameObject.CompareTag("Spikes"))
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

    // 初期位置に戻す処理
    private void Respawn()
    {
        transform.position = initialPosition; // 初期位置に戻す
        rb.velocity = Vector2.zero; // 動きをリセット
    }
}
