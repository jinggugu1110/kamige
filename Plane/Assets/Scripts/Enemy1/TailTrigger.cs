using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TailTrigger : MonoBehaviour
{
    public global::Enemy enemy;

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
        if (other.CompareTag("PostIt")|| other.CompareTag("conflictPostIt") || other.CompareTag("nonconflictPostIt"))
        {
            Transform postItRoot = other.transform.root;
            if (postItRoot.CompareTag("Player"))
            {
                return;
            }
            else if (other.transform.root == this.transform.root)
            {
                return;
            }
            else if (enemy != null && enemy.isGrounded && enemy.waitStun == false && enemy.isRecovering == false)
            {
                enemy.OnTailHit();
            }      

        }
    }
}
