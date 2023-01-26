using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class scr_mn_NewGame : MonoBehaviour
{
    //Menu SFX
    public AudioSource menu_sfx_1;
    public AudioSource menu_sfx_2;
    public AudioSource menu_sfx_3;

    //button object reference
    public GameObject obj_btn_easy;
    public GameObject obj_btn_normal;
    public GameObject obj_btn_hard;
    public GameObject obj_btn_extreme;

    //menu object reference
    public GameObject obj_mn_NewGame;
    public GameObject obj_mn_root;

    public TMPro.TMP_Text txt_button_context;
    public GameObject obj_crossfade;

    private int save_file_choice = globalVars.save_file_choice; //temporary, will be managed by a value on save file //DEBUG
    private string selected_game_type;
    private Vector2 ctr_nav;
    private int selected_btn;
    private bool is_held = false;
    private List<GameObject> button_list = new List<GameObject>();
    private GameObject selected_btn_obj;
    private Guid uid_opt;
    private Sequence sequence_opt;
    private float alpha_in = 0.4313726f; //alpha 110
    private float alpha_out = 0;
    private float fade_time = 0.15f;

    private void OnEnable()
    {
        button_list = new List<GameObject>(); //clear the button list when we leave the menu
        selected_game_type = globalVars.game_type;
        gameTypeCheck();
    }

    private void gameTypeCheck()
    {
        //button selection
        selected_btn_obj = obj_btn_normal;
        selected_btn = 1;
        
        //button index list
        button_list.Add(obj_btn_easy);    //0
        button_list.Add(obj_btn_normal);  //1
        button_list.Add(obj_btn_hard);    //2
        button_list.Add(obj_btn_extreme); //3
        
        //button visibility
        obj_btn_easy.SetActive(true);
        obj_btn_normal.SetActive(true);
        obj_btn_hard.SetActive(true);
        obj_btn_extreme.SetActive(true);
        
        ///button selection
        sequence_opt = DOTween.Sequence();
        sequence_opt.Append(obj_btn_normal.GetComponent<Image>().DOFade(alpha_in, fade_time))
            .Insert(0, obj_btn_easy.GetComponent<Image>().DOFade(alpha_out, fade_time))
            .Insert(0, obj_btn_hard.GetComponent<Image>().DOFade(alpha_out, fade_time))
            .Insert(0, obj_btn_extreme.GetComponent<Image>().DOFade(alpha_out, fade_time));
        uid_opt = System.Guid.NewGuid();
        sequence_opt.id = uid_opt;
        sequence_opt.Play();
        txt_button_context.text = "Standard difficulty level.";
    }

    public void nav_trigger_mouse2() { buttonAnimation(1); } //btn_easy
    public void nav_trigger_mouse3() { buttonAnimation(2); } //btn_normal
    public void nav_trigger_mouse4() { buttonAnimation(3); } //btn_hard
    public void nav_trigger_mouse5() { buttonAnimation(4); } //btn_extreme

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

            if (obj_mn_NewGame.activeInHierarchy == true) { buttonAnimation(0); } //execute if active (required to avoid error between menus)
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
                case 0: //btn_easy
                    globalVars.game_type = "easy";
                    menu_sfx_3.Play(); //play SFX
                    easy();
                break;

                case 1: //btn_normal
                    globalVars.game_type = "normal";
                    menu_sfx_3.Play(); //play SFX
                    normal();
                break;

                case 2: //btn_hard
                    globalVars.game_type = "hard";
                    menu_sfx_3.Play(); //play SFX
                    hard();
                break;

                case 3: //btn_extreme
                    globalVars.game_type = "extreme";
                    menu_sfx_3.Play(); //play SFX
                    extreme();
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
            obj_mn_NewGame.SetActive(false);
            obj_mn_root.SetActive(true);
        }
    }

    private IEnumerator crt_timer()
    {
        yield return new WaitForSeconds(0.4f); //1st delay
        is_held = true;
        if (obj_mn_NewGame.activeInHierarchy == true) { buttonAnimation(0); } //execute if active (required to avoid error between menus)
    }

    private IEnumerator crt_btn_held_repeater()
    {
        if (is_held == true)
        {
            yield return new WaitForSeconds(0.25f); //delay after each entry
            if (obj_mn_NewGame.activeInHierarchy == true) { buttonAnimation(0); } //execute if active (required to avoid error between menus)
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
        }

        switch (keyOrMouse)
        {
            case 0:
                switch (selected_btn)
                {
                    case 0: //btn_easy
                        sequence_opt = DOTween.Sequence();
                        sequence_opt.Append(obj_btn_easy.GetComponent<Image>().DOFade(alpha_in, fade_time))
                            .Insert(0, obj_btn_normal.GetComponent<Image>().DOFade(alpha_out, fade_time))
                            .Insert(0, obj_btn_hard.GetComponent<Image>().DOFade(alpha_out, fade_time))
                            .Insert(0, obj_btn_extreme.GetComponent<Image>().DOFade(alpha_out, fade_time));
                        uid_opt = System.Guid.NewGuid();
                        sequence_opt.id = uid_opt;
                        sequence_opt.Play();
                        txt_button_context.text = "For non action-experts.";
                    break;

                    case 1: //btn_normal
                        sequence_opt = DOTween.Sequence();
                        sequence_opt.Append(obj_btn_normal.GetComponent<Image>().DOFade(alpha_in, fade_time))
                            .Insert(0, obj_btn_easy.GetComponent<Image>().DOFade(alpha_out, fade_time))
                            .Insert(0, obj_btn_hard.GetComponent<Image>().DOFade(alpha_out, fade_time))
                            .Insert(0, obj_btn_extreme.GetComponent<Image>().DOFade(alpha_out, fade_time));
                        uid_opt = System.Guid.NewGuid();
                        sequence_opt.id = uid_opt;
                        sequence_opt.Play();
                        txt_button_context.text = "Standard difficulty level.";
                    break;

                    case 2: //btn_hard
                        sequence_opt = DOTween.Sequence();
                        sequence_opt.Append(obj_btn_hard.GetComponent<Image>().DOFade(alpha_in, fade_time))
                            .Insert(0, obj_btn_easy.GetComponent<Image>().DOFade(alpha_out, fade_time))
                            .Insert(0, obj_btn_normal.GetComponent<Image>().DOFade(alpha_out, fade_time))
                            .Insert(0, obj_btn_extreme.GetComponent<Image>().DOFade(alpha_out, fade_time));
                        uid_opt = System.Guid.NewGuid();
                        sequence_opt.id = uid_opt;
                        sequence_opt.Play();
                        txt_button_context.text = "For confident players.";
                    break;

                    case 3: //btn_extreme
                        sequence_opt = DOTween.Sequence();
                        sequence_opt.Append(obj_btn_extreme.GetComponent<Image>().DOFade(alpha_in, fade_time))
                            .Insert(0, obj_btn_easy.GetComponent<Image>().DOFade(alpha_out, fade_time))
                            .Insert(0, obj_btn_normal.GetComponent<Image>().DOFade(alpha_out, fade_time))
                            .Insert(0, obj_btn_hard.GetComponent<Image>().DOFade(alpha_out, fade_time));
                        uid_opt = System.Guid.NewGuid();
                        sequence_opt.id = uid_opt;
                        sequence_opt.Play();
                        txt_button_context.text = "For action game experts.";
                    break;

                    default:
                    break;
                }
                if (is_held == true) { StartCoroutine(crt_btn_held_repeater()); }
            break;

            case 1: //btn_easy
                selected_btn = 0;
                selected_btn_obj = button_list[selected_btn]; //update the selected btn object

                sequence_opt = DOTween.Sequence();
                sequence_opt.Append(obj_btn_easy.GetComponent<Image>().DOFade(alpha_in, fade_time))
                    .Insert(0, obj_btn_normal.GetComponent<Image>().DOFade(alpha_out, fade_time))
                    .Insert(0, obj_btn_hard.GetComponent<Image>().DOFade(alpha_out, fade_time))
                    .Insert(0, obj_btn_extreme.GetComponent<Image>().DOFade(alpha_out, fade_time));
                uid_opt = System.Guid.NewGuid();
                sequence_opt.id = uid_opt;
                sequence_opt.Play();
                menu_sfx_2.Play(); //play sfx
                txt_button_context.text = "For non action-experts.";
            break;

            case 2: //btn_normal
                selected_btn = 0;
                selected_btn_obj = button_list[selected_btn]; //update the selected btn object

                sequence_opt = DOTween.Sequence();
                sequence_opt.Append(obj_btn_normal.GetComponent<Image>().DOFade(alpha_in, fade_time))
                    .Insert(0, obj_btn_easy.GetComponent<Image>().DOFade(alpha_out, fade_time))
                    .Insert(0, obj_btn_hard.GetComponent<Image>().DOFade(alpha_out, fade_time))
                    .Insert(0, obj_btn_extreme.GetComponent<Image>().DOFade(alpha_out, fade_time));
                uid_opt = System.Guid.NewGuid();
                sequence_opt.id = uid_opt;
                sequence_opt.Play();
                menu_sfx_2.Play(); //play sfx
                txt_button_context.text = "Standard difficulty level.";
            break;

            case 3: //btn_hard
                selected_btn = 0;
                selected_btn_obj = button_list[selected_btn]; //update the selected btn object

                sequence_opt = DOTween.Sequence();
                sequence_opt.Append(obj_btn_hard.GetComponent<Image>().DOFade(alpha_in, fade_time))
                    .Insert(0, obj_btn_easy.GetComponent<Image>().DOFade(alpha_out, fade_time))
                    .Insert(0, obj_btn_normal.GetComponent<Image>().DOFade(alpha_out, fade_time))
                    .Insert(0, obj_btn_extreme.GetComponent<Image>().DOFade(alpha_out, fade_time));
                uid_opt = System.Guid.NewGuid();
                sequence_opt.id = uid_opt;
                sequence_opt.Play();
                menu_sfx_2.Play(); //play sfx
                txt_button_context.text = "For confident players.";
            break;

            case 4: //btn_extreme
                selected_btn = 0;
                selected_btn_obj = button_list[selected_btn]; //update the selected btn object

                sequence_opt = DOTween.Sequence();
                sequence_opt.Append(obj_btn_extreme.GetComponent<Image>().DOFade(alpha_in, fade_time))
                    .Insert(0, obj_btn_easy.GetComponent<Image>().DOFade(alpha_out, fade_time))
                    .Insert(0, obj_btn_normal.GetComponent<Image>().DOFade(alpha_out, fade_time))
                    .Insert(0, obj_btn_hard.GetComponent<Image>().DOFade(alpha_out, fade_time));
                uid_opt = System.Guid.NewGuid();
                sequence_opt.id = uid_opt;
                sequence_opt.Play();
                menu_sfx_2.Play(); //play sfx
                txt_button_context.text = "For action game experts.";
            break;

            default:
            break;
        }
    }

    public void easy()
    {
        Debug.Log("scn_Game loaded in 'Easy' Difficulty");
        StartCoroutine(switch_scenes("scn_Game"));
    }

    public void normal()
    {
        Debug.Log("scn_Game loaded in 'Normal' Difficulty");
        StartCoroutine(switch_scenes("scn_Game"));
    }

    public void hard()
    {
        Debug.Log("scn_Game loaded in 'Hard' Difficulty");
        StartCoroutine(switch_scenes("scn_Game"));
    }

    public void extreme()
    {
        Debug.Log("scn_Game loaded in 'Extreme' Difficulty");
        StartCoroutine(switch_scenes("scn_Game"));
    }

    private IEnumerator switch_scenes(string scene_n)
    {
        yield return new WaitForSeconds(0.15f);
        obj_crossfade.GetComponent<scr_fade_levelLoader>().scene_name = scene_n;
        scr_fade_levelLoader.make_fade_black_on_startup = true; //make the fade be of alpha 1 on startup of next scene
        yield return obj_crossfade.GetComponent<scr_fade_levelLoader>().sceneSwitchFadeIn(0.5f, false, Ease.InCubic);
        yield return new WaitForSeconds(3.0f);
        SceneManager.LoadScene(scene_n);
    }
}
