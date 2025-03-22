using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player; // �v���C���[��Transform
    public Vector3 offset = new Vector3(0f, 0f, -10f); // �J�����̃I�t�Z�b�g�iZ=-10�ɌŒ�j

    void LateUpdate()
    {
        if (player != null)
        {
            // �v���C���[�̈ʒu + �I�t�Z�b�g�ŃJ������z�u
            transform.position = player.position + offset;
        }
    }
}
