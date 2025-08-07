using System.Collections.Generic;
using ROI.DataEntity;
namespace ROI
{
	public interface ICardManaSystem
	{
		void InitCardMana(ChampionData championData, CardSkillData cardSkillData);

		bool ConsumeMana(ChampionData championData, string cardKeyName);

		void AddManaCostBonus(ChampionData championData, string cardKeyName, int bonus);
		void AddManaCostBonus(ChampionData championData, int bonus);

		void AddManaCostBonusToAll(uint userNetId, int bonus);

		List<CardManaData> GetAllCards(uint userNetId);

		uint GetCurrentCardManaCost(ChampionData championData, string cardKeyName);

		bool GetCardManaData(uint userNetId, uint championNetId, string cardKeyName, out CardManaData cardManaData);
		bool GetCardManaData(ChampionData championData, string cardKeyName, out CardManaData cardManaData);

		bool GetCardManaData(ChampionData championData, out List<CardManaData> cardManaData);

		void Clear();
	}
}