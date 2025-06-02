using UnityEngine;

public class PoyoController : MonoBehaviour
{
    public Transform player;               // プレイヤーのTransform
    public Transform graveOrigin;          // 墓のTransform
    public float appearRange = 5f;         // 出現する範囲
    public float disappearRange = 6f;      // 消える範囲（appear より少し大きく）
    public float moveSpeed = 2f;           // 追跡速度
    public float maxChaseDistance = 5f;    // 墓からどこまで追えるか

    private Rigidbody2D rb;
    private bool isActive = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        SetActive(false); // 最初は非表示
    }

    void Update()
    {
        float distanceToGrave = Vector2.Distance(player.position, graveOrigin.position);

        if (!isActive && distanceToGrave <= appearRange)
        {
            SetActive(true);
        }
        else if (isActive && distanceToGrave > disappearRange)
        {
            SetActive(false);
        }

        if (isActive)
        {
            float distanceFromGrave = Vector2.Distance(transform.position, graveOrigin.position);
            if (distanceFromGrave <= maxChaseDistance)
            {
                Vector2 direction = (player.position - transform.position).normalized;
                rb.velocity = direction * moveSpeed;
            }
            else
            {
                rb.velocity = Vector2.zero;
            }
        }
    }


    private void SetActive(bool value)
    {
        isActive = value;
        gameObject.SetActive(value);
    }
}
