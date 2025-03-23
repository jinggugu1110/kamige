using UnityEngine;

public class GravityFlipPostIt : MonoBehaviour
{
    private float touchTime = 0f; // 触れている時間
    public float activationTime = 5f; // 5秒間触れ続けたら発動
    private Rigidbody2D targetRb; // 影響を与えるオブジェクトのRigidbody2D
    private Transform attachedObject; // 付箋が付くオブジェクト
    private bool isGravityFlipped = false; // 重力が反転しているか

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Attachable")) // プレイヤーまたは付箋を貼れるオブジェクト
        {
            if (targetRb == null) // 初回接触時にRigidbody2Dを取得
            {
                targetRb = other.GetComponent<Rigidbody2D>();
                attachedObject = other.transform; // 付箋を付ける対象を記録
                transform.SetParent(attachedObject); // 付箋をオブジェクトに追従させる
            }

            touchTime += Time.deltaTime;

            if (touchTime >= activationTime && !isGravityFlipped)
            {
                ActivateGravityFlip();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Attachable"))
        {
            touchTime = 0f; // 離れたらカウントリセット
            transform.SetParent(null); // 付箋の親子関係を解除

            if (isGravityFlipped) // 重力反転が有効なら元に戻す
            {
                targetRb.gravityScale *= -1;
                isGravityFlipped = false;
                Debug.Log("重力が元に戻った！");
            }

            targetRb = null; // 参照リセット
        }
    }

    private void ActivateGravityFlip()
    {
        if (targetRb != null)
        {
            targetRb.gravityScale *= -1; // 重力反転
            isGravityFlipped = true;
            Debug.Log("重力反転付箋が発動！");
        }
    }
}
