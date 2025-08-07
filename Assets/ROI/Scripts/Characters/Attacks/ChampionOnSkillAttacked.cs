using Mirror;
using ROI.Skills;

namespace ROI
{
    class ChampionOnSkillAttacked : NetworkBehaviour, ISkillAttack
    {
        //private ChampionDamageText _championDamageText;
        private ChampionData _championData;

        //private DamageDealtData _skillDamageDealtData = new DamageDealtData();

        private void Awake()
        {
            _championData = GetComponent<ChampionData>();
        }


        [Server]
        public void ApplyDamage(BaseCharacterCardController cardController)
        {
            if (cardController == null || isServer == false)
                return;
            
            var dataSkillAttack = cardController.dataSkillAttack;
            var attacker = cardController.GetComponent<ChampionData>();

            // _skillDamageDealtData.attackData = new AttackData();
            // _skillDamageDealtData.attackData.damage = (int)dataSkillAttack.baseDamage;
            // _skillDamageDealtData.attackData.damageType = dataSkillAttack.damageType;
            // _skillDamageDealtData.attackData.critDamage = dataSkillAttack.critDamage;
            // _skillDamageDealtData.attackData.critDamageChance = dataSkillAttack.critChance;
            // _skillDamageDealtData.damageSource = DamageSources.CoreActiveSkill;
            //
            // _skillDamageDealtData.healthReduction = DamageCalc.CalcHeathReduction(_championData,
            //     _skillDamageDealtData.attackData, 
            //     out _skillDamageDealtData.totalAttackDamage,
            //     out _skillDamageDealtData.critDamageBonus,
            //     out _skillDamageDealtData.isCit);

//            var health = _championData.healthData.health;

           // var attacker = affectEntity.GetComponent<ChampionData>();
           // attacker.attacker.AttackEnemy(_championData, _skillDamageDealtData);

           
        }

    }
}