using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using DG.Tweening;

public class scr_mn_quit : MonoBehaviour
{
    //Menu SFX
    public AudioSource menu_sfx_1;
    public AudioSource menu_sfx_2;
    public AudioSource menu_sfx_3;

    //button object reference
    public GameObject obj_btn_yes;
    public GameObject obj_btn_no;

    //menu object reference
    public GameObject obj_mn_root;
    public GameObject obj_mn_quit;

    private Vector2 ctr_nav;
    private int selected_btn;
    private Guid uid_opt;
    private Sequence sequence_opt;
    private float alpha_in = 0.4313726f; //alpha 110
    private float alpha_out = 0;
    private float fade_time = 0.15f;

    private List<GameObject> button_list = new List<GameObject>();
    private GameObject selected_btn_obj;
    public GameObject obj_crossfade;

    void OnEnable()
    {
        button_list = new List<GameObject>(); //clear the button list when we leave the menu

        //button index list
        button_list.Add(obj_btn_no);  //0
        button_list.Add(obj_btn_yes); //1

        selected_btn_obj = obj_btn_no;
        selected_btn = 0;

        ///button selection (btn_NewGame)
        sequence_opt = DOTween.Sequence();
        sequence_opt.Append(obj_btn_no.GetComponent<Image>().DOFade(alpha_in, fade_time))
            .Insert(0, obj_btn_yes.GetComponent<Image>().DOFade(alpha_out, fade_time));
        uid_opt = System.Guid.NewGuid();
        sequence_opt.id = uid_opt;
        sequence_opt.Play();
    }

    public void nav_trigger_mouse1() { buttonAnimation(1); } //btn_no
    public void nav_trigger_mouse2() { buttonAnimation(2); } //btn_yes

    //play SFX (for mouse click)
    public void sfx_play_menu_1() { menu_sfx_1.Play(); }
    public void sfx_play_menu_2() { menu_sfx_2.Play(); }
    public void sfx_play_menu_3() { menu_sfx_3.Play(); }

    public void nav_trigger(InputAction.CallbackContext context)
    {
        ctr_nav = context.ReadValue<Vector2>(); //update the navigation value

        if (context.performed) //key pressed
        {
            //btn index boundaries
            if (ctr_nav.x == -1) //LEFT
            {
                selected_btn++;
                if (selected_btn > (button_list.Count-1)) { selected_btn = 0; }
                menu_sfx_2.Play(); //play sfx
                selected_btn_obj = button_list[selected_btn]; //update the selected btn object
            }
            else if (ctr_nav.x == 1) //RIGHT
            {
                selected_btn--;
                if (selected_btn < 0) { selected_btn = (button_list.Count-1); }
                menu_sfx_2.Play(); //play sfx
                selected_btn_obj = button_list[selected_btn]; //update the selected btn object
            }

            if (obj_mn_quit.activeInHierarchy == true) { buttonAnimation(0); } //execute if active (required to avoid error between menus)
        }
    }

    public void enter_trigger(InputAction.CallbackContext context)
    {
        if (context.performed) //key pressed
        {
            switch (selected_btn)
            {
                case 0: //btn_no
                    menu_sfx_1.Play(); //play SFX
                    obj_mn_root.SetActive(true); obj_mn_quit.SetActive(false);
                break;

                case 1: //btn_yes
                    menu_sfx_3.Play(); //play SFX
                    StartCoroutine(GameEnd());
                break;

                default:
                break;
            }
        }
    }

    public void back_trigger(InputAction.CallbackContext context)
    {
        if (context.performed) //key pressed
        {
            menu_sfx_1.Play();
            obj_mn_root.SetActive(true); obj_mn_quit.SetActive(false);
        }
    }

    private void buttonAnimation(int keyOrMouse) //0 = keyboard || 1 = mouse opt 1 || 2 = mouse opt 2
    {
        switch (keyOrMouse)
        {
            case 0:
                switch (selected_btn)
                {
                    case 0: //btn_no
                        sequence_opt = DOTween.Sequence();
                        sequence_opt.Append(obj_btn_no.GetComponent<Image>().DOFade(alpha_in, fade_time))
                            .Insert(0, obj_btn_yes.GetComponent<Image>().DOFade(alpha_out, fade_time));
                        uid_opt = System.Guid.NewGuid();
                        sequence_opt.id = uid_opt;
                        sequence_opt.Play();
                    break;

                    case 1: //btn_yes
                        sequence_opt = DOTween.Sequence();
                        sequence_opt.Append(obj_btn_yes.GetComponent<Image>().DOFade(alpha_in, fade_time))
                            .Insert(0, obj_btn_no.GetComponent<Image>().DOFade(alpha_out, fade_time));
                        uid_opt = System.Guid.NewGuid();
                        sequence_opt.id = uid_opt;
                        sequence_opt.Play();
                    break;

                    default:
                    break;
                }
            break;

            case 1: //btn_no
                selected_btn = 0;
                selected_btn_obj = button_list[selected_btn]; //update the selected btn object

                sequence_opt = DOTween.Sequence();
                sequence_opt.Append(obj_btn_no.GetComponent<Image>().DOFade(alpha_in, fade_time))
                    .Insert(0, obj_btn_yes.GetComponent<Image>().DOFade(alpha_out, fade_time));
                uid_opt = System.Guid.NewGuid();
                sequence_opt.id = uid_opt;
                sequence_opt.Play();
                menu_sfx_2.Play(); //play sfx
            break;

            case 2: //btn_yes
                selected_btn = 1;
                selected_btn_obj = button_list[selected_btn]; //update the selected btn object

                sequence_opt = DOTween.Sequence();
                sequence_opt.Append(obj_btn_yes.GetComponent<Image>().DOFade(alpha_in, fade_time))
                    .Insert(0, obj_btn_no.GetComponent<Image>().DOFade(alpha_out, fade_time));
                uid_opt = System.Guid.NewGuid();
                sequence_opt.id = uid_opt;
                sequence_opt.Play();
                menu_sfx_2.Play(); //play sfx
            break;

            default:
            break;
        }
    }

    public void quit() { StartCoroutine(GameEnd()); }

    private IEnumerator GameEnd()
    {
        yield return new WaitForSeconds(0.15f);
        scr_fade_levelLoader.make_fade_black_on_startup = true; //make the fade be of alpha 1 on startup of next scene
        yield return obj_crossfade.GetComponent<scr_fade_levelLoader>().sceneSwitchFadeIn(0.5f, false, Ease.InCubic);
        yield return new WaitForSeconds(1.0f);
        Debug.Log("game.end");
        Application.Quit();
    }
}
