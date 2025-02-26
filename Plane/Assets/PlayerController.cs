using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 2f;
    private Rigidbody2D rb;
    private bool isGrounded;
    private bool gravityInverted = false; //重力が反転しているかどうか

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        //左右移動
        float move = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(move * speed, rb.velocity.y);

        //通常ジャンプ
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector2.up * jumpForce * (gravityInverted ? -1 : 1), ForceMode2D.Impulse);
        }

        //Vキーで重力反転
        if (Input.GetKeyDown(KeyCode.V))
        {
            gravityInverted = !gravityInverted; //反転フラグを切り替え
            rb.gravityScale *= -1; //重力を反転
            transform.Rotate(90f, 0f, 0f); //プレイヤーを上下反転
        }


        if (transform.position.y < -10) //画面外(y=-10より下に落ちたら
        {
            Respawn();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }

        if (collision.gameObject.CompareTag("Spikes"))
        {
            Respawn(); //リスポーン処理を実行
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }


    void Respawn()
    {
        transform.position = new Vector3(0, 0, 0); //初期位置に戻す
        rb.velocity = Vector2.zero; //速度をリセット
    }
}
