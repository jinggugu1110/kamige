using UnityEngine;

public class poyoController : MonoBehaviour
{
    public float moveSpeed = 2f;    //速度
    public float escapeRange = 5f;  //PostItから逃げる距離
    public float followRange = 10f; //プレイヤー追跡距離
    public Transform player;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        //プレイヤーのTransformを取得
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("nonconflictPostIt");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }
        //ぽよんしーの初期設定
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
        //付箋を貼って剥がされたときに重力が適応されてぽよんしーが浮けなくなる問題の修正用
        if (rb.gravityScale == 1.0f)
        {
            rb.gravityScale = 0.0f; // 重力を無効化
        }

        //ぽよんしーがレイヤーから出られないようにするための処理
        //ぽよんしーだけが触れるレイヤーを設定
        //ぽよんしー以外は触れないようにする
        int poyolayer =LayerMask.NameToLayer("poyoArea");


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
            return; //逃げる時はプレイヤーを優先的には追跡しない
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



//// int layerMask = 1 << LayerMask.NameToLayer("TargetLayer");
//if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, Mathf.Infinity, layerMask))
//{
//    Debug.Log("Hit: " + hit.collider.name);
//}