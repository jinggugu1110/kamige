//using UnityEngine;

//public class GravityFlipPostIt : MonoBehaviour
//{
//    private float touchTime = 0f; // 触れている時間
//    public float activationTime = 5f; // 5秒間触れ続けたら発動
//    private Rigidbody2D targetRb; // 影響を与えるオブジェクトのRigidbody2D
//    private Transform attachedObject; // 付箋が付くオブジェクト
//    private bool isGravityFlipped = false; // 重力が反転しているか

//    private void OnTriggerStay2D(Collider2D other)
//    {
//        if (other.CompareTag("Player") || other.CompareTag("Attachable")) // プレイヤーまたは付箋を貼れるオブジェクト
//        {
//            if (targetRb == null) // 初回接触時にRigidbody2Dを取得
//            {
//                targetRb = other.GetComponent<Rigidbody2D>();
//                attachedObject = other.transform; // 付箋を付ける対象を記録
//                transform.SetParent(attachedObject); // 付箋をオブジェクトに追従させる
//            }

//            touchTime += Time.deltaTime;

//            if (touchTime >= activationTime && !isGravityFlipped)
//            {
//                ActivateGravityFlip();
//            }
//        }
//    }

//    private void OnTriggerExit2D(Collider2D other)
//    {
//        if (other.CompareTag("Player") || other.CompareTag("Attachable"))
//        {
//            touchTime = 0f; // 離れたらカウントリセット
//            transform.SetParent(null); // 付箋の親子関係を解除

//            if (isGravityFlipped) // 重力反転が有効なら元に戻す
//            {
//                targetRb.gravityScale *= -1;
//                isGravityFlipped = false;
//                Debug.Log("重力が元に戻った！");
//            }

//            targetRb = null; // 参照リセット
//        }
//    }

//    private void ActivateGravityFlip()
//    {
//        if (targetRb != null)
//        {
//            targetRb.gravityScale *= -1; // 重力反転
//            isGravityFlipped = true;
//            Debug.Log("重力反転付箋が発動！");
//        }
//    }
//}

using UnityEngine;

public class GravityFlipPostIt : MonoBehaviour
{
    private Rigidbody2D targetRb;
    private float touchTime = 0f;
    public float activationTime = 5f;
    private bool isGravityFlipped = false;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Attachable"))
        {
            if (targetRb == null)
            {
                targetRb = other.GetComponent<Rigidbody2D>();
                if (targetRb != null)
                {
                    transform.SetParent(targetRb.transform); // 付箋をプレイヤーやオブジェクトにくっつける
                }
            }

            touchTime += Time.deltaTime;

            if (touchTime >= activationTime)
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
            ResetGravity(); // 重力を元に戻す
            targetRb = null;
            transform.SetParent(null); // 付箋を元の状態に戻す
        }
    }

    private void ActivateGravityFlip()
    {
        if (targetRb != null)
        {
            isGravityFlipped = true;
            UpdateGravityBasedOnRotation(transform.eulerAngles.z);
            Debug.Log("重力反転付箋が発動！");
        }
    }

    public void UpdateGravityBasedOnRotation(float rotationZ)
    {
        if (targetRb != null)
        {
            switch ((int)rotationZ)
            {
                case 0:
                    targetRb.gravityScale = -1; // 上向き
                    break;
                case 90:
                    targetRb.gravityScale = 0; // 右向き（無重力）
                    targetRb.velocity = new Vector2(3f, 0f); // 右に移動（仮）
                    break;
                case 180:
                    targetRb.gravityScale = 1; // 下向き（通常）
                    break;
                case 270:
                    targetRb.gravityScale = 0; // 左向き（無重力）
                    targetRb.velocity = new Vector2(-3f, 0f); // 左に移動（仮）
                    break;
            }
            Debug.Log("重力方向変更: " + targetRb.gravityScale);
        }
    }

    private void ResetGravity()
    {
        if (targetRb != null)
        {
            targetRb.gravityScale = 1; // 通常の重力に戻す
            isGravityFlipped = false;
        }
    }
}