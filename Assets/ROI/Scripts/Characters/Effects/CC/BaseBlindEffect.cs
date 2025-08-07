using Mirror;

namespace ROI
{


    public class BaseBlindEffect : DisplayableEffect,IEffectSystem,IOnHitEnemy
    {
        public void ApplyEffect(ChampionData champion, StatusData arg)
        {
            if (effectSound)
            {
                SoundManager.PlaySfx(effectSound);
            }
            champion.handles.OnHitEnemies.Add(this);
            ApplyVFX(champion);
            ApplyIcon(champion,1, ChampionEffects.Blind, arg.remain_duration, arg.setting.duration);
        }

        public void RemoveEffect(ChampionData champion, StatusData arg)
        {
            champion.handles.OnHitEnemies.Remove(this);
            ClearVFX();
            RemoveIcon(champion, ChampionEffects.Blind, arg.level + 1);
            RemoveEffect();
        }

        public void ReApplyEffect(ChampionData champion, StatusData arg)
        {
            if (effectSound)
            {
                SoundManager.PlaySfx(effectSound);
            }
            arg.remain_duration = arg.remain_duration_unscaled = arg.setting.duration;
        }
        
        public void OnHitEnemy(ChampionData enemy, DamageDealtData damageDealtData)
        {
            if (damageDealtData.damageSource == DamageSources.BasicAttack)
            {
                damageDealtData.dodgeChancePercent = 1;
            }
        }
        [ClientRpc]
        private void RemoveEffect()
        {
            gameObject.Recycle();
        }
    }
}