using System;
using System.Runtime.CompilerServices;
using Mirror;
using ROI.Scripts.Controller;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ROI
{
    [RequireComponent(typeof(IObjectPool))]
    class ChampionDamageText : NetworkBehaviour
    {
        public static ChampionDamageText instance;
      
        private IObjectPool _objectPool;

        [SerializeField] private Transform _parent;
        [SerializeField] private Object _damagePrefab;
        [SerializeField] private uint _poolSize = 16;
        [SerializeField] private uint minDamageTextSize = 30;
        [SerializeField] private uint maxDamageTextSize = 45;
        [SerializeField]private float damageTextScale = 0f;

        [SerializeField]
        private int normalDamageFontSize => (int)(minDamageTextSize * (1 + damageTextScale));

        [SerializeField] private int critDamageFontSize => (int)(maxDamageTextSize * (1 + damageTextScale));

        [SerializeField] private Color normalDamageColor = Color.white;
        [SerializeField] private Color magicDamageColor = Color.cyan;

        [SerializeField] private Color healingColor = Color.green;
        [SerializeField] private Color critDamageColor = Color.yellow;

        [SerializeField] private Color critMagicDamageColor = Color.blue;
        [SerializeField] private Color PoisonDamageColor = Color.magenta;
        [SerializeField] private Color blindColor = Color.red;

        [SerializeField] private TMP_FontAsset normalDamageFont;
        [SerializeField] private TMP_FontAsset healingFont;
        [SerializeField] private TMP_FontAsset critDamageFont;
        [SerializeField] private TMP_FontAsset blindFont;

        private int _poolID;
        private Camera _camera;
        private void Awake()
        {
            _objectPool = GetComponent<IObjectPool>();
            _camera = Camera.main;
            instance = this;
        }

        private void Start()
        {
            _poolID = _objectPool.CreatPool(_damagePrefab, _poolSize);
        }

        /// <summary>
        /// show efectt Blind
        /// </summary>
        /// <param name="champion"></param>
        public void RpcShowBlind(ChampionData champion)
        {
            var floatText = GetFloatingText(champion);
            floatText.SetText($"Miss", blindColor, normalDamageFontSize);
        }
        /// <summary>
        /// show efectt Blind
        /// </summary>
        /// <param name="champion"></param>
        public void RpcShowBlock(ChampionData champion)
        {
            var floatText = GetFloatingText(champion);
            floatText.SetText($"Block", blindColor, normalDamageFontSize);
        }
        /// <summary>
        /// show normal damage on a champion
        /// </summary>
        /// <param name="damage"></param>
        /// <param name="champion"></param>
        private void RpcShowNormalDamage(ChampionData champion, float damage)
        {
            // setting for show normal damage
            if (damage > 0)
            {
                var floatText = GetFloatingText(champion);
                var d = Mathf.FloorToInt(damage);
                floatText.SetText($"{d}", normalDamageColor, normalDamageFontSize, false, normalDamageFont);
            }
        }

        private void RpcShowMagicDamage(ChampionData champion, float damage)
        {
            // setting for show normal damage
            if (damage > 0)
            {
                var floatText = GetFloatingText(champion);
                var d = Mathf.FloorToInt(damage);
                floatText.SetText($"{d}", magicDamageColor, normalDamageFontSize, false, normalDamageFont);
            }
        }
        
        private void RpcShowCritMagicDamage(ChampionData champion, float damage)
        {
            if (damage > 0)
            {
                var floatText = GetFloatingText(champion);
                var d = Mathf.FloorToInt(damage);
                floatText.SetText($"{d}", critMagicDamageColor, critDamageFontSize, true, critDamageFont);
            }
        }

        private void RpcShowPoisonDamage(ChampionData champion, float damage)
        {
            var floatText = GetFloatingText(champion);
            var d = Mathf.FloorToInt(damage);
            floatText.SetText($"{d}", PoisonDamageColor, critDamageFontSize, false, critDamageFont);
        }
        
        private void RpcShowBlessDamage(ChampionData champion, float damage)
        {
            GameObserver.GetInstance().OnHealHp(champion, damage);
            var floatText = GetFloatingText(champion);
            var d = Mathf.FloorToInt(damage);
            floatText.SetText($"{d}", healingColor, critDamageFontSize, false, critDamageFont);
        }

        public void ShowDamage(ChampionData champion, DamageDealtData damageDealtData)
        {
            // zero damage
            // if(damageDealtData.zeroDamage)
            //    return;
            damageTextScale = Mathf.Lerp(0, 1, Mathf.Min(damageDealtData.finalDamage / 1000, 1));
            if (damageDealtData.isDodge)
            {
                RpcShowBlind(champion);
            }
            else if (Math.Abs(damageDealtData.blockPercent - 1) < 0.001f)
            {   
                RpcShowBlock(champion);
                
            }
            switch (damageDealtData.damageType)
            {
                case DamageTypes.Magic:
                    ShowMagicDamage(champion, damageDealtData);
                    break;
                case DamageTypes.Physic:
                    ShowBasicDamage(champion, damageDealtData);
                    break;
                case DamageTypes.True:
                    var floatText = GetFloatingText(champion);
                    var d = Mathf.FloorToInt(damageDealtData.finalDamage);
                    floatText.SetText($"{d}", PoisonDamageColor, normalDamageFontSize, false, normalDamageFont);
                    break;
            }
        }
        
        // [ClientRpc]
        public void ShowBasicDamage(ChampionData champion, DamageDealtData damageDealtData)
        {
            // Logs.Info($"{champion.gameObject.name} ShowBasicDamage: {damageDealtData.healthReduction}");
            var healthReduction = damageDealtData.GetFinalDamage();
            
            if (damageDealtData.isCrit)
                RpcShowCritDamage(champion, healthReduction);
            else
                RpcShowNormalDamage(champion, healthReduction);
        }
        
        // [ClientRpc]
        public void ShowMagicDamage(ChampionData champion, DamageDealtData damageDealtData)
        {
            var healthReduction = damageDealtData.GetFinalDamage();
            
            if (damageDealtData.isCrit)
                RpcShowCritMagicDamage(champion, healthReduction);
            else
                RpcShowMagicDamage(champion, healthReduction);
        }

        internal void ShowPoisonDamage(ChampionData champion, int actual_dmg)
        {
            RpcShowPoisonDamage(champion, actual_dmg);
        }
        
        internal void ShowHealDamage(ChampionData champion, int actual_dmg)
        {
            RpcShowBlessDamage(champion, actual_dmg);
        }


        /// <summary>
        /// show healing on a champion
        /// </summary>
        /// <param name="damage"></param>
        /// <param name="champion"></param>
        [ClientRpc]
        public void RpcShowhealing(ChampionData champion, int damage)
        {
            var floatText = GetFloatingText(champion);
            floatText.SetText($"{damage}", healingColor, normalDamageFontSize, false, healingFont);
        }

        /// <summary>
        /// show damage with critical strike
        /// </summary>
        /// <param name="champion"></param>
        /// <param name="damage"></param>
        private void RpcShowCritDamage(ChampionData champion, float damage)
        {
            var floatText = GetFloatingText(champion);
            var d = Mathf.FloorToInt(damage);
            floatText.SetText($"{d}", critDamageColor, critDamageFontSize, true, critDamageFont);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private DamageFloatingText GetFloatingText(ChampionData championData)
        {
            if (_objectPool.Use(_poolID, out var go) == false)
            {
                Logs.Error("Use Damage Floating Text Fail!!!");
                return default;
            }

            go.transform.SetParent(_parent);
            go.transform.localScale = Vector3.one;
            go.transform.position = _camera.WorldToScreenPoint(championData.CenterPosition);
            return go.GetComponent<DamageFloatingText>();
        }

        
    }
}