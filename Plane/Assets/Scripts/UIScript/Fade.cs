using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/*
StageSelectButton アニメーション付きStageオブジェクトにつける
ButtonSceneChanger シーン遷移の関数まとまり
Fade Fade関数
SetPanel　パネルの表示非表示
 */
public class Fade : MonoBehaviour
{
    [Header("Fade Settings")]
    public Image fadeImage;
    public float fadeSpeed = 2f;

    private static Fade instance;
    private bool isFading = false;
    private string targetScene = "";

    void Awake()
    {
        // シングルトン化
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(gameObject);

            // 初期化
            if (fadeImage != null)
            {
                Color c = fadeImage.color;
                c.a = 0f;
                fadeImage.color = c;
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static void FadeToScene(string sceneName)
    {
        if (instance != null && !instance.isFading)
        {
            instance.StartFade(sceneName);
        }
    }

    void StartFade(string sceneName)
    {
        targetScene = sceneName;
        isFading = true;
    }

    void Update()
    {
        if (isFading && fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a += fadeSpeed * Time.deltaTime;
            fadeImage.color = c;

            if (c.a >= 1f)
            {
                SceneManager.LoadScene(targetScene);
            }
        }
    }
}