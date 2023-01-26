using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class scr_button_instances : MonoBehaviour
{
    //mouse controls for button instances
    public void nav_trigger_mouse_instance() { scr_mn_LoadGame.instance.nav_trigger_mouse_instance(gameObject.transform.localPosition.y); }

    public void nav_trigger_mouse_scroll_instance()
    {
        Vector2 scroll_val = Mouse.current.scroll.ReadValue();
        if (scroll_val.y > 0) { scr_mn_LoadGame.instance.nav_trigger_mouse_scroll_instance(true); } //UP
        else { if (scroll_val.y < 0) { scr_mn_LoadGame.instance.nav_trigger_mouse_scroll_instance(false); }} //DOWN
    }
}
