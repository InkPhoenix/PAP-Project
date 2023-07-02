using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class scr_UI_win_lose : MonoBehaviour
{
    public GameObject UI_win;
    public GameObject UI_lose;
    public GameObject mn_game_win;
    public GameObject subcrossfade;

    private float fade_time = 1.0f;

    private void Start()
    {
        if (UI_win == null) { UI_win = GameObject.Find("GameWinMenu"); }
        if (UI_lose == null) { UI_lose = GameObject.Find("GameOverMenu"); }
        if (subcrossfade == null) { subcrossfade = GameObject.Find("subCrossfade"); }
        if (mn_game_win == null) { mn_game_win = GameObject.Find("mn_game_win"); }

        scr_guard.onGuardSpottedPlayer += showUI_lose;
        FindObjectOfType<scr_player_controller>().onFinishedLevel += showUI_win;
    }

    private void showUI_win()
    {
        subcrossfade.GetComponent<CanvasGroup>().alpha = 0;
        mn_game_win.GetComponent<CanvasGroup>().alpha = 0;
        onGameOver(UI_win);
        subcrossfade.GetComponent<CanvasGroup>().DOFade(1, fade_time).SetEase(Ease.InCubic).OnComplete(complete);
    }
    private void complete() { StartCoroutine(fadeInWinUI()); }

    private IEnumerator fadeInWinUI()
    {
        yield return new WaitForSeconds(0.5f);
        mn_game_win.GetComponent<CanvasGroup>().DOFade(1, fade_time).SetEase(Ease.InCubic);
        yield return null;
    }

    private void showUI_lose()
    {
        try { onGameOver(UI_lose); }
        catch (MissingReferenceException e) { UI_lose = GameObject.Find("GameOverMenu"); }
    }

    private void onGameOver(GameObject UI)
    {
        UI.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        scr_guard.onGuardSpottedPlayer -= showUI_lose;
        FindObjectOfType<scr_player_controller>().onFinishedLevel -= showUI_win;
    }
}
