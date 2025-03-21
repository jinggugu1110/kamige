using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;  // プレイヤーのTransform
    public float smoothSpeed = 5f;  // カメラの追従速度
    public Vector3 offset = new Vector3(0f, 0f, -10f);  // カメラの位置調整

    void LateUpdate()
    {
        if (player != null)
        {
            Vector3 targetPosition = player.position + offset;
            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
        }
    }
}
