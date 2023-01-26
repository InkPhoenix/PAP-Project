using System;
using System.Linq;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class scr_mn_LoadGame : MonoBehaviour
{
    public static scr_mn_LoadGame instance;
    private void Awake() { instance = this; }

    public GameObject obj_mn_root;
    public GameObject obj_mn_LoadGame;
    public GameObject save_UI_prefab;
    public GameObject obj_ara_saves;
    public GameObject obj_content;
    private GameObject button_instances;

    public TMPro.TMP_Text obj_txt_pages;
    public TMPro.TMP_Text obj_txt_no_saves;
    public GameObject obj_spr_selectedBTN;
    public InputActionAsset ctr_controls;

    private Vector2 ctr_nav;
    private Guid uid_opt;
    private Sequence sequence_opt;
    private date_order last_modified_file;

    //arrow sprites
    public GameObject obj_spr_arrow_left;
    public GameObject obj_spr_arrow_right;

    public TMPro.TMP_Text obj_txt_test1;    //DEBUG
    public TMPro.TMP_Text obj_txt_test2;    //DEBUG
    public TMPro.TMP_Text obj_txt_test3;    //DEBUG
    public TMPro.TMP_Text obj_txt_playtime; //DEBUG

    //Menu SFX
    public AudioSource menu_sfx_1;
    public AudioSource menu_sfx_2;
    public AudioSource menu_sfx_3;

    //navigation values
    int selected_btn;
    int index_selected;
    int selected_page;
    double page_num;                   //total number of pages
    int last_page_entries_n;           //number of entries in the last page
    int n_entries;                     //saves_list.Count (number of valid save files)
    const int n_entries_per_page = 10;
    float button_height;
    List<string> saves_list;

    bool is_held = false;

    public void nav_trigger(InputAction.CallbackContext context)
    {
        ctr_nav = context.ReadValue<Vector2>(); //update the navigation value
        Debug.Log("ctr_nav: " + ctr_nav);

        if (context.performed) //key pressed
        {
            Debug.Log("key pressed");
            crt_move();
            StartCoroutine("crt_timer");
        }
        if (context.canceled) //key released
        {
            Debug.Log("key released");
            StopCoroutine("crt_timer");
            is_held = false;
        }
    }

    public void back_trigger(InputAction.CallbackContext context)
    {
        if (context.performed) //key pressed
        {
            menu_sfx_1.Play();
            obj_mn_LoadGame.SetActive(false);
            obj_mn_root.SetActive(true);
        }
    }

    //play SFX (for mouse click)
    public void sfx_play_menu_1() { menu_sfx_1.Play(); }
    public void sfx_play_menu_2() { menu_sfx_2.Play(); }
    public void sfx_play_menu_3() { menu_sfx_3.Play(); }

    private IEnumerator crt_timer()
    {
        yield return new WaitForSeconds(0.4f);
        is_held = true;
        crt_move();
    }

    private IEnumerator crt_btn_held_repeater()
    {
        if (is_held == true)
        {
            yield return new WaitForSeconds(0.07f);
            crt_move();
        }
        else { yield return null; }
    }

    private void crt_move()
    {
        if (ctr_nav.y == 1) //UP
        {
            if (isDivisible(selected_btn-1, n_entries_per_page) == true & selected_page > 1) { selected_page--; } //update page
            //update arrow sprite
            if (page_num <= 1) //only 1 page
            {
                obj_spr_arrow_left.GetComponent<CanvasGroup>().alpha = 0;
                obj_spr_arrow_right.GetComponent<CanvasGroup>().alpha = 0;
            } else {
                if (selected_page == 1 & page_num > 1) //first page
                {
                    obj_spr_arrow_left.GetComponent<CanvasGroup>().alpha = 0;
                    obj_spr_arrow_right.GetComponent<CanvasGroup>().alpha = 1;
                } else {
                    if (selected_page < page_num & selected_page > 1) //in the middle of first and last page
                    {
                        obj_spr_arrow_left.GetComponent<CanvasGroup>().alpha = 1;
                        obj_spr_arrow_right.GetComponent<CanvasGroup>().alpha = 1;
                    } else {
                        if (selected_page == page_num & page_num > 1) //last page
                        {
                            obj_spr_arrow_left.GetComponent<CanvasGroup>().alpha = 1;
                            obj_spr_arrow_right.GetComponent<CanvasGroup>().alpha = 0;
                        }
                    }
                }
            }

            if (selected_btn <= 1) //loop around
            {
                if (is_held == true) {}
                else
                {
                    selected_btn = n_entries;
                    if (n_entries > n_entries_per_page)
                    {
                        menu_sfx_2.Play(); //play sfx
                        index_selected = n_entries_per_page;
                        obj_content.GetComponent<RectTransform>().localPosition = new Vector3(obj_content.GetComponent<RectTransform>().localPosition.x, obj_content.GetComponent<RectTransform>().localPosition.y+((n_entries-n_entries_per_page)*button_height), obj_content.GetComponent<RectTransform>().localPosition.z);

                        //boundary check (obj_content.y)
                        if (obj_content.GetComponent<RectTransform>().localPosition.y < 0) { obj_content.GetComponent<RectTransform>().localPosition = new Vector3(obj_content.GetComponent<RectTransform>().localPosition.x, 0, obj_content.GetComponent<RectTransform>().localPosition.z); }
                        if (obj_content.GetComponent<RectTransform>().localPosition.y > button_height*(n_entries-1)) { obj_content.GetComponent<RectTransform>().localPosition = new Vector3(obj_content.GetComponent<RectTransform>().localPosition.x, button_height*(n_entries-1), obj_content.GetComponent<RectTransform>().localPosition.z); }

                        obj_spr_selectedBTN.GetComponent<RectTransform>().localPosition = new Vector3(obj_spr_selectedBTN.GetComponent<RectTransform>().localPosition.x, obj_spr_selectedBTN.transform.localPosition.y-((n_entries-1)*button_height), obj_spr_selectedBTN.GetComponent<RectTransform>().localPosition.z);
                        selected_page = Convert.ToInt32(page_num);
                    }
                    else
                    {
                        menu_sfx_2.Play(); //play sfx
                        index_selected = n_entries;
                        obj_spr_selectedBTN.GetComponent<RectTransform>().localPosition = new Vector3(obj_spr_selectedBTN.GetComponent<RectTransform>().localPosition.x, obj_spr_selectedBTN.transform.localPosition.y-((n_entries-1)*button_height), obj_spr_selectedBTN.GetComponent<RectTransform>().localPosition.z);
                    }
                }
                //update arrow sprite
                if (page_num <= 1) //only 1 page
                {
                    obj_spr_arrow_left.GetComponent<CanvasGroup>().alpha = 0;
                    obj_spr_arrow_right.GetComponent<CanvasGroup>().alpha = 0;
                } else {
                    if (selected_page == 1 & page_num > 1) //first page
                    {
                        obj_spr_arrow_left.GetComponent<CanvasGroup>().alpha = 0;
                        obj_spr_arrow_right.GetComponent<CanvasGroup>().alpha = 1;
                    } else {
                        if (selected_page < page_num & selected_page > 1) //in the middle of first and last page
                        {
                            obj_spr_arrow_left.GetComponent<CanvasGroup>().alpha = 1;
                            obj_spr_arrow_right.GetComponent<CanvasGroup>().alpha = 1;
                        } else {
                            if (selected_page == page_num & page_num > 1) //last page
                            {
                                obj_spr_arrow_left.GetComponent<CanvasGroup>().alpha = 1;
                                obj_spr_arrow_right.GetComponent<CanvasGroup>().alpha = 0;
                            }
                        }
                    }
                }
            }
            else
            {
                if (index_selected <= 1)
                {
                    index_selected = 1;
                    if (selected_btn <= 1) { selected_btn = 1; }
                    else
                    {
                        menu_sfx_2.Play(); //play sfx
                        selected_btn--;
                        obj_content.GetComponent<RectTransform>().localPosition = new Vector3(obj_content.GetComponent<RectTransform>().localPosition.x, obj_content.GetComponent<RectTransform>().localPosition.y-button_height, obj_content.GetComponent<RectTransform>().localPosition.z);

                        //boundary check (obj_content.y)
                        if (obj_content.GetComponent<RectTransform>().localPosition.y < 0) { obj_content.GetComponent<RectTransform>().localPosition = new Vector3(obj_content.GetComponent<RectTransform>().localPosition.x, 0, obj_content.GetComponent<RectTransform>().localPosition.z); }
                        if (obj_content.GetComponent<RectTransform>().localPosition.y > button_height*(n_entries-1)) { obj_content.GetComponent<RectTransform>().localPosition = new Vector3(obj_content.GetComponent<RectTransform>().localPosition.x, button_height*(n_entries-1), obj_content.GetComponent<RectTransform>().localPosition.z); }

                        obj_spr_selectedBTN.GetComponent<RectTransform>().localPosition = new Vector3(obj_spr_selectedBTN.GetComponent<RectTransform>().localPosition.x, ((button_height/2)-button_height)-((selected_btn-1)*button_height), obj_spr_selectedBTN.GetComponent<RectTransform>().localPosition.z);
                    }
                }
                else
                {
                    menu_sfx_2.Play(); //play sfx
                    index_selected--;
                    if (index_selected < 1) { index_selected = 1; }   //boundary check
                    if (index_selected > 10) { index_selected = 10; } //boundary check
                    selected_btn--;
                    obj_spr_selectedBTN.GetComponent<RectTransform>().DOLocalMoveY(((button_height/2)-button_height)-((selected_btn-1)*button_height), 0.1f);
                }
            }

            obj_txt_pages.text = $"PAGE {selected_page.ToString("00")}/{page_num.ToString("00")}"; //update txt_pages with the current selected page
            obj_txt_test2.text = "ENTRY: " + selected_btn; //DEBUG
            obj_txt_test1.text = "PAGE: " + selected_page;   //DEBUG
            obj_txt_test3.text = "INDEX: " + index_selected; //DEBUG

            if (is_held == true) { StartCoroutine(crt_btn_held_repeater()); }
        }
        else if (ctr_nav.y == -1) //DOWN
        {
            if (isDivisible(selected_btn, n_entries_per_page) == true) { selected_page++; } //update page
            //update arrow sprite
            if (page_num <= 1) //only 1 page
            {
                obj_spr_arrow_left.GetComponent<CanvasGroup>().alpha = 0;
                obj_spr_arrow_right.GetComponent<CanvasGroup>().alpha = 0;
            } else {
                if (selected_page == 1 & page_num > 1) //first page
                {
                    obj_spr_arrow_left.GetComponent<CanvasGroup>().alpha = 0;
                    obj_spr_arrow_right.GetComponent<CanvasGroup>().alpha = 1;
                } else {
                    if (selected_page < page_num & selected_page > 1) //in the middle of first and last page
                    {
                        obj_spr_arrow_left.GetComponent<CanvasGroup>().alpha = 1;
                        obj_spr_arrow_right.GetComponent<CanvasGroup>().alpha = 1;
                    } else {
                        if (selected_page == page_num & page_num > 1) //last page
                        {
                            obj_spr_arrow_left.GetComponent<CanvasGroup>().alpha = 1;
                            obj_spr_arrow_right.GetComponent<CanvasGroup>().alpha = 0;
                        }
                    }
                }
            }

            if (selected_btn >= n_entries) //loop around
            {
                if (is_held == true) {}
                else
                {
                    menu_sfx_2.Play(); //play sfx
                    selected_btn = 1;
                    index_selected = 1;
                    selected_page = 1;
                    obj_content.GetComponent<RectTransform>().localPosition = new Vector3(obj_content.GetComponent<RectTransform>().localPosition.x, 0, obj_content.GetComponent<RectTransform>().localPosition.z);

                    //boundary check (obj_content.y)
                    if (obj_content.GetComponent<RectTransform>().localPosition.y < 0) { obj_content.GetComponent<RectTransform>().localPosition = new Vector3(obj_content.GetComponent<RectTransform>().localPosition.x, 0, obj_content.GetComponent<RectTransform>().localPosition.z); }
                    if (obj_content.GetComponent<RectTransform>().localPosition.y > button_height*(n_entries-1)) { obj_content.GetComponent<RectTransform>().localPosition = new Vector3(obj_content.GetComponent<RectTransform>().localPosition.x, button_height*(n_entries-1), obj_content.GetComponent<RectTransform>().localPosition.z); }

                    obj_spr_selectedBTN.GetComponent<RectTransform>().localPosition = new Vector3(obj_spr_selectedBTN.GetComponent<RectTransform>().localPosition.x, 0-(obj_spr_selectedBTN.GetComponent<RectTransform>().rect.height/2), obj_spr_selectedBTN.GetComponent<RectTransform>().localPosition.z);
                }
                //update arrow sprite
                if (page_num <= 1) //only 1 page
                {
                    obj_spr_arrow_left.GetComponent<CanvasGroup>().alpha = 0;
                    obj_spr_arrow_right.GetComponent<CanvasGroup>().alpha = 0;
                } else {
                    if (selected_page == 1 & page_num > 1) //first page
                    {
                        obj_spr_arrow_left.GetComponent<CanvasGroup>().alpha = 0;
                        obj_spr_arrow_right.GetComponent<CanvasGroup>().alpha = 1;
                    } else {
                        if (selected_page < page_num & selected_page > 1) //in the middle of first and last page
                        {
                            obj_spr_arrow_left.GetComponent<CanvasGroup>().alpha = 1;
                            obj_spr_arrow_right.GetComponent<CanvasGroup>().alpha = 1;
                        } else {
                            if (selected_page == page_num & page_num > 1) //last page
                            {
                                obj_spr_arrow_left.GetComponent<CanvasGroup>().alpha = 1;
                                obj_spr_arrow_right.GetComponent<CanvasGroup>().alpha = 0;
                            }
                        }
                    }
                }
            }
            else
            {
                selected_btn++;
                if (index_selected < n_entries_per_page)
                {
                    menu_sfx_2.Play(); //play sfx
                    index_selected++;
                    if (index_selected < 1) { index_selected = 1; }   //boundary check (index)
                    if (index_selected > 10) { index_selected = 10; } //boundary check (index)
                    obj_spr_selectedBTN.GetComponent<RectTransform>().DOLocalMoveY(((button_height/2)-button_height)-((selected_btn-1)*button_height), 0.1f);
                }
                else
                {
                    menu_sfx_2.Play(); //play sfx
                    obj_content.GetComponent<RectTransform>().localPosition = new Vector3(obj_content.GetComponent<RectTransform>().localPosition.x, obj_content.GetComponent<RectTransform>().localPosition.y + button_height, obj_content.GetComponent<RectTransform>().localPosition.z);

                    //boundary check (obj_content.y)
                    if (obj_content.GetComponent<RectTransform>().localPosition.y < 0) { obj_content.GetComponent<RectTransform>().localPosition = new Vector3(obj_content.GetComponent<RectTransform>().localPosition.x, 0, obj_content.GetComponent<RectTransform>().localPosition.z); }
                    if (obj_content.GetComponent<RectTransform>().localPosition.y > button_height*(n_entries-1)) { obj_content.GetComponent<RectTransform>().localPosition = new Vector3(obj_content.GetComponent<RectTransform>().localPosition.x, button_height*(n_entries-1), obj_content.GetComponent<RectTransform>().localPosition.z); }

                    obj_spr_selectedBTN.GetComponent<RectTransform>().localPosition = new Vector3(obj_spr_selectedBTN.GetComponent<RectTransform>().localPosition.x, ((button_height/2)-button_height)-((selected_btn-1)*button_height), obj_spr_selectedBTN.GetComponent<RectTransform>().localPosition.z);
                }
            }

            obj_txt_pages.text = $"PAGE {selected_page.ToString("00")}/{page_num.ToString("00")}"; //update txt_pages with the current selected page
            obj_txt_test2.text = "ENTRY: " + selected_btn; //DEBUG
            obj_txt_test1.text = "PAGE: " + selected_page;   //DEBUG
            obj_txt_test3.text = "INDEX: " + index_selected; //DEBUG

            if (is_held == true) { StartCoroutine(crt_btn_held_repeater()); }
        }

        if (ctr_nav.x == -1) //LEFT
        {
            if (selected_btn <= 1) {}
            else
            {
                if (obj_content.GetComponent<RectTransform>().localPosition.y <= button_height*(n_entries_per_page-1)) //reset to 1 if page doesn't align
                {
                    menu_sfx_2.Play(); //play sfx
                    obj_content.GetComponent<RectTransform>().localPosition = new Vector3(obj_content.GetComponent<RectTransform>().localPosition.x, 0, obj_content.GetComponent<RectTransform>().localPosition.z);
                    obj_spr_selectedBTN.GetComponent<RectTransform>().localPosition = new Vector3(obj_spr_selectedBTN.GetComponent<RectTransform>().localPosition.x, 0-(obj_spr_selectedBTN.GetComponent<RectTransform>().rect.height/2), obj_spr_selectedBTN.GetComponent<RectTransform>().localPosition.z);
                    selected_btn = 1;
                    index_selected = 1;
                    selected_page = 1;
                    //update arrow sprite
                    if (selected_page == 1 & page_num > 1) //first page
                    {
                        obj_spr_arrow_left.GetComponent<CanvasGroup>().alpha = 0;
                        obj_spr_arrow_right.GetComponent<CanvasGroup>().alpha = 1;
                    }
                }
                else
                {
                    if (selected_page > 1)
                    {
                        menu_sfx_2.Play(); //play sfx
                        selected_page--;
                        selected_btn = selected_btn - n_entries_per_page;
                        obj_content.GetComponent<RectTransform>().localPosition = new Vector3(obj_content.GetComponent<RectTransform>().localPosition.x, obj_content.GetComponent<RectTransform>().localPosition.y-(n_entries_per_page*button_height), obj_content.GetComponent<RectTransform>().localPosition.z);

                        //boundary check (obj_content.y)
                        if (obj_content.GetComponent<RectTransform>().localPosition.y < 0) { obj_content.GetComponent<RectTransform>().localPosition = new Vector3(obj_content.GetComponent<RectTransform>().localPosition.x, 0, obj_content.GetComponent<RectTransform>().localPosition.z); }
                        if (obj_content.GetComponent<RectTransform>().localPosition.y > button_height*(n_entries-1)) { obj_content.GetComponent<RectTransform>().localPosition = new Vector3(obj_content.GetComponent<RectTransform>().localPosition.x, button_height*(n_entries-1), obj_content.GetComponent<RectTransform>().localPosition.z); }

                        obj_spr_selectedBTN.GetComponent<RectTransform>().localPosition = new Vector3(obj_spr_selectedBTN.GetComponent<RectTransform>().localPosition.x, obj_spr_selectedBTN.transform.localPosition.y+(n_entries_per_page*button_height), obj_spr_selectedBTN.GetComponent<RectTransform>().localPosition.z);
                        //update arrow sprite
                        if (page_num <= 1) //only 1 page
                        {
                            obj_spr_arrow_left.GetComponent<CanvasGroup>().alpha = 0;
                            obj_spr_arrow_right.GetComponent<CanvasGroup>().alpha = 0;
                        } else {
                            if (selected_page == 1 & page_num > 1) //first page
                            {
                                obj_spr_arrow_left.GetComponent<CanvasGroup>().alpha = 0;
                                obj_spr_arrow_right.GetComponent<CanvasGroup>().alpha = 1;
                            } else {
                                if (selected_page < page_num & selected_page > 1) //in the middle of first and last page
                                {
                                    obj_spr_arrow_left.GetComponent<CanvasGroup>().alpha = 1;
                                    obj_spr_arrow_right.GetComponent<CanvasGroup>().alpha = 1;
                                } else {
                                    if (selected_page == page_num & page_num > 1) //last page
                                    {
                                        obj_spr_arrow_left.GetComponent<CanvasGroup>().alpha = 1;
                                        obj_spr_arrow_right.GetComponent<CanvasGroup>().alpha = 0;
                                    }
                                }
                            }
                        }
                    }
                }

                obj_txt_pages.text = $"PAGE {selected_page.ToString("00")}/{page_num.ToString("00")}"; //update txt_pages with the current selected page
                obj_txt_test1.text = "PAGE: " + selected_page;   //DEBUG
                obj_txt_test2.text = "ENTRY: " + selected_btn;   //DEBUG
                obj_txt_test3.text = "INDEX: " + index_selected; //DEBUG

                if (is_held == true) { StartCoroutine(crt_btn_held_repeater()); }
            }
        }
        else if (ctr_nav.x == 1) //RIGHT
        {
            if (selected_btn >= n_entries) {} //do nothing if it's the last entry
            else
            {
                if (selected_page == page_num & selected_btn != n_entries) //if it's the last page and it isn't the last entry
                {
                    if (obj_content.GetComponent<RectTransform>().localPosition.y < (page_num-1)*(n_entries_per_page*button_height)) //if content.y is in the last page
                    {
                        menu_sfx_2.Play(); //play sfx
                        if (obj_content.GetComponent<RectTransform>().localPosition.y == button_height*(n_entries-n_entries_per_page)) { obj_content.GetComponent<RectTransform>().localPosition = new Vector3(obj_content.GetComponent<RectTransform>().localPosition.x, button_height*(n_entries-last_page_entries_n), obj_content.GetComponent<RectTransform>().localPosition.z); }
                        else { obj_content.GetComponent<RectTransform>().localPosition = new Vector3(obj_content.GetComponent<RectTransform>().localPosition.x, obj_content.GetComponent<RectTransform>().localPosition.y+(n_entries_per_page*button_height), obj_content.GetComponent<RectTransform>().localPosition.z); }
                        //boundary check (obj_content.y)
                        if (obj_content.GetComponent<RectTransform>().localPosition.y < 0) { obj_content.GetComponent<RectTransform>().localPosition = new Vector3(obj_content.GetComponent<RectTransform>().localPosition.x, 0, obj_content.GetComponent<RectTransform>().localPosition.z); }

                        index_selected = (index_selected+(n_entries-selected_btn))-n_entries_per_page;
                        if (index_selected < 1) { index_selected = 1; }   //boundary check
                        if (index_selected > 10) { index_selected = 10; } //boundary check
                    }
                    else
                    {
                        menu_sfx_2.Play(); //play sfx
                        if (obj_content.GetComponent<RectTransform>().localPosition.y == button_height*(n_entries-n_entries_per_page)) { obj_content.GetComponent<RectTransform>().localPosition = new Vector3(obj_content.GetComponent<RectTransform>().localPosition.x, button_height*(n_entries-last_page_entries_n), obj_content.GetComponent<RectTransform>().localPosition.z); }
                        else { obj_content.GetComponent<RectTransform>().localPosition = new Vector3(obj_content.GetComponent<RectTransform>().localPosition.x, (selected_page-1)*(n_entries_per_page*button_height), obj_content.GetComponent<RectTransform>().localPosition.z); }
                        //boundary check (obj_content.y)
                        if (obj_content.GetComponent<RectTransform>().localPosition.y < 0) { obj_content.GetComponent<RectTransform>().localPosition = new Vector3(obj_content.GetComponent<RectTransform>().localPosition.x, 0, obj_content.GetComponent<RectTransform>().localPosition.z); }

                        index_selected = last_page_entries_n;
                        if (index_selected < 1) { index_selected = 1; }   //boundary check
                        if (index_selected > 10) { index_selected = 10; } //boundary check
                    }
                    obj_spr_selectedBTN.GetComponent<RectTransform>().localPosition = new Vector3(obj_spr_selectedBTN.GetComponent<RectTransform>().localPosition.x, obj_spr_selectedBTN.transform.localPosition.y-button_height*(n_entries-selected_btn), obj_spr_selectedBTN.GetComponent<RectTransform>().localPosition.z);
                    selected_btn = selected_btn+(n_entries-selected_btn);
                }
                if (selected_page < page_num) //if it's not the last page
                {
                    selected_page++;
                    if (selected_btn + n_entries_per_page <= n_entries)
                    {
                        menu_sfx_2.Play(); //play sfx
                        selected_btn = selected_btn + n_entries_per_page;
                        if (obj_content.GetComponent<RectTransform>().localPosition.y == button_height*(n_entries-n_entries_per_page)) { obj_content.GetComponent<RectTransform>().localPosition = new Vector3(obj_content.GetComponent<RectTransform>().localPosition.x, button_height*(n_entries-last_page_entries_n), obj_content.GetComponent<RectTransform>().localPosition.z); }
                        else { obj_content.GetComponent<RectTransform>().localPosition = new Vector3(obj_content.GetComponent<RectTransform>().localPosition.x, obj_content.GetComponent<RectTransform>().localPosition.y + (n_entries_per_page*button_height), obj_content.GetComponent<RectTransform>().localPosition.z); }

                        //boundary check (obj_content.y)
                        if (obj_content.GetComponent<RectTransform>().localPosition.y < 0) { obj_content.GetComponent<RectTransform>().localPosition = new Vector3(obj_content.GetComponent<RectTransform>().localPosition.x, 0, obj_content.GetComponent<RectTransform>().localPosition.z); }

                        obj_spr_selectedBTN.GetComponent<RectTransform>().localPosition = new Vector3(obj_spr_selectedBTN.GetComponent<RectTransform>().localPosition.x, obj_spr_selectedBTN.transform.localPosition.y - (n_entries_per_page*button_height), obj_spr_selectedBTN.GetComponent<RectTransform>().localPosition.z);
                    }
                    else
                    {
                        menu_sfx_2.Play(); //play sfx
                        selected_btn = selected_btn + n_entries_per_page;
                        if (obj_content.GetComponent<RectTransform>().localPosition.y == button_height*(n_entries-n_entries_per_page)) { obj_content.GetComponent<RectTransform>().localPosition = new Vector3(obj_content.GetComponent<RectTransform>().localPosition.x, button_height*(n_entries-last_page_entries_n), obj_content.GetComponent<RectTransform>().localPosition.z); }
                        else { obj_content.GetComponent<RectTransform>().localPosition = new Vector3(obj_content.GetComponent<RectTransform>().localPosition.x, (selected_page-1)*(n_entries_per_page*button_height), obj_content.GetComponent<RectTransform>().localPosition.z); }

                        //boundary check (obj_content.y)
                        if (obj_content.GetComponent<RectTransform>().localPosition.y < 0) { obj_content.GetComponent<RectTransform>().localPosition = new Vector3(obj_content.GetComponent<RectTransform>().localPosition.x, 0, obj_content.GetComponent<RectTransform>().localPosition.z); }

                        obj_spr_selectedBTN.GetComponent<RectTransform>().localPosition = new Vector3(obj_spr_selectedBTN.GetComponent<RectTransform>().localPosition.x, obj_spr_selectedBTN.transform.localPosition.y-((n_entries_per_page-(selected_btn-n_entries))*button_height), obj_spr_selectedBTN.GetComponent<RectTransform>().localPosition.z);
                        index_selected = n_entries - ((selected_page-1)*n_entries_per_page);
                        if (index_selected < 1) { index_selected = 1; }   //boundary check
                        if (index_selected > 10) { index_selected = 10; } //boundary check
                        selected_btn = n_entries;
                    }
                }
                //update arrow sprite
                if (page_num <= 1) //only 1 page
                {
                    obj_spr_arrow_left.GetComponent<CanvasGroup>().alpha = 0;
                    obj_spr_arrow_right.GetComponent<CanvasGroup>().alpha = 0;
                } else {
                    if (selected_page == 1 & page_num > 1) //first page
                    {
                        obj_spr_arrow_left.GetComponent<CanvasGroup>().alpha = 0;
                        obj_spr_arrow_right.GetComponent<CanvasGroup>().alpha = 1;
                    } else {
                        if (selected_page < page_num & selected_page > 1) //in the middle of first and last page
                        {
                            obj_spr_arrow_left.GetComponent<CanvasGroup>().alpha = 1;
                            obj_spr_arrow_right.GetComponent<CanvasGroup>().alpha = 1;
                        } else {
                            if (selected_page == page_num & page_num > 1) //last page
                            {
                                obj_spr_arrow_left.GetComponent<CanvasGroup>().alpha = 1;
                                obj_spr_arrow_right.GetComponent<CanvasGroup>().alpha = 0;
                            }
                        }
                    }
                }
            }
            obj_txt_pages.text = $"PAGE {selected_page.ToString("00")}/{page_num.ToString("00")}"; //update txt_pages with the current selected page
            obj_txt_test1.text = "PAGE: " + selected_page;   //DEBUG
            obj_txt_test2.text = "ENTRY: " + selected_btn;   //DEBUG
            obj_txt_test3.text = "INDEX: " + index_selected; //DEBUG

            if (is_held == true) { StartCoroutine(crt_btn_held_repeater()); }
        }
    }

    public void nav_trigger_mouse_scroll_instance(bool up_or_down) //mouse controls for button instances
    {
        if (up_or_down) //UP
        {
            obj_content.GetComponent<RectTransform>().localPosition = new Vector3(obj_content.GetComponent<RectTransform>().localPosition.x, obj_content.GetComponent<RectTransform>().localPosition.y-button_height, obj_content.GetComponent<RectTransform>().localPosition.z);

            //boundary check (obj_content.y)
            if (obj_content.GetComponent<RectTransform>().localPosition.y<0) { obj_content.GetComponent<RectTransform>().localPosition = new Vector3(obj_content.GetComponent<RectTransform>().localPosition.x, 0, obj_content.GetComponent<RectTransform>().localPosition.z); }
        }
        else //DOWN
        {
            obj_content.GetComponent<RectTransform>().localPosition = new Vector3(obj_content.GetComponent<RectTransform>().localPosition.x, obj_content.GetComponent<RectTransform>().localPosition.y+button_height, obj_content.GetComponent<RectTransform>().localPosition.z);

            //boundary check (obj_content.y)
            if (obj_content.GetComponent<RectTransform>().localPosition.y>button_height*(n_entries-1)) { obj_content.GetComponent<RectTransform>().localPosition = new Vector3(obj_content.GetComponent<RectTransform>().localPosition.x, button_height*(n_entries-1), obj_content.GetComponent<RectTransform>().localPosition.z); }
        }

        //update arrow sprite
        if (page_num <= 1) //only 1 page
        {
            obj_spr_arrow_left.GetComponent<CanvasGroup>().alpha = 0;
            obj_spr_arrow_right.GetComponent<CanvasGroup>().alpha = 0;
        } else {
            if (selected_page == 1 & page_num > 1) //first page
            {
                obj_spr_arrow_left.GetComponent<CanvasGroup>().alpha = 0;
                obj_spr_arrow_right.GetComponent<CanvasGroup>().alpha = 1;
            } else {
                if (selected_page < page_num & selected_page > 1) //in the middle of first and last page
                {
                    obj_spr_arrow_left.GetComponent<CanvasGroup>().alpha = 1;
                    obj_spr_arrow_right.GetComponent<CanvasGroup>().alpha = 1;
                } else {
                    if (selected_page == page_num & page_num > 1) //last page
                    {
                        obj_spr_arrow_left.GetComponent<CanvasGroup>().alpha = 1;
                        obj_spr_arrow_right.GetComponent<CanvasGroup>().alpha = 0;
                    }
                }
            }
        }
    }

    public void nav_trigger_mouse_instance(float inst_y) //mouse controls for button instances
    {
        menu_sfx_2.Play(); //play sfx

        //calculate selected_entry
        selected_btn = Convert.ToInt32(inst_y/(button_height-(button_height*2))+0.5);
        Debug.Log("[MOUSE] selected_entry: " + selected_btn);

        //calculate selected_page
        double temp_double = (selected_btn-1)/n_entries_per_page;
        selected_page = Convert.ToInt32(Math.Ceiling(temp_double)+1);
        Debug.Log("[MOUSE] selected_page: " + selected_page);

        //update selected_index
        index_selected = Convert.ToInt32((inst_y+obj_content.GetComponent<RectTransform>().localPosition.y)/(button_height-(button_height*2)+0.5));

        ///update obj_spr_selectedBTN.y
        obj_spr_selectedBTN.GetComponent<RectTransform>().DOLocalMoveY(inst_y, 0.1f);

        //update arrow sprite
        if (page_num <= 1) //only 1 page
        {
            obj_spr_arrow_left.GetComponent<CanvasGroup>().alpha = 0;
            obj_spr_arrow_right.GetComponent<CanvasGroup>().alpha = 0;
        } else {
            if (selected_page == 1 & page_num > 1) //first page
            {
                obj_spr_arrow_left.GetComponent<CanvasGroup>().alpha = 0;
                obj_spr_arrow_right.GetComponent<CanvasGroup>().alpha = 1;
            } else {
                if (selected_page < page_num & selected_page > 1) //in the middle of first and last page
                {
                    obj_spr_arrow_left.GetComponent<CanvasGroup>().alpha = 1;
                    obj_spr_arrow_right.GetComponent<CanvasGroup>().alpha = 1;
                } else {
                    if (selected_page == page_num & page_num > 1) //last page
                    {
                        obj_spr_arrow_left.GetComponent<CanvasGroup>().alpha = 1;
                        obj_spr_arrow_right.GetComponent<CanvasGroup>().alpha = 0;
                    }
                }
            }
        }

        obj_txt_pages.text = $"PAGE {selected_page.ToString("00")}/{page_num.ToString("00")}"; //update txt_pages with the current selected page
        obj_txt_test1.text = "PAGE: " + selected_page;   //DEBUG
        obj_txt_test2.text = "ENTRY: " + selected_btn; //DEBUG
        obj_txt_test3.text = "INDEX: " + index_selected; //DEBUG
    }

    private void OnEnable()
    {
        obj_txt_no_saves.GetComponent<CanvasGroup>().alpha = 0;

        button_instances = new GameObject("entry_instances"); //create parent obj for the button instances

        button_height = save_UI_prefab.GetComponent<RectTransform>().rect.height;
        index_selected = 1;
        selected_btn = 1;
        selected_page = 1;

        obj_txt_playtime.text = "PLAYTIME: " + globalVars.playtime.ToString("mm':'ss'.'ff"); //update txt //DEBUG

        bool saves_exist = checkLoad();

        if (saves_exist) //if save files exist
        {
            obj_ara_saves.SetActive(true);
            ///Sort the save file list and select the last modified file
            //calculate selected_entry
            selected_btn = Convert.ToInt32(last_modified_file.save_list_index+1); Debug.Log("selected_entry: " + selected_btn);
          
            ///calculate selected_page
            double temp_double = last_modified_file.save_list_index/n_entries_per_page;
            selected_page = Convert.ToInt32(Math.Ceiling(temp_double)); Debug.Log("selected_page: " + selected_page);
          
            ///calculate index_selected
            if (selected_page == 1) { index_selected = selected_btn; }
            else { index_selected = n_entries_per_page; }
          
            ///update position of obj_selectedBTN
            obj_spr_selectedBTN.GetComponent<RectTransform>().localPosition = new Vector3(obj_spr_selectedBTN.GetComponent<RectTransform>().localPosition.x, ((button_height/2)-button_height)-((selected_btn-1)*button_height), obj_spr_selectedBTN.GetComponent<RectTransform>().localPosition.z);
          
            ///update content.y
            obj_content.GetComponent<RectTransform>().localPosition = new Vector3(obj_content.GetComponent<RectTransform>().localPosition.x, 0+((selected_btn-n_entries_per_page)*button_height), obj_content.GetComponent<RectTransform>().localPosition.z);
          
            //boundary check (obj_content.y)
            if (obj_content.GetComponent<RectTransform>().localPosition.y < 0) { obj_content.GetComponent<RectTransform>().localPosition = new Vector3(obj_content.GetComponent<RectTransform>().localPosition.x, 0, obj_content.GetComponent<RectTransform>().localPosition.z); }
            if (obj_content.GetComponent<RectTransform>().localPosition.y > button_height*(n_entries-1)) { obj_content.GetComponent<RectTransform>().localPosition = new Vector3(obj_content.GetComponent<RectTransform>().localPosition.x, button_height*(n_entries-1), obj_content.GetComponent<RectTransform>().localPosition.z); }
          
            //update arrow sprite
            if (page_num <= 1) //only 1 page
            {
                obj_spr_arrow_left.GetComponent<CanvasGroup>().alpha = 0;
                obj_spr_arrow_right.GetComponent<CanvasGroup>().alpha = 0;
            } else {
                if (selected_page == 1 & page_num > 1) //first page
                {
                    obj_spr_arrow_left.GetComponent<CanvasGroup>().alpha = 0;
                    obj_spr_arrow_right.GetComponent<CanvasGroup>().alpha = 1;
                } else {
                    if (selected_page < page_num & selected_page > 1) //in the middle of first and last page
                    {
                        obj_spr_arrow_left.GetComponent<CanvasGroup>().alpha = 1;
                        obj_spr_arrow_right.GetComponent<CanvasGroup>().alpha = 1;
                    } else {
                        if (selected_page == page_num & page_num > 1) //last page
                        {
                            obj_spr_arrow_left.GetComponent<CanvasGroup>().alpha = 1;
                            obj_spr_arrow_right.GetComponent<CanvasGroup>().alpha = 0;
                        }
                    }
                }
            }
          
            obj_txt_pages.text = $"PAGE {selected_page.ToString("00")}/{page_num.ToString("00")}"; //update txt_pages with the current selected page
          
            Debug.Log("selected_entry: " + selected_btn);  //DEBUG
            Debug.Log("selected_page: " + selected_page);    //DEBUG
            Debug.Log("selected_index: " + index_selected);  //DEBUG
            obj_txt_test1.text = "PAGE: " + selected_page;   //DEBUG
            obj_txt_test2.text = "ENTRY: " + selected_btn; //DEBUG
            obj_txt_test3.text = "INDEX: " + index_selected; //DEBUG
        }
        else //no save file exists
        {
            obj_ara_saves.SetActive(false);
            obj_spr_arrow_left.GetComponent<CanvasGroup>().alpha = 0;
            obj_spr_arrow_right.GetComponent<CanvasGroup>().alpha = 0;
            obj_txt_pages.text = "PAGE 00/00"; //update txt_pages
            obj_txt_no_saves.GetComponent<CanvasGroup>().alpha = 1;
        }
    }

    private void OnDisable() { Destroy(button_instances); }

    private bool isDivisible(int x, int n) { return (x % n) == 0; }

    public void quickTest()
    {
        string filename = "save_69.sav";     //DEBUG

        DateTime header_test = DateTime.Now; //DEBUG

        globalVars.game_type = "likeMGS1"; //DEBUG
        globalVars.difficulty = "hard";    //DEBUG

        ///Save
        List<Save_Header> header = new List<Save_Header>();
        header.Add(new Save_Header("OSE", "1", header_test.ToString("dd-MM-yyyy HH:mm:ss"))); //create the header list
        List<scr_save_data> game_data = new List<scr_save_data>();
        game_data.Add(new scr_save_data(globalVars.game_type, globalVars.difficulty.ToUpper(), globalVars.playtime.ToString("d':'hh':'mm':'ss'.'fff"), globalVars.is_playtime_counting)); //create the list of all variables and populate it

        scr_save_data_manager.saveToJSON(header, game_data, filename); //save the header & game data lists

        //load
        List<Save_Header> header_l = scr_save_data_manager.loadHeaderFromJSON<Save_Header>(filename); //load the header

        DateTime a_test = DateTime.ParseExact(header_l[0].date_last_modified, "dd-MM-yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture); //DEBUG
        Debug.Log("after: " + a_test); //DEBUG

        List<scr_save_data> game_data_l = scr_save_data_manager.loadListFromJSON<scr_save_data>(filename); //load the game data

        TimeSpan yeet = TimeSpan.ParseExact(game_data_l[0].playtime, "d':'hh':'mm':'ss'.'fff", System.Globalization.CultureInfo.InvariantCulture); //DEBUG
        Debug.Log("yeet: " + yeet.ToString("mm':'ss'.'ff")); //DEBUG
    }

    ///load check
    public bool checkLoad()
    {
        saves_list = checkSaves();
        for (int h=0; h<saves_list.Count; h++)
        {
            Debug.Log($"final result {h}: {saves_list[h]}");
        }
        Debug.Log("Nº of save files: " + saves_list.Count);

        if (saves_list.Count == 0) { return false; } //if there's 0 valid save files

        n_entries = saves_list.Count(); //limit nº of entries in menu

        //calculate Nº of pages required (10 entries per page)
        page_num = n_entries;
        last_page_entries_n = n_entries;

        //calculate Nº of pages
        page_num = page_num / n_entries_per_page;
        page_num = Math.Ceiling(page_num);
        if (page_num < 1) { page_num = 1; } Debug.Log("page_num(just calculated): " + page_num); Debug.Log("n_entries: " + n_entries);

        obj_txt_pages.text = $"PAGE {selected_page.ToString("00")}/{page_num.ToString("00")}"; //update txt_pages with the current selected page

        //calculate the Nº of entries in the last page (only runs if there's more than 1 page)
        for (int v=0; v<(page_num-1); v++) { last_page_entries_n = last_page_entries_n - n_entries_per_page; }

        //sort the saves numerically based on the [index] value
        saves_list = saves_list.OrderBy(l => int.Parse(l.Substring(l.LastIndexOf("_")+1, (l.LastIndexOf(".")-1)-l.LastIndexOf("_")))).ToList();

        //create parent so that we can destroy instances later
        button_instances.transform.SetParent(obj_content.transform);
        button_instances.transform.localPosition = new Vector3(0, 0, 0);
        button_instances.transform.localScale = new Vector3(1, 1, 1); //set the scale of parent obj to 1, fixes wrong scaling in the buttons

        List<date_order> last_modified_date_list = new List<date_order>(); //datetime list for sorting last modified

        //spawn entry instances
        for (int i=0; i<saves_list.Count; i++)
        {
            List<scr_save_data> gamedata_ins = scr_save_data_manager.loadListFromJSON<scr_save_data>(saves_list[i]); //load the Game Data
            List<Save_Header> header_ins = scr_save_data_manager.loadHeaderFromJSON<Save_Header>(saves_list[i]);     //load the Header

            GameObject prefab_obj = Instantiate(save_UI_prefab);         //instance of the prefab
            prefab_obj.transform.SetParent(button_instances.transform);  //set the parent object of the instance as mn_LoadGame
            prefab_obj.transform.localScale = new Vector3(1, 1, 1);      //set the scale of button to 1, fixes wrong scaling in the buttons

            //set the position to relative 0
            prefab_obj.transform.localPosition = new Vector3(obj_ara_saves.GetComponent<RectTransform>().rect.width/2, ((button_height/2)-button_height)-(button_height*i), 0);

            //index
            GameObject obj_txt_index = prefab_obj.transform.GetChild(0).gameObject; //get child text "txt_index"
            obj_txt_index.GetComponent<TMPro.TMP_Text>().text = int.Parse(saves_list[i].Substring(saves_list[i].LastIndexOf("_")+1, (saves_list[i].LastIndexOf(".")-1)-saves_list[i].LastIndexOf("_"))).ToString("000"); //set text on txt_index

            //difficulty
            GameObject obj_txt_difficulty = prefab_obj.transform.GetChild(1).gameObject; //get child text "txt_difficulty"
            obj_txt_difficulty.GetComponent<TMPro.TMP_Text>().text = gamedata_ins[0].difficulty;

            //playtime
            DateTime temp_date = DateTime.ParseExact(header_ins[0].date_last_modified, "dd-MM-yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            last_modified_date_list.Add(new date_order(DateTime.ParseExact(header_ins[0].date_last_modified, "dd-MM-yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture), i));

            GameObject obj_txt_date = prefab_obj.transform.GetChild(2).gameObject; //get child text "txt_date"
            obj_txt_date.GetComponent<TMPro.TMP_Text>().text = temp_date.ToString("yyyy.MM.dd"); //"dd.MM.yyyy"

            //last date modified
            TimeSpan temp_time = TimeSpan.ParseExact(gamedata_ins[0].playtime, "d':'hh':'mm':'ss'.'fff", System.Globalization.CultureInfo.InvariantCulture);
            string temp_time_formated = string.Format("{0:0000}:{1:00}:{2:00}", (int)(temp_time.TotalHours), temp_time.Minutes, temp_time.Seconds);

            GameObject obj_txt_playtime = prefab_obj.transform.GetChild(3).gameObject; //get child text "txt_playtime"
            obj_txt_playtime.GetComponent<TMPro.TMP_Text>().text = temp_time_formated;
        }

        for (int kl=0; kl<last_modified_date_list.Count; kl++) //DEBUG
        {
            Debug.Log($"last_modified_date_list[{kl}]: {last_modified_date_list[kl]}");
        }

        last_modified_file = last_modified_date_list.OrderByDescending(x => x.date_and_time.Ticks).FirstOrDefault();

        Debug.Log("datetime a: " + last_modified_file.date_and_time); //DEBUG
        Debug.Log("index a: " + last_modified_file.save_list_index); //position of the last modified save file (save_list index) //DEBUG

        return true;
    }

    private List<string> checkSaves() //check all save files and return a list of all the valid ones
    {
        string full_path = Application.dataPath + "/.saves";
        Directory.CreateDirectory(full_path); //create save dir if it doesn't exist

        DirectoryInfo di = new DirectoryInfo(full_path);
        int sav_num = di.GetFiles("*.sav", SearchOption.TopDirectoryOnly).Length;  //number of all .sav files inside the .saves dir (not subdirs)
        FileInfo[] sav_list = di.GetFiles("*.sav", SearchOption.TopDirectoryOnly); //info of all .sav files inside the .saves dir (not subdirs)

        int valid_counter = 0;
        List<string> valid_filenames = new List<string>();
        for (int y=0; y<sav_num; y++) //check for valid filenames
        {
            if (checkFilename(sav_list[y].Name) == true)
            {
                valid_filenames.Add(sav_list[y].Name);
                valid_counter++;
            }
        }

        List<Save_Header> header = new List<Save_Header>();
        List<string> valid_saves = new List<string>();

        for (int z=0; z<valid_counter; z++) //checks for valid headers
        {
            bool header_is_null = false;
            try { header = scr_save_data_manager.loadHeaderFromJSON<Save_Header>(valid_filenames[z]); } //load the header 
            catch (ArgumentNullException) { header_is_null = true; } //if it doesn't exist then the save is invalid

            if (header_is_null == true) { Debug.Log($"save '{valid_filenames[z]}' is invalid (reason: header | header missing)"); }
            else
            {
                if (header[0].GameID == "OSE") //game ID is valid
                {
                    Debug.Log($"save '{valid_filenames[z]}' is valid");
                    valid_saves.Add(valid_filenames[z]);
                }
                else { Debug.Log($"save '{valid_filenames[z]}' is invalid (reason: header | gameID mismatch)"); } //game ID is a mismatch
            }
        }
        return valid_saves;
    }

    private bool checkFilename(string filename) //checks if the filename follows naming conventions (must have 1 '_' between
    {                                           //[name] and [index] and must end in .sav) (i.e "InkSave_03.sav"
        int i = 1;
        while (filename[filename.Length - i] != '_')
        {
            //if it doesn't have '_' in filename then it's invalid
            if (i == filename.Length) { Debug.Log($"save '{filename}' is invalid (reason: filename | no '_' to separate between name and index)"); return false; }
            i++;
        }
        i--;
        //Debug.Log($"nº of index digits in '{test_str}': " + (i-4)); //number of digits in save file index

        if (i-4 == 0) { Debug.Log($"save '{filename}' is invalid (reason: filename | no numbers at the end)"); return false; } //if no numbers betwwen '_' & '.' then it's invalid
        else //check if they're actually numbers
        {
            int e = 0;
            for (int j=1; j<(i-3); j++)
            {
                if (filename[filename.Length-(j+4)] != '0' & filename[filename.Length-(j+4)] != '1' & filename[filename.Length-(j+4)] != '2' &
                    filename[filename.Length-(j+4)] != '3' & filename[filename.Length-(j+4)] != '4' & filename[filename.Length-(j+4)] != '5' &
                    filename[filename.Length-(j+4)] != '6' & filename[filename.Length-(j+4)] != '7' & filename[filename.Length-(j+4)] != '8' &
                    filename[filename.Length-(j+4)] != '9')
                {
                    Debug.Log($"save '{filename}' is invalid (reason: filename | character in position '" + ((i-3)-j) + "' is invalid)");
                    e++;
                }
            }
            if (e != 0) { return false; } //logs all invalid char positions before returning
        }
        return true;
    }
}

public class date_order //class to order the save files by last modified
{
    public DateTime date_and_time;
    public double save_list_index;

    public date_order(DateTime date_and_time, double save_list_index) //can cast arrays, also i recommend casting floats as doubles instead to retain accuracy
    {
        this.date_and_time = date_and_time;
        this.save_list_index = save_list_index;
    }
}
