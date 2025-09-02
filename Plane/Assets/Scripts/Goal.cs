using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Goal : MonoBehaviour
{
    public Transform player;
    private Rigidbody2D targetRb;
    [Header("リザルトパネル")] 
    public GameObject panel;

    private void Start()
    {
        panel.SetActive(false);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.transform.root.CompareTag("PostIt"))
        {
            if (targetRb == null) // 初回接触時にRigidbody2Dを取得
            {
                targetRb = other.GetComponent<Rigidbody2D>();
                //SceneManager.LoadScene("Result");
                panel.SetActive(true);

            }
        }      
    }
}