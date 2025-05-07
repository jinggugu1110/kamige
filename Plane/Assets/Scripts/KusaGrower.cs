using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KusaGrower : MonoBehaviour
{
    public GameObject leafPrefab;     // 
    public int maxLeafCount = 30;     // 
    public float leafSpacing = 0.5f;  // 

    private List<GameObject> grownLeaves = new List<GameObject>();
    private bool isGrown = false;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isGrown && collision.gameObject.layer == LayerMask.NameToLayer("PostIt"))
        {
            GrowLeaves();
            isGrown = true;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (isGrown && collision.gameObject.layer == LayerMask.NameToLayer("PostIt"))
        {
            ClearLeaves();
            isGrown = false;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void GrowLeaves()
    {
        Vector3 startPos = transform.position;

        for (int i = 1; i <= maxLeafCount; i++)
        {
            Vector3 nextPos = startPos + Vector3.right * leafSpacing * i;

            // ⛔ 
            Collider2D hit = Physics2D.OverlapCircle(nextPos, 0.3f, LayerMask.GetMask("Ground"));
            if (hit != null)
            {
                Debug.Log("Hit wall at " + nextPos);
                break;
            }

            GameObject newLeaf = Instantiate(leafPrefab, nextPos, Quaternion.identity, transform);
            grownLeaves.Add(newLeaf);
        }
    }
    void ClearLeaves()
    {
        foreach (GameObject leaf in grownLeaves)
        {
            if (leaf != null)
                Destroy(leaf);
        }
        grownLeaves.Clear();
    }
}

