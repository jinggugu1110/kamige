using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
ワープするスクリプト
入り口のオブジェクトにアタッチしてください
出口は、入り口の子オブジェクトにしてください
*/

public class Warp : MonoBehaviour
{
    Transform Warp_OutPoint;
    private bool GoalisinStage = false;//出口がステージ内にあればtrue
    private float minX = -10f;
    private float maxX = 10f;
    private float minY = -10f;

    private void Start()
    {
        Warp_OutPoint = transform.childCount > 0 ? transform.GetChild(0) : null;
    }

    private void Update()
    {
        GoalisinStage = !(Warp_OutPoint.position.x < minX || Warp_OutPoint.position.x > maxX || Warp_OutPoint.position.y < minY);

    }

    private void OnTriggerStay2D(Collider2D other)
    {
        //ワープ先がステージ内ならワープ
        if (other.CompareTag("Player"))
        {
            if (Warp_OutPoint != null && GoalisinStage)
            {
                other.transform.position = Warp_OutPoint.position;
            }
            else
            {
                //プレイヤー死亡
            }
        }
    }
    
}
