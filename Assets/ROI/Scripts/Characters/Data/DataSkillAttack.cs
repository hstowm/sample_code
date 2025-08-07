using ROI.Skills;
using UnityEngine;

namespace ROI
{
    [System.Serializable]
  public  class DataSkillAttack : MonoBehaviour
    {
        public float baseDamage;
        public DamageTypes damageType;
        public float critChance; // 1 laf 100%
        public float critDamage;
        public float blockTime;
        public float stunTime;
        internal ChampionEffects effects;
        public BaseCard baseCard;
    }

}