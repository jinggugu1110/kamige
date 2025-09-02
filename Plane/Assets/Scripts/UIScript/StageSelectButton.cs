using UnityEngine;
using UnityEngine.SceneManagement;

//セレクト画面にて、アニメーションついてるStageオブジェクトにつける。

public class StageSelectButton : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject memoObject;      // メモ用紙
    public GameObject faceObject;      // 顔
    public GameObject legObject;       // 足

    [Header("Settings")]
    public string Nextscene;                // 遷移先シーン名
    public float runSpeed = 10f;           // 走る速度
    public float offScreenDistance = 15f;  // 画面外に出る距離

    private bool isSelected = false;
    private bool isRunning = false;
    private Vector3 originalPosition;

    void Start()
    {
        // 初期状態: 顔と足を非表示
        if (faceObject != null)
            faceObject.SetActive(false);

        if (legObject != null)
            legObject.SetActive(false);

        originalPosition = transform.position;
    }

    void OnMouseEnter()
    {
        if (!isSelected && faceObject != null)
        {
            faceObject.SetActive(true);
        }
    }

    void OnMouseExit()
    {
        if (!isSelected && faceObject != null)
        {
            faceObject.SetActive(false);
        }
    }

    void OnMouseDown()
    {
        if (!isSelected)
        {
            isSelected = true;
            StartRunAnimation();
        }
    }

    void StartRunAnimation()
    {
        // 顔と足を表示
        if (faceObject != null)
            faceObject.SetActive(true);

        if (legObject != null)
            legObject.SetActive(true);

        isRunning = true;
    }
    void Update()
    {
        if (isRunning)
        {
            // 右に走る
            transform.Translate(Vector3.right * runSpeed * Time.deltaTime);

            // 画面外に出たらシーン遷移
            if (transform.position.x > originalPosition.x + offScreenDistance)
            {
                // Fade.FadeToScene(Nextscene);
                SceneManager.LoadScene(Nextscene); // 指定したシーンに遷移
                isSelected = false;
            }
        }
    }
}