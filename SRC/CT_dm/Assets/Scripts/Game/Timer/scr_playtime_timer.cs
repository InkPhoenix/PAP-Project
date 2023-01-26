using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_playtime_timer : MonoBehaviour
{
    public static scr_playtime_timer instance;

    private static float frame_time;

    private void Awake() { instance = this; DontDestroyOnLoad(gameObject); }

    public void startTimer() //starts timer from whatever value it had previously 
    {
        globalVars.is_playtime_counting = true;
        instance.StartCoroutine(instance.updateTimer());
    }

    public void stopTimer() //pauses the timer
    {
        globalVars.is_playtime_counting = false;
    }

    public void resetTimer() //resets the timer
    {
        globalVars.is_playtime_counting = false;
        globalVars.playtime = TimeSpan.Zero;
        frame_time = 0;
    }

    public IEnumerator updateTimer()
    {
        while (globalVars.is_playtime_counting)
        {
            frame_time += Time.deltaTime;
            globalVars.playtime = TimeSpan.FromSeconds(frame_time);
            Debug.Log("Playtime: " + globalVars.playtime.ToString("d':'hh':'mm':'ss'.'fff"));

            yield return null;
        }
    }
}
