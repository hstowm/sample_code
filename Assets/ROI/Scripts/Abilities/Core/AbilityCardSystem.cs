using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Mirror;
using ROI.DataEntity;
using UnityEngine;

namespace ROI
{
	/// <summary>
	/// Init & Do All Ability Cards For a champion
	/// </summary>
	[RequireComponent(typeof(ICardManaSystem))]
	[RequireComponent(typeof(IObjectPool))]
	public class AbilityCardSystem : NetworkBehaviour, IAbilityCardSystem
	{
		private readonly Dictionary<uint, List<BaseActiveAbilityCard>> _championActiveSkills = new(16);

		private IReticleSystem _reticleSystem;
		private IObjectPool _objectPool;
		private ChampionManager _championManager;
		private ICardManaSystem _cardManaSystem;

		private void Awake()
		{
			_reticleSystem = FindObjectOfType<ReticleSystem>(true);
			_objectPool = GetComponent<IObjectPool>();
			_championManager = GetComponent<ChampionManager>();
			_cardManaSystem = GetComponent<ICardManaSystem>();

			ChampionCardExts.Init(this);
		}

		public override void OnStartServer()
		{
			base.OnStartServer();
			_cardManaSystem.Clear();
		}

		public override void OnStartClient()
		{
			base.OnStartClient();
			_championActiveSkills.Clear();
		}

		private void OnDisable()
		{
			foreach (KeyValuePair<uint,List<BaseActiveAbilityCard>> skill in _championActiveSkills)
			{
				foreach (BaseActiveAbilityCard abilityCard in skill.Value)
				{
					abilityCard.gameObject.Recycle();
				}
			}
			
			_championActiveSkills.Clear();
		}

		// ------------------------------------------------- INIT CARDs --------------------------------------------

		[Server]
		public void InitAbilityCards(ChampionData championData, List<string> listAbilityCards)
		{
			RpcInitCoreCard(championData);

			if (listAbilityCards == null || listAbilityCards.Count < 1)
				return;

			foreach (var keyCard in listAbilityCards)
			{
				RpcInitAbilityCards(championData, keyCard);
			}
		}

		[ClientRpc]
		private void RpcInitCoreCard(ChampionData championData)
		{
			// Init Mana For Core Active Card
			var cardSkillMana = championData.GetComponent<ChampionSkillCard>();
			// cardSkillMana.InitObjectPool(_objectPool);

			if (!cardSkillMana)
				return;

			if (isServer)
				_cardManaSystem.InitCardMana(championData, cardSkillMana.baseData.active);

			cardSkillMana.InitObjectPool(_objectPool);
		}

		/// <summary>
		/// Init ability card on champion
		/// </summary>
		/// <param name="championData"></param>
		/// <param name="cardKeyName"></param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[ClientRpc]
		private void RpcInitAbilityCards(ChampionData championData, string cardKeyName)
		{
			if (string.IsNullOrEmpty(cardKeyName))
				return;

			if (false == GameData.abilityCardDB.logicImplementor.TryGetValue(
				    cardKeyName,
				    out var abilityCard))
			{
				Logs.Error($"Cant Find Logic Implementor for Card Key Name: {cardKeyName} To the Champion: {championData.gameObject.name}");
				return;
			}

			// init passive card
			if (abilityCard is BasePassiveAbilityCard passiveAbilityCard)
			{
				if (isServer)
					_cardManaSystem.InitCardMana(championData, passiveAbilityCard.cardSkillData);

				passiveAbilityCard.OnInit(championData);
				Logs.Info($"Init PassiveCard: {passiveAbilityCard.name}");
				return;
			}

			// int active card
			if (abilityCard is BaseActiveAbilityCard activeCard)
			{
				if (isServer)
					_cardManaSystem.InitCardMana(championData, activeCard.cardSkillData);

				Logs.Info($"RpcCreateActiveCard: {activeCard.name}");
				CreateActiveCard(championData, activeCard);

				return;
			}

			Logs.Error($"Dont Found Logic Implementor For Card Key Name: {cardKeyName}");
		}


