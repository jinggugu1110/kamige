using UnityEngine;
using UnityEngine.SceneManagement; // �V�[���Ǘ����g�����߂ɒǉ�

public class SceneLoader : MonoBehaviour
{
    public string nextSceneName; // �J�ڐ�̃V�[������ݒ�

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // �v���C���[���S�[���ɐG�ꂽ��
        {
            SceneManager.LoadScene(nextSceneName); // �w�肵���V�[���ɑJ��


        }
    }
}
