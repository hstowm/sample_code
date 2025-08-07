using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ROI.DataEntity;
using Object = UnityEngine.Object;

namespace ROI
{
    public interface IChampionFactory
    {
        /// <summary>
        /// Create champion from base data
        /// </summary>
        /// <param name="championBaseData"></param>
        /// <param name="ownedByHost"></param>
        /// <param name="creatorNetID"></param>
        /// <param name="isIllusion"></param>
        /// <param name="userChampionUID"></param>
        /// <returns></returns>
        Task<ChampionData> Create(ChampionBaseData championBaseData, bool ownedByHost, uint creatorNetID = 0, bool isIllusion = false, string userChampionUID = null);
        
        /// <summary>
        /// create champion from Model Prefab
        /// </summary>
        /// <param name="championPrefab"></param>
        /// <param name="ownedByHost"></param>
        /// <param name="creatorNetID"></param>
        /// <param name="isIllusion"></param>
        /// <param name="userChampionUID"></param>
        /// <returns></returns>
        ChampionData Create(Object championPrefab, bool ownedByHost, uint creatorNetID = 0, bool isIllusion = false, string userChampionUID = null);
    }
}