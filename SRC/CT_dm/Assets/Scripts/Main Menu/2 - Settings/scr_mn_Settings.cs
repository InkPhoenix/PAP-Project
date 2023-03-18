using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using DG.Tweening;

public class scr_mn_Settings : MonoBehaviour
{
    //Menu SFX
    public AudioSource menu_sfx_1;
    public AudioSource menu_sfx_2;
    public AudioSource menu_sfx_3;

    //button object reference
    public GameObject obj_btn_audio;
    public GameObject obj_btn_video;
    public GameObject obj_btn_controls;
    public GameObject obj_btn_language;

    //menu object reference
    public GameObject obj_mn_root;
    public GameObject obj_mn_settings;
    public GameObject obj_mn_settings_audio;
    public GameObject obj_mn_settings_video;
    public GameObject obj_mn_settings_controls;
    public GameObject obj_mn_settings_language;

    public TMPro.TMP_Text txt_menu;

    private Vector2 ctr_nav;
    private int selected_btn;
    private bool is_held = false;
    private Guid uid_opt;
    private Sequence sequence_opt;
    private float alpha_in = 0.4313726f; //alpha 110
    private float alpha_out = 0;
    private float fade_time = 0.15f;

    private List<GameObject> button_list = new List<GameObject>();
    private GameObject selected_btn_obj;

    private void OnEnable()
    {
        button_list = new List<GameObject>(); //clear the button list when we leave the menu

        //button selection
        selected_btn_obj = obj_btn_audio;
        selected_btn = 0;

        //button index list
        button_list.Add(obj_btn_audio);    //0
        button_list.Add(obj_btn_video);    //1
        button_list.Add(obj_btn_controls); //2
        button_list.Add(obj_btn_language); //3

        ///button selection
        sequence_opt = DOTween.Sequence();
        sequence_opt.Append(obj_btn_audio.GetComponent<Image>().DOFade(alpha_in, fade_time))
            .Insert(0, obj_btn_video.GetComponent<Image>().DOFade(alpha_out, fade_time))
            .Insert(0, obj_btn_controls.GetComponent<Image>().DOFade(alpha_out, fade_time))
            .Insert(0, obj_btn_language.GetComponent<Image>().DOFade(alpha_out, fade_time));
        uid_opt = System.Guid.NewGuid();
        sequence_opt.id = uid_opt;
        sequence_opt.Play();
        txt_menu.text = "AUDIO OPTIONS";
    }

    ///animations
    //mouse hover triggers
    public void nav_trigger_mouse1() { buttonAnimation(1); }   //btn_Audio
    public void nav_trigger_mouse2() { buttonAnimation(2); }   //btn_Video
    public void nav_trigger_mouse3() { buttonAnimation(3); }   //btn_Controls
    public void nav_trigger_mouse4() { buttonAnimation(4); }   //btn_Language

    //play SFX (for mouse click)
    public void play_sfx_menu_1() { menu_sfx_1.Play(); }
    public void play_sfx_menu_2() { menu_sfx_2.Play(); }
    public void play_sfx_menu_3() { menu_sfx_3.Play(); }

    public void nav_trigger(InputAction.CallbackContext context)
    {
        ctr_nav = context.ReadValue<Vector2>(); //update the navigation value

        if (context.performed) //key pressed
        {
            //btn index boundaries
            if (ctr_nav.y == 1) //UP
            {
                selected_btn--;
                if (selected_btn < 0) { selected_btn = (button_list.Count-1); }
                menu_sfx_2.Play(); //play sfx
                selected_btn_obj = button_list[selected_btn]; //update the selected btn object
            }
            else if (ctr_nav.y == -1) //DOWN
            {
                selected_btn++;
                if (selected_btn > (button_list.Count-1)) { selected_btn = 0; }
                menu_sfx_2.Play(); //play sfx
                selected_btn_obj = button_list[selected_btn]; //update the selected btn object
            }

            if (obj_mn_settings.activeInHierarchy == true) { buttonAnimation(0); } //execute if active (required to avoid error between menus)
            StartCoroutine("crt_timer");
        }
        if (context.canceled) //key released
        {
            is_held = false;
            StopCoroutine("crt_timer");
        }
    }

