using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float pl_moveSpeed = 5f;
    public float pl_jumpForce = 10f;

    // ����pTransform
    public Transform groundCheck;
    public Transform leftCheck;
    public Transform rightCheck;
    public Transform topRightCheck;
    public Transform bottomRightCheck;
    public Transform topLeftCheck;
    public Transform bottomLeftCheck;
    public Transform rightMidCheck; // RightCheck��TopRightCheck�̊�
    public Transform rightLowCheck; // RightCheck��BottomRightCheck�̊�
    public Transform leftMidCheck;  // LeftCheck��TopLeftCheck�̊�
    public Transform leftLowCheck;  // LeftCheck��BottomLeftCheck�̊�

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
    private Vector3 initialPosition; // �����ʒu��ۑ�

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        initialPosition = transform.position; // �Q�[���J�n���̈ʒu��ۑ�
    }

    void Update()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");
        Vector2 moveVelocity = new Vector2(moveInput * pl_moveSpeed, rb.velocity.y);

        float rayDistance = 0.1f; // ���C�̋���

        // �ǂ̔���i���E�j
        isBlockedLeft = Physics2D.Raycast(leftCheck.position, Vector2.left, rayDistance, LayerMask.GetMask("Ground"));
        isBlockedRight = Physics2D.Raycast(rightCheck.position, Vector2.right, rayDistance, LayerMask.GetMask("Ground"));

        // �p�̔���i�㉺�j
        isBlockedTopRight = Physics2D.Raycast(topRightCheck.position, Vector2.up, rayDistance, LayerMask.GetMask("Ground"));
        isBlockedBottomRight = Physics2D.Raycast(bottomRightCheck.position, Vector2.down, rayDistance, LayerMask.GetMask("Ground"));
        isBlockedTopLeft = Physics2D.Raycast(topLeftCheck.position, Vector2.up, rayDistance, LayerMask.GetMask("Ground"));
        isBlockedBottomLeft = Physics2D.Raycast(bottomLeftCheck.position, Vector2.down, rayDistance, LayerMask.GetMask("Ground"));

        //���E�̒���
        isBlockedRightMid = Physics2D.Raycast(rightMidCheck.position, Vector2.right, rayDistance, LayerMask.GetMask("Ground"));
        isBlockedRightLow = Physics2D.Raycast(rightLowCheck.position, Vector2.right, rayDistance, LayerMask.GetMask("Ground"));
        isBlockedLeftMid = Physics2D.Raycast(leftMidCheck.position, Vector2.left, rayDistance, LayerMask.GetMask("Ground"));
        isBlockedLeftLow = Physics2D.Raycast(leftLowCheck.position, Vector2.left, rayDistance, LayerMask.GetMask("Ground"));

        //���E�̕ǂ�����ꍇ�͈ړ���h��
        if ((isBlockedLeft || isBlockedLeftMid || isBlockedLeftLow) && moveInput < 0)
        {
            moveVelocity.x = 0;
        }
        if ((isBlockedRight || isBlockedRightMid || isBlockedRightLow) && moveInput > 0)
        {
            moveVelocity.x = 0;
        }

        rb.velocity = moveVelocity;

        //�n�ʔ���i���C�L���X�g���g�p�j
        onGround = Physics2D.Raycast(groundCheck.position, Vector2.down, rayDistance, LayerMask.GetMask("Ground"));

        //�W�����v����
        if (Input.GetButtonDown("Jump") && onGround)
        {
            rb.velocity = new Vector2(rb.velocity.x, pl_jumpForce);
            isJumping = true;
            onGround = false; // �󒆂ɂ���̂� false
        }

        //��ʊO�ɏo���烊�X�|�[��
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
