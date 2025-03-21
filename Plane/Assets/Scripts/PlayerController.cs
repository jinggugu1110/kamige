using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public Transform groundCheck; // 地面判定用のTransform（プレイヤーの足元に設置）

    private Rigidbody2D rb;
    private bool isJumping = false;
    private bool onGround = false;
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

        // 横移動時の壁判定 & 押し戻し処理
        Vector2 position = rb.position;
        Vector2 checkDirection = Vector2.right * moveInput;
        RaycastHit2D hit = Physics2D.Raycast(position, checkDirection, 0.1f, LayerMask.GetMask("Ground"));

        if (hit.collider != null)
        {
            Debug.Log("Wall detected! Stopping movement. Object: " + hit.collider.name);
            float pushAmount = hit.point.x - position.x;
            rb.position = new Vector2(position.x - pushAmount, position.y);
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
