using UnityEngine;

public class poyospawn : MonoBehaviour
{
    public GameObject poyoPrefab;
    public Transform spawnPoint;
    public Transform player;
    public float spawnDistance = 5f;

    private GameObject spawnedPoyo;

    void Update()
    {
        float distance = Vector2.Distance(player.position, transform.position);

        if (distance <= spawnDistance && spawnedPoyo == null)
        {
            spawnedPoyo = Instantiate(poyoPrefab, spawnPoint.position, Quaternion.identity);
            PoyoController controller = spawnedPoyo.GetComponent<PoyoController>();
            controller.player = player;
            controller.graveOrigin = transform;
        }

        if (distance > spawnDistance + 1f && spawnedPoyo != null)
        {
            Destroy(spawnedPoyo);
        }
    }
}
