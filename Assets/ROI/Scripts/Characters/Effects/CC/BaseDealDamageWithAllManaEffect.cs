using System.Collections.Generic;
using UnityEngine;
using NetworkBehaviour = Mirror.NetworkBehaviour;
namespace ROI
{
    public class BaseDealDamageWithAllManaEffect : NetworkBehaviour, IEffectSystem
    {
		[SerializeField] private ChampionDamageText _championDamageText;
		[SerializeField] private ManaManager _manaManager;
		public void ApplyEffect(ChampionData champion, StatusData arg)
		{
			if (_championDamageText == null)
			{
				_championDamageText = ChampionDamageText.instance;
			}

			if(_manaManager == null)
            {
            _manaManager = FindObjectOfType<ManaManager>(true);
			}
		
		float	manaUsing = _manaManager.GetCurrentUserMana();
			_manaManager.UseMana(arg.creator.creatorNetId, (int)manaUsing);

			StatusParam current_level = arg.GetCurrentParam();

			foreach (KeyValuePair<StatusParamKeyWord, float> entry in current_level.param_list)
			{
				switch (entry.Key)
				{
					case StatusParamKeyWord.NormalDamage:
						arg.creator.attacker.AttackEnemy(champion, entry.Value * manaUsing, DamageSources.ActiveCardSkill, DamageTypes.Physic);
						break;
					case StatusParamKeyWord.MagicDamage:
						arg.creator.attacker.AttackEnemy(champion, entry.Value * manaUsing, DamageSources.ActiveCardSkill, DamageTypes.Magic
						);
						break;
					case StatusParamKeyWord.DamagePercent:
						arg.creator.attacker.AttackEnemy(champion, entry.Value * manaUsing, DamageSources.ActiveCardSkill, DamageTypes.Physic);
						break;
				}
			}


		}

		//Simply destroy or recyle this effect system
		public void RemoveEffect(ChampionData champion, StatusData arg)
		{
			Destroy(this.gameObject);
		}

		public void ReApplyEffect(ChampionData champion, StatusData arg)
		{

		}

		// Start is called before the first frame update
	}
}