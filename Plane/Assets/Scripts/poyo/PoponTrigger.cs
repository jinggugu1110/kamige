using UnityEngine;

public class PoponTrigger : MonoBehaviour
{
    public GameObject poponPrefab;
    public Transform spawnPoint;
    private GameObject currentPopon;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && currentPopon == null)
        {
            currentPopon = Instantiate(poponPrefab, spawnPoint.position, Quaternion.identity);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && currentPopon != null)
        {
            Destroy(currentPopon);
        }
    }
}
