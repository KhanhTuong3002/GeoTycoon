using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Singletons/MasterManager")]
public class MasterManager : SingletonScriptableObject<MasterManager> 
{
    [SerializeField]
    private GameSetting _gameSetting;
    public static GameSetting GameSetting { get { return Instance._gameSetting; } }
}
