using UnityEngine;

public class poyoController : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float detectionRange = 10f;
    public Transform player;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        //プレイヤー参照を取得設定されていなければプレイヤータグをもったオブジェクトを自動取得
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("nonconflictPostIt");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
            else
            {
                //プレイヤーが見つからない場合のコメント
                Debug.LogWarning("Player not found. Tag 'Player' must be set.");
            }
        }

        // 回転を固定しておく
        if (rb != null)
        {
            rb.freezeRotation = true;
        }

        // Layer制御 "Enemy" は Layer 10、"Ground" は Layer 7）
        Physics2D.IgnoreLayerCollision(10, 2); //"Enemy" と "Ground" の衝突を無視
    }

    void Update()
    {
        //プレイヤーがいるときに座標取得して追いかける処理
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= detectionRange)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            transform.position += (Vector3)(direction * moveSpeed * Time.deltaTime);
        }
    }
}

