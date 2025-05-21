using UnityEngine;

public class SwitchPlayerPostIt : MonoBehaviour
{
    private bool isActive = false;
    private GameObject targetObject;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isActive && other.CompareTag("Player"))
        {
            // �\��t���Ώۂ��L�^
            targetObject = other.gameObject;

            // ���C���[�؂�ւ�
            targetObject.layer = LayerMask.NameToLayer("Default");

            // �^�[�Q�b�g�� TemporaryPlayerController ������ΗL����
            if (targetObject.TryGetComponent(out TemporaryPlayerController temp))
            {
                temp.enabled = true;
            }

            // �^�[�Q�b�g�� PlayerController ������Α���\��
            if (targetObject.TryGetComponent(out PlayerController player))
            {
                player.canMove = true;
            }


            // ���̕tⳂ��^�[�Q�b�g�ɒǏ]������
            transform.SetParent(targetObject.transform);
           
            isActive = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (isActive && other.gameObject == targetObject)
        {
            // ���C���[�߂�
            targetObject.layer = LayerMask.NameToLayer("PlayerJump");


            // �ꎞ�v���C���[������I��
            if (targetObject.TryGetComponent(out TemporaryPlayerController temp))
            {
                temp.enabled = false;
            }

            // �^�[�Q�b�g�� PlayerController ������Α����~
            if (targetObject.TryGetComponent(out PlayerController player))
            {
                player.canMove = false;
            }

            // �Ǐ]������
            transform.SetParent(null);
            targetObject = null;
            isActive = false;
        }
    }

    //�}�E�X�{�^���������ꂽ�Ƃ��ɌĂ΂��
    // ����ɂ��A�}�E�X�{�^���𗣂����Ƃ��ɃJ�����̃^�[�Q�b�g��؂�ւ��邱�Ƃ��ł��܂�
    private void OnMouseUp()
    {
        if (isActive && targetObject != null)
        {
            // �J������\��t�����I�u�W�F�N�g�ɐ؂�ւ�
            CameraFollow cam = Camera.main.GetComponent<CameraFollow>();
            if (cam != null)
            {
                cam.SetTarget(targetObject.transform);
            }
        }
    }
}
