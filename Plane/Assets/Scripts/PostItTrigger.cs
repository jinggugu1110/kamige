using UnityEngine;

public class PostItTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {

        // TriggerChecker（体パーツ）に当たった？
        TriggerChecker body = other.GetComponent<TriggerChecker>();
        if (body != null)
        {
            body.isTouchingPostIt = true;
            Debug.Log("体に命中！");
            return;
        }

        // しっぽに当たった？
        TailTrigger tail = other.GetComponent<TailTrigger>();
        if (tail != null )
        {
            Debug.Log("尻尾に命中！");
            tail.enemy.OnTailHit();  // 尻尾ヒット
            return;
        }

        body.isTouchingPostIt =false;


        // それ以外は無視
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        TriggerChecker body = other.GetComponent<TriggerChecker>();
        if (body != null)
        {
            body.isTouchingPostIt = false;
        }
    }
}
