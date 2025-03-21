using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;  // �v���C���[��Transform
    public float smoothSpeed = 5f;  // �J�����̒Ǐ]���x
    public Vector3 offset = new Vector3(0f, 0f, -10f);  // �J�����̈ʒu����

    void LateUpdate()
    {
        if (player != null)
        {
            Vector3 targetPosition = player.position + offset;
            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
        }
    }
}
