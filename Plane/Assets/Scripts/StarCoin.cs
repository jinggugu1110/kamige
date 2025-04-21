using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

/*
１ステージあたりコイン
*/

public class StarCoin : MonoBehaviour
{
    public bool isGetcoin = false;//Playerがコインに触れたらtrue
 
    private void Start()
    {

    }

    private void Update()
    {
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isGetcoin = true;
            gameObject.SetActive(false);
        }
    }
}
