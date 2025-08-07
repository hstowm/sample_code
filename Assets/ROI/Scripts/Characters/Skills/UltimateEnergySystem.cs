using System;
using Mirror;
using UnityEngine;

namespace ROI
{
    struct UltimateEnergyData
    {
        public uint maxUltimateEnergy;
        public float manaToUltEnergyRate;
        public float currentUltiEnergy;

        public bool IsMax => currentUltiEnergy >= maxUltimateEnergy && maxUltimateEnergy > 0;
    }
    /// <summary>
    /// Ultimate Energy System
    /// </summary>
    public class UltimateEnergySystem : NetworkBehaviour
    {
        [SerializeField] private uint _maxUltimateEnergy = 20;
        public uint maxUltimateEnergy
        {
            get => _maxUltimateEnergy;
        }
        [SerializeField] private float _manaToUltimateEnergyRate = 1;

        private readonly SyncDictionary<uint, UltimateEnergyData> _ultimateEnergy = new();

        public override void OnStartServer()
        {
            base.OnStartServer();
            
            _ultimateEnergy.Clear();
        }

        void Awake()
        {
            UltimateEnergySystemExts.Init(this);
        }

        [Server]
        public void InitChampion(ChampionData championData)
        {
            var ultimateEnergy = new UltimateEnergyData()
            {
                currentUltiEnergy =  0,
                manaToUltEnergyRate = _manaToUltimateEnergyRate,
                maxUltimateEnergy = _maxUltimateEnergy
            };

            if(championData.maxUltimateEnergy != _maxUltimateEnergy && championData.maxUltimateEnergy != 0)
            {
                ultimateEnergy.maxUltimateEnergy = championData.maxUltimateEnergy;
            }

            _ultimateEnergy[championData.netId] = ultimateEnergy;
        }

        //[Server]
        public float GetUltimateEnergy(ChampionData championData)
        {
            if (_ultimateEnergy.TryGetValue(championData.netId, out var energy) == false)
            {
                Logs.Error($"Cant Get Ultimate Energy: {championData.name}");
                return 0;
            }

            return energy.currentUltiEnergy;
        }

        public uint GetMaxUltimateEnergy(ChampionData championData)
        {
            if (_ultimateEnergy.TryGetValue(championData.netId, out var energy) == false)
            {
                Logs.Error($"Cant Get Ultimate Energy: {championData.name}");
                return 0;
            }

            return energy.maxUltimateEnergy;
        }

        [Server]
        public void AddBonusWhenUseMana(ChampionData championData, int mana) 
        {
            var energy = (int) (mana * _manaToUltimateEnergyRate);
            AddBonus(championData, energy);
        }

        [Server]
        public void AddBonus(ChampionData championData, int ultimateEnergyBonus)
        {
            if (!championData.canUseEnergy)
                return;
            
            AddBonus(championData,(float) ultimateEnergyBonus);
        }

        [Server]
        public void AddBonus(ChampionData championData, float ultimateEnergyBonus)
        {
            if (!championData.canUseEnergy)
                return;
            
            if (_ultimateEnergy.TryGetValue(championData.netId, out var energyData) == false)
            {
                Logs.Error($"Cant Get Ultimate Energy: {championData.name}");
                return;
            }

            energyData.currentUltiEnergy = Math.Clamp(energyData.currentUltiEnergy + ultimateEnergyBonus, 0, _maxUltimateEnergy);
            _ultimateEnergy[championData.netId] = energyData;

            Debug.Log("Energy of " +championData.name + " is " + _ultimateEnergy[championData.netId].currentUltiEnergy);

            if (IsFull(championData))
            {
                if(!championData.isNPC)
                {
                    RpcShowUltimateCard(championData);
                }
                else
                {
                    Debug.Log("NPC begin skill");
                    championData.NpcDoSkill();
                    championData.ResetUltimateEnergy();
                }
            }
        }

        [Server]
        public void ResetEnergy(ChampionData championData)
        {
            if (_ultimateEnergy.TryGetValue(championData.netId, out var energy) == false)
            {
                Logs.Error($"Cant Get Ultimate Energy: {championData.name}");
                return;
            }

            energy.currentUltiEnergy = 0;

            _ultimateEnergy[championData.netId] = energy;
        }

        public bool HaveUltimateMana(ChampionData championData) => championData.isIllusion == false && _ultimateEnergy.ContainsKey(championData.netId);


        public bool IsFull(ChampionData championData) => championData.isIllusion == false && _ultimateEnergy.ContainsKey(championData.netId) && _ultimateEnergy[championData.netId].IsMax;// == _maxUltimateEnergy;

        [ClientRpc]
        private void RpcShowUltimateCard(ChampionData championData)
        {
            championData.ShowUltimateCard(true);
        }
    }

    public static class UltimateEnergySystemExts
    {
        private static UltimateEnergySystem _instance;

        public static void Init(UltimateEnergySystem ultimateEnergySystem) => _instance = ultimateEnergySystem;
        public static void InitUltimateEnergy(this ChampionData championData) => _instance.InitChampion(championData);
        public static bool IsFullUltimateEnergy(this ChampionData championData) => _instance.IsFull(championData);
        public static void ResetUltimateEnergy(this ChampionData championData) => _instance.ResetEnergy(championData);
        public static void AddBonusUltimateEnergy(this ChampionData championData, int bonusEnergy) => _instance.AddBonus(championData, bonusEnergy);
        public static void AddBonusUltimateEnergy(this ChampionData championData, float bonusEnergy) => _instance.AddBonus(championData, bonusEnergy);
        public static bool HasUltimateEnergy(this ChampionData championData) => _instance.HaveUltimateMana(championData);
        public static float GetUltimateEnergy(this ChampionData championData) => _instance.GetUltimateEnergy(championData);
        public static void AddUltimateEnergyWhenUseMana(this ChampionData championData, int mana) => _instance.AddBonusWhenUseMana(championData, mana);
        public static uint GetMaxUltimateEnergy(this ChampionData championData) => _instance.GetMaxUltimateEnergy(championData);
    }
}