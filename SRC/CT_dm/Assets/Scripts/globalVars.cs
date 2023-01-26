using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class globalVars : MonoBehaviour
{
    public static globalVars instance;
    private void Awake() { instance = this; DontDestroyOnLoad(gameObject); }

    //save values
    public static string game_type;  //play1st || likeMGS1 || likeMGS2 || likeMGS3
    public static string difficulty; //v_easy || easy || normal || hard || extreme || eu_extreme
    public static TimeSpan playtime;
    public static bool is_playtime_counting;
    public static int save_file_choice = 0; //0 = new boot || 1 = existing save file || 2 = existing save file (i like MGS3)

    //global values
    //public static string loaded_save_file;
}
