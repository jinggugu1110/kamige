using UnityEngine;

public class poyoController : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float escapeRange = 5f; // PostItから逃げる距離
    public float followRange = 10f; // プレイヤー追跡距離
    public Transform player;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("nonconflictPostIt");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
            else
            {
                Debug.LogWarning("Player not found. Make sure to tag the player as 'Player'.");
            }
        }

        if (rb != null)
        {
            rb.freezeRotation = true;
            rb.gravityScale = 0f;
        }

        // 例：Enemy (Layer 10) と Ground (Layer 2) の衝突を無視
        Physics2D.IgnoreLayerCollision(10, 2);
    }

    void Update()
    {
        //PostItをチェックして逃げる方向を優先
        GameObject[] postIts = GameObject.FindGameObjectsWithTag("PostIt");
        Vector2 escapeDirection = Vector2.zero;

        foreach (GameObject postIt in postIts)
        {
            float distance = Vector2.Distance(transform.position, postIt.transform.position);
            if (distance <= escapeRange)
            {
                escapeDirection += (Vector2)(transform.position - postIt.transform.position).normalized;
            }
        }

        if (escapeDirection != Vector2.zero)
        {
            //PostItから逃げる処理
            transform.position += (Vector3)(escapeDirection.normalized * moveSpeed * Time.deltaTime);
            return; //逃げる時はプレイヤーを追跡しない
        }

        //通常時プレイヤーを追いかける処理
        if (player != null)
        {
            float playerDistance = Vector2.Distance(transform.position, player.position);
            if (playerDistance <= followRange)
            {
                Vector2 direction = (player.position - transform.position).normalized;
                transform.position += (Vector3)(direction * moveSpeed * Time.deltaTime);
            }
        }
    }
}