		//[ClientRpc]
		private void CreateActiveCard(ChampionData champion, BaseActiveAbilityCard abilityCard)
		{
			// if (false == GameData.abilityCardDB.logicImplementor.TryGetValue(
			// 	    cardKeyName,
			// 	    out var abilityCard))
			// {
			// 	Logs.Error($"Init Card Key name: {cardKeyName}");
			// 	return;
			// }

			//if (abilityCard is BaseActiveAbilityCard activeCard)
			//{
			Logs.Info("Create Basic Active Ability Card");
			_objectPool.CreatPool(abilityCard.gameObject);
			if (_objectPool.Use(abilityCard.gameObject, out GameObject obj) == false)
				return;

			var activeAbilityInstance = obj.GetComponent<BaseActiveAbilityCard>();
			activeAbilityInstance.OnInit(champion);
			activeAbilityInstance.skillsPlayer.InitObjectPool(_objectPool);

			if (champion.isIllusion)
				activeAbilityInstance.OnStartAlive();


			Logs.Info($"Init Active Card: {activeAbilityInstance.gameObject.name}");

			if (_championActiveSkills.TryGetValue(champion.netId, out var listSkills))
			{
				listSkills.Add(activeAbilityInstance);
				return;
			}

			_championActiveSkills.Add(champion.netId, new List<BaseActiveAbilityCard>(6) { activeAbilityInstance });
			//}
		}

        [Server]
        public void NPCDoRandomSkill(ChampionData championData)
        {
            if (false == _championActiveSkills.TryGetValue(championData.netId, out var available_skills))
                return;

            if (championData.GetMaxUltimateEnergy() <= 0 || !championData.canUseEnergy)
                return;

            var random_skill = available_skills[Random.Range(0, available_skills.Count-1)];
            var pos = SkillAutoTarget.FindBestPosition(championData, random_skill.cardSkillData);
            DoActiveCard(championData, random_skill.cardSkillData.KeyName, pos);
        }

		[Server]
		private bool TryGetListTargets(
			ChampionData championData,
			CardSkillData cardSkillData,
			Vector3 inputPos,
			out List<ChampionData> listTargets,
			out Vector3 clampedPos)
		{
			listTargets = new List<ChampionData>();
			var indicator = _reticleSystem.InitReticle(championData, cardSkillData);
			indicator.UpdateTarget(inputPos);

			clampedPos = indicator.TargetPosition;

			if (indicator.IsValid == false)
				return false;

			listTargets = indicator.ListTargets;
			if (listTargets == null)
			{
				listTargets = new List<ChampionData>();
			}

			// clear up dead or null target
			for (int i = listTargets.Count - 1; i >= 0; i--)
			{
				if (listTargets[i] == null || listTargets[i].IsDeath)
					listTargets.RemoveAt(i);
			}

			return true;
		}


		// ------------------------------------------------- DO CORE ACTIVE CARD --------------------------------------------

		[Command(requiresAuthority = false)]
		public void DoCoreActiveCard(
			ChampionData championData,
			Vector3 inputPos,
			NetworkConnectionToClient sender = null)
		{
			Logs.Info($"DoCoreActiveCard for champion. Input Position: {inputPos}");

			var skillCardData = championData.GetComponent<ChampionSkillCard>();
			var coreCard = skillCardData.baseData.active;

			if (TryGetListTargets(championData, coreCard, inputPos, out var listTargets, out var clampedPos) == false)
			{
				Logs.Info("No Targets");
				return;
			}

			if (championData.ConsumeMana(coreCard.KeyName) == false)
			{
				Logs.Error("Not Enough Mana");
				return;
			}

			RpcDoChampionSkill(championData, clampedPos, listTargets, false);
		}


		[Command(requiresAuthority = false)]
		public void DoUltimateCard(ChampionData championData, Vector3 inputPos, NetworkConnectionToClient sender = null)
		{
			Logs.Info($"DoUltimateCard for champion. Input Position: {inputPos}");

			if (championData.IsFullUltimateEnergy() == false)
				return;

			var coreCardSkillData = championData.GetComponent<ChampionSkillCard>();
			var ultimateCard = coreCardSkillData.baseData.ultimate;

			if (TryGetListTargets(championData, ultimateCard, inputPos, out var listTargets, out var clampedPos) ==
			    false)
			{
				Logs.Info("No Targets");
				return;
			}

			// reset on use ultimate
			championData.ResetUltimateEnergy();

			RpcDoChampionSkill(championData, clampedPos, listTargets, true);
		}


