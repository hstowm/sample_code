using UnityEngine;

namespace ROI
{

	/// <summary>
	/// When I Crit, chill that enemy if they aren't already chilled.
	/// </summary>
	[CreateAssetMenu(fileName = "ChilledPrecision", menuName = "ROI/Data/AbilityPassiveCards/ChilledPrecision")]
	public class ChilledPrecision : BasePassiveAbilityCard
	{
		

		public override void OnInit(ChampionData champion)
		{
			champion.handles.OnHitEnemies.Add(new ChilledPrecisionInject(champion));
		}


		class ChilledPrecisionInject : IOnFinalDamage, IOnHitEnemy
		{
			
			private ChampionData _championData;
			private ChampionData _enemy;

			public ChilledPrecisionInject(ChampionData championData)
			{
				_championData = championData;
			}
			
			public void OnHitEnemy(ChampionData enemy, DamageDealtData damageDealtData)
			{
				_enemy = enemy;
				damageDealtData.hooks.OnFinalDamages.Add(this);
			}
		
			public void OnDamageCalculated(DamageDealtData damageDealtData)
			{
				if (damageDealtData.isCrit)
				{
					if (_enemy.currentEffect.HasEffect(ChampionEffects.Chilled))
						return;

					GeneralEffectSystem.Instance.ApplyEffect(_enemy, new StatusData("Chilled", _championData, Vector3.zero));
				}
			}
		}
		
		
	}
}