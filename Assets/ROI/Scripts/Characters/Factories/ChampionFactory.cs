using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ROI.DataEntity;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ROI
{
	[RequireComponent(typeof(IObjectPool))]
	public class ChampionFactory : MonoBehaviour, IChampionFactory
	{
		private IObjectPool _objectPool;

		private void Awake()
		{
			_objectPool = GetComponent<IObjectPool>();
		}

		/// <summary>
		/// Create A Basic Champion From A Champion Prefab with level 1 star 1
		/// </summary>
		/// <param name="championPrefab"></param>
		/// <param name="creatorNetID"></param>
		/// <param name="userChampionUID"></param>
		/// <param name="ownedByHost"></param>
		/// <param name="isIllusion"></param>
		/// <returns></returns>
		public ChampionData Create(Object championPrefab, bool ownedByHost, uint creatorNetID = 0, bool isIllusion = false, string userChampionUID = null)
		{
			_objectPool.CreatPool(championPrefab);

			if (false == _objectPool.Use(championPrefab.GetInstanceID(), out var obj))
			{
				Logs.Error($"Create Champion From Prefab: {championPrefab.name} Fail!");
				return default;
			}

			var championData = obj.GetComponent<ChampionData>();

			// init basic data
			championData.state = ChampionStates.Stopped;
			championData.target = null;

			championData.currentEffect = ChampionEffects.None;
			championData.imperishable = false;

			// championData.transform.position = Vector3.zero;

			championData.ownedByHost = ownedByHost;
			championData.isIllusion = isIllusion;

			championData.creatorNetId = creatorNetID;
			championData.userChampionUID = userChampionUID;

			// clear & re-add handles
			championData.handles = new ChampionHandles(obj);

			return championData;
		}


		public async Task<ChampionData> Create(ChampionBaseData championBaseData, bool ownedByHost, uint creatorNetID = 0, bool isIllusion = false, string userChampionUID = null)
		{
			// load model
			var modelLoader = await championBaseData.LoadModelAsync();

			// create model
			var champion = Create(modelLoader, ownedByHost, creatorNetID, isIllusion, userChampionUID);

			// init base data
			if (string.IsNullOrEmpty(userChampionUID))
			{
				champion.InitBaseStat(championBaseData.CalculateBaseStat());
				return champion;
			}

			// init base data with team
			var userTeam = Data.UserTeam.GetDefaultTeam();
			var index = userTeam.championsTeam.FindIndex(c => c.championUserUID == userChampionUID);

			if (index < 0)
				champion.InitBaseStat(userChampionUID);
			else
				champion.InitBaseStat(userTeam.championsTeam[index]);

			return champion;

		}

		// /// <summary>
		// /// Initialize base data
		// /// </summary>
		// /// <param name="championData"></param>
		// /// <param name="baseData"></param>
		// private void InitChampionData(ChampionData championData, ChampionBaseData baseData)
		// {
		//     var championBaseData = baseData.championBaseData;
		//
		//     // attack data
		//     championData.attackData.damage = championBaseData.Stat.championStat.Attack_damage;
		//     championData.attackData.baseDamage = championBaseData.Stat.championStat.Attack_damage;
		//
		//     championData.attackData.baseRange = championBaseData.Stat.championStat.Attack_range;
		//     championData.attackData.range = championBaseData.Stat.championStat.Attack_range;
		//
		//     championData.attackData.speed = championBaseData.Stat.championStat.Attack_speed;
		//     championData.attackData.baseSpeed = championBaseData.Stat.championStat.Attack_speed;
		//
		//     championData.attackData.critDamageChance = championBaseData.Stat.championStat.Crit_chance;
		//     championData.attackData.critDamage = championBaseData.Stat.championStat.CritDamage;
		//
		//     // defense
		//     championData.defenseData.basePhysicDef = championBaseData.Stat.championStat.Physical_armor;
		//     championData.defenseData.physicDef = championBaseData.Stat.championStat.Physical_armor;
		//
		//     championData.defenseData.baseMagicDef = championBaseData.Stat.championStat.Magic_defense;
		//     championData.defenseData.magicDef = championBaseData.Stat.championStat.Magic_defense;
		//
		//     // movement
		//     championData.moveData.baseMoveSpeed = championBaseData.Stat.championStat.MovementSpeed;
		//     championData.moveData.moveSpeed = championBaseData.Stat.championStat.MovementSpeed;
		//
		//     championData.healthData.health = (int)championBaseData.Stat.championStat.Total_heath;
		//     championData.healthData.maxHealth = (int)championBaseData.Stat.championStat.Total_heath;
		//
		//     championData.specialStatData.abilityPower = championBaseData.Stat.championStat.AbilityPower;
		//     championData.specialStatData.healthRegen = championBaseData.Stat.championStat.HeathRegen;
		//     championData.specialStatData.dodgeChance = championBaseData.Stat.championStat.Dogde;
		//     championData.specialStatData.critChanceOnAbility = championBaseData.Stat.championStat.SkillCritChance;
		//     championData.specialStatData.lifeSteal = championBaseData.Stat.championStat.LifeSteal;
		//     championData.specialStatData.armorPenetration = championBaseData.Stat.championStat.ArmorPenetration;
		//     championData.specialStatData.magicPenetration = championBaseData.Stat.championStat.MDPenetration;
		//     championData.specialStatData.damageTaken = championBaseData.Stat.championStat.DamageTaken;
		// }
	}
}