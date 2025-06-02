using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TailTrigger : MonoBehaviour
{
    public global::Enemy enemy;
    private bool movingRight = true;
    private float wallCheckDistance = 0.6f;
    private LayerMask wallLayer;
    bool isHit = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PostIt"))
        {
            Transform postItRoot = other.transform.root;
            if (postItRoot.CompareTag("Player"))
            {
                return;
            }
            else if (other.transform.root == this.transform.root)// 自分の親がEnemyと一致していれば無視（＝自分自身に当たった）
            {
                return;
            }else if (enemy != null)
            {
                enemy.OnTailHit();
            }
        }
    }
}
