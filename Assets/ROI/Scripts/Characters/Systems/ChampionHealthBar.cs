using System.Collections.Generic;
using ROI.UI;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ROI
{
    [RequireComponent(typeof(IObjectPool))]
    public class ChampionHealthBar : MonoBehaviour
    {
        [SerializeField] private Object _healthBarPrefab;
        [SerializeField] private uint _poolSize = 16;
        [SerializeField] private Transform _parent;

        private IObjectPool _objectPool;
        private int _poolID;

        public bool display_heath_bar = true;

        public readonly Dictionary<uint, HealthBar> healthBars = new(16);

        public static ChampionHealthBar instance;

        private void Awake()
        {
            _objectPool = GetComponent<IObjectPool>();
            instance = this;
        }

        private void Start()
        {
            _poolID = _objectPool.CreatPool(_healthBarPrefab, _poolSize);
        }

        public void HideAll()
        {
            display_heath_bar = false;
            foreach (var healthBar in healthBars.Values)
            {
                healthBar.show_heathbar = display_heath_bar;
            }
                
        }

        public void ShowAll()
        {

            display_heath_bar = true;
            foreach (var healthBar in healthBars.Values)
            {
                healthBar.show_heathbar = display_heath_bar;
            }


        }


        public void SetHealthBar(ChampionData championData)
        {
            if (false == _objectPool.Use(_poolID, out var go))
            {
                Logs.Error("Use Health Bar Prefab Fail!");

                return;
            }

            go.transform.SetParent(_parent);
            go.transform.SetSiblingIndex(0);

            var healthBar = go.GetComponent<HealthBar>();
            healthBar.Init(championData);

            if (healthBars.ContainsKey(championData.netId))
                healthBars.Remove(championData.netId);

            healthBars.Add(championData.netId, healthBar);
            healthBar.show_heathbar = display_heath_bar;
            /*if (championData.isIllusion)
                go.SetActive(false);*/
        }

        public void ResetParent(HealthBar go)
        {
            go.transform.SetParent(_parent);
            go.transform.SetSiblingIndex(0);
        }
    }
}