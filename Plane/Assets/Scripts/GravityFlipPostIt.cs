using UnityEngine;

public class GravityFlipPostIt : MonoBehaviour
{
    private float touchTime = 0f; // �G��Ă��鎞��
    public float activationTime = 5f; // 5�b�ԐG�ꑱ�����甭��
    private Rigidbody2D targetRb; // �e����^����I�u�W�F�N�g��Rigidbody2D
    private Transform attachedObject; // �tⳂ��t���I�u�W�F�N�g
    private bool isGravityFlipped = false; // �d�͂����]���Ă��邩

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Attachable")) // �v���C���[�܂��͕tⳂ�\���I�u�W�F�N�g
        {
            if (targetRb == null) // ����ڐG����Rigidbody2D���擾
            {
                targetRb = other.GetComponent<Rigidbody2D>();
                attachedObject = other.transform; // �tⳂ�t����Ώۂ��L�^
                transform.SetParent(attachedObject); // �tⳂ��I�u�W�F�N�g�ɒǏ]������
            }

            touchTime += Time.deltaTime;

            if (touchTime >= activationTime && !isGravityFlipped)
            {
                ActivateGravityFlip();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Attachable"))
        {
            touchTime = 0f; // ���ꂽ��J�E���g���Z�b�g
            transform.SetParent(null); // �tⳂ̐e�q�֌W������

            if (isGravityFlipped) // �d�͔��]���L���Ȃ猳�ɖ߂�
            {
                targetRb.gravityScale *= -1;
                isGravityFlipped = false;
                Debug.Log("�d�͂����ɖ߂����I");
            }

            targetRb = null; // �Q�ƃ��Z�b�g
        }
    }

    private void ActivateGravityFlip()
    {
        if (targetRb != null)
        {
            targetRb.gravityScale *= -1; // �d�͔��]
            isGravityFlipped = true;
            Debug.Log("�d�͔��]�tⳂ������I");
        }
    }
}
