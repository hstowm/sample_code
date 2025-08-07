using System.Linq;
using ROI.DataEntity;
using ROI;
using UnityEngine;


namespace ROI
{
    
    /// <summary>
    /// Equipment that give me Armor also gives me 25% that much Magic Resist as well.
    /// </summary>
    
    [CreateAssetMenu(fileName = "FrostbittenAura", menuName = "ROI/Data/AbilityPassiveCards/FrostbittenAura", order = 1)]
    public class FrostbittenAura : BasePassiveAbilityCard,IOnStartAlive
    {
        private ChampionData _championData;
        
        public override void OnInit(ChampionData champion)
        {
            _championData = champion;
            champion.handles.OnStartAlive.Add(this);
        }

        public void OnStartAlive()
        {
            Debug.Log("champion UID: " + _championData.userChampionUID);
            //AddBonusMagicDef();
        }
        
        public void AddBonusMagicDef()
        {
            float mgBonus = 0;

            var team = Data.UserTeam.GetDefaultTeam().championsTeam;
            var championTeam = team.Find(inTeam => inTeam.championUserUID == _championData.userChampionUID);
            for (int i = 0; i < championTeam.equipmentsTeam.Count; i++)
            {
                 Data.UserEquipmentInTeam equipmentTeam = championTeam.equipmentsTeam[i];
                 string idEquipmentBase = Data.UserData.listEquipments.Where(data => data.equipmentUserUID == equipmentTeam.equipmentUserUID).Select(data => data.equipmentBaseDataUUID).FirstOrDefault();
                 if (!string.IsNullOrEmpty(idEquipmentBase))
                 {
                     var data = GameData.equipmentDB.equipmentBaseDatas.Where(data => data.Key == idEquipmentBase).FirstOrDefault();
                     if (data.Value!=null)
                     {
                         var armorAdded = data.Value.equipmentData.stat.FindIndex(stat => stat.type == BaseStatTypes.Armor);
                         if (armorAdded > 0)
                         {
                             mgBonus += (data.Value.equipmentData.stat[armorAdded].value * 0.25f);
                         }
                        
                     }
                 }
            }
            Debug.Log("armor : " + mgBonus);
            _championData.statModifier.ApplyModify(new StatTypeData(StatTypes.MagicDef,
                 mgBonus));
        }
        
    }
    
   

}

