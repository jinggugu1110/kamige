using UnityEngine;
using UnityEngine.SceneManagement; // シーン管理を使うために追加

public class SceneLoader : MonoBehaviour
{
    public string nextSceneName; // 遷移先のシーン名を設定

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // プレイヤーがゴールに触れたら
        {
            SceneManager.LoadScene(nextSceneName); // 指定したシーンに遷移


        }
    }
}
