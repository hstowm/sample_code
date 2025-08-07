using System.Collections.Generic;
using kcp2k;
using UnityEngine;

namespace ROI
{
    public class PulseEcho : BaseActiveAbilityCard
    {
        bool isServer;

        public override void StartSkill(Vector3 inputPosition, List<ChampionData> targets, bool isServer)
        {
            this.isServer = isServer;

            base.StartSkill(inputPosition, targets, isServer);

            skillsPlayer.PlayFeedbacks();
        }


        public void OnInitImpact(ROI_Insaniatate instantiateObject)
        {
            if (championsEffectBySkill[0] != null)
            {
                instantiateObject.TargetPosition = championsEffectBySkill[0].transform.position;

                if (isServer)
                {
                    GeneralEffectSystem.Instance.ApplyEffect(_championData, new StatusData("Frenzy", _championData, Vector3.zero));
                    
                    // Applies one of different possible effects (Engulfed, Blessed, 10% shield, 10% life steal)
                    var effectsPool = new List<string> { "Engulfing", "Bless", "PulseEcho_ApplyShield", "PulseEcho_ApplyLifeSteal" };
                    var randomEffect = effectsPool[Random.Range(0, effectsPool.Count)];
                    Logs.Info($"PulseEcho: Applying effect {randomEffect}");
                    GeneralEffectSystem.Instance.ApplyEffect(_championData, new StatusData(randomEffect, _championData, Vector3.zero));
                }
            }
        }
    }
}