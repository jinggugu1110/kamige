using UnityEngine;
using UnityEngine.SceneManagement;
//•\¦/”ñ•\¦‚ğ‚«‚è‚©‚¦‚é
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

            //ˆê’â~
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
        Time.timeScale = 1f; //ˆê’â~‰ğœ
        SceneManager.LoadScene("StageSelect");
    }
    public void RetryScene()
    {
        Time.timeScale = 1f; // ˆê’â~‰ğœ
        UnityEngine.SceneManagement.Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}