    public void enter_trigger(InputAction.CallbackContext context)
    {
        if (context.performed) //key pressed
        {
            switch (selected_btn)
            {
                case 0: //btn_Audio
                    menu_sfx_3.Play(); //play SFX
                    obj_mn_settings.SetActive(false); obj_mn_settings_audio.SetActive(true); test1();
                break;

                case 1: //btn_Video
                    menu_sfx_3.Play(); //play SFX
                    obj_mn_settings.SetActive(false); obj_mn_settings_video.SetActive(true); test2();
                break;

                case 2: //btn_Controls
                    menu_sfx_3.Play(); //play SFX
                    obj_mn_settings.SetActive(false); obj_mn_settings_controls.SetActive(true); test3();
                break;

                case 3: //btn_Language
                    menu_sfx_3.Play(); //play SFX
                    obj_mn_settings.SetActive(false); obj_mn_settings_language.SetActive(true); test4();
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
            obj_mn_settings.SetActive(false);
            obj_mn_root.SetActive(true);
        }
    }

    private IEnumerator crt_timer()
    {
        yield return new WaitForSeconds(0.4f); //1st delay
        is_held = true;
        if (obj_mn_settings.activeInHierarchy == true) { buttonAnimation(0); } //execute if active (required to avoid error between menus)
    }

    private IEnumerator crt_btn_held_repeater()
    {
        if (is_held == true)
        {
            yield return new WaitForSeconds(0.25f); //delay after each entry
            if (obj_mn_settings.activeInHierarchy == true) { buttonAnimation(0); } //execute if active (required to avoid error between menus)
        }
        else { yield return null; }
    }

    private void buttonAnimation(int keyOrMouse) //0 = keyboard || 1 = mouse opt 1 || 2 = mouse opt 2 || 3 = mouse opt 3 || ...
    {
        if (is_held == true) //only run when button is held
        {
            //btn index boundaries
            if (ctr_nav.y == 1) //UP
            {
                selected_btn--;
                if (selected_btn < 0) { selected_btn = 0; }
                else { menu_sfx_2.Play(); } //play sfx
                selected_btn_obj = button_list[selected_btn]; //update the selected btn object
            }
            else if (ctr_nav.y == -1) //DOWN
            {
                selected_btn++;
                if (selected_btn > (button_list.Count-1)) { selected_btn = (button_list.Count-1); }
                else { menu_sfx_2.Play(); } //play sfx
                selected_btn_obj = button_list[selected_btn]; //update the selected btn object
            }
        }

        switch (keyOrMouse)
        {
            case 0:
                switch (selected_btn)
                {
                    case 0: //btn_Audio
                        sequence_opt = DOTween.Sequence();
                        sequence_opt.Append(obj_btn_audio.GetComponent<Image>().DOFade(alpha_in, fade_time))
                            .Insert(0, obj_btn_video.GetComponent<Image>().DOFade(alpha_out, fade_time))
                            .Insert(0, obj_btn_controls.GetComponent<Image>().DOFade(alpha_out, fade_time))
                            .Insert(0, obj_btn_language.GetComponent<Image>().DOFade(alpha_out, fade_time));
                        uid_opt = System.Guid.NewGuid();
                        sequence_opt.id = uid_opt;
                        sequence_opt.Play();
                        txt_menu.text = "AUDIO OPTIONS";
                    break;

                    case 1: //btn_Video
                        sequence_opt = DOTween.Sequence();
                        sequence_opt.Append(obj_btn_video.GetComponent<Image>().DOFade(alpha_in, fade_time))
                            .Insert(0, obj_btn_audio.GetComponent<Image>().DOFade(alpha_out, fade_time))
                            .Insert(0, obj_btn_controls.GetComponent<Image>().DOFade(alpha_out, fade_time))
                            .Insert(0, obj_btn_language.GetComponent<Image>().DOFade(alpha_out, fade_time));
                        uid_opt = System.Guid.NewGuid();
                        sequence_opt.id = uid_opt;
                        sequence_opt.Play();
                        txt_menu.text = "VIDEO OPTIONS";
                    break;

                    case 2: //btn_Controls
                        sequence_opt = DOTween.Sequence();
                        sequence_opt.Append(obj_btn_controls.GetComponent<Image>().DOFade(alpha_in, fade_time))
                            .Insert(0, obj_btn_audio.GetComponent<Image>().DOFade(alpha_out, fade_time))
                            .Insert(0, obj_btn_video.GetComponent<Image>().DOFade(alpha_out, fade_time))
                            .Insert(0, obj_btn_language.GetComponent<Image>().DOFade(alpha_out, fade_time));
                        uid_opt = System.Guid.NewGuid();
                        sequence_opt.id = uid_opt;
                        sequence_opt.Play();
                        txt_menu.text = "CONTROLS";
                    break;

                    case 3: //btn_Language
                        sequence_opt = DOTween.Sequence();
                        sequence_opt.Append(obj_btn_language.GetComponent<Image>().DOFade(alpha_in, fade_time))
                            .Insert(0, obj_btn_audio.GetComponent<Image>().DOFade(alpha_out, fade_time))
                            .Insert(0, obj_btn_video.GetComponent<Image>().DOFade(alpha_out, fade_time))
                            .Insert(0, obj_btn_controls.GetComponent<Image>().DOFade(alpha_out, fade_time));
                        uid_opt = System.Guid.NewGuid();
                        sequence_opt.id = uid_opt;
                        sequence_opt.Play();
                        txt_menu.text = "LANGUAGE OPTIONS";
                    break;

                    default:
                    break;
                }
                if (is_held == true) { StartCoroutine(crt_btn_held_repeater()); }
            break;

            case 1: //btn_Audio
                selected_btn = 0;
                selected_btn_obj = button_list[selected_btn]; //update the selected btn object

                sequence_opt = DOTween.Sequence();
                sequence_opt.Append(obj_btn_audio.GetComponent<Image>().DOFade(alpha_in, fade_time))
                    .Insert(0, obj_btn_video.GetComponent<Image>().DOFade(alpha_out, fade_time))
                    .Insert(0, obj_btn_controls.GetComponent<Image>().DOFade(alpha_out, fade_time))
                    .Insert(0, obj_btn_language.GetComponent<Image>().DOFade(alpha_out, fade_time));
                uid_opt = System.Guid.NewGuid();
                sequence_opt.id = uid_opt;
                sequence_opt.Play();
                menu_sfx_2.Play(); //play sfx
                txt_menu.text = "AUDIO OPTIONS";
            break;

            case 2: //btn_Video
                selected_btn = 1;
                selected_btn_obj = button_list[selected_btn]; //update the selected btn object

                sequence_opt = DOTween.Sequence();
                sequence_opt.Append(obj_btn_video.GetComponent<Image>().DOFade(alpha_in, fade_time))
                    .Insert(0, obj_btn_audio.GetComponent<Image>().DOFade(alpha_out, fade_time))
                    .Insert(0, obj_btn_controls.GetComponent<Image>().DOFade(alpha_out, fade_time))
                    .Insert(0, obj_btn_language.GetComponent<Image>().DOFade(alpha_out, fade_time));
                uid_opt = System.Guid.NewGuid();
                sequence_opt.id = uid_opt;
                sequence_opt.Play();
                menu_sfx_2.Play(); //play sfx
                txt_menu.text = "VIDEO OPTIONS";
            break;

            case 3: //btn_Controls
                selected_btn = 2;
                selected_btn_obj = button_list[selected_btn]; //update the selected btn object

                sequence_opt = DOTween.Sequence();
                sequence_opt.Append(obj_btn_controls.GetComponent<Image>().DOFade(alpha_in, fade_time))
                    .Insert(0, obj_btn_audio.GetComponent<Image>().DOFade(alpha_out, fade_time))
                    .Insert(0, obj_btn_video.GetComponent<Image>().DOFade(alpha_out, fade_time))
                    .Insert(0, obj_btn_language.GetComponent<Image>().DOFade(alpha_out, fade_time));
                uid_opt = System.Guid.NewGuid();
                sequence_opt.id = uid_opt;
                sequence_opt.Play();
                menu_sfx_2.Play(); //play sfx
                txt_menu.text = "CONTROLS";
            break;

            case 4: //btn_Language
                selected_btn = 3;
                selected_btn_obj = button_list[selected_btn]; //update the selected btn object

                sequence_opt = DOTween.Sequence();
                sequence_opt.Append(obj_btn_language.GetComponent<Image>().DOFade(alpha_in, fade_time))
                    .Insert(0, obj_btn_audio.GetComponent<Image>().DOFade(alpha_out, fade_time))
                    .Insert(0, obj_btn_video.GetComponent<Image>().DOFade(alpha_out, fade_time))
                    .Insert(0, obj_btn_controls.GetComponent<Image>().DOFade(alpha_out, fade_time));
                uid_opt = System.Guid.NewGuid();
                sequence_opt.id = uid_opt;
                sequence_opt.Play();
                menu_sfx_2.Play(); //play sfx
                txt_menu.text = "LANGUAGE OPTIONS";
            break;

            default:
            break;
        }
    }

    public void test1() { Debug.Log("btn_Audio was the real selection"); }    //DEBUG
    public void test2() { Debug.Log("btn_Video was the real selection"); }    //DEBUG
    public void test3() { Debug.Log("btn_Controls was the real selection"); } //DEBUG
    public void test4() { Debug.Log("btn_Language was the real selection"); } //DEBUG
}
