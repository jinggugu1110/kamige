//using UnityEngine;

//public class LockObjectPostit : MonoBehaviour
//{
//    private Rigidbody2D targetRb;
//    private bool isAttached = false;

//    private void OnTriggerEnter2D(Collider2D other)
//    {
//        if (!isAttached && IsValidTarget(other))
//        {
//            targetRb = other.GetComponent<Rigidbody2D>();
//            if (targetRb != null)
//            {
//                // 物理移動を止める
//                targetRb.bodyType = RigidbodyType2D.Kinematic;

//                // 対象に付箋を貼る（子にする）
//                transform.SetParent(other.transform);
//                isAttached = true;
//            }
//        }
//    }

//    private void OnTriggerExit2D(Collider2D other)
//    {
//        if (isAttached && other.GetComponent<Rigidbody2D>() == targetRb)
//        {
//            // 動きを元に戻す
//            targetRb.bodyType = RigidbodyType2D.Dynamic;

//            // 親から切り離す
//            transform.SetParent(null);
//            isAttached = false;
//            targetRb = null;
//        }
//    }

//    private bool IsValidTarget(Collider2D collider)
//    {
//        return collider.CompareTag("Player") ||
//               collider.CompareTag("Attachable") ||
//               collider.CompareTag("Enemy") ||
//               collider.CompareTag("Poyo");
//    }
//}

using UnityEngine;

public class LockObjectPostit : MonoBehaviour
{
    private Rigidbody2D targetRb;
    private bool isAttached = false;

    // チェック対象タグ一覧（※ユーザー指定の正確なタグ名）
    private readonly string[] validTags = { "Player", "Enemy", "Attachable", "Poyo" };

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isAttached && IsValidTarget(other))
        {
            targetRb = other.GetComponent<Rigidbody2D>();
            if (targetRb != null)
            {
                // 動かないように完全に固定
                targetRb.bodyType = RigidbodyType2D.Dynamic;
                targetRb.constraints = RigidbodyConstraints2D.FreezeAll;

                // 対象に付箋を貼る（子にする）
                transform.SetParent(other.transform);
                isAttached = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (isAttached && other.GetComponent<Rigidbody2D>() == targetRb)
        {
            // 動きを元に戻す
            targetRb.constraints = RigidbodyConstraints2D.FreezeRotation;

            // 親から切り離す
            transform.SetParent(null);
            isAttached = false;
            targetRb = null;
        }
    }

    private bool IsValidTarget(Collider2D collider)
    {
        foreach (var tag in validTags)
        {
            if (collider.CompareTag(tag)) return true;
        }
        return false;
    }
}
