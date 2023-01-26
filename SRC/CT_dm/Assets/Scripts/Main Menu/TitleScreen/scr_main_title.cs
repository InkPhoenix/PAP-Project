using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class scr_main_title : MonoBehaviour
{
    public CanvasGroup UI_element;
    private float fade_time = 2.0f;
    private Guid uid_title;
    private Sequence sequence_title;
    private int sequence_stopped;
    public AudioSource musicSource;
    public GameObject obj_crossfade;

    //Menu SFX
    public AudioSource menu_sfx_1;
    public AudioSource menu_sfx_2;
    public AudioSource menu_sfx_3;

    private IEnumerator fadeFromBlack()
    {
        scr_fade_levelLoader.make_fade_black_on_startup = false;
        obj_crossfade.GetComponent<CanvasGroup>().alpha = 1;
        yield return new WaitForSeconds(0.1f); //give it slight delay just to look better
        StartCoroutine(obj_crossfade.GetComponent<scr_fade_levelLoader>().sceneSwitchFadeOut(1.25f, false, Ease.Linear));
    }

    private IEnumerator startupFade()
    {
        yield return new WaitForSeconds(1.0f); //wait before playing the BGM
        musicSource.Play();
        yield return new WaitForSeconds(0.5f); //wait before fading-in the title
        startFade();
    }

    private void Start()
    {
        sequence_stopped = 0;
        UI_element.alpha = 0;
        if (scr_fade_levelLoader.make_fade_black_on_startup == true) { StartCoroutine(fadeFromBlack()); }
        StartCoroutine(startupFade());
    }

    public void startFade()
    {
        if (sequence_title == null) //create if there's none before
        {
            sequence_title = DOTween.Sequence();
            sequence_title.Append(UI_element.DOFade(1, fade_time).OnComplete(setValue));
            uid_title = System.Guid.NewGuid();
            sequence_title.id = uid_title;
        }
        sequence_title.Play();
    }

    private void setValue() { sequence_stopped = 3; }

    public void stopFade()
    {
        switch (sequence_stopped)
        {
            case 0: //code to skip animation
                sequence_stopped++;

                //kill fade
                obj_crossfade.GetComponent<scr_fade_levelLoader>().killSequence();
                obj_crossfade.GetComponent<CanvasGroup>().alpha = 0;

                //kill title anim
                DOTween.Kill(uid_title);
                sequence_title = null;
                UI_element.alpha = 1;
            break;

            case 1:
                sequence_stopped++;
            break;

            case 2:
                sequence_stopped++;
            break;

            case 3: //code to change scenes
                sequence_stopped++;
                UI_element.alpha = 1;

                scn_switch(); //start scene switching fade

                menu_sfx_3.Play(); //play sfx

                //animate title fade-out
                Vector3 title_original_scale = transform.localScale;
                Vector3 title_scale_to = title_original_scale * 1.8f; //scale up the title by 1.8
            
                UI_element.transform.DOScale(title_scale_to, 2.0f);
                UI_element.DOFade(0, 2.0f);

                DOTween.To(() => musicSource.volume, x => musicSource.volume = x, 0, 2.0f).SetEase(Ease.InSine); //fade-out BGM volume
            break;

            default:
            break;
        }
    }

    private void scn_switch() { StartCoroutine(switch_scenes("scn_MainMenu")); }

    private IEnumerator switch_scenes(string scene)
    {
        yield return new WaitForSeconds(0.15f);
        obj_crossfade.GetComponent<scr_fade_levelLoader>().scene_name = scene;
        scr_fade_levelLoader.make_fade_black_on_startup = true; //make the fade be of alpha 1 on startup of next scene
        yield return obj_crossfade.GetComponent<scr_fade_levelLoader>().sceneSwitchFadeIn(2.0f, true, Ease.InCubic);
    }
}
