using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class scr_mn_root : MonoBehaviour
{
    //Menu SFX
    public AudioSource menu_sfx_1;
    public AudioSource menu_sfx_2;
    public AudioSource menu_sfx_3;

    //button object reference
    public GameObject obj_btn_NewGame;
    public GameObject obj_btn_LoadGame;
    public GameObject obj_btn_Options;
    public GameObject obj_btn_extras;
    public GameObject obj_btn_Quit;

    //menu object reference
    public GameObject obj_mn_root;
    public GameObject obj_mn_NewGame;
    public GameObject obj_mn_LoadGame;
    public GameObject obj_mn_Settings;
    public GameObject obj_mn_extras;
    public GameObject obj_mn_quit;

    public TMPro.TMP_Text txt_menu;

    public int save_file_choice; //temporary, will be managed by a value on save file //DEBUG
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
    public GameObject obj_crossfade;

    private void Start()
    {
        button_list = new List<GameObject>(); //clear the button list when we leave the menu
        save_file_choice = globalVars.save_file_choice; //temporary, will be managed by a value on save file //DEBUG
        saveFileCheck(); //arranges the buttons according to save file
    }

    private void saveFileCheck()
    {
        //button selection
        selected_btn_obj = obj_btn_NewGame;
        selected_btn = 0;

        //button index list
        button_list.Add(obj_btn_NewGame);  //0
        button_list.Add(obj_btn_LoadGame); //1
        button_list.Add(obj_btn_Options);  //2
        button_list.Add(obj_btn_extras);   //3
        button_list.Add(obj_btn_Quit);     //4

        ///button selection (btn_NewGame)
        sequence_opt = DOTween.Sequence();
        sequence_opt.Append(obj_btn_NewGame.GetComponent<Image>().DOFade(alpha_in, fade_time))
            .Insert(0, obj_btn_LoadGame.GetComponent<Image>().DOFade(alpha_out, fade_time))
            .Insert(0, obj_btn_Options.GetComponent<Image>().DOFade(alpha_out, fade_time))
            .Insert(0, obj_btn_extras.GetComponent<Image>().DOFade(alpha_out, fade_time))
            .Insert(0, obj_btn_Quit.GetComponent<Image>().DOFade(alpha_out, fade_time));
        uid_opt = System.Guid.NewGuid();
        sequence_opt.id = uid_opt;
        sequence_opt.Play();

        txt_menu.text = "NEW GAME";
    }

    ///animations
    //mouse hover triggers
    public void nav_trigger_mouse1() { buttonAnimation(1); }   //btn_NewGame
    public void nav_trigger_mouse2() { buttonAnimation(2); }   //btn_LoadGame
    public void nav_trigger_mouse5() { buttonAnimation(3); }   //btn_Options
    public void nav_trigger_mouse6() { buttonAnimation(4); }   //btn_Special
    public void nav_trigger_mouse7() { buttonAnimation(5); }   //btn_Quit

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

            if (obj_mn_root.activeInHierarchy == true) { buttonAnimation(0); } //execute if active (required to avoid error between menus)
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
                case 0: //btn_NewGame
                    menu_sfx_3.Play(); //play SFX
                    obj_mn_root.SetActive(false); obj_mn_NewGame.SetActive(true); test1();
                break;

                case 1: //btn_LoadGame
                    menu_sfx_3.Play(); //play SFX
                    obj_mn_LoadGame.SetActive(true); obj_mn_root.SetActive(false); test2();
                break;

                case 2: //btn_options
                    menu_sfx_3.Play(); //play SFX
                    obj_mn_Settings.SetActive(true); obj_mn_root.SetActive(false); test3();
                break;

                case 3: //btn_extras
                    menu_sfx_3.Play(); //play SFX
                    obj_mn_extras.SetActive(true); obj_mn_root.SetActive(false); test4();
                break;

                case 4: //btn_quit
                    menu_sfx_3.Play(); //play SFX
                    obj_mn_quit.SetActive(true); obj_mn_root.SetActive(false); test5();
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
            StartCoroutine(switch_scenes("scn_titlescreen"));
        }
    }

    private IEnumerator crt_timer()
    {
        yield return new WaitForSeconds(0.4f); //1st delay
        is_held = true;
        if (obj_mn_root.activeInHierarchy == true) { buttonAnimation(0); } //execute if active (required to avoid error between menus)
    }

    private IEnumerator crt_btn_held_repeater()
    {
        if (is_held == true)
        {
            yield return new WaitForSeconds(0.25f); //delay after each entry
            if (obj_mn_root.activeInHierarchy == true) { buttonAnimation(0); } //execute if active (required to avoid error between menus)
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
                    case 0: //btn_NewGame
                        sequence_opt = DOTween.Sequence();
                        sequence_opt.Append(obj_btn_NewGame.GetComponent<Image>().DOFade(alpha_in, fade_time))
                            .Insert(0, obj_btn_LoadGame.GetComponent<Image>().DOFade(alpha_out, fade_time))
                            .Insert(0, obj_btn_Options.GetComponent<Image>().DOFade(alpha_out, fade_time))
                            .Insert(0, obj_btn_extras.GetComponent<Image>().DOFade(alpha_out, fade_time))
                            .Insert(0, obj_btn_Quit.GetComponent<Image>().DOFade(alpha_out, fade_time));
                        uid_opt = System.Guid.NewGuid();
                        sequence_opt.id = uid_opt;
                        sequence_opt.Play();
                        txt_menu.text = "NEW GAME";
                    break;

                    case 1: //btn_LoadGame
                        sequence_opt = DOTween.Sequence();
                        sequence_opt.Append(obj_btn_LoadGame.GetComponent<Image>().DOFade(alpha_in, fade_time))
                            .Insert(0, obj_btn_NewGame.GetComponent<Image>().DOFade(alpha_out, fade_time))
                            .Insert(0, obj_btn_Options.GetComponent<Image>().DOFade(alpha_out, fade_time))
                            .Insert(0, obj_btn_extras.GetComponent<Image>().DOFade(alpha_out, fade_time))
                            .Insert(0, obj_btn_Quit.GetComponent<Image>().DOFade(alpha_out, fade_time));
                        uid_opt = System.Guid.NewGuid();
                        sequence_opt.id = uid_opt;
                        sequence_opt.Play();
                        txt_menu.text = "LOAD GAME";
                    break;

                    case 2: //btn_options
                        sequence_opt = DOTween.Sequence();
                        sequence_opt.Append(obj_btn_Options.GetComponent<Image>().DOFade(alpha_in, fade_time))
                            .Insert(0, obj_btn_NewGame.GetComponent<Image>().DOFade(alpha_out, fade_time))
                            .Insert(0, obj_btn_LoadGame.GetComponent<Image>().DOFade(alpha_out, fade_time))
                            .Insert(0, obj_btn_extras.GetComponent<Image>().DOFade(alpha_out, fade_time))
                            .Insert(0, obj_btn_Quit.GetComponent<Image>().DOFade(alpha_out, fade_time));
                        uid_opt = System.Guid.NewGuid();
                        sequence_opt.id = uid_opt;
                        sequence_opt.Play();
                        txt_menu.text = "OPTIONS MODE";
                    break;

                    case 3: //btn_Special
                        sequence_opt = DOTween.Sequence();
                        sequence_opt.Append(obj_btn_extras.GetComponent<Image>().DOFade(alpha_in, fade_time))
                            .Insert(0, obj_btn_NewGame.GetComponent<Image>().DOFade(alpha_out, fade_time))
                            .Insert(0, obj_btn_LoadGame.GetComponent<Image>().DOFade(alpha_out, fade_time))
                            .Insert(0, obj_btn_Options.GetComponent<Image>().DOFade(alpha_out, fade_time))
                            .Insert(0, obj_btn_Quit.GetComponent<Image>().DOFade(alpha_out, fade_time));
                        uid_opt = System.Guid.NewGuid();
                        sequence_opt.id = uid_opt;
                        sequence_opt.Play();
                        txt_menu.text = "SPECIAL MODE";
                    break;

                    case 4: //btn_Quit
                        sequence_opt = DOTween.Sequence();
                        sequence_opt = DOTween.Sequence();
                        sequence_opt.Append(obj_btn_Quit.GetComponent<Image>().DOFade(alpha_in, fade_time))
                            .Insert(0, obj_btn_NewGame.GetComponent<Image>().DOFade(alpha_out, fade_time))
                            .Insert(0, obj_btn_LoadGame.GetComponent<Image>().DOFade(alpha_out, fade_time))
                            .Insert(0, obj_btn_Options.GetComponent<Image>().DOFade(alpha_out, fade_time))
                            .Insert(0, obj_btn_extras.GetComponent<Image>().DOFade(alpha_out, fade_time));
                        uid_opt = System.Guid.NewGuid();
                        sequence_opt.id = uid_opt;
                        sequence_opt.Play();
                        txt_menu.text = "QUIT GAME";
                    break;

                    default:
                    break;
                }
                if (is_held == true) { StartCoroutine(crt_btn_held_repeater()); }
            break;

            case 1: //btn_NewGame
                selected_btn = 0;
                selected_btn_obj = button_list[selected_btn]; //update the selected btn object

                sequence_opt = DOTween.Sequence();
                sequence_opt.Append(obj_btn_NewGame.GetComponent<Image>().DOFade(alpha_in, fade_time))
                    .Insert(0, obj_btn_LoadGame.GetComponent<Image>().DOFade(alpha_out, fade_time))
                    .Insert(0, obj_btn_Options.GetComponent<Image>().DOFade(alpha_out, fade_time))
                    .Insert(0, obj_btn_extras.GetComponent<Image>().DOFade(alpha_out, fade_time))
                    .Insert(0, obj_btn_Quit.GetComponent<Image>().DOFade(alpha_out, fade_time));
                uid_opt = System.Guid.NewGuid();
                sequence_opt.id = uid_opt;
                sequence_opt.Play();
                menu_sfx_2.Play(); //play sfx
                txt_menu.text = "NEW GAME";
            break;

            case 2: //btn_LoadGame
                selected_btn = 1;
                selected_btn_obj = button_list[selected_btn]; //update the selected btn object

                sequence_opt = DOTween.Sequence();
                sequence_opt.Append(obj_btn_LoadGame.GetComponent<Image>().DOFade(alpha_in, fade_time))
                    .Insert(0, obj_btn_NewGame.GetComponent<Image>().DOFade(alpha_out, fade_time))
                    .Insert(0, obj_btn_Options.GetComponent<Image>().DOFade(alpha_out, fade_time))
                    .Insert(0, obj_btn_extras.GetComponent<Image>().DOFade(alpha_out, fade_time))
                    .Insert(0, obj_btn_Quit.GetComponent<Image>().DOFade(alpha_out, fade_time));
                uid_opt = System.Guid.NewGuid();
                sequence_opt.id = uid_opt;
                sequence_opt.Play();
                menu_sfx_2.Play(); //play sfx
                txt_menu.text = "LOAD GAME";
            break;

            case 3: //btn_options
                selected_btn = 2;
                selected_btn_obj = button_list[selected_btn]; //update the selected btn object

                sequence_opt = DOTween.Sequence();
                sequence_opt.Append(obj_btn_Options.GetComponent<Image>().DOFade(alpha_in, fade_time))
                    .Insert(0, obj_btn_NewGame.GetComponent<Image>().DOFade(alpha_out, fade_time))
                    .Insert(0, obj_btn_LoadGame.GetComponent<Image>().DOFade(alpha_out, fade_time))
                    .Insert(0, obj_btn_extras.GetComponent<Image>().DOFade(alpha_out, fade_time))
                    .Insert(0, obj_btn_Quit.GetComponent<Image>().DOFade(alpha_out, fade_time));
                uid_opt = System.Guid.NewGuid();
                sequence_opt.id = uid_opt;
                sequence_opt.Play();
                menu_sfx_2.Play(); //play sfx
                txt_menu.text = "OPTIONS MODE";
            break;

            case 4: //btn_Special
                selected_btn = 3;
                selected_btn_obj = button_list[selected_btn]; //update the selected btn object

                sequence_opt = DOTween.Sequence();
                sequence_opt.Append(obj_btn_extras.GetComponent<Image>().DOFade(alpha_in, fade_time))
                    .Insert(0, obj_btn_NewGame.GetComponent<Image>().DOFade(alpha_out, fade_time))
                    .Insert(0, obj_btn_LoadGame.GetComponent<Image>().DOFade(alpha_out, fade_time))
                    .Insert(0, obj_btn_Options.GetComponent<Image>().DOFade(alpha_out, fade_time))
                    .Insert(0, obj_btn_Quit.GetComponent<Image>().DOFade(alpha_out, fade_time));
                uid_opt = System.Guid.NewGuid();
                sequence_opt.id = uid_opt;
                sequence_opt.Play();
                menu_sfx_2.Play(); //play sfx
                txt_menu.text = "SPECIAL MODE";
            break;

            case 5: //btn_Quit
                selected_btn = 4;
                selected_btn_obj = button_list[selected_btn]; //update the selected btn object

                sequence_opt = DOTween.Sequence();
                sequence_opt.Append(obj_btn_Quit.GetComponent<Image>().DOFade(alpha_in, fade_time))
                    .Insert(0, obj_btn_NewGame.GetComponent<Image>().DOFade(alpha_out, fade_time))
                    .Insert(0, obj_btn_LoadGame.GetComponent<Image>().DOFade(alpha_out, fade_time))
                    .Insert(0, obj_btn_Options.GetComponent<Image>().DOFade(alpha_out, fade_time))
                    .Insert(0, obj_btn_extras.GetComponent<Image>().DOFade(alpha_out, fade_time));
                uid_opt = System.Guid.NewGuid();
                sequence_opt.id = uid_opt;
                sequence_opt.Play();
                menu_sfx_2.Play(); //play sfx
                txt_menu.text = "QUIT GAME";
            break;

            default:
            break;
        }
    }

    public void test1() { Debug.Log("btn_NewGame was the real selection"); }  //DEBUG
    public void test2() { Debug.Log("btn_LoadGame was the real selection"); } //DEBUG
    public void test3() { Debug.Log("btn_Options was the real selection"); }  //DEBUG
    public void test4() { Debug.Log("btn_Extras was the real selection"); }   //DEBUG
    public void test5() { Debug.Log("btn_Quit was the real selection"); }     //DEBUG

    private IEnumerator switch_scenes(string scene_n)
    {
        yield return new WaitForSeconds(0.15f);
        obj_crossfade.GetComponent<scr_fade_levelLoader>().scene_name = scene_n;
        scr_fade_levelLoader.make_fade_black_on_startup = true; //make the fade be of alpha 1 on startup of next scene
        yield return obj_crossfade.GetComponent<scr_fade_levelLoader>().sceneSwitchFadeIn(0.5f, false, Ease.InCubic);
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(scene_n);
    }
}
