using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

/*
 移動処理は鎖で引っ張ってるイメージです

 */

/*課題　めも
 * 体をリストで追加できるようにする
 * 体の左右反転処理いれる
 * 重力フセンの影響をウケてるときに、そのフセンがしっぽに当たると気絶してしまう
 ⇒前はTailTriggerで気絶処理を読んでたが、影響をウケている最中なのか判定できないため、
あらたにPostItTriggerクラスを作って気絶処理を読んでみたが失敗。惜しい？かも。

次試すこと
istakeGravity bool変数を各体につけて、フセンからtrueにする。⇒TailTrigger内でrootでistakeGravityがtrueか探索して trueなら無効にする？

 */


public class Enemy : MonoBehaviour
{
    [Header("移動設定")]
    public float moveSpeed = 2f; // 敵の移動速度

    [Header("壁検知設定")]
    public Transform wallCheckPoint;         // Rayの発射位置（敵の前方に配置）
    public float wallCheckDistance = 0.6f;   // Rayの距離
    public LayerMask wallLayer;              // 壁レイヤー

    //敵の体
    private Rigidbody2D rb;
    private Collider2D HeadCol;//当たり判定を避けさせるため、頭と体を分ける

    private bool movingRight = true;//進む方向
    private int bodyCount = 4;
    public GameObject BodyPrefab;
   // private List<GameObject> BodyParts = new List<GameObject>();
    public List<GameObject> BodyParts = new List<GameObject>(); // インスペクターで手動追加

    private Collider2D tailCollider;
    private List<Vector3> PositionHistory = new List<Vector3>();
    private float Gap;//体の感覚

    [Header("気絶設定")]
    public float stunTime = 3.0f;        // 気絶している時間
    float stretchAnimSpeed = 0.1f;        // 伸びるアニメーションの速さ
    private List<TriggerChecker> bodyTriggers = new List<TriggerChecker>();


    // 気絶関連の変数
    private bool isStunned = false;          // 気絶中か
    private bool isStretchingBody = false;   // 体を伸ばすアニメーション中か
    private SpriteRenderer[] bodyRenderers;
    private Coroutine stunCoroutine;
    private bool facingRight => movingRight; // 右向きかどうか（気絶アニメーション用）
    bool isUnconscious = false;

    // 気絶から回復時の体のスムーズな移動のための変数
    private Vector2[] targetBodyPositions; // 目標となる体の位置
    private float recoveryTransitionTime = 1.0f; // 回復時の移動にかける時間
    private bool isRecovering = false; // 回復中のフラグ

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        HeadCol = GetComponent<Collider2D>();//衝突無効化のため

        //bodyの1/3が重なる
        float bodyWidth = BodyPrefab.transform.localScale.x;
        float stepPerFrame = moveSpeed * Time.fixedDeltaTime;
         Gap = Mathf.RoundToInt((bodyWidth * (4f / 5f)) / stepPerFrame);
        //Gap = Mathf.RoundToInt((bodyWidth * 0.7f) / stepPerFrame);

        for (int i = 0; i < bodyCount; i++)
        {
            GrowBody();
        }

        TailTrigger tailScript = BodyParts[BodyParts.Count - 1].AddComponent<TailTrigger>();
        tailScript.enemy = this;

        // すべての体パーツのスプライトレンダラーを取得
        bodyRenderers = new SpriteRenderer[BodyParts.Count + 1]; // 頭 + 体パーツ
        bodyRenderers[0] = GetComponent<SpriteRenderer>(); //[0]は頭
        for (int i = 0; i < BodyParts.Count; i++)
        {
            bodyRenderers[i + 1] = BodyParts[i].GetComponent<SpriteRenderer>();
        }

        // 位置履歴の初期化
        for (int i = 0; i < 100; i++) // historySize = 100と仮定
        {
            PositionHistory.Add(transform.position);
        }

