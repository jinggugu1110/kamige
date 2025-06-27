using UnityEngine;

public class PoponTrigger : MonoBehaviour
{
    public GameObject poponPrefab;
    public Transform spawnPoint;
    private GameObject currentPopon;
    private BoxCollider2D mySpawnArea;

    private void Start()
    {
        // この墓オブジェクトの子にある BoxCollider2D（紫の枠）を取得
        mySpawnArea = GetComponentInChildren<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // プレイヤー付箋が入ったとき、まだ生成されてなければ生成
        if (other.CompareTag("nonconflictPostIt") && currentPopon == null)
        {
            currentPopon = Instantiate(poponPrefab, spawnPoint.position, Quaternion.identity);

            // 生成直後に紫の範囲を渡す
            var controller = currentPopon.GetComponent<poyoController>();
            if (controller != null && mySpawnArea != null)
            {
                controller.SetSpawnArea(mySpawnArea);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // プレイヤー付箋が出たら破棄
        if (other.CompareTag("nonconflictPostIt") && currentPopon != null)
        {
            Destroy(currentPopon);
            currentPopon = null;
        }
    }
}
