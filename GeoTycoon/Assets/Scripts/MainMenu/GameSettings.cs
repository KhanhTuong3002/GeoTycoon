using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameSettings
{
    public static List<Setting> settingsList = new List<Setting>();

    public static void AddSetting(Setting setting)
    {
        settingsList.Add(setting);
        Debug.Log(setting.playerName + "+" + setting.selectedType+"+" + setting.selectColor);
    }

    //public static List<Setting> SettingsList => settingsList;
}

public class Setting
{
    public string playerName;
    public int selectedType;
    public int selectColor;

    public Setting(string _name, int type, int color)
    {
        playerName = _name;
        selectedType = type;
        selectColor = color;
    }
}
