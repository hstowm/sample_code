using System.Collections.Generic;
using Mirror;
using ROI.DataEntity;
using UnityEngine;

namespace ROI
{


	public class CardManaSystem : NetworkBehaviour, ICardManaSystem
	{

		[SerializeField]
		private int _minimumManaCost = 1;

		private ManaManager _manaManager;

		// [SyncVar(hook = nameof(OnCardManaChanged))]
		private readonly SyncDictionary<uint, List<CardManaData>> _userCardManas = new SyncDictionary<uint, List<CardManaData>>();


		void Awake()
		{
			_manaManager = FindObjectOfType<ManaManager>(true);
			CardManaSystemExts.Init(this);
		}


		/// <summary>
		/// Initialize champion card data
		/// </summary>
		/// <param name="championData"></param>
		/// <param name="cardSkillData"></param>
		[Server]
		public void InitCardMana(ChampionData championData, CardSkillData cardSkillData)
		{
			// dont init card mana for illusion champion
			if (championData.isIllusion || championData.isNPC)
				return;

			// Logs.Info($"InitCardMana ChampionData: {championData.name}/{championData.netId}. CardSkillData: {cardSkillData.KeyName}. Creator net ID: {championData.creatorNetId}");

			var cardMana = new CardManaData(
				championData.netId,
				cardSkillData.KeyName,
				cardSkillData.manaCost,
				0,
				cardSkillData.manaCost > 0 ? _minimumManaCost : 0);

			// add new card mana collection
			if (_userCardManas.TryGetValue(championData.creatorNetId, out var listCardManaData) == false)
			{
				listCardManaData = new List<CardManaData>();
				listCardManaData.Add(cardMana);

				_userCardManas.Add(championData.creatorNetId, listCardManaData);
				//Logs.Info($"_userCardManas.Add COUNT: {_userCardManas.Count}");
				return;
			}

			// update or add card mana to collection
			var index = listCardManaData.FindIndex(c => c.ChampionNetId == championData.netId && c.CardKey.Equals(cardSkillData.KeyName));
			if (index < 0)
				listCardManaData.Add(cardMana);
			else
				listCardManaData[index] = cardMana;

			_userCardManas[championData.creatorNetId] = listCardManaData;
		}

		/// <summary>
		/// Add mana for a card
		/// </summary>
		/// <param name="championData"></param>
		/// <param name="cardKeyName"></param>
		/// <param name="bonus"></param>
		[Server]
		public void AddManaCostBonus(ChampionData championData, string cardKeyName, int bonus)
		{
			// get user net from sender
			if (false == GetCardManaData(championData.creatorNetId, championData.netId, cardKeyName, out var cardSkillData, out var index))
			{
				Logs.Error($"Cant Find Card Data {cardKeyName} In Champion {championData.name}:");
				return;
			}

			// update to dictionary
			_userCardManas[championData.creatorNetId][index] = cardSkillData.Clone(cardSkillData.BonusCost + bonus);

			// Update mana cost
			RpcUpdateCardManaCost(championData.creatorNetId, _userCardManas[championData.creatorNetId]);
		}

		public void AddManaCostBonusToAll(uint userNetId, int bonus)
		{
			if (_userCardManas.TryGetValue(userNetId, out var listCardData) == false)
			{
				Logs.Error($"{userNetId}-Not Found In Card Mana Data");
				return;
			}
			var c = listCardData.Count;

			for (int i = 0; i < c; i++)
			{
				var currentMana = listCardData[i];
				listCardData[i] = currentMana.Clone(currentMana.BonusCost + bonus);
			}

			_userCardManas[userNetId] = listCardData;

			RpcUpdateCardManaCost(userNetId, listCardData);
		}

		public List<CardManaData> GetAllCards(uint userNetId)
		{
			if (_userCardManas.TryGetValue(userNetId, out var listCardData) == false)
			{
				Logs.Error($"{userNetId}-Not Found In Card Mana Data");
				return new List<CardManaData>();
			}

			return listCardData;
		}

		/// <summary>
		/// Consume
		/// </summary>
		/// <param name="championData"></param>
		/// <param name="cardKeyName"></param>
		/// <returns></returns>
		[Server]
		public bool ConsumeMana(ChampionData championData, string cardKeyName)
		{
			// illusion dont use mana
			if (championData.isIllusion || championData.isNPC)
				return true;

			// get user net from sender
			if (false == GetCardManaData(championData.creatorNetId, championData.netId, cardKeyName, out var cardSkillData, out _))
			{
				Logs.Error($"Cant Find Card Data {cardKeyName} In Champion {championData.name}:");
				return false;
			}


			var originCost = Mathf.FloorToInt(cardSkillData.OriginCost);
			var currentCost = Mathf.FloorToInt(cardSkillData.CurrentCost);

			// add origin Cost to ultimate energy
			championData.AddUltimateEnergyWhenUseMana(originCost);

			// use current mana cost
			return _manaManager.UseMana(championData.creatorNetId, currentCost);
		}

