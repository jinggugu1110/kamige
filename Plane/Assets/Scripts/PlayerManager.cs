using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;

    [Header("�v���C���[�Q��")]
    public GameObject mainPlayer;           // �ʏ�v���C���[
    public GameObject temporaryPlayer;      // ���v���C���[

    private void Awake()
    {
        // �V���O���g����
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // �V�[�����܂����ł��c���Ȃ�K�v
        }
        else
        {
            Destroy(gameObject);
        }

        // SwitchToMainPlayer(); �� �N������ temporaryPlayer �𖳌������Ȃ��悤�ɂ���
    }

    /// <summary>
    /// ���v���C���[�ɐ؂�ւ�
    /// </summary>
    public void SwitchToTemporaryPlayer(Vector2 spawnPosition)
    {
        if (temporaryPlayer == null || mainPlayer == null) return;

        // ���C���v���C���[�𖳌���
        mainPlayer.GetComponent<PlayerController>().enabled = false;

        // ���v���C���[��L����
        temporaryPlayer.transform.position = spawnPosition;
        temporaryPlayer.SetActive(true);
        temporaryPlayer.GetComponent<TemporaryPlayerController>().enabled = true;
    }

    /// <summary>
    /// ���C���v���C���[�ɖ߂�
    /// </summary>
    public void SwitchToMainPlayer()
    {
        if (temporaryPlayer == null || mainPlayer == null) return;

        // ���v���C���[�𖳌���
        temporaryPlayer.GetComponent<TemporaryPlayerController>().enabled = false;
        temporaryPlayer.SetActive(false);

        // ���C���v���C���[��L����
        mainPlayer.GetComponent<PlayerController>().enabled = true;
    }
}
