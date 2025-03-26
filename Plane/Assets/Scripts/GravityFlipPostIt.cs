//using UnityEngine;

//public class GravityFlipPostIt : MonoBehaviour
//{
//    private float touchTime = 0f; // �G��Ă��鎞��
//    public float activationTime = 5f; // 5�b�ԐG�ꑱ�����甭��
//    private Rigidbody2D targetRb; // �e����^����I�u�W�F�N�g��Rigidbody2D
//    private Transform attachedObject; // �tⳂ��t���I�u�W�F�N�g
//    private bool isGravityFlipped = false; // �d�͂����]���Ă��邩

//    private void OnTriggerStay2D(Collider2D other)
//    {
//        if (other.CompareTag("Player") || other.CompareTag("Attachable")) // �v���C���[�܂��͕tⳂ�\���I�u�W�F�N�g
//        {
//            if (targetRb == null) // ����ڐG����Rigidbody2D���擾
//            {
//                targetRb = other.GetComponent<Rigidbody2D>();
//                attachedObject = other.transform; // �tⳂ�t����Ώۂ��L�^
//                transform.SetParent(attachedObject); // �tⳂ��I�u�W�F�N�g�ɒǏ]������
//            }

//            touchTime += Time.deltaTime;

//            if (touchTime >= activationTime && !isGravityFlipped)
//            {
//                ActivateGravityFlip();
//            }
//        }
//    }

//    private void OnTriggerExit2D(Collider2D other)
//    {
//        if (other.CompareTag("Player") || other.CompareTag("Attachable"))
//        {
//            touchTime = 0f; // ���ꂽ��J�E���g���Z�b�g
//            transform.SetParent(null); // �tⳂ̐e�q�֌W������

//            if (isGravityFlipped) // �d�͔��]���L���Ȃ猳�ɖ߂�
//            {
//                targetRb.gravityScale *= -1;
//                isGravityFlipped = false;
//                Debug.Log("�d�͂����ɖ߂����I");
//            }

//            targetRb = null; // �Q�ƃ��Z�b�g
//        }
//    }

//    private void ActivateGravityFlip()
//    {
//        if (targetRb != null)
//        {
//            targetRb.gravityScale *= -1; // �d�͔��]
//            isGravityFlipped = true;
//            Debug.Log("�d�͔��]�tⳂ������I");
//        }
//    }
//}

using UnityEngine;

public class GravityFlipPostIt : MonoBehaviour
{
    private Rigidbody2D targetRb;
    private float touchTime = 0f;
    public float activationTime = 5f;
    private bool isGravityFlipped = false;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Attachable"))
        {
            if (targetRb == null)
            {
                targetRb = other.GetComponent<Rigidbody2D>();
                if (targetRb != null)
                {
                    transform.SetParent(targetRb.transform); // �tⳂ��v���C���[��I�u�W�F�N�g�ɂ�������
                }
            }

            touchTime += Time.deltaTime;

            if (touchTime >= activationTime)
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
            ResetGravity(); // �d�͂����ɖ߂�
            targetRb = null;
            transform.SetParent(null); // �tⳂ����̏�Ԃɖ߂�
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

    public void UpdateGravityBasedOnRotation(float rotationZ)
    {
        if (targetRb != null)
        {
            switch ((int)rotationZ)
            {
                case 0:
                    targetRb.gravityScale = -1; // �����
                    break;
                case 90:
                    targetRb.gravityScale = 0; // �E�����i���d�́j
                    targetRb.velocity = new Vector2(3f, 0f); // �E�Ɉړ��i���j
                    break;
                case 180:
                    targetRb.gravityScale = 1; // �������i�ʏ�j
                    break;
                case 270:
                    targetRb.gravityScale = 0; // �������i���d�́j
                    targetRb.velocity = new Vector2(-3f, 0f); // ���Ɉړ��i���j
                    break;
            }
            Debug.Log("�d�͕����ύX: " + targetRb.gravityScale);
        }
    }

    private void ResetGravity()
    {
        if (targetRb != null)
        {
            targetRb.gravityScale = 1; // �ʏ�̏d�͂ɖ߂�
            isGravityFlipped = false;
        }
    }
}