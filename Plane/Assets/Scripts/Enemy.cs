using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

/*
バグ
3ワープにふれたら消える

やること
2しっぽにフセンがついたらピンと伸びて気絶
1ピンと伸びる

*/

public class Enemy : MonoBehaviour
{
    [Header("移動設定")]
    public float moveSpeed = 2f; // 敵の移動速度

    [Header("壁検知設定")]
    public Transform wallCheckPoint;         // Rayの発射位置（敵の前方に配置）
    public float wallCheckDistance = 0.5f;   // Rayの距離
    public LayerMask wallLayer;              // 壁レイヤー

    //敵の体
    private Rigidbody2D rb;
    private Collider2D HeadCol;//当たり判定を避けさせるため、頭と体を分ける

    private bool movingRight = true;//進む方向
    public int bodyCount = 4;
    public GameObject BodyPrefab;
    private List<GameObject> BodyParts = new List<GameObject>();
    private List<Vector3> PositionHistory = new List<Vector3>();
    public int Gap;

    public bool isTail = false; // しっぽかどうかフラグ


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        HeadCol = GetComponent<Collider2D>(); // 自分のCollider取得

        //bodyの1/3が重なる
        float bodyWidth = BodyPrefab.transform.localScale.x;
        float stepPerFrame = moveSpeed * Time.fixedDeltaTime;
        Gap = Mathf.RoundToInt((bodyWidth * (2f / 3f)) / stepPerFrame);

        for (int i = 0; i < bodyCount; i++)
        {
            GrowBody();
        }
    }

    private void FixedUpdate()
    {
        Move();
        CheckWall();
    }

    private void Move()
    {
        float moveDir = movingRight ? 1f : -1f;
        rb.velocity = new Vector2(moveDir * moveSpeed, rb.velocity.y);

        //新しい位置を保存
        PositionHistory.Insert(0, transform.position);

        //Bodyを動かす
        int index = 0;
        foreach (var body in BodyParts)
        {
            Vector3 point = PositionHistory[Mathf.Min(index * Gap, PositionHistory.Count - 1)];
            body.transform.position = point;
            index++;
        }

    }

    // ======= 壁検知 =======
    private void CheckWall()
    {
        Vector2 direction = movingRight ? Vector2.right : Vector2.left;
        RaycastHit2D hit = Physics2D.Raycast(wallCheckPoint.position, direction, wallCheckDistance, wallLayer);

        if (hit.collider != null)
        {
            Flip();
        }
    }

    // ======= 向き反転 =======
    private void Flip()
    {
        movingRight = !movingRight;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    // ======= デバッグ可視化 =======
    private void OnDrawGizmos()
    {
        if (wallCheckPoint == null) return;

        Gizmos.color = Color.red;
        Vector3 direction = (movingRight ? Vector3.right : Vector3.left) * wallCheckDistance;
        Gizmos.DrawLine(wallCheckPoint.position, wallCheckPoint.position + direction);
    }

    // ======= 体を生成 =======
    private void GrowBody()
    {
        GameObject body = Instantiate(BodyPrefab);
        BodyParts.Add(body);//リストに追加

        // 衝突を無効化（自分の頭とのみ）
        Collider2D bodyCol = body.GetComponent<Collider2D>();
        if (HeadCol != null && bodyCol != null)
        {
           // bodyCol.isTrigger = (BodyParts.Count == bodyCount);//()条件文で,最後尾だけtrueにする
            Physics2D.IgnoreCollision(HeadCol, bodyCol);
        }
    }

    //private void OnTriggerEnter2D(Collider2D other)
    //{
    //    //if (!enabled) return; // 無効化時は無視

    //    if (other.CompareTag("PostIt"))
    //    {
    //        // 最後尾の Body だった場合のみ実行
    //        Collider2D tailCollider = BodyParts[BodyParts.Count - 1].GetComponent<Collider2D>();
    //        if (other == tailCollider)
    //        {
    //            Debug.Log("しっぽにフセンが触れた！");
    //            // TODO: ピンと伸びる処理・気絶処理など
    //        }
    //    }
    //}

}
