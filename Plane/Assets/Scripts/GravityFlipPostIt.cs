using UnityEngine;

public class GravityFlipPostIt : MonoBehaviour
{
    public float activationTime = 1.0f; // �����܂ł̎���
    private float touchTime = 0.0f;
    private Rigidbody2D targetRb;
    private bool isGravityFlipped = false;
    private float horizontalGravity = 9.8f; // X�����̏d��

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Attachable"))
        {
            if (targetRb == null)
            {
                targetRb = other.GetComponent<Rigidbody2D>();
                if (targetRb != null)
                {
                    transform.SetParent(targetRb.transform);
                }
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if ((other.CompareTag("Player") || other.CompareTag("Attachable")) && targetRb != null)
        {
            if (!isGravityFlipped) // ���łɔ����ς݂Ȃ珈�����Ȃ�
            {
                touchTime += Time.deltaTime; // �v���C���[���Î~���Ă��Ă����Ԃ��i��
                if (touchTime >= activationTime)
                {
                    ActivateGravityFlip();
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<Rigidbody2D>() == targetRb)
        {
            ResetGravity();
            transform.SetParent(null);
            targetRb = null;
            touchTime = 0.0f;
        }
    }

    private void ActivateGravityFlip()
    {
        if (targetRb != null)
        {
            isGravityFlipped = true;
            UpdateGravityBasedOnRotation(transform.eulerAngles.z);
            Debug.Log("�d�͔��]�tⳂ������I");
        }
    }

    private void ResetGravity()
    {
        if (targetRb != null)
        {
            targetRb.gravityScale = 1.0f; // �d�͂����ɖ߂�
            targetRb.linearVelocity = Vector2.zero; // ���x�����Z�b�g
            isGravityFlipped = false;
            Debug.Log("�d�͔��]����");
        }
    }

    public void UpdateGravityBasedOnRotation(float rotationZ)
    {
        if (targetRb == null) return;

        if (rotationZ == 0)
        {
            targetRb.gravityScale = -1.0f; // ������ɏd��
        }
        else if (rotationZ == 90)
        {
            targetRb.gravityScale = 0.0f; // �d�͂𖳌���
        }
        else if (rotationZ == 180)
        {
            targetRb.gravityScale = 1.0f; // �������i�ʏ�̏d�́j
        }
        else if (rotationZ == 270)
        {
            targetRb.gravityScale = 0.0f; // �d�͂𖳌���
        }
    }

    private void FixedUpdate()
    {
        if (targetRb == null) return;

        float rotationZ = transform.eulerAngles.z;

        if (rotationZ == 90) // �������d��
        {
            targetRb.AddForce(new Vector2(-horizontalGravity, 0), ForceMode2D.Force);
        }
        else if (rotationZ == 270) // �E�����d��
        {
            targetRb.AddForce(new Vector2(horizontalGravity, 0), ForceMode2D.Force);
        }
    }
}
