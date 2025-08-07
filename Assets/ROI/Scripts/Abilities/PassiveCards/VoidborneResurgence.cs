using ROI;
using UnityEngine;
/// <summary>
/// Upon reaching 25% health or less, gain a shield equal to 10% of my maximum health and +15% life Steal that last up to 5 seconds (only once per battle)
/// </summary>
[CreateAssetMenu(fileName = "VoidborneResurgence", menuName = "ROI/Data/AbilityPassiveCards/VoidborneResurgence", order = 1)]
public class VoidborneResurgence : BasePassiveAbilityCard, IAbilityCard
{
    [SerializeField] private float healthThreshold;
    [SerializeField] private StatusSetting shield;
    [SerializeField] private StatusSetting statBonus;
    public override void OnInit(ChampionData champion)
    {
        champion.handles.OnAttacked.Add(new BonusStatWhenLowHealth(champion, healthThreshold, shield, statBonus));
    }
    

    public class BonusStatWhenLowHealth : IOnAttacked
    {
        private ChampionData _championData;
        private float _healthThreshold;
        private float _currentShieldBonus;
        private StatusSetting shield;
        private StatusSetting statBonus;
        public BonusStatWhenLowHealth(ChampionData championData, float healthThreshold, StatusSetting shieldSetting, StatusSetting lifeStealBonus)
        {
            _healthThreshold = healthThreshold;
            _championData = championData;
            shield = shieldSetting;
            statBonus = lifeStealBonus;
        }

        public void OnHit(ChampionData attacker, DamageDealtData damageDealtData)
        {
            if ( _championData != null && (float)_championData.healthData.health / (float)_championData.healthData.maxHealth < _healthThreshold)
            {
                GeneralEffectSystem.Instance.ApplyEffect(_championData, new StatusData(shield, _championData, new Vector3()));
                GeneralEffectSystem.Instance.ApplyEffect(_championData, new StatusData(statBonus, _championData, new Vector3()));
                _championData.handles.OnAttacked.Remove(this);
            }
        }
    }
}
