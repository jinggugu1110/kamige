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
    private Vector3 growDirection = Vector3.right; // default right
    private Vector3 GetDirectionFromAngle(float angleZ)
    {
        angleZ = Mathf.Round(angleZ) % 360; // 防止小数误差

        if (angleZ == 0f)
            return Vector3.right;
        else if (angleZ == 90f)
            return Vector3.up;
        else if (angleZ == 180f)
            return Vector3.left;
        else if (angleZ == 270f)
            return Vector3.down;
        else
            return Vector3.right; // fallback
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isGrown && collision.gameObject.layer == LayerMask.NameToLayer("PostIt"))
        {
            // 🎯 获取旋转角度并转换为方向
            float angleZ = collision.transform.eulerAngles.z;
            growDirection = GetDirectionFromAngle(angleZ);

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
        float kusaWidth = 1.0f;
        float leafWidth = leafPrefab.GetComponent<SpriteRenderer>().bounds.size.x;
        float leafHeight = leafPrefab.GetComponent<SpriteRenderer>().bounds.size.y;

        Vector3 startEdge = transform.position + growDirection * (kusaWidth / 2);
       

        for (int i = 0; i <= maxLeafCount; i++)
        {
            Vector3 nextPos = startEdge + growDirection * (leafWidth * i + leafWidth / 2);

            // 
            Vector2 checkSize = new Vector2(leafWidth * 0.95f, leafHeight * 0.95f);
            Collider2D hit = Physics2D.OverlapBox(nextPos, checkSize, 0f, LayerMask.GetMask("Ground"));
            if (hit != null)
            {
                Debug.Log("Hit wall at " + nextPos);
                break;
            }

            Quaternion rotation = Quaternion.identity;
            // 根据方向决定旋转角度
            if (growDirection == Vector3.up)
                rotation = Quaternion.Euler(0, 0, 90);
            else if (growDirection == Vector3.left)
                rotation = Quaternion.Euler(0, 0, 180);
            else if (growDirection == Vector3.down)
                rotation = Quaternion.Euler(0, 0, 270);
            // 向右就默认 0

            GameObject newLeaf = Instantiate(leafPrefab, nextPos, rotation, transform);
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

