using ROI.DataEntity;
using UnityEngine;

namespace ROI
{
	[CreateAssetMenu(fileName = "ManaOverflowPassive",
		menuName = "ROI/Data/AbilityPassiveCards/ManaOverflow")]
	public class ManaOverflowPassive : BasePassiveAbilityCard, IAbilityCard
	{
		public float damagePercentagePerSkill;

		// [Server]

		public override void OnInit(ChampionData champion)
		{
			Logs.Info($"Init ManaOverflowPassive: {champion.netId}");
			ManaOverflowInject manaOverflowInject = new ManaOverflowInject(champion, damagePercentagePerSkill);
			champion.handles.OnHitEnemies.Add(manaOverflowInject);
		}

		class ManaOverflowInject : IOnHitEnemy, IOnFinalDamage
		{

			float damagePercentagePerSkill;
			ChampionData championData;
			ManaManager _manaManager;

			public ManaOverflowInject(ChampionData championData, float damagePercentagePerSkill)
			{
				this.championData = championData;
				this.damagePercentagePerSkill = damagePercentagePerSkill;
				_manaManager = FindObjectOfType<ManaManager>(true);
			}

			public void OnHitEnemy(ChampionData enemy, DamageDealtData damageDealtData)
			{
				if (!damageDealtData.damageSource.IsAbility() || _manaManager == null)
					return;
				
				Logs.Info($"Current Mana: {_manaManager.GetCurrentMana(championData.creatorNetId)} ");

				var currentMana = Mathf.Clamp(_manaManager.GetCurrentMana(championData.creatorNetId), 0, 3);

				float percentDamage = damagePercentagePerSkill * currentMana;

				Logs.Info($"Percent Damage Bonus: {percentDamage}");
				damageDealtData.AddBonusDamage(percentDamage, StatValueType.Percent);

				damageDealtData.hooks.OnFinalDamages.Add(this);
			}
			public void OnDamageCalculated(DamageDealtData damageDealtData)
			{
				damageDealtData.Print();
			}
		}
	}
}