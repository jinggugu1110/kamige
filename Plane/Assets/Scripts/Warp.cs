using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* ワープするスクリプト
   入り口のオブジェクトにアタッチしてください
   出口は、入り口の子オブジェクトにしてください */
public class Warp : MonoBehaviour
{
    Transform Warp_OutPoint;
    private bool OutisinStage = false; //出口がステージ内にあればtrue
    private float minX = -10f;
    private float maxX = 10f;
    private float minY = -10f;
    private HashSet<GameObject> WarpedList = new HashSet<GameObject>(); //ワープ済みリスト。HashSetは「重複しない要素」を格納する。
    private float warpCooldown = 0.5f; // ワープのクールダウン時間

    private void Start()
    {
        Warp_OutPoint = transform.childCount > 0 ? transform.GetChild(0) : null;
    }

    private void Update()
    {
        OutisinStage = !(Warp_OutPoint.position.x < minX || Warp_OutPoint.position.x > maxX || Warp_OutPoint.position.y < minY);

        // 定期的にWarpedListをクリアする
        if (WarpedList.Count > 0)
        {
            StartCoroutine(ClearWarpedObject());
        }
    }

    private IEnumerator ClearWarpedObject()
    {
        yield return new WaitForSeconds(warpCooldown);
        WarpedList.Clear();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // 既にワープ処理をしたオブジェクトならスキップ
        if (WarpedList.Contains(other.gameObject))
        {
            return;
        }

        if (other.CompareTag("Attachable")) //ただのオブジェクト
        {
            if (Warp_OutPoint != null && OutisinStage)
            {
                Transform root = FindRootObject(other.transform);//root = 最上位の親
                WarpObject(root);
            }
            else
            {
                // プレイヤー死亡処理
            }
        }
        else if (other.CompareTag("PostIt")|| other.CompareTag("Player")) //プレイヤーふせん
        {
　          Transform root = FindRootObject(other.transform);
            if (root.CompareTag("Player")|| root.CompareTag("PostIt"))
            {
                if (Warp_OutPoint != null && OutisinStage)
                {
                    WarpObject(root);
                }
                else
                {
                    // プレイヤー死亡処理
                }
            }
            else
            {
                // PostItだけでもワープ
                //if (Warp_OutPoint != null && OutisinStage)
                //{
                //    WarpObject(other.transform);
                //}
            }
        }
        else if (other.CompareTag("Player")) // プレイヤー
        {
            if (Warp_OutPoint != null && OutisinStage)
            {
                WarpObject(other.transform);
            }
            else
            {
                // プレイヤー死亡処理
            }
        }
    }

    //最上位の親を見つける関数
    private Transform FindRootObject(Transform obj)
    {
        Transform root = obj;
        while (root.parent != null)
        {
            root = root.parent;
        }
        return root;
    }

    //ワープさせる関数
    private void WarpObject(Transform obj)
    {
        obj.position = Warp_OutPoint.position;

        WarpedList.Add(obj.gameObject);
        foreach (Transform child in obj)
        {
            AddAllChildrenToWarpedList(child);
        }
    }

    //子オブジェクトを再帰的にWarpedListに追加する
    private void AddAllChildrenToWarpedList(Transform obj)
    {
        WarpedList.Add(obj.gameObject);
        foreach (Transform child in obj)
        {
            AddAllChildrenToWarpedList(child);
        }
    }
}