using System.Collections.Generic;
using UnityEngine;

public class Warp : MonoBehaviour
{
    public Transform warpOutPoint;
    private HashSet<GameObject> warpedObjects = new HashSet<GameObject>();

    private float minX = -1000f;
    private float maxX = 1000f;
    private float minY = -10f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Transform root = FindRootObject(other.transform);
        GameObject obj = root.gameObject;

        if (warpedObjects.Contains(obj)) return;
        if (!IsWarpValid(root)) return;

        if (warpOutPoint != null && IsInStage(warpOutPoint.position))
        {
            WarpToDestination(root);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Transform root = FindRootObject(other.transform);
        GameObject obj = root.gameObject;

        if (warpedObjects.Contains(obj))
        {
            warpedObjects.Remove(obj); // ó£ÇÍÇΩÇÁçƒÉèÅ[Évâ¬î\Ç…
        }
    }

    private void WarpToDestination(Transform obj)
    {
        obj.position = warpOutPoint.position;
        warpedObjects.Add(obj.gameObject);

        // èoå˚ë§ÇÃWarpÇ…Ç‡ìoò^ÇµÇƒÇ®Ç≠Åië¶ñﬂÇËñhé~Åj
        Warp outWarp = warpOutPoint.GetComponent<Warp>();
        if (outWarp != null)
        {
            outWarp.warpedObjects.Add(obj.gameObject);
        }
    }

    private Transform FindRootObject(Transform obj)
    {
        Transform root = obj;
        while (root.parent != null)
        {
            root = root.parent;
        }
        return root;
    }

    private bool IsWarpValid(Transform obj)
    {
        return obj.CompareTag("Player") || obj.CompareTag("PostIt") || obj.CompareTag("Attachable");
    }

    private bool IsInStage(Vector3 pos)
    {
        return pos.x > minX && pos.x < maxX && pos.y > minY;
    }
}