        // 目標位置配列の初期化
        targetBodyPositions = new Vector2[BodyParts.Count];
    }

    private void FixedUpdate()
    {
        if (!isStunned && !isRecovering)
        {
            Move();
            CheckWall();
        }
        else if (isRecovering)
        {
            rb.velocity = Vector2.zero;
            PositionHistory.Insert(0, transform.position);
            if (PositionHistory.Count > 100)
            {
                PositionHistory.RemoveAt(PositionHistory.Count - 1);
            }
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

    // 回復後の体の位置を保持する変数
    private bool useRecoveryPositions = false;
    private Vector2[] recoveredBodyPositions;

    private void Move()
    {
        // 移動方向の設定
        float moveDir = movingRight ? 1f : -1f;
        rb.velocity = new Vector2(moveDir * moveSpeed, rb.velocity.y);

        // 位置履歴の管理
        PositionHistory.Insert(0, transform.position);
        if (PositionHistory.Count > 100) // 履歴の最大数を超えたら古いものを削除
        {
            PositionHistory.RemoveAt(PositionHistory.Count - 1);
        }

        // Bodyを動かす
        if (!isStretchingBody)
        {
            // 回復直後は回復時の体の位置を使用
            if (useRecoveryPositions && recoveredBodyPositions != null)
            {
                for (int i = 0; i < BodyParts.Count; i++)
                {
                    if (i < recoveredBodyPositions.Length)
                    {
                        BodyParts[i].transform.position = recoveredBodyPositions[i];
                    }
                }

                // 数フレーム後に通常の移動に戻す（急なリセットを防ぐ）
                StartCoroutine(ResetRecoveryPositionsAfterDelay(10));
            }
            else
            {
                // 通常の体パーツ移動
                MoveBodyParts();
            }
        }
    }

    // 一定フレーム後に回復位置のフラグをリセットする
    private IEnumerator ResetRecoveryPositionsAfterDelay(int frames)
    {
        // 指定フレーム数待機
        for (int i = 0; i < frames; i++)
        {
            yield return null;
        }

        // 履歴のギャップを埋めるために位置履歴を更新
        // これにより、通常移動に戻った時にスムーズに移行できる
        for (int i = 0; i < BodyParts.Count; i++)
        {
            int targetIndex = (int)Mathf.Min(i * Gap, PositionHistory.Count - 1);
            // 回復後の位置が通常の履歴の位置になるように調整
            PositionHistory[targetIndex] = BodyParts[i].transform.position;
        }

        useRecoveryPositions = false;
    }

    // ボディパーツの移動を別メソッドに分離
    private void MoveBodyParts()
    {
        // 配列の境界チェック用の最大インデックス
        int maxHistoryIndex = PositionHistory.Count - 1;

        for (int i = 0; i < BodyParts.Count; i++)
        {
            int historyIndex = (int)Mathf.Min((i + 1) * Gap, maxHistoryIndex);
            Vector3 targetPosition = PositionHistory[historyIndex];

            // スムーズな移動を実装
            float smoothFactor = 15f; //スムーズさ係数
            BodyParts[i].transform.position = Vector3.Lerp(
                BodyParts[i].transform.position,
                targetPosition,
                Time.deltaTime * smoothFactor
            );
        }

        //float maxDistance = 0.6f; // セグメント間の最大距離
        //float minDistance = 0.1f; // セグメント間の最小距離
        //Vector3 prevPos = transform.position; // 頭の位置

        //for (int i = 0; i < BodyParts.Count; i++)
        //{
        //    Vector3 curPos = BodyParts[i].transform.position;
        //    Vector3 delta = curPos - prevPos;
        //    float dist = delta.magnitude;

        //    if (dist > maxDistance || dist < minDistance)
        //    {
        //        float clampedDist = Mathf.Clamp(dist, minDistance, maxDistance);
        //        Vector3 dir = delta.normalized;
        //        Vector3 targetPos = prevPos + dir * clampedDist;
        //        BodyParts[i].transform.position = Vector3.Lerp(curPos, targetPos, 0.5f); // 少しだけ引っ張る感じ
        //    }

        //    prevPos = BodyParts[i].transform.position;
        //}
    }

    // ======= 壁検知 =======
    private bool IsTouchingWall()
    {
        Vector2 direction = movingRight ? Vector2.right : Vector2.left;
        RaycastHit2D hit = Physics2D.Raycast(wallCheckPoint.position, direction, wallCheckDistance, wallLayer);
        return hit.collider != null;
    }

    private void CheckWall()
    {
        if (IsTouchingWall())
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
        //GameObject body = Instantiate(BodyPrefab);
        //BodyParts.Add(body);//リストに追加

        //// 衝突を無効化（自分の頭とのみ）
        //Collider2D bodyCol = body.GetComponent<Collider2D>();
        //if (HeadCol != null && bodyCol != null)
        //{
        //    // bodyCol.isTrigger = (BodyParts.Count == bodyCount);//()条件文で,最後尻だけtrueにする
        //    Physics2D.IgnoreCollision(HeadCol, bodyCol);
        //}


        foreach (GameObject bodyPart in BodyParts)
        {
            if (bodyPart == null) continue;

            Collider2D bodyCollider = bodyPart.GetComponent<Collider2D>();
            if (bodyCollider != null)
            {
                Physics2D.IgnoreCollision(HeadCol, bodyCollider);
            }
        }
    }

    // ======= 気絶処理 =======
    public void OnTailHit()
    {
        Debug.Log("Enemy気絶処理");

        if (isStunned) return;

        StartStun(BodyParts.Count - 1);
    }

    // 気絶処理
    private void StartStun(int hitPartIndex)
    {
        isStunned = true;
        isStretchingBody = true;

        // 前の気絶処理を停止
        if (stunCoroutine != null)
        {
            StopCoroutine(stunCoroutine);
        }

        // 気絶アニメーション開始
        stunCoroutine = StartCoroutine(StunAnimation(hitPartIndex));
    }

    // 気絶アニメーション
    private IEnumerator StunAnimation(int hitPartIndex)
    {
        Vector2 headOriginalPosition = transform.position;
        Vector2[] originalPositions = new Vector2[BodyParts.Count];
        for (int i = 0; i < BodyParts.Count; i++)
        {
            originalPositions[i] = BodyParts[i].transform.position;//伸びる前のposを一旦保存
        }

        float stretchTime = 0f;
        while (stretchTime < stretchAnimSpeed)
        {
            if (IsTouchingWall())
            {
                Debug.Log("頭が壁に当たったのでストレッチ中断");
                // アニメーションループを抜けて気絶状態にする
                isUnconscious = true;
                // 現在の位置でアニメーションを停止
                break;
            }

            stretchTime += Time.deltaTime;
            float t = stretchTime / stretchAnimSpeed;

            // 頭伸ばす
            Vector2 stretchDirection = facingRight ? Vector2.right : Vector2.left;
            float headStretchLength = 0.3f * (hitPartIndex + 1);
            Vector2 headTargetPosition = headOriginalPosition + stretchDirection * headStretchLength * t;
            transform.position = headTargetPosition;

            // ヒットした部分から頭方向に向かって順に伸ばす
            for (int i = hitPartIndex; i >= 0; i--)
            {
                // 伸ばす方向（頭の向きに合わせる）
                // 頭からの距離に応じて伸ばす長さを調整
                float stretchLength = 0.25f * (hitPartIndex - i + 1);
                Vector2 targetPosition = originalPositions[i] + stretchDirection * stretchLength * t;
                BodyParts[i].transform.position = targetPosition;
            }

            yield return null;
        }

        // 回復待機
        yield return new WaitForSeconds(stunTime);

        // 回復処理を開始
        StartCoroutine(RecoverFromStun());
    }

    // 気絶から回復する処理
    // RecoverFromStunメソッドも修正する必要があります
    private IEnumerator RecoverFromStun()
    {
        // 回復中フラグをセット
        isRecovering = true;

        // 現在の頭の位置を記録（回復中は頭を固定するため）
        Vector2 headPosition = transform.position;

        // 現在の体の各パーツの位置を記録
        Vector2[] currentBodyPositions = new Vector2[BodyParts.Count];
        for (int i = 0; i < BodyParts.Count; i++)
        {
            currentBodyPositions[i] = BodyParts[i].transform.position;
        }

        // 理想的な体の位置を計算する
        // 頭の位置を基準にして、通常のGapを使って理想的な体の位置を計算
        targetBodyPositions = new Vector2[BodyParts.Count];
        Vector2 direction = movingRight ? Vector2.right : Vector2.left;

        for (int i = 0; i < BodyParts.Count; i++)
        {
            // 頭からの距離に基づいて理想的な位置を計算
            float distance = (i + 1) * Gap * Time.fixedDeltaTime * moveSpeed;
            targetBodyPositions[i] = (Vector2)headPosition - direction * distance;
        }

        // 尻尾から頭に向かって順番に縮める（尻尾から頭に向かって回復）
        // 各パーツの回復にかける時間
        float partRecoveryTime = recoveryTransitionTime / 2f; // 全体の回復時間の半分を使用

        // 最後尻尾のパーツから順番に処理
        for (int partIndex = BodyParts.Count - 1; partIndex >= 0; partIndex--)
        {
            float startTime = Time.time;
            Vector2 startPos = BodyParts[partIndex].transform.position;

            // 目標位置を設定（自分の前のパーツに追従、または頭に追従）
            Vector2 targetPos;
            if (partIndex > 0)
            {
                // 自分の前のパーツ（インデックスが小さいほど頭に近い）に向かって縮む
                targetPos = targetBodyPositions[partIndex];
            }
            else
            {
                // 最初のパーツは頭に向かって縮む
                targetPos = targetBodyPositions[0];
            }

            // このパーツの縮むアニメーション
            while (Time.time - startTime < partRecoveryTime)
            {
                float t = (Time.time - startTime) / partRecoveryTime;
                t = Mathf.SmoothStep(0, 1, t); // イージング

                // 頭は固定
                rb.velocity = Vector2.zero;

                // パーツを徐々に目標位置に移動
                BodyParts[partIndex].transform.position = Vector2.Lerp(startPos, targetPos, t);

                // 前のパーツ（頭に近いパーツ）は既に移動済みなので、位置を維持
                for (int i = partIndex - 1; i >= 0; i--)
                {
                    BodyParts[i].transform.position = targetBodyPositions[i];
                }

                yield return null;
            }

            // このパーツの最終位置を確定
            BodyParts[partIndex].transform.position = targetPos;
        }

        // 全パーツが目標位置に到達した後、少し待機（オプション）
        yield return new WaitForSeconds(0.2f);

        // 回復後の体の位置を保存
        recoveredBodyPositions = new Vector2[BodyParts.Count];
        for (int i = 0; i < BodyParts.Count; i++)
        {
            recoveredBodyPositions[i] = BodyParts[i].transform.position;
        }

        // Move()で回復後の位置を使用するようフラグを設定
        useRecoveryPositions = true;

        // 位置履歴を更新（頭は動いていないが、履歴は正しく維持する必要がある）
        for (int i = 0; i < 10; i++) // 数フレーム分の履歴を更新
        {
            PositionHistory.Insert(0, transform.position);
            if (PositionHistory.Count > 100)
            {
                PositionHistory.RemoveAt(PositionHistory.Count - 1);
            }
            yield return null;
        }

        // 完全に回復
        isStunned = false;
        isStretchingBody = false;
        isRecovering = false;
        isUnconscious = false;

        // 通常の移動を再開（回復した体の位置を保持する）
        float moveDir = movingRight ? 1f : -1f;
        rb.velocity = new Vector2(moveDir * moveSpeed, rb.velocity.y);
    }
}