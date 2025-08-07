using ROI.Data;
using ROI.Scripts.Quests;

namespace ROI
{
    public static class ChampionLevelSystem
    {
        const int MaxLevel = 10;
        const int MaxStar = 5;

        /// <summary>
        /// Upgrade level of the champion
        /// </summary>
        /// <param name="userChampion"></param>
        public static void UpLevel(this UserChampion userChampion)
        {
            if (userChampion.level == GameData.constData.ChampionMaxLv)
            {
                Logs.Error("Cant Upgrade Level. It is max");
                userChampion.currentExp = 0;
                return;
            }
            userChampion.level += 1;
            userChampion.currentExp = 0;
            ObserverService.Notify(ObserverSubject.QuestLevelUpChampion, userChampion.UserChampionUID);
            ObserverService.Notify(ObserverSubject.QuestLevelUpChampion);

            // unlock new equipment slot at level 10
            //userChampion.unlockedEquipSlot += userChampion.level > 10 ? 1u : 0;
        }


        /// <summary>
        /// Ascends Star
        /// </summary>
        /// <param name="userChampion"></param>
        public static void AscendStar(this UserChampion userChampion)
        {
            if (userChampion.star == MaxStar || userChampion.level < MaxLevel)
            {
                Logs.Error("Cant Ascend Star. It's max star or level is not max");
                return;
            }

            // increase star and reset level
            userChampion.level = 1;
            userChampion.star += 1;

            // Add new trait when star is 3 or 5
            if (userChampion.star == 3 || userChampion.star == 5)
            {
                Logs.Info("Add new trait UID");
                userChampion.traitUIDs.Add("UID");
            }

            // unlock new card slot when star is 4 and 5
            userChampion.unlockedCardSlot += userChampion.star > 3 ? 1u : 0;
        }

        // public static ChampionBaseData GetCurrentStat(this UserChampion userChampion)
        // {
        //     // get champion base data level 1;
        //     if (GameData.chamBaseDB.listChampionBases.TryGetValue(userChampion.championUID, out var championBaseData) == false)
        //     {
        //         Logs.Error($"Cant Get Champion Base Data with Base UID: {userChampion.championUID}");
        //         return null;
        //     }
        //
        //     // championBaseData.CalculateBaseStat();
        //     
        //
        //     return championBaseData;
        // }
    }
}