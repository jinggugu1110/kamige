using UnityEngine;

public class EffectAutoHider : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer sr;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        sr.enabled = false; // 初期非表示
    }

    public void PlayEffect()
    {
        sr.enabled = true;
        animator.Play("husenEfe", -1, 0f); // アニメーション開始

        StartCoroutine(HideAfter(animator.GetCurrentAnimatorStateInfo(0).length));
    }

    private System.Collections.IEnumerator HideAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        sr.enabled = false;
    }
}
