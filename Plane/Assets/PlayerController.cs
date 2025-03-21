using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public Transform groundCheck; // �n�ʔ���p��Transform�i�v���C���[�̑����ɐݒu�j

    private Rigidbody2D rb;
    private bool isJumping = false;
    private bool onGround = false;
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

        // ���ړ����̕ǔ��� & �����߂�����
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
