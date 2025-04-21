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
            //transform.SetParent(null);
            targetObject = null;
            isActive = false;
        }
    }
}
