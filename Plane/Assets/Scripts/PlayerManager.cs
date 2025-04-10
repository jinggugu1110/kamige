using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;

    [Header("プレイヤー参照")]
    public GameObject mainPlayer;           // 通常プレイヤー
    public GameObject temporaryPlayer;      // 仮プレイヤー

    private void Awake()
    {
        // シングルトン化
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // シーンをまたいでも残すなら必要
        }
        else
        {
            Destroy(gameObject);
        }

        // SwitchToMainPlayer(); ← 起動時に temporaryPlayer を無効化しないようにする
    }

    /// <summary>
    /// 仮プレイヤーに切り替え
    /// </summary>
    public void SwitchToTemporaryPlayer(Vector2 spawnPosition)
    {
        if (temporaryPlayer == null || mainPlayer == null) return;

        // メインプレイヤーを無効化
        mainPlayer.GetComponent<PlayerController>().enabled = false;

        // 仮プレイヤーを有効化
        temporaryPlayer.transform.position = spawnPosition;
        temporaryPlayer.SetActive(true);
        temporaryPlayer.GetComponent<TemporaryPlayerController>().enabled = true;
    }

    /// <summary>
    /// メインプレイヤーに戻す
    /// </summary>
    public void SwitchToMainPlayer()
    {
        if (temporaryPlayer == null || mainPlayer == null) return;

        // 仮プレイヤーを無効化
        temporaryPlayer.GetComponent<TemporaryPlayerController>().enabled = false;
        temporaryPlayer.SetActive(false);

        // メインプレイヤーを有効化
        mainPlayer.GetComponent<PlayerController>().enabled = true;
    }
}
