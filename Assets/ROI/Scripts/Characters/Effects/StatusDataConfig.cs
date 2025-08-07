using System.Collections.Generic;
using UnityEngine;
using ROI;


[CreateAssetMenu(fileName = "StatusDataConfig", menuName = "ROI/Data/StatusDataConfig", order = 1)]
public class StatusDataConfig: SingletonScriptable<StatusDataConfig>
{
    // Start is called before the first frame update
    public Dictionary<string, StatusSetting> config_list = new Dictionary<string, StatusSetting>();

    public StatusSetting GetStatusSettingFromName(string name)
    {
        if(config_list.TryGetValue(name, out StatusSetting result))
        {
            return result;
        }
        
        Logs.Error($"Cant find Status Setting Name: {name}");
        return null;
    }

}