		[ClientRpc]
		private void RpcDoChampionSkill(
			ChampionData champion,
			Vector3 inputPosition,
			List<ChampionData> listTargets,
			bool isUltimate)
		{

			if (_championManager.listChampions.ContainsKey(champion.netId) == false)
				return;

			var championData = _championManager.listChampions[champion.netId];
			var coreCardSkill = championData.GetComponent<ChampionSkillCard>();

			if (coreCardSkill == null)
			{
				Logs.Error($"The Champion: {championData.name}. Dont Implement Logic: Core Active Card and Ultimate!!!");
				return;
			}

			var skillData = isUltimate
				? coreCardSkill.baseData.ultimate
				: coreCardSkill.baseData.active;

			OnUseCard(championData, skillData, inputPosition, listTargets, isServer);

			if (isUltimate)
			{
				coreCardSkill.DoUltimateSkill(inputPosition, listTargets, isServer);
				return;
			}

			championData.animatorNetwork.animator.fireEvents = false;
			coreCardSkill.DoActiveSkill(inputPosition, listTargets, isServer);

			if (champion.IsFullUltimateEnergy())
				champion.ShowUltimateCard(true);
		}


		// ------------------------------------------------- DO ACTIVE CARD --------------------------------------------

		[Command(requiresAuthority = false)]
		public void DoActiveCard(
			ChampionData championData,
			string cardUID,
			Vector3 inputPos,
			NetworkConnectionToClient sender = null)
		{
			// var championData = _championManager.listChampionsByNetId[champion.netId];
			Logs.Info($"DoActiveCard {cardUID} for champion: {championData.name}. Input Position: {inputPos}");
			var lists = _championActiveSkills[championData.netId];

			var index = lists.FindIndex(c => c.cardSkillData.KeyName == cardUID);
			if (index < 0)
			{
				Logs.Error($"Cant Use Active Card UID: {cardUID}");
				return;
			}

			if (TryGetListTargets(championData, lists[index].cardSkillData, inputPos, out var listTargets,
				    out var clampedPos) == false)
			{
				Logs.Error("Can not find the targets.");
				return;
			}

			// consume mana
			if (championData.ConsumeMana(lists[index].cardSkillData.KeyName) == false)
			{

				Logs.Error($"Cant Enough Mana to Use Card: {cardUID}");

				return;
			}

			RpcDoActiveCard(championData, cardUID, clampedPos, listTargets);
		}

		[ClientRpc]
		private void RpcDoActiveCard(
			ChampionData championData,
			string cardUID,
			Vector3 clampedPos,
			List<ChampionData> listTargets)
		{
			var lists = _championActiveSkills[championData.netId];

			var index = lists.FindIndex(c => c.cardSkillData.KeyName == cardUID);
			if (index < 0)
			{
				Logs.Error($"Cant Find Card Data with uuid: {cardUID}");
				return;
			}

			var baseActive = lists[index];

			// call handle when on use active card

			OnUseCard(championData, baseActive.cardSkillData, clampedPos, listTargets, isServer);
			
			baseActive.StartSkill(clampedPos, listTargets, isServer);

			if (championData.isIllusion == false && championData.IsFullUltimateEnergy())
				championData.ShowUltimateCard(true);
		}

		private void OnUseCard(ChampionData championData, CardSkillData cardSkillData, Vector3 inputPosition, List<ChampionData> listTargets, bool isServerSide)
		{
			// dont call handle on use card for illusion
			if (championData.isIllusion)
				return;

			var activeHandles = championData.handles.OnUseCards.ToArray();

			for (int i = 0; i < activeHandles.Length; i++)
			{
				activeHandles[i].OnUseActiveCard(cardSkillData, inputPosition, listTargets, isServerSide);
			}
		}
	}
}