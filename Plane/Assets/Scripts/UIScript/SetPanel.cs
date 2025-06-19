using UnityEngine;
using UnityEngine.SceneManagement;

public class SetPanel : MonoBehaviour
{
    [Header("ESCで切り替えるパネル")]
    public GameObject panel;
    private bool isPaused = false;

    void Start()
    {
        panel.SetActive(false);  
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Toggle();
            if (panel.activeSelf)
            {
                Debug.Log("パネル　表示");
            }
            else {
                Debug.Log("パネル　ひ表示");

            }
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
}
