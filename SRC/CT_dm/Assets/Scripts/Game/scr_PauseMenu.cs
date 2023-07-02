using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class scr_PauseMenu : MonoBehaviour
{
    public static bool game_pause;
    public GameObject pause_UI;
    public GameObject obj_crossfade;

    private void Awake()
    {
        Cursor.visible = false;
        game_pause = false;
    }

    public void f_Pause() //function called when the "Resume" button is pressed
    {
        if (game_pause == true) { resume(); }
        else { pause(); }
    }

    private void resume()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        pause_UI.SetActive(false); //deactivates the UI
        Time.timeScale = 1f;       //sets the game speed to 1 (normal time)
        
        game_pause = false;
    }

    private void pause()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        pause_UI.SetActive(true); //activates the UI
        Time.timeScale = 0f;      //sets the game speed to 0 (freezes time)
        
        game_pause = true;
    }

    public void quit() { Application.Quit(); }

    public void quitMenu() { StartCoroutine(switch_scenes("scn_MainMenu")); }

    public void restartLevel()
    {
        DevCon.Commands.SceneCommands.RestartScene();
    }

    private IEnumerator switch_scenes(string scene_n)
    {
        Debug.Log("Quiting to Main Menu");
        Time.timeScale = 1f;
        yield return new WaitForSeconds(0.15f);
        obj_crossfade.GetComponent<scr_fade_levelLoader>().scene_name = scene_n;
        scr_fade_levelLoader.make_fade_black_on_startup = true; //make the fade be of alpha 1 on startup of next scene
        yield return obj_crossfade.GetComponent<scr_fade_levelLoader>().sceneSwitchFadeIn(0.5f, false, Ease.InCubic);
        yield return new WaitForSeconds(0.5f);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene(scene_n);
    }
}
