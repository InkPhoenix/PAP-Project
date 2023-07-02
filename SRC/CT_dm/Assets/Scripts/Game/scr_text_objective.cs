using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class scr_text_objective : MonoBehaviour
{
    public CanvasGroup fade;
    private float fade_time = 1.5f;

    private void Start() { fadeObjective(); }

    public void fadeObjective()
    {
        fade.alpha = 0; //just to be sure
        StartCoroutine(fadeIn());
    }

    private void complete() { StartCoroutine(fadeOut()); }

    private IEnumerator fadeIn()
    {
        yield return new WaitForSeconds(0.5f);
        fade.DOFade(1, fade_time).SetEase(Ease.InCubic).OnComplete(complete);
        yield return null;
    }

    private IEnumerator fadeOut()
    {
        yield return new WaitForSeconds(4.0f);
        fade.DOFade(0, fade_time).SetEase(Ease.InCubic);
        yield return null;
    }
}
