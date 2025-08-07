using Mirror;

namespace ROI
{
    class ChampionOnDamagedText : NetworkBehaviour, IOnDamaged, IInjectInstance<ChampionDamageText>
    {
        private ChampionData _championData;
        private ChampionDamageText _championDamageText;
        private bool _isShowDamage = true; 

        private void Awake()
        {
            _championData = GetComponent<ChampionData>();
        }

        public void OnInject(ChampionDamageText instance)
        {
            // Debug.Log("OnInject");
            _championDamageText = instance;
            _isShowDamage = true;
        }

        /// <summary>
        /// On Health Reduction
        /// </summary>
        /// <param name="damageDealtData"></param>
        /// <param name="attacker"></param>
        public void OnDamaged(ChampionData attacker,DamageDealtData damageDealtData)
        {
            if (_isShowDamage)
                _championDamageText.ShowDamage(_championData, damageDealtData);
            _isShowDamage = !_championData.IsDeath;
        }


// #if UNITY_EDITOR
//
//         private int totalAttack = 1000;
//
//         private void OnEnable()
//         {
//             var totalCrit = 0f;
//             var chance = _championData.attackData.critDamageChance;
//
//
//             for (int i = 0; i < totalAttack; i++)
//             {
//                 if (DamageCalc.IsCritical(chance))
//                     totalCrit++;
//             }
//
//             //Logs.Warning(
//             //    $"Total Crit On {totalAttack} attacks Is: {totalCrit}. (chance: {chance}.real percent:{totalCrit * 100f / totalAttack}%) ");
//         }
// #endif
    }

    // public class DamageData
    // {
    //     public int value;
    // }
}