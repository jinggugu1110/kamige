using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*課題！
 * 回復⇒通常でかくつく
 * 頭と体が重なってる
 */

/*
 * Bodypos[]をつくり、頭と体の位置を管理する。
「気絶」 
しっぽから順に伸びる
頭の位置をBodypos[]に格納
「回復」
頭の位置を基準にGapを利用して、通常の体位置を計算してもとに戻る。
完了したら通常の移動を始める
「通常」
頭を移動する。
頭の位置を基準にGapを利用して、通常の体位置を計算して体の位置を移動する。

 */
public class Enemy2 : MonoBehaviour
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
    public int bodyCount = 4;
    public GameObject BodyPrefab;
    private List<GameObject> BodyParts = new List<GameObject>();
    private Collider2D tailCollider;
    private List<Vector3> PositionHistory = new List<Vector3>();
    private Vector3[] bodyPositions;

    private float Gap;//体の感覚

    [Header("気絶設定")]
    public float stunTime = 3.0f;        // 気絶している時間
    float stretchAnimSpeed = 0.1f;        // 伸びるアニメーションの速さ

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

        TailTrigger2 tailScript = BodyParts[BodyParts.Count - 1].AddComponent<TailTrigger2>();
        tailScript.enemy = this;

        // すべての体パーツのスプライトレンダラーを取得
        bodyRenderers = new SpriteRenderer[BodyParts.Count + 1]; // 頭 + 体パーツ
        bodyRenderers[0] = GetComponent<SpriteRenderer>(); //[0]は頭
        for (int i = 0; i < BodyParts.Count; i++)
        {
            bodyRenderers[i + 1] = BodyParts[i].GetComponent<SpriteRenderer>();
        }

        bodyPositions = new Vector3[bodyCount + 1];
        for (int i = 0; i < bodyPositions.Length; i++)
        {
            bodyPositions[i] = transform.position;
        }

        // 目標位置配列の初期化
        targetBodyPositions = new Vector2[BodyParts.Count];
    }

    private void FixedUpdate()
    {
        if (!isStunned && !isRecovering)
        {
            // 通常の動作時（スタンや回復中でない）
            Move();
            CheckWall();
        }
        else if (isRecovering)
        {
            // 回復中は頭は固定したまま、体のパーツはRecoverFromStun内で移動
            // 頭の移動を無効化
            rb.velocity = Vector2.zero;

            // 位置履歴は更新する（RecoverFromStun内でも更新しているが、念のため）
            PositionHistory.Insert(0, transform.position);
            if (PositionHistory.Count > 100)
            {
                PositionHistory.RemoveAt(PositionHistory.Count - 1);
            }
        }
        else
        {
            // 気絶中は停止
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

    // ボディパーツの移動を別メソッドに分離
    private void MoveBodyParts()
    {
        // 配列の境界チェック用の最大インデックス
        int maxHistoryIndex = PositionHistory.Count - 1;

        for (int i = 0; i < BodyParts.Count; i++)
        {
            // 安全なインデックス計算
            int historyIndex = (int)Mathf.Min(i * Gap, maxHistoryIndex);
            Vector3 targetPosition = PositionHistory[historyIndex];

            // スムーズな移動を実装
            float smoothFactor = 15f; // 調整可能なスムーズさ係数
            BodyParts[i].transform.position = Vector3.Lerp(
                BodyParts[i].transform.position,
                targetPosition,
                Time.deltaTime * smoothFactor
            );
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
        GameObject body = Instantiate(BodyPrefab);
        BodyParts.Add(body);//リストに追加

        // 衝突を無効化（自分の頭とのみ）
        Collider2D bodyCol = body.GetComponent<Collider2D>();
        if (HeadCol != null && bodyCol != null)
        {
            // bodyCol.isTrigger = (BodyParts.Count == bodyCount);//()条件文で,最後尻だけtrueにする
            Physics2D.IgnoreCollision(HeadCol, bodyCol);
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
            float headStretchLength = 0.25f * (hitPartIndex + 1);
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

        // 理想的な体の位置を計算する（Gap間隔で等間隔に並ぶ）
        targetBodyPositions = new Vector2[BodyParts.Count];
        Vector2 direction = movingRight ? Vector2.left : Vector2.right; // 復帰時は逆方向に縮む

        for (int i = 0; i < BodyParts.Count; i++)
        {
            float distance = (i + 1) * Gap * Time.fixedDeltaTime * moveSpeed;
            targetBodyPositions[i] = headPosition + direction * distance;
        }

        // 各パーツを滑らかに移動させる（同時に）
        float elapsed = 0f;
        while (elapsed < recoveryTransitionTime)
        {
            float t = elapsed / recoveryTransitionTime;
            t = Mathf.SmoothStep(0, 1, t); // イージング補間

            for (int i = 0; i < BodyParts.Count; i++)
            {
                Vector2 start = currentBodyPositions[i];
                Vector2 end = targetBodyPositions[i];
                BodyParts[i].transform.position = Vector2.Lerp(start, end, t);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        // 最終位置を補正
        for (int i = 0; i < BodyParts.Count; i++)
        {
            BodyParts[i].transform.position = targetBodyPositions[i];
        }

        // 回復後、直後は通常移動に戻すと不自然になるので補間を入れる
        recoveredBodyPositions = new Vector2[BodyParts.Count];
        for (int i = 0; i < BodyParts.Count; i++)
        {
            recoveredBodyPositions[i] = BodyParts[i].transform.position;
        }
        useRecoveryPositions = true;

        // フラグリセット
        isRecovering = false;
        isStunned = false;
        isStretchingBody = false;
        isUnconscious = false;
    }

}
