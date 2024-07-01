using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameSettings
{
    public static List<Setting> settingsList = new List<Setting>();

    public static void AddSetting(Setting setting)
    {
        settingsList.Add(setting);
        Debug.Log(setting.playerName + "+" +setting.selectedType+"+" + setting.selectedColor);
    }

    public static List<Setting> SettingsList => settingsList;
}

public class Setting
{
    public static string playerName;
    public static int selectedType;
    public static int selectedColor;

    public Setting(string _name, int type, int color)
    {
        playerName = _name;
        selectedType = type;
        selectedColor = color;
    }
}