		/// <summary>
		/// Add mana cost for all card in a champion
		/// </summary>
		/// <param name="championData"></param>
		/// <param name="bonus"></param>
		[Server]
		public void AddManaCostBonus(ChampionData championData, int bonus)
		{
			if (false == _userCardManas.TryGetValue(championData.creatorNetId, out var listCards))
			{
				Logs.Error("Cant Find List Card For User Net Id");
				return;
			}

			for (int i = 0; i < listCards.Count; i++)
			{
				if (listCards[i].OriginCost == 0 || listCards[i].ChampionNetId != championData.netId)
					continue;

				var currentMana = listCards[i];
				listCards[i] = currentMana.Clone(currentMana.BonusCost + bonus);
			}

			_userCardManas[championData.creatorNetId] = listCards;

			RpcUpdateCardManaCost(championData.creatorNetId, listCards);
		}


		[ClientRpc]
		private void RpcUpdateCardManaCost(uint userNetId, List<CardManaData> listCards)
		{
			if (_manaManager.CurrentManaKey == userNetId)
				CardPoolManager.Instance.UpdateManaCost(listCards);
		}

		/// <summary>
		/// Get Card Mana Data
		/// </summary>
		/// <param name="userNetId"></param>
		/// <param name="cardKeyName"></param>
		/// <param name="cardManaData"></param>
		/// <param name="championNetId"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		private bool GetCardManaData(uint userNetId, uint championNetId, string cardKeyName, out CardManaData cardManaData, out int index)
		{
			cardManaData = default;
			index = -1;
			if (_userCardManas.TryGetValue(userNetId, out var listCardData) == false)
			{
				//	Logs.Warning($"{userNetId}-Not Found In Card Mana Data");
				return false;
			}

			index = listCardData.FindIndex(c => c.CardKey == cardKeyName && c.ChampionNetId == championNetId);
			if (index < 0)
			{
//				Logs.Warning($"Cant Find Card key Name: {cardKeyName}. User net Id: {userNetId}. List Card Added: {listCardData.Count}");
				return false;
			}

			// Logs.Warning($"User Net ID: {userNetId}. {cardKeyName}. Current Cost:{listCardData[index].CurrentCost}");
			cardManaData = listCardData[index];
			return true;
		}

		public bool GetCardManaData(uint userNetId, uint championNetId, string cardKeyName, out CardManaData cardManaData)
		{
			if (GetCardManaData(userNetId, championNetId, cardKeyName, out cardManaData, out var index) == false)
				return false;

			cardManaData = _userCardManas[userNetId][index];
			return true;
		}

		public bool GetCardManaData(ChampionData championData, string cardKeyName, out CardManaData cardManaData)
			=> GetCardManaData(championData.creatorNetId, championData.netId, cardKeyName, out cardManaData);

		public bool GetCardManaData(ChampionData championData, out List<CardManaData> cardManaData)
			=> _userCardManas.TryGetValue(championData.creatorNetId, out cardManaData);

		/// <summary>
		/// Get Current Card Mana Cost
		/// </summary>
		/// <param name="championData"></param>
		/// <param name="cardKeyName"></param>
		/// <returns></returns>
		public uint GetCurrentCardManaCost(ChampionData championData, string cardKeyName)
		{
			if (GetCardManaData(championData.creatorNetId, championData.netId, cardKeyName, out var cardManaData, out _) == false)
			{
				return 0;
			}

			return (uint)cardManaData.CurrentCost;
		}

		[Server]
		public void Clear()
		{
			_userCardManas.Clear();
		}

	}


	public static class CardManaSystemExts
	{
		static ICardManaSystem _cardManaSystem;
		public static void Init(ICardManaSystem cardManaSystem) => _cardManaSystem = cardManaSystem;

		public static void AddManaCostBonus(this ChampionData championData, int bonus)
			=> _cardManaSystem.AddManaCostBonus(championData, bonus);

		public static void AddManaCostBonus(this ChampionData championData, string cardKeyName, int bonus)
			=> _cardManaSystem.AddManaCostBonus(championData, cardKeyName, bonus);

		public static bool ConsumeMana(this ChampionData championData, string cardKeyName)
			=> _cardManaSystem.ConsumeMana(championData, cardKeyName);

		public static uint GetCurrentCardManaCost(this ChampionData championData, string cardKeyName)
			=> _cardManaSystem.GetCurrentCardManaCost(championData, cardKeyName);

		public static bool GetCardManaData(this ChampionData championData, string cardKeyName, out CardManaData cardManaData)
			=> _cardManaSystem.GetCardManaData(championData.creatorNetId, championData.netId, cardKeyName, out cardManaData);

		public static bool GetCardManaData(this ChampionData championData, out List<CardManaData> cardManaData)
			=> _cardManaSystem.GetCardManaData(championData, out cardManaData);

		public static bool GetCardManaData(uint userNetId, uint championNetId, string cardKeyName, out CardManaData cardManaData)
			=> _cardManaSystem.GetCardManaData(userNetId, championNetId, cardKeyName, out cardManaData);

	}
}