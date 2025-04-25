using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Goal : MonoBehaviour
{
    public Transform player;
    private Rigidbody2D targetRb;
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.transform.root.CompareTag("PostIt"))
        {
            if (targetRb == null) // ����ڐG����Rigidbody2D���擾
            {
                targetRb = other.GetComponent<Rigidbody2D>();
                SceneManager.LoadScene("Result");
            }          
        }      
    }
}