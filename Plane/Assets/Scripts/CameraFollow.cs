using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player; // プレイヤーのTransform
    public Vector3 offset = new Vector3(0f, 0f, -10f); // カメラのオフセット
    public float smoothSpeed = 0.125f; // カメラ追従のスムーズさ

    void LateUpdate()
    {
        if (player != null)
        {
            // プレイヤーの現在の位置にオフセットを加えた目標位置を計算
            Vector3 targetPosition = player.position + offset;

            // カメラをスムーズに追従させる
            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);
        }
    }
}
