using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace ROI
{
	public interface IAbilityCardSystem
	{
		// void InitAbilityCards(ChampionData championData);

		void InitAbilityCards(ChampionData championData, List<string> listActiveCards);
        void NPCDoRandomSkill(ChampionData championData);

        // void InitAbilityPassiveCard(ChampionData championData, string cardKeyName);

        void DoCoreActiveCard(ChampionData championData, Vector3 input, NetworkConnectionToClient sender = null);

		void DoUltimateCard(ChampionData championData, Vector3 input, NetworkConnectionToClient sender = null);

		void DoActiveCard(ChampionData championData, string cardUID, Vector3 input, NetworkConnectionToClient sender = null);

		// void IllusionDoActiveCard(ChampionData illusion, Vector3 inputPos, string cardKeyName);

		// void IllusionDoCoreActiveCard(ChampionData illusion, Vector3 inputPos, string cardKeyName);
	}
}