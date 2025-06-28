using Unity.VisualScripting;
using UnityEngine;

public class GravityFlipPostIt : MonoBehaviour
{
    public float activationTime = 1.0f; // 発動までの時間
    private float touchTime = 0.0f;

    private Rigidbody2D targetRb;   // 貼る対象
    private Rigidbody2D gravityRb;  // 実際に重力操作するRB
    Enemy headEnemy = null;

    private bool isGravityFlipped = false;
    private float horizontalGravity = 9.8f; // X方向の重力

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Attachable") || other.CompareTag("Enemy"))
        {
            if (other.CompareTag("Enemy"))
            {
                BodyPart enemyBody = other.GetComponent<BodyPart>();
                headEnemy = enemyBody.head;
                if (headEnemy.isStunned) {
                    headEnemy = null;
                    return;
                }
                targetRb = other.GetComponent<Rigidbody2D>();
                gravityRb = headEnemy.GetComponent<Rigidbody2D>();
            }
            else
            {
                targetRb = other.GetComponent<Rigidbody2D>();
                gravityRb = targetRb;
            }

            if (targetRb != null)
            {
                transform.SetParent(targetRb.transform);
                Debug.Log($"targetRbを{targetRb.gameObject.name}に設定しました");
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if ((other.CompareTag("Player") || other.CompareTag("Enemy") || other.CompareTag("Attachable")) && targetRb != null)
        {
            if (!isGravityFlipped) // すでに発動済みなら処理しない
            {
                touchTime += Time.deltaTime; // プレイヤーが静止していても時間が進む
                if (touchTime >= activationTime)
                {
                    ActivateGravityFlip();
                }
            }
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (targetRb != null && other.transform.IsChildOf(targetRb.transform))
        {
            ResetGravity();

            // 安全に親を外す
            if (transform.parent != null)
            {
                transform.SetParent(null);
            }

            targetRb = null;
            gravityRb = null;
            touchTime = 0.0f;
            Debug.Log("反転解除");
        }
        else if (targetRb == null || !other.transform.IsChildOf(targetRb.transform))
        {
            Debug.Log("反転解除出来なかった！");
        }
      
    }

    private void ActivateGravityFlip()
    {
        if (gravityRb != null && headEnemy.isStunned == false)
        {
            isGravityFlipped = true;
            UpdateGravityBasedOnRotation(transform.eulerAngles.z);
        }
    }

    private void ResetGravity()
    {
        if (gravityRb != null)
        {
            gravityRb.gravityScale = 1.0f; // 重力を元に戻す
            gravityRb.velocity = Vector2.zero; ; // 速度をリセット
        }

        isGravityFlipped = false;
    }

    public void UpdateGravityBasedOnRotation(float rotationZ)
    {
        if (gravityRb == null) return;

        if (rotationZ == 0)
        {
            gravityRb.gravityScale = -1.0f; // 上方向に重力
        }
        else if (rotationZ == 90)
        {
            gravityRb.gravityScale = 0.0f; // 重力を無効化
        }
        else if (rotationZ == 180)
        {
            gravityRb.gravityScale = 1.0f; // 下方向（通常の重力）
        }
        else if (rotationZ == 270)
        {
            gravityRb.gravityScale = 0.0f; // 重力を無効化
        }

    }

    private void FixedUpdate()
    {
        if (gravityRb == null) return;

        float rotationZ = transform.eulerAngles.z;

        if (rotationZ == 90) // 左方向重力
        {
            gravityRb.AddForce(new Vector2(-horizontalGravity, 0), ForceMode2D.Force);
        }
        else if (rotationZ == 270) // 右方向重力
        {
            gravityRb.AddForce(new Vector2(horizontalGravity, 0), ForceMode2D.Force);
        }

    }

}
