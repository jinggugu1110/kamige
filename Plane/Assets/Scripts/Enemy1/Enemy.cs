using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/*課題 
 * 移動処理　
 * フレームじゃなくて、各体に「前の体からどれくらい離すか」の変数？を持たせて、毎フレーム更新する。

体はキネマティックなのでそもそもrb.velocityは使えない！！

 * 反転処理めも
自前vector作る
oldposとposもつlistを作る
関数内で更新する
!=したら　反転する

 */

public class Enemy : MonoBehaviour
{

    [Header("壁検知設定")]
    public Transform wallCheckPoint;         // 敵に頭につけること。
    public float wallCheckDistance = 0.6f;
    public LayerMask wallLayer;

    [Header("敵１の体")]
    public float moveSpeed = 2f;
    public List<GameObject> BodyParts = new List<GameObject>();

    private Rigidbody2D rb;
    [HideInInspector] public Collider2D HeadCol;//当たり判定を避けさせるため 頭専用
    private bool movingRight = true;

    private List<Vector3> PositionHistory = new List<Vector3>();
    private float Gap;      //体の幅
    private float rayLen;   //天井と地面判定のレイの長さ

    [Header("気絶設定")]
    public float stunTime = 3.0f;        // 気絶時間
    float stretchAnimSpeed = 0.1f;       // 回復animの速さ

    [HideInInspector] public bool isStunned = false;          // 気絶中か 他のコード内でも使う。
    [HideInInspector] public bool isGravityFlipped = false;
    private bool isStretchingBody = false;   // 体を伸ばすアニメーション中か
    private SpriteRenderer[] bodyRenderers;
    private Coroutine stunCoroutine;

    //折り返しで気絶しないための。
    [HideInInspector] public bool waitStun = false;
    [HideInInspector] public bool isGrounded = true;//空中にいないtrue/空中にいるfalse
    private float waitStunDuration = 2.0f;
    private float Timer_waitStun = 0f;

    //回復⇒通常
    private Vector2[] targetBodyPositions;
    private float recoveryTransitionTime = 1.0f;
    [HideInInspector] public bool isRecovering = false; // 回復中


    //画像反転 vectorが使えないので、各体の方向をクラスで管理する
    class E_pos
    {
        public float oldpos;
        public float curpos;
        public bool facingRight = true;
        public bool previousFacingRight = true; // 前フレームの向きを記録

        // 反転判定用の閾値（微細な揺れを無視するため）
        public float threshold = 0.01f;

        public bool HasFlipped()
        {
            return facingRight != previousFacingRight;
        }

        public void UpdateFacing()
        {
            previousFacingRight = facingRight;

            // 位置の変化量が閾値を超えた場合のみ更新
            float deltaPos = curpos - oldpos;
            if (Mathf.Abs(deltaPos) > threshold)
            {
                facingRight = deltaPos > 0;
            }
        }
    }

    List<E_pos> BodyVec = new List<E_pos>();

    private void Start()
    {
        wallLayer = LayerMask.GetMask("Ground", "Grass", "PlayerJump", "Ignore Raycast");

        rb = GetComponent<Rigidbody2D>();
        HeadCol = GetComponent<Collider2D>();//衝突無効化のため
        rayLen = this.transform.localScale.y/2 + 0.3f;//足場確認
        rb = GetComponent<Rigidbody2D>();

        float bodyWidth = this.transform.localScale.x;
        float stepPerFrame = moveSpeed * Time.fixedDeltaTime;
        //Gap = Mathf.RoundToInt((bodyWidth * (4f / 5f)) / stepPerFrame);
        Gap = Mathf.RoundToInt(bodyWidth/ stepPerFrame);

        //BodyVec = new List<E_pos>();

        if (BodyParts.Count > 0)
        {
            for (int i = 0; i < BodyParts.Count; i++)
            {
                if (BodyParts[i] != null)
                {
                    IgnoreBody();

                    E_pos bodyPos = new E_pos();
                    bodyPos.oldpos = BodyParts[i].transform.position.x;
                    bodyPos.curpos = BodyParts[i].transform.position.x;
                    bodyPos.facingRight = movingRight; //初期状態は頭と同じ向き
                    bodyPos.previousFacingRight = movingRight;
                    BodyVec.Add(bodyPos);
                }
            }
        }
        else
        {
            Debug.LogWarning("No BodyParts assigned in inspector!");
        }

        bodyRenderers = new SpriteRenderer[BodyParts.Count + 1]; // 頭 + 体パーツ
        bodyRenderers[0] = GetComponent<SpriteRenderer>(); //[0]は頭
        for (int i = 0; i < BodyParts.Count; i++)
        {
            if (BodyParts[i] != null)
            {
                bodyRenderers[i + 1] = BodyParts[i].GetComponent<SpriteRenderer>();
            }
        }

        // 位置履歴の初期化
        for (int i = 0; i < 100; i++)
        {
            PositionHistory.Add(transform.position);
        }

        targetBodyPositions = new Vector2[BodyParts.Count];
    }

