using System;
using System.Collections.Generic;
using ROI.Data;
using UnityEngine;
using UnityEngine.Windows;

namespace ROI
{

	public static class ChampionCardExts
	{
		private static IAbilityCardSystem _abilityCardSystem;

		public static void Init(IAbilityCardSystem cardSystem) => _abilityCardSystem = cardSystem;

		public static void DoActiveCard(this ChampionData champion, string cardUID, Vector3 input)
		{
			Logs.Info($"{champion.name} Do activeCard {cardUID} : {input}");
			_abilityCardSystem.DoActiveCard(champion, cardUID, input);
		}

		public static void DoUltimateCard(this ChampionData championData, Vector3 input)
		{
			Logs.Info($"{championData.name} Do Ultimate Card: {input}");
			_abilityCardSystem.DoUltimateCard(championData, input);
		}

		public static void DoCoreCard(this ChampionData championData, Vector3 input)
		{
			Logs.Info($"{championData.name}Do Core Card: {input}");
			_abilityCardSystem.DoCoreActiveCard(championData, input);
		}

        public static void NpcDoSkill(this ChampionData championData)
        {
            Logs.Info($"{championData.name}Do random Card");
            _abilityCardSystem.NPCDoRandomSkill(championData);
        }

		public static void InitAbilityCards(this ChampionData championData)
		{
			var listCards = championData.GetListAbilityCards();

			_abilityCardSystem.InitAbilityCards(championData, listCards);
		}

		public static void InitAbilityCards(this ChampionData championData, List<string> listCards)
		{
			_abilityCardSystem.InitAbilityCards(championData, listCards);
		}

		/// <summary>
		/// Get List Equipments
		/// </summary>
		/// <param name="championData"></param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		public static List<UserEquipment> GetListEquipments(this ChampionData championData)
		{
			if (PlayerNetwork.Instance.isServer == false)
				throw new Exception("This function only call from server");

			var listEquipment = new List<UserEquipment>();

			// get owned
			if (championData.ownedByHost)
			{
				var team = UserTeam.GetDefaultTeam();
				var index = team.championsTeam.FindIndex(c => c.championUserUID == championData.userChampionUID);

				if (index < 0)
				{
					Logs.Error($"Not Found userChampionUID: {championData.userChampionUID}");
					return listEquipment;
				}

				var championInTeam = team.championsTeam[index];
				foreach (var userEquipmentInTeam in championInTeam.equipmentsTeam)
				{
					index = UserData.listEquipments.FindIndex(data => data.equipmentUserUID == userEquipmentInTeam.equipmentUserUID);
					if (index < 0)
						continue;

					Logs.Info($"Apply Passive Card: {UserData.listCards[index].cardUID} On Illusion: {championData.name}");
					listEquipment.Add(UserData.listEquipments[index]);
				}

				return listEquipment;
			}



			return listEquipment;
		}

		public static List<string> GetListAbilityCards(this ChampionData championData) => GetListAbilityCards(championData, out _, out _);

		/// <summary>
		/// Get List all Cards Of the champion
		/// </summary>
		/// <param name="championData"></param>
		/// <param name="skillCard"></param>
		/// <param name="ultimateCard"></param>
		/// <returns></returns>
		public static List<string> GetListAbilityCards(this ChampionData championData, out string skillCard, out string ultimateCard)
		{

			if (PlayerNetwork.Instance.isServer == false)
				throw new Exception("This function only call from server");

			var listCards = new List<string>();

			skillCard = string.Empty;
			ultimateCard = string.Empty;

			//if (includeChampionCard)
			// {
			var championSkill = championData.GetComponent<ChampionSkillCard>();

			if (championSkill != null && championSkill.baseData != null)
			{
				ultimateCard = championSkill.baseData.ultimate.KeyName;
				skillCard = championSkill.baseData.active.KeyName;
			}
			//}

			if (championData.ownedByHost)
			{
				var team = UserTeam.GetDefaultTeam();
				var index = team.championsTeam.FindIndex(c => c.championUserUID == championData.userChampionUID);

				if (index < 0)
				{
					Logs.Warning($"Not Found userChampionUID: {championData.userChampionUID} in Default Team");
					return listCards;
				}

				var championInTeam = team.championsTeam[index];
				foreach (var cardInTeam in championInTeam.abilityCardsTeam)
				{
					index = UserData.listCards.FindIndex(data => data.userCardUID == cardInTeam.cardUserUID);
					if (index < 0)
						continue;

					//Logs.Info($"Apply Passive Card: {UserData.listCards[index].cardUID} On Illusion: {championData.name}");
					// _abilityCardSystem.InitAbilityPassiveCard(championData, UserData.listCards[index].cardUID);
					listCards.Add(UserData.listCards[index].cardUID);
				}

				return listCards;
			}

			var idx = PlayerNetwork.Instance.listOpponentChampions.FindIndex(c => c.userChampionUID == championData.userChampionUID);
			if (idx < 0)
			{
				Logs.Error($"Not Found Opponent userChampionUID: {championData.userChampionUID}");
				return listCards;
			}

			var championTeamStat = PlayerNetwork.Instance.listOpponentChampions[idx];
			foreach (var abilityCard in championTeamStat.listAbilityCards)
			{
				// _abilityCardSystem.InitAbilityPassiveCard(championData, abilityCard);
				listCards.Add(abilityCard);
			}

			return listCards;
		}
	}
}