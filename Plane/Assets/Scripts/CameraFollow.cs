using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player; // プレイヤーのTransform
    public Vector3 offset = new Vector3(0f, 0f, -10f); // カメラのオフセット（Z=-10に固定）

    void LateUpdate()
    {
        if (player != null)
        {
            // プレイヤーの位置 + オフセットでカメラを配置
            transform.position = player.position + offset;
        }
    }
}
