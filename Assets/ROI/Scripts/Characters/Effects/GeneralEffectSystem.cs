
using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ROI
{

    public class GeneralEffectSystem : NetworkBehaviour
    {
        // Start is called before the first frame update
        //private static GeneralEffectSystem instance;

        public static readonly Dictionary<uint, List<StatusData>> ListEffectData =
            new Dictionary<uint, List<StatusData>>();

        public static GeneralEffectSystem Instance;
        public List<GameObject> effectsApplier = new List<GameObject>();
        public Dictionary<uint, Action<StatusData>> applyEffectActions = new Dictionary<uint, Action<StatusData>>();
        public Dictionary<uint, Action<StatusData>> removeEffectActions = new Dictionary<uint, Action<StatusData>>();
        [SerializeField] private BaseDealDamageEffect dealDamageEffect;
        [SerializeField] public HealEffect healEffect;
        [SerializeField] private GameObject dealDamageEffectPrefab;
        [SerializeField] public GameObject healEffectPrefab;
        private ObjectPool pool;
        public void Awake()
        {
            Instance = this;
            pool = GetComponent<ObjectPool>();
        }

        public override void OnStartClient()
        {
            foreach (KeyValuePair<string, StatusSetting> e in StatusDataConfig.instance.config_list)
            {
                if (e.Value == null)
                {
                    Logs.Error($"Status Config key: {e.Key} has Value Null");
                    continue;
                }
                if (e.Value.StatusApplier != null)
                {
                    if (!NetworkClient.prefabs.ContainsKey(
                            e.Value.StatusApplier.GetComponent<NetworkIdentity>().assetId))
                    {
                        // Logs.Info($"Register StatusApplier: {e.Key}");
                        NetworkClient.RegisterPrefab(e.Value.StatusApplier);
                        pool.CreatPool(e.Value.StatusApplier, 1);
                    }
                }
            }
            ListEffectData.Clear();
        }

        public void ApplyEffect(ChampionData targeted, StatusData data)
        {
            if (isServer)
            {
                ApplyEffectInServer(targeted, data);
                return;
            }
        }

        /*[Command(requiresAuthority = false)]
        public void ApplyEffectCommand(ChampionData targeted, string _name, ChampionData _creator, Vector3 _position)
        {
            StatusData status_data_server = new StatusData(_name, _creator, _position);
            ApplyEffectInServer(targeted, status_data_server);
        }*/

        public void ReApplyEffect(ChampionData targeted, StatusData st)
        {
            st.remain_duration = st.remain_duration_unscaled = st.setting.duration;
            if (st.target == null) st.target = targeted;
            st.effect_system.ReApplyEffect(targeted, st);
        }

        public void ApplyEffectInServer(ChampionData targeted, StatusData data)
        {
            if (GameStateManager.instance.IsGameOver)
                return;
            if (data.setting == null)
            {
                return;
            }
            var listEffects = ListEffectData;
            if (listEffects.TryGetValue(targeted.netId, out var dat) == false)
            {
                dat = new List<StatusData>() { data };
                listEffects.Add(targeted.netId, dat);
            }
            else
            {
                //Check if effect reapply
                foreach (StatusData st in dat)
                {
                    if (st.setting.GetInstanceID() == data.setting.GetInstanceID())
                    {
                        ReApplyEffect(targeted, st);
                        OnApplyEffect(st.key_name, data.creator, targeted, st.level, st.type);
                        return;
                    }
                }
                dat.Add(data);
            }

            Logs.Info("Add An Effect... " + data.key_name);
            //champion.currentEffect.AddEffect(data.Effect);

            // Throw event apply new effect
            IEffectSystem _effectSystem = GetEffectApplier(data);
            data.target = targeted;
            data.effect_system = _effectSystem;
            _effectSystem.ApplyEffect(targeted, data);
            OnApplyEffect(data.key_name, data.creator, data.target, data.level, data.type);
        }

        public void RemoveEffect(ChampionData champion, StatusData data)
        {

            if (data.effect_system == null)
                return;
            var listEffects = ListEffectData;

            if (listEffects.TryGetValue(champion.netId, out var dat))
            {
                // Logs.Info("Remove An Effect...");
                data.effect_system.RemoveEffect(champion, data);
                dat.Remove(data);
                if (isServer && !champion.IsDeath && removeEffectActions.TryGetValue(champion.netId, out var action))
                    action?.Invoke(data);
                //_effectSystem.RemoveEffect(champion, data);
            }
        }

        internal void Update()
        {
            if (isServer)
            {
                UpdateTimeForStatus();
            }
        }

        private void UpdateTimeForStatus()
        {
            float delta_time = Time.deltaTime;
            float unsclaed_delta_time = Time.unscaledDeltaTime;
            foreach (KeyValuePair<uint, List<StatusData>> entry in ListEffectData)
            {
                for (int i = entry.Value.Count - 1; i >= 0; i--)
                {
                    var status = entry.Value[i];

                    status.remain_duration -= delta_time;
                    status.remain_duration_unscaled -= unsclaed_delta_time;
                    float remain = status.unscaled ? status.remain_duration : status.remain_duration_unscaled;
                    if (remain <= 0)
                        RemoveEffect(status.target, status);
                }
            }
        }

        internal IEffectSystem GetEffectApplier(StatusData data)
        {
            if (data.effect_system == null)
            {
                pool.CreatPool(data.setting.StatusApplier);
                if (!pool.Use(data.setting.StatusApplier, out var obj))
                {
                    Logs.Error("Cannot create object applier");
                    return null;
                }
                IEffectSystem effectSystem = obj.GetComponent<IEffectSystem>();
                Debug.Log($"Spawn object {obj.name}");
                NetworkServer.Spawn(obj);

                return effectSystem;
            }

            return data.effect_system;
        }

        public void ClearEffect()
        {
            foreach (var championNetID in ListEffectData.Keys)
            {
                ClearEffectSystemDataOnChampion(championNetID);
            }
            applyEffectActions.Clear();
            removeEffectActions.Clear();
            ListEffectData.Clear();
        }

        public void ClearEffectSystemDataOnChampion(uint championNetID)
        {
            if (!ListEffectData.ContainsKey(championNetID))
            {
                Debug.Log("Not contain champion in effect list");
                return;
            }
            foreach (var effect in ListEffectData[championNetID].ToArray())
            {
                RemoveEffect(effect.target, effect);
            }

            ListEffectData[championNetID].Clear();
        }

        public void RemoveEffectOnChampion(uint championNetID, StatusData.EffectType type)
        {
            if (!ListEffectData.ContainsKey(championNetID))
            {
                Debug.Log("Not contain champion in effect list");
                return;
            }
            foreach (var effect in ListEffectData[championNetID].ToArray())
            {
                if (effect.type == type)
                    effect.remain_duration = effect.remain_duration_unscaled = 0;
            }

        }

        public void DealDynamicDamageOnChampion(ChampionData attacker, ChampionData attacked, float damage, DamageSources damageSources, DamageTypes damageTypes)
        {
            pool.CreatPool(dealDamageEffectPrefab);
            if (!pool.Use(dealDamageEffectPrefab, 1f, out var dealDamageObj))
            {
                Logs.Error("Cannot create object applier");
            }
            else
            {
                NetworkServer.Spawn(dealDamageObj);
                dealDamageEffect = dealDamageObj.GetComponent<BaseDealDamageEffect>();
                dealDamageEffect.DealDamageToEnemy(attacker, attacked, damage, damageSources, damageTypes);

            }
        }

        public void HealDynamicOnChampion(ChampionData champion, float hpHeal)
        {
            pool.CreatPool(healEffectPrefab);
            if (!pool.Use(healEffectPrefab, 1, out var healObject))
            {
                Logs.Error("Cannot create object applier");
            }
            else
            {
                NetworkServer.Spawn(healObject);
                healEffect = healObject.GetComponent<HealEffect>();
            }
            healEffect.HealChampion(champion, hpHeal);
        }

        [ClientRpc]
        public void OnChampionAlive(ChampionData championData)
        {
            removeEffectActions[championData.netId] = delegate (StatusData data) { };
            applyEffectActions[championData.netId] = delegate (StatusData data) { };
            championData.handles.OnDeads.Add(new ClearVFXOnChampionDead(championData));
            championData.rigid.isKinematic = false;
        }

        [ClientRpc]
        public void OnApplyEffect(string effectName, ChampionData creator, ChampionData target, int level, StatusData.EffectType type)
        {
            StatusData data = new StatusData(effectName, creator, new Vector3());
            data.target = target;
            data.level = level;
            data.type = type;
            if (applyEffectActions.TryGetValue(data.target.netId, out var action))
                action?.Invoke(data);
        }
    }

    public class ClearVFXOnChampionDead : IOnDead
    {
        private ChampionData _championData;

        public ClearVFXOnChampionDead(ChampionData championData)
        {
            this._championData = championData;
        }
        public void OnDead()
        {
            if (!GameStateManager.instance.IsGameOver)
            {
                GeneralEffectSystem.Instance.ClearEffectSystemDataOnChampion(_championData.netId);
            }
        }
    }

    public static class EffectData
    {
        public static string Vulernable = "Vulnerable";
        public static string Poisoned = "Poisoned";
        public static string Engulfing = "Engulfing";
        public static string Frenzied = "Frenzy";
        public static string Blessed = "Bless";
        public static string Chill = "Chilled";
    }
}

