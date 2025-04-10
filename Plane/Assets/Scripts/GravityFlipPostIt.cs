using UnityEngine;

public class GravityFlipPostIt : MonoBehaviour
{
    public float activationTime = 1.0f; // 発動までの時間
    private float touchTime = 0.0f;
    private Rigidbody2D targetRb;
    private bool isGravityFlipped = false;
    private float horizontalGravity = 9.8f; // X方向の重力

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Attachable"))
        {
            if (targetRb == null)
            {
                targetRb = other.GetComponent<Rigidbody2D>();
                if (targetRb != null)
                {
                    transform.SetParent(targetRb.transform);
                }
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if ((other.CompareTag("Player") || other.CompareTag("Attachable")) && targetRb != null)
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

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<Rigidbody2D>() == targetRb)
        {
            ResetGravity();
            transform.SetParent(null);
            targetRb = null;
            touchTime = 0.0f;
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

    private void ResetGravity()
    {
        if (targetRb != null)
        {
            targetRb.gravityScale = 1.0f; // 重力を元に戻す
            targetRb.velocity = Vector2.zero; // 速度をリセット
            isGravityFlipped = false;
            Debug.Log("重力反転解除");
        }
    }

    public void UpdateGravityBasedOnRotation(float rotationZ)
    {
        if (targetRb == null) return;

        if (rotationZ == 0)
        {
            targetRb.gravityScale = -1.0f; // 上方向に重力
        }
        else if (rotationZ == 90)
        {
            targetRb.gravityScale = 0.0f; // 重力を無効化
        }
        else if (rotationZ == 180)
        {
            targetRb.gravityScale = 1.0f; // 下方向（通常の重力）
        }
        else if (rotationZ == 270)
        {
            targetRb.gravityScale = 0.0f; // 重力を無効化
        }
    }

    private void FixedUpdate()
    {
        if (targetRb == null) return;

        float rotationZ = transform.eulerAngles.z;

        if (rotationZ == 90) // 左方向重力
        {
            targetRb.AddForce(new Vector2(-horizontalGravity, 0), ForceMode2D.Force);
        }
        else if (rotationZ == 270) // 右方向重力
        {
            targetRb.AddForce(new Vector2(horizontalGravity, 0), ForceMode2D.Force);
        }
    }
}
