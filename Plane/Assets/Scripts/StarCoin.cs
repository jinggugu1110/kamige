using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class StarCoin : MonoBehaviour
{
    public bool isGetcoin = false;//PlayerÇ™ÉRÉCÉìÇ…êGÇÍÇΩÇÁtrue
    private bool playerisexist = false;

    private void Start()
    {

    }

    private void Update()
    {
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Transform root = FindRootObject(other.transform);

        if (other.CompareTag("PostIt") || other.CompareTag("Player"))
        {
            if (root.CompareTag("Player") || root.CompareTag("PostIt"))
            {
                isGetcoin = true;
                gameObject.SetActive(false);
            }
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
}
