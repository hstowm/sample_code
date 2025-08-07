using System.Collections.Generic;
using UnityEngine;
namespace ROI
{
	public class ExecutionerStrike : BaseActiveAbilityCard
	{
		public StatusSetting statusDealDamePhysic;
		private ManaManager _manaManager;
		public bool isServer;
		[SerializeField] private AudioClip skillSound;
		
		private void Awake()
		{
			_manaManager = FindObjectOfType<ManaManager>(true);
		}

		public override void StartSkill(Vector3 inputPosition, List<ChampionData> targets, bool isServer)
		{
			this.isServer = isServer;
			base.StartSkill(inputPosition, targets, isServer);
			skillsPlayer.PlayFeedbacks();
			SoundManager.PlaySfxPrioritize(skillSound);
		}

		public void ChampionJump(ROI_OverridePositionFeedback positionFeedback)
		{
			if (championsEffectBySkill.Count == 0) return;
			positionFeedback.AnimatePositionTarget = _championData.gameObject;
			//  positionFeedback.InitialPosition = _championData.transform.position;
			positionFeedback.TrackingPositionTransform = championsEffectBySkill[0].transform;
		}

		public void AddDynamicInstantiate(ROI_Insaniatate instantiateObject)
		{
			instantiateObject.ParentTransform = _championData.transform;
			instantiateObject.TargetPosition = _championData.transform.position;
		}

		public void AddDynamicInstantiateImpact(ROI_Insaniatate instantiateObject)
		{
			if (championsEffectBySkill.Count > 0)
			{
				instantiateObject.TargetPosition = championsEffectBySkill[0].transform.position;
			} else
			{
				instantiateObject.TargetPosition = _championData.transform.position;
			}
		}

		public void OnActionDealDame(GameObject impact)
		{
			if (championsEffectBySkill.Count == 0) return;
			DealDame(championsEffectBySkill[0]);
		}

		void DealDame(ChampionData enemyData)
		{
			// con thieu check xem no la champion, boss, quai thuong
			ExecutionerStrikeOnReduceHealth executionerStrikeOnAttacked = new ExecutionerStrikeOnReduceHealth(_manaManager, enemyData, _championData, (int)cardSkillData.manaCost);
			if (!enemyData.handles.OnDamageds.Contains(executionerStrikeOnAttacked))
				enemyData.handles.OnDamageds.Add(executionerStrikeOnAttacked);

			if (isServer)
			{
				GeneralEffectSystem.Instance.ApplyEffect(enemyData, new StatusData(statusDealDamePhysic.name, _championData, Vector3.zero));
			}
		}

		class ExecutionerStrikeOnReduceHealth : IOnDamaged
		{
			ChampionData champion;
			ChampionData attacker;
			int manaCost;

			private ManaManager _manaManager;

			public ExecutionerStrikeOnReduceHealth(ManaManager manaManager, ChampionData champion, ChampionData attacker, int manacost)
			{
				this.champion = champion;
				this.attacker = attacker;
				this.manaCost = manacost;
				_manaManager = manaManager;
			}

			public void OnDamaged(ChampionData attacker, DamageDealtData damageDealtData)
			{
				if ((damageDealtData.damageSource & DamageSources.ActiveCardSkill) != 0)
				{
					champion.handles.OnDamageds.Remove(this);
					 var healthReduction = damageDealtData.GetFinalDamage();
					if (healthReduction >= champion.healthData.health)
					{

						_manaManager?.AddMana(attacker.creatorNetId, manaCost);
					}
				}
			}

			public bool Equals(ExecutionerStrikeOnReduceHealth other)
			{
				return other != null && other.champion.Equals(champion);
			}

			public override bool Equals(object obj)
			{
				return Equals(obj as ExecutionerStrikeOnReduceHealth);
			}


		}


	}
}