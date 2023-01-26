using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable] public class scr_save_data
{
    public string game_type;
    public string difficulty;
    public string playtime;
    public bool is_playtime_counting;

    public scr_save_data(string game_type, string difficulty, string playtime, bool is_playtime_counting) //can cast arrays, also i recommend casting floats as doubles instead to retain accuracy
    {
        this.game_type = game_type;
        this.difficulty = difficulty;
        this.playtime = playtime;
        this.is_playtime_counting = is_playtime_counting;
    }
}

[Serializable] public class Save_Header
{
    public string GameID;
    public string game_version;
    public string date_last_modified;

    public Save_Header(string GameID, string game_version, string date_last_modified)
    {
        this.GameID = GameID;
        this.game_version = game_version;
        this.date_last_modified = date_last_modified;
    }
}
