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
        Time.timeScale = 1f; // àÍéûí‚é~âèú
        UnityEngine.SceneManagement.Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    public void Fade_SetScene(string targetScene)
    {
        Fade.FadeToScene(targetScene);
    }


}