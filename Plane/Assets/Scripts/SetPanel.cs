using UnityEngine;
using UnityEngine.SceneManagement;
//表示/非表示をきりかえる
public class SetPanel : MonoBehaviour
{
    [SerializeField] public GameObject panel;
    private void Start()
    {
        panel.SetActive(false);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            panel.SetActive(true);

            //一時停止
            Time.timeScale = panel.activeSelf ? 0f : 1f;
        }
    }

    public void Show()
    {
        panel.SetActive(true);
    }

    public void Hide()
    {
        panel.SetActive(false);
    }

    public void Toggle()
    {
        panel.SetActive(!panel.activeSelf);
    }
    public void ReturnToSelectScene()
    {
        Time.timeScale = 1f; //一時停止解除
        SceneManager.LoadScene("StageSelect");
    }
    public void RetryScene()
    {
        Time.timeScale = 1f; // 一時停止解除
        UnityEngine.SceneManagement.Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}
