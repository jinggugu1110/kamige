using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
public class ButtonSceneChanger : MonoBehaviour
{
    public void GoToStageSelect()
    {
        Debug.Log("GoToStageSelect()");
        Fade.FadeToScene("0_StageSelect");

    }
    public void GoToTitle()
    {
        Fade.FadeToScene("Title");
    }

    public void RetryScene()
    {
        Debug.Log("RetryScene‚ª‰Ÿ‚³‚ê‚½");
        Time.timeScale = 1f; // ˆê’â~‰ğœ
        UnityEngine.SceneManagement.Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    public void Fade_SetScene(string targetScene)
    {
        Fade.FadeToScene(targetScene);
    }


}