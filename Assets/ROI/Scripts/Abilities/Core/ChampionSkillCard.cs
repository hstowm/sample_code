using System.Collections.Generic;
using ROI.DataEntity;
using UnityEngine;

namespace ROI
{
	public class ChampionSkillCard : MonoBehaviour, IInjectInstance<ChampionData>, IPauseHandle
	{
		public ChampionBaseData baseData;

		public GameObject skill1;
		public GameObject ultimate;
		public GameObject passiveObj;
		protected ChampionData _championData;
		private ISkillCardTrigger _skill;
		private ISkillCardTrigger _ultimate;

		public IChampionPassiveHandle passive;
		// [HideInInspector] public ChampionSkillData skillCardData;
		[HideInInspector] public SkillsPlayer skills1SkillPlayer;
		[HideInInspector] public SkillsPlayer UltimateSkillsPlayer;

		protected virtual void Awake()
		{
			skills1SkillPlayer = skill1.GetComponent<SkillsPlayer>();
			UltimateSkillsPlayer = ultimate.GetComponent<SkillsPlayer>();
			if (passiveObj) passive = passiveObj.GetComponent<IChampionPassiveHandle>();
			if (skills1SkillPlayer)
				skills1SkillPlayer.Events.OnComplete.AddListener(OnSkill1Completed);
			else
			{
				// init hope skill hardcode
				var hopeSkill = skill1.GetComponent<HopeSkillTrigger>();
				if (hopeSkill)
				{
					var skillPlayer = hopeSkill.skillTargetEnemy.GetComponent<SkillsPlayer>();

					skillPlayer?.Events.OnComplete.AddListener(OnSkill1Completed);

					skillPlayer = hopeSkill.SkillTargetAlies.GetComponent<SkillsPlayer>();
					skillPlayer?.Events.OnComplete.AddListener(OnSkill1Completed);

				}
			}

			UltimateSkillsPlayer.Events.OnComplete.AddListener(OnUltimateCompleted);
		}


		public void OnInject(ChampionData instance)
		{
			_championData = instance;
			_skill = skill1.GetComponent<ISkillCardTrigger>();
			_ultimate = ultimate.GetComponent<ISkillCardTrigger>();

			instance.GetComponent<IObjectPool>();

			//_championData.handles.OnStartAutoAttacks.Add(this);
			// _championData.handles.OnStopAttacks.Add(this);
		}

		public void InitObjectPool(IObjectPool objectPool)
		{
			UltimateSkillsPlayer.InitObjectPool(objectPool);

			if (skills1SkillPlayer)
				skills1SkillPlayer.InitObjectPool(objectPool);
			else
			{
				var hopeSkill = skill1.GetComponent<HopeSkillTrigger>();
				if (hopeSkill)
				{
					var skillPlayer = hopeSkill.skillTargetEnemy.GetComponent<SkillsPlayer>();

					skillPlayer?.InitObjectPool(objectPool);

					skillPlayer = hopeSkill.SkillTargetAlies.GetComponent<SkillsPlayer>();
					skillPlayer?.InitObjectPool(objectPool);

				}
			}

		}

		public virtual void DoActiveSkill(Vector3 inputPosition, List<ChampionData> targets, bool isServer)
		{
			_championData.controller.Pause(this);
			_championData.animatorNetwork.animator.fireEvents = false;
			_skill.StartSkill(inputPosition, targets, isServer);
		}

		public virtual void DoUltimateSkill(Vector3 inputPosition, List<ChampionData> targets, bool isServer)
		{
			// dont use when not full
			_championData.controller.Pause(this);
			_championData.ShowUltimateCard(false);
			_championData.animatorNetwork.animator.fireEvents = false;
			_ultimate.StartSkill(inputPosition, targets, isServer);
		}

		public virtual void OnSkill1Completed()
		{
			// resume
			IsPaused = false;
		}

		public virtual void OnUltimateCompleted()
		{
			// resume
			IsPaused = false;
		}

		public bool IsPaused { get; set; }

		protected virtual void OnEndSkill(){}
		protected virtual void OnEnd(){}
	}
}