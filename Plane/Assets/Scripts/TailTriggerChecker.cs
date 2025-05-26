using UnityEngine;

public class TriggerChecker : MonoBehaviour
{
    public bool isTouchingPostIt = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PostIt"))
        {
            isTouchingPostIt = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("PostIt"))
        {
            isTouchingPostIt = false;
        }
    }
}
