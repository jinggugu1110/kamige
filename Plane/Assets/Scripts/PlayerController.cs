using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float pl_moveSpeed = 5f;
    public float pl_jumpForce = 5f;

    // ����pTransform
    public Transform groundCheck;
    public Transform topGroundCheck; // �t�d�͎��̒n�ʔ���
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
    private Vector3 initialPosition; // �����ʒu��ۑ�

    // ���X�|�[���Ɏg��
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

        // ���Ԃ̔���i���E�j
        isBlockedRightMid = Physics2D.Raycast(rightMidCheck.position, Vector2.right, rayDistance, LayerMask.GetMask("Ground"));
        isBlockedRightLow = Physics2D.Raycast(rightLowCheck.position, Vector2.right, rayDistance, LayerMask.GetMask("Ground"));
        isBlockedLeftMid = Physics2D.Raycast(leftMidCheck.position, Vector2.left, rayDistance, LayerMask.GetMask("Ground"));
        isBlockedLeftLow = Physics2D.Raycast(leftLowCheck.position, Vector2.left, rayDistance, LayerMask.GetMask("Ground"));

        // ���E�̕ǂ�����ꍇ�͈ړ���h��
        if ((isBlockedLeft || isBlockedLeftMid || isBlockedLeftLow) && moveInput < 0)
        {
            moveVelocity.x = 0;
        }
        if ((isBlockedRight || isBlockedRightMid || isBlockedRightLow) && moveInput > 0)
        {
            moveVelocity.x = 0;
        }

        rb.velocity = moveVelocity;

        // �n�ʔ���̐؂�ւ��i�ʏ펞�� groundCheck�A�d�͔��]���� topGroundCheck�j
        if (rb.gravityScale > 0)
        {
            onGround = Physics2D.Raycast(groundCheck.position, Vector2.down, rayDistance, LayerMask.GetMask("Ground"));
        }
        else
        {
            onGround = Physics2D.Raycast(topGroundCheck.position, Vector2.up, rayDistance, LayerMask.GetMask("Ground"));
        }

        // �f�o�b�O�p���C�\��
        Debug.DrawRay(groundCheck.position, Vector2.down * rayDistance, Color.red);
        Debug.DrawRay(topGroundCheck.position, Vector2.up * rayDistance, Color.blue);

        if (onGround)
        {
            Debug.Log("�v���C���[�͒n�ʂɐڐG���Ă���");
        }

        // �W�����v�����̕���
        if (Input.GetButtonDown("Jump") && onGround)
        {
            if (rb.gravityScale > 0)
            {
                NormalJump();
            }
            else
            {
                GravityJump();
            }
        }

        // ��ʊO�ɏo���烊�X�|�[��
        if (transform.position.y < -10f)
        {
            Respawn();
        }
    }

    // �ʏ�W�����v����
    private void NormalJump()
    {
        rb.velocity = new Vector2(rb.velocity.x, pl_jumpForce);
        isJumping = true;
        onGround = false;
        Debug.Log("�ʏ�W�����v");
    }

    // ���]�W�����v����
    private void GravityJump()
    {
        rb.velocity = new Vector2(rb.velocity.x, -pl_jumpForce); // �������i���]�����u�W�����v�v�j
        isJumping = true;
        onGround = false;
        Debug.Log("�d�͔��]�W�����v");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if ((rb.gravityScale > 0 && contact.normal.y > 0.5f) || (rb.gravityScale < 0 && contact.normal.y < -0.5f))
                {
                    onGround = true;
                    isJumping = false;
                }
            }
        }

        if (collision.gameObject.CompareTag("Spikes"))
        {
            Respawn();
        }
    }

    // Ground�����̂���object�Ƃ̓����蔻��
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
        transform.position = initialPosition;
        rb.velocity = Vector2.zero;
    }
}
