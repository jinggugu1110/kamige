using UnityEngine;

public class SwitchPlayerPostIt : MonoBehaviour
{
    private bool isActive = false;
    private GameObject targetObject;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isActive && other.CompareTag("Player"))
        {
            // 貼り付け対象を記録
            targetObject = other.gameObject;

           

            // ターゲットに TemporaryPlayerController があれば有効化
            if (targetObject.TryGetComponent(out TemporaryPlayerController temp))
            {
                temp.enabled = true;
            }

            // ターゲットに PlayerController があれば操作可能に
            if (targetObject.TryGetComponent(out PlayerController player))
            {
                player.canMove = true;
            }

            // この付箋をターゲットに追従させる
            transform.SetParent(targetObject.transform);
            isActive = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (isActive && other.gameObject == targetObject)
        {
           

            // 一時プレイヤー操作を終了
            if (targetObject.TryGetComponent(out TemporaryPlayerController temp))
            {
                temp.enabled = false;
            }

            // ターゲットに PlayerController があれば操作停止
            if (targetObject.TryGetComponent(out PlayerController player))
            {
                player.canMove = false;
            }

            // 追従を解除
            //transform.SetParent(null);
            targetObject = null;
            isActive = false;
        }
    }
}
