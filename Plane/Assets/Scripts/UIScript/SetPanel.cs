using UnityEngine;
using UnityEngine.SceneManagement;

public class SetPanel : MonoBehaviour
{
    [Header("ESC‚ÅØ‚è‘Ö‚¦‚éƒpƒlƒ‹")]
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
