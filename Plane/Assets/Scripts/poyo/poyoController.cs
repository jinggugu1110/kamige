using UnityEngine;

public class poyoController : MonoBehaviour
{
    public float moveSpeed = 2f;    //速度
    public float escapeRange = 5f;  //PostItから逃げる距離
    public float followRange = 10f; //プレイヤー追跡距離
    public Transform player;
    public BoxCollider2D spawnArea; // 紫の出現制限エリア（Trigger）

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // プレイヤーのTransformを取得
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("nonconflictPostIt");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }
        // spawnAreaの初期設定
        if (spawnArea == null)
        {
            GameObject areaObj = GameObject.Find("poyoiki"); //出現オブジェクト名
            if (areaObj != null)
            {
                spawnArea = areaObj.GetComponent<BoxCollider2D>();
            }
        }


        if (rb != null)
        {
            rb.freezeRotation = true;
            rb.gravityScale = 0f;
        }

        Physics2D.IgnoreLayerCollision(10, 2); //Enemy (10) と Ground (2)
    }

    public void SetSpawnArea(BoxCollider2D area)
    {
        spawnArea = area;
    }

    void Update()
    {
        if (rb.gravityScale == 1.0f)
        {
            rb.gravityScale = 0.0f; // 常に無重力
        }

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
            transform.position += (Vector3)(escapeDirection.normalized * moveSpeed * Time.deltaTime);
        }
        else if (player != null)
        {
            float playerDistance = Vector2.Distance(transform.position, player.position);
            if (playerDistance <= followRange)
            {
                Vector2 direction = (player.position - transform.position).normalized;
                transform.position += (Vector3)(direction * moveSpeed * Time.deltaTime);
            }
        }

        // 範囲外に出られないよう制限
        if (spawnArea != null)
        {
            transform.position = ClampToBounds(transform.position, spawnArea.bounds);
        }
    }

    private Vector3 ClampToBounds(Vector3 position, Bounds bounds)
    {
        float x = Mathf.Clamp(position.x, bounds.min.x, bounds.max.x);
        float y = Mathf.Clamp(position.y, bounds.min.y, bounds.max.y);
        return new Vector3(x, y, position.z);
    }
}
