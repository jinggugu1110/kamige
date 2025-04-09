using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player; // �v���C���[��Transform
    public Vector3 offset = new Vector3(0f, 0f, -10f); // �J�����̃I�t�Z�b�g
    public float smoothSpeed = 0.125f; // �J�����Ǐ]�̃X���[�Y��

    void LateUpdate()
    {
        if (player != null)
        {
            // �v���C���[�̌��݂̈ʒu�ɃI�t�Z�b�g���������ڕW�ʒu���v�Z
            Vector3 targetPosition = player.position + offset;

            // �J�������X���[�Y�ɒǏ]������
            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);
        }
    }
}