    private void FixedUpdate()
    {
        if (!isStunned && !isRecovering)
        {
            Move();

            if (CheckWall())
            {

                StartWaitStun();
            }
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

        //折り返したとき少し待ってからwaitStunをfalseにする
        UpdateWaitStun();
        isGrounded = checkCanStun();

        //画像反転
        UpdateBodyFlipping();

        Debug.DrawRay(transform.position, Vector2.up * rayLen, Color.red);
        Debug.DrawRay(BodyParts[BodyParts.Count - 1].transform.position, Vector2.up * rayLen, Color.red);
        Debug.DrawRay(transform.position, Vector2.down * rayLen, Color.blue);
        Debug.DrawRay(BodyParts[BodyParts.Count - 1].transform.position, Vector2.down * rayLen, Color.blue);

    }

    private void UpdateBodyFlipping()
    {
        for (int i = 0; i < BodyParts.Count; i++)
        {
            if (i < BodyVec.Count)
            {
                BodyVec[i].curpos = BodyParts[i].transform.position.x;
                BodyVec[i].UpdateFacing();

                if (BodyVec[i].HasFlipped())
                {
                    FlipBodyPart(i);
                }

                //3フレームごとにoldposを更新
                if (Time.fixedTime % (Time.fixedDeltaTime * 3) < Time.fixedDeltaTime)
                {
                    BodyVec[i].oldpos = BodyVec[i].curpos;
                }
            }
        }
    }
    private void FlipBodyPart(int bodyIndex)
    {
        if (bodyIndex >= 0 && bodyIndex < BodyParts.Count)
        {
            GameObject bodyPart = BodyParts[bodyIndex];
            Vector3 scale = bodyPart.transform.localScale;
            scale.x *= -1;
            bodyPart.transform.localScale = scale;
        }
    }

    // 回復後の体の位置を保持する変数
    private bool useRecoveryPositions = false;
    private Vector2[] recoveredBodyPositions;
    public bool checkCanStun()
    {
        bool headUpHit = Physics2D.Raycast(transform.position, Vector2.up, rayLen, wallLayer);
        bool tailUpHit = Physics2D.Raycast(BodyParts[BodyParts.Count - 1].transform.position, Vector2.up, rayLen, wallLayer);
        bool headDownHit = Physics2D.Raycast(transform.position, Vector2.down, rayLen, wallLayer);
        bool tailDownHit = Physics2D.Raycast(BodyParts[BodyParts.Count - 1].transform.position, Vector2.down, rayLen, wallLayer);

        if (GetComponentInChildren<GravityFlipPostIt>() != null)
        {
            return false;
        }

        if (headUpHit && tailUpHit || headDownHit && tailDownHit)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void StartWaitStun()
    {
        waitStun = true;
        Timer_waitStun = waitStunDuration;
    }

    private void UpdateWaitStun()
    {
        if (waitStun)
        {
            Timer_waitStun -= Time.fixedDeltaTime;
            if (Timer_waitStun <= 0f)
            {
                waitStun = false;
            }
        }
    }

    private void Move()
    {
        //向き
        float moveDir = movingRight ? 1f : -1f;
        rb.velocity = new Vector2(moveDir * moveSpeed, rb.velocity.y);

        //古いものを削除
        PositionHistory.Insert(0, transform.position);
        if (PositionHistory.Count > 100) 
        {
            PositionHistory.RemoveAt(PositionHistory.Count - 1);
        }

        //動かす
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
                return;
            }
                MoveBodyParts();
            
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
        // 配列の境界チェック用の最大インデックス 履歴ベース
        int maxHistoryIndex = PositionHistory.Count - 1;
        for (int i = 0; i < BodyParts.Count; i++)
        {
            int historyIndex = (int)Mathf.Min((i + 1) * Gap, maxHistoryIndex);

            int targetIndex = Mathf.Max(0, historyIndex - (i + 1));
            targetIndex = Mathf.Min(targetIndex, maxHistoryIndex);
            Vector3 targetPosition = PositionHistory[targetIndex];//ここが334行目


            // スムーズな移動を実装
            float smoothFactor = 15f; //スムーズさ係数
            BodyParts[i].transform.position = Vector3.Lerp(
                BodyParts[i].transform.position,
                targetPosition,
                Time.deltaTime * smoothFactor
            );
        }

    }

    // ======= 壁検知 =======
    private bool IsTouchingWall()
    {
        
        Vector2 direction = movingRight ? Vector2.right : Vector2.left;
        RaycastHit2D hit = Physics2D.Raycast(wallCheckPoint.position, direction, wallCheckDistance, wallLayer);
        //if (hit.collider) { turnPoint = this.transform.position; Debug.Log("敵１ IsTouchingWall() true"); } else { turnPoint = Vector3.zero; }// Debug.Log("敵１ IsTouchingWall() false"); }

        return hit.collider != null;
    }

    private bool CheckWall()
    {
        if (IsTouchingWall())
        {
            Flip();
            return true;
        }
        else
        {
            return false;
        }
    }

    // ======= 向き反転 =======
    private void Flip()
    {

        movingRight = !movingRight;
        transform.localScale = new Vector3(transform.localScale.x　* -1, transform.localScale.y, transform.localScale.z);      
       // movingRight = rb.velocity.x > 0 ? true : false;

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
    private void IgnoreBody()
    {
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
                // 現在の位置でアニメーションを停止
                break;
            }

            stretchTime += Time.deltaTime;
            float t = stretchTime / stretchAnimSpeed;

            // 頭伸ばす
            Vector2 stretchDirection = movingRight ? Vector2.right : Vector2.left;
            float headStretchLength = 0.1f * (hitPartIndex + 1);
            Vector2 headTargetPosition = headOriginalPosition + stretchDirection * headStretchLength * t;
            transform.position = headTargetPosition;

            // ヒットした部分から頭方向に向かって順に伸ばす
            for (int i = hitPartIndex; i >= 0; i--)
            {
                // 伸ばす方向（頭の向きに合わせる）
                // 頭からの距離に応じて伸ばす長さを調整
                float stretchLength = 0.1f * (hitPartIndex - i + 1);
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

        // 通常の移動を再開（回復した体の位置を保持する）
        float moveDir = movingRight ? 1f : -1f;
        rb.velocity = new Vector2(moveDir * moveSpeed, rb.velocity.y);
    }
}