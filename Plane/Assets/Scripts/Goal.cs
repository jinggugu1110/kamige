using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Goal : MonoBehaviour
{
    public string charaName = "Player";

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.name == charaName)
        {
            SceneManager.LoadScene("Result");
        }
    }
}
