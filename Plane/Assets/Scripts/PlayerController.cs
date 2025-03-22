//using UnityEngine;

//public class PlayerController : MonoBehaviour
//{
//    public float moveSpeed = 5f;
//    public float jumpForce = 10f;
//    public Transform groundCheck; // �n�ʔ���p��Transform�i�v���C���[�̑����ɐݒu�j
//    public Transform leftCheck;   // �����̕ǔ���p
//    public Transform rightCheck;  // �E���̕ǔ���p

//    private Rigidbody2D rb;
//    private bool isJumping = false;
//    private bool onGround = false;
//    private bool isBlockedLeft = false;
//    private bool isBlockedRight = false;
//    private Vector3 initialPosition; // �����ʒu��ۑ�

//    void Start()
//    {
//        rb = GetComponent<Rigidbody2D>();
//        initialPosition = transform.position; // �Q�[���J�n���̈ʒu��ۑ�
//    }

//    void Update()
//    {
//        float moveInput = Input.GetAxisRaw("Horizontal");
//        Vector2 moveVelocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

//        // �ǂ̔���i���E�� Raycast�j
//        isBlockedLeft = Physics2D.Raycast(leftCheck.position, Vector2.left, 0.1f, LayerMask.GetMask("Ground"));
//        isBlockedRight = Physics2D.Raycast(rightCheck.position, Vector2.right, 0.1f, LayerMask.GetMask("Ground"));

//        // ���E�̕ǂ�����ꍇ�͈ړ���h��
//        if ((isBlockedLeft && moveInput < 0) || (isBlockedRight && moveInput > 0))
//        {
//            moveVelocity.x = 0;
//        }

//        rb.velocity = moveVelocity;

//        // �n�ʔ���i���C�L���X�g���g�p�j
//        onGround = Physics2D.Raycast(groundCheck.position, Vector2.down, 0.1f, LayerMask.GetMask("Ground"));

//        // �W�����v����
//        if (Input.GetButtonDown("Jump") && onGround)
//        {
//            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
//            isJumping = true;
//            onGround = false; // �󒆂ɂ���̂� false
//        }

//        // ��ʊO�ɏo���烊�X�|�[��
//        if (transform.position.y < -10f) // ��: Y���W�� -10 �ȉ��ɂȂ����珉���ʒu�ɖ߂�
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

//        // Spike �ɓ��������烊�X�|�[��
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

//    // �����ʒu�ɖ߂�����
//    private void Respawn()
//    {
//        transform.position = initialPosition; // �����ʒu�ɖ߂�
//        rb.velocity = Vector2.zero; // ���������Z�b�g
//    }
//}


using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 10f;

    // ����pTransform
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
    private Vector3 initialPosition; // �����ʒu��ۑ�

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        initialPosition = transform.position; // �Q�[���J�n���̈ʒu��ۑ�
    }

    void Update()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");
        Vector2 moveVelocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        // �ǂ̔���i���E�j
        isBlockedLeft = Physics2D.Raycast(leftCheck.position, Vector2.left, 0.1f, LayerMask.GetMask("Ground"));
        isBlockedRight = Physics2D.Raycast(rightCheck.position, Vector2.right, 0.1f, LayerMask.GetMask("Ground"));

        // �p�̔���i�l���j
        isBlockedTopRight = Physics2D.Raycast(topRightCheck.position, Vector2.right, 0.1f, LayerMask.GetMask("Ground"));
        isBlockedBottomRight = Physics2D.Raycast(bottomRightCheck.position, Vector2.right, 0.1f, LayerMask.GetMask("Ground"));
        isBlockedTopLeft = Physics2D.Raycast(topLeftCheck.position, Vector2.left, 0.1f, LayerMask.GetMask("Ground"));
        isBlockedBottomLeft = Physics2D.Raycast(bottomLeftCheck.position, Vector2.left, 0.1f, LayerMask.GetMask("Ground"));

        // ���E�̕ǂ�����ꍇ�͈ړ���h��
        if ((isBlockedLeft && moveInput < 0) || (isBlockedRight && moveInput > 0))
        {
            moveVelocity.x = 0;
        }

        rb.velocity = moveVelocity;

        // �n�ʔ���i���C�L���X�g���g�p�j
        onGround = Physics2D.Raycast(groundCheck.position, Vector2.down, 0.1f, LayerMask.GetMask("Ground"));

        // �W�����v����
        if (Input.GetButtonDown("Jump") && onGround)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            isJumping = true;
            onGround = false; // �󒆂ɂ���̂� false
        }

        // ��ʊO�ɏo���烊�X�|�[��
        if (transform.position.y < -10f) // ��: Y���W�� -10 �ȉ��ɂȂ����珉���ʒu�ɖ߂�
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

        // Spike �ɓ��������烊�X�|�[��
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

    // �����ʒu�ɖ߂�����
    private void Respawn()
    {
        transform.position = initialPosition; // �����ʒu�ɖ߂�
        rb.velocity = Vector2.zero; // ���������Z�b�g
    }
}
