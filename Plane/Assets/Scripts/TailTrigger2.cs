using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TailTrigger2 : MonoBehaviour
{
    public global::Enemy2 enemy;
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

            if (enemy != null)
            {
                enemy.OnTailHit();
            }
        }
    }
}
