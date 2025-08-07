using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mirror;
using ROI.Data;
using ROI.DataEntity;
using ROI;
using ROI.Scripts.Controller;
using UnityEngine;

namespace ROI
{
    [RequireComponent(typeof(ObjectPool))]
    [RequireComponent(typeof(ChampionDamageText))]
    [RequireComponent(typeof(ChampionHealthBar))]
    [RequireComponent(typeof(BulletSystem))]
    [RequireComponent(typeof(IChampionFactory))]
    [RequireComponent(typeof(ChampionVfxSystem))]
    [RequireComponent(typeof(IAbilityCardSystem))]
    [RequireComponent(typeof(ChampionStatSystem))]
    public class ChampionManager : NetworkBehaviour
    {
        [SyncVar] public int maxChampionInGame = 5;

        public readonly SyncList<ChampionData> listChampionsInHost = new SyncList<ChampionData>();
        public readonly SyncList<ChampionData> listChampionsOnClient = new SyncList<ChampionData>();

        public readonly SyncList<ChampionData> listIllusionNonTarget = new SyncList<ChampionData>();

        public readonly SyncDictionary<uint, ChampionData> listChampions = new SyncDictionary<uint, ChampionData>();

        private IObjectPool _objectPool;
        private ChampionDamageText _championDamageText;
        private ChampionHealthBar _championHealthBar;
        private ChampionVfxSystem _championVfxSystem;
        private IAbilityCardSystem _abilityCardSystem;
        private ChampionStatSystem _championStatSystem;


        private BulletSystem _bulletSystem;

        public readonly List<ChampionAgent> agents = new List<ChampionAgent>(16);

        public readonly SyncList<Coordinate> listPlayerCells = new SyncList<Coordinate>();
        public readonly SyncList<Coordinate> listEnemyCells = new SyncList<Coordinate>();

        private IChampionFactory _championFactory;

        private CardPoolManager _cardPoolManager;

        private MapSystem _mapSystem;


        //public Vector3 camPos;
        //public Vector3 camRotate;

        private CellHighlightController _cellHighlightController;
        //private ChampionPickPhase _championPickPhase;
        private ChampionSpecialStatManager _championSpecialStat;
        private void Awake()
        {
            _objectPool = GetComponent<IObjectPool>();
            _championDamageText = GetComponent<ChampionDamageText>();
            _championHealthBar = GetComponent<ChampionHealthBar>();
            _bulletSystem = GetComponent<BulletSystem>();
            _championFactory = GetComponent<IChampionFactory>();

            _championVfxSystem = GetComponent<ChampionVfxSystem>();
            _abilityCardSystem = GetComponent<IAbilityCardSystem>();
            _championStatSystem = GetComponent<ChampionStatSystem>();
            _cellHighlightController = GetComponent<CellHighlightController>();

            // championNetworkManager = FindObjectOfType<ChampionNetworkManager>(true);
            _cardPoolManager = FindObjectOfType<CardPoolManager>(true);
            //_ultimateEnergySystem = GetComponent<UltimateEnergySystem>();
            _mapSystem = FindObjectOfType<MapSystem>(true);
            // _championPickPhase = FindObjectOfType<ChampionPickPhase>(true);
            _championSpecialStat = GetComponent<ChampionSpecialStatManager>();
        }

        public MapSystem Map
        {
            get
            {
                if (null == _mapSystem)
                    _mapSystem = FindObjectOfType<MapSystem>(true);
                return _mapSystem;
            }
        }

        private void SetupCameraRotate()
        {
            // var camera = Camera.main;
            //
            // var camSetting = FindObjectOfType<CameraController>();
            // camPos = camSetting.camPos;
            // camRotate = camSetting.camRotate;
            //
            //
            // if (!isServer)
            // {
            // 	camera.transform.localRotation = Quaternion.Euler(camRotate.x, -180, camRotate.z);
            // 	camera.transform.localPosition = new Vector3(camPos.x, camPos.y, 16);
            // } else
            // {
            // 	camera.transform.localRotation = Quaternion.Euler(camRotate);
            // 	camera.transform.localPosition = camPos;
            // }

            Map.RotatePivot(isServer);
        }


        public override void OnStartServer()
        {
            base.OnStartServer();

            listChampions.Clear();
            listChampionsOnClient.Clear();
            listChampionsInHost.Clear();
            listIllusionNonTarget.Clear();
        }


        [Command(requiresAuthority = false)]
        public void CmdCreateOpponentChampion(string baseName, string userChampionUID, Vector3 _pointChampionSelect, bool _isSpawnRandom, NetworkConnectionToClient sender = null)
        {
            if (listChampionsOnClient.Count >= maxChampionInGame)
            {
                Logs.Error($"cant Create New Champion '{baseName}' when reach to max champion.");
                return;
            }

            foreach (var _champion in listChampions.Values)
            {
                if (_champion.userChampionUID == userChampionUID)
                {
                    return;
                }
            }

            Vector3 pointChampionSelect = Vector3.zero;

            if (_isSpawnRandom == false)
            {
                if (SetPositionOpponentChampion(_pointChampionSelect, out Vector3 posOut))
                {
                    pointChampionSelect = posOut;
                }
                else
                {
                    _isSpawnRandom = true;

                }
            }

            var senderID = sender == null ? 0 : sender.identity.netId;

            CreateOpponentChampion(baseName, userChampionUID, senderID, pointChampionSelect, _isSpawnRandom);
        }

        [Server]
        public async void CreateOpponentChampion(string baseName, string userChampionUID, uint creatorNetID, Vector3 pointChampionSelect, bool isSpawnRandom, bool isBotOrClient = false)
        {
            if (isBotOrClient) isSpawnRandom = true;
            if (listChampionsOnClient.Count >= maxChampionInGame)
            {
                Logs.Error($"cant Create New Champion '{baseName}' when reach to max champion.");
                return;
            }

            //var senderID = sender == null ? 0 : sender.identity.netId;
            var index = PlayerNetwork.Instance.listOpponentChampions.FindIndex(c => c.championBaseUID == baseName);

            if (index < 0)
            {
                Logs.Error($"Cant Create Opponent Champion: {baseName}");
                return;
            }

            if (GameData.chamBaseDB.listChampionBasesInTool.TryGetValue(baseName, out var champion) == false)
            {
                Logs.Error($"Cant Find Champion Base: {baseName}");
                return;
            }

            var championData = await _championFactory.Create(champion, false, creatorNetID, false, userChampionUID);

            var chamTeamStat = PlayerNetwork.Instance.listOpponentChampions[index];
            // int stat
            championData.InitBaseStat(chamTeamStat.chamStat);
            // spawn champion on network
            SpawnChampion(championData, false, false, pointChampionSelect, isSpawnRandom);
        }


        public void CreateChampion(string championName, string userChampionUID, Vector3 _pointChampionSelect, bool _isSpawnRandom)
        {

            // is client call to command server to spawn
            if (isServer == false)
            {
                CmdCreateOpponentChampion(championName, userChampionUID, _pointChampionSelect, _isSpawnRandom);
                return;
            }

            foreach (var _champion in listChampions.Values)
            {
                if (_champion.userChampionUID == userChampionUID)
                {
                    return;
                }
            }

            var creatorID = NetworkClient.connection.identity.netId;
            CreateChampionFromName(championName, creatorID, userChampionUID, _pointChampionSelect, _isSpawnRandom);
        }

        [Server]
        public async void CreateChampionFromBaseData(
            ChampionBaseData baseData,
            string userChampionUID,
            uint senderNetId,
            bool ownedByHost,
            Vector3 pointChampionSelect,
            bool isSpawnRandom = true)
        {
            var championData = await _championFactory.Create(baseData, ownedByHost, senderNetId, false, userChampionUID);

            SpawnChampion(championData, false, false, pointChampionSelect, isSpawnRandom);

        }

        [Server]
        public async Task<ChampionData> CreateIllusion(ChampionData owner, ChampionBaseData illusionBaseData, Vector3 spawnPosition)
        {
            var illusion = await _championFactory.Create(illusionBaseData, owner.ownedByHost, owner.creatorNetId, true, owner.userChampionUID);

            illusion.attackData = owner.attackData;
            illusion.userChampionUID = owner.userChampionUID;
            illusion.enemies = owner.enemies;
            illusion.allies = owner.allies;
            illusion.specialStatData = owner.specialStatData;
            illusion.moveAnim = owner.moveAnim;
            illusion.attackers = owner.attackers;
            illusion.attackAnim = owner.attackAnim;
            illusion.healthData = owner.healthData;
            illusion.moveData = owner.moveData;

            illusion.transform.position = spawnPosition;

            NetworkServer.Spawn(illusion.gameObject);

            // on champion alive
            GeneralEffectSystem.Instance.OnChampionAlive(illusion);

            // apply trait
            // ApplyTrait(illusion);

            var listCards = owner.GetListAbilityCards();

            _abilityCardSystem.InitAbilityCards(illusion, listCards);

            AddChampionOnSpawned(illusion);

            RpcOnSpawnChampion(illusion, false);

            return illusion;
        }


        [Server]
        public async Task CreateNpcFromNpcData(NpcData npcData, string userChampionUID, uint senderNetId, bool isManualPosition = false)
        {
            //npcData.npcAppearance.
            Logs.Info($"Create NPC: {npcData.npcAppearance.npcName}");

            if (npcData.npcAppearance.modelRef == null)
            {
                Logs.Error("Model Ref Is Null");
                return;
            }

            var npcModel = await npcData.npcAppearance.modelRef.LoadAsync<GameObject>();

            Logs.Info($"Create NPC: {npcModel.name}");

            var championData = _championFactory.Create(npcModel, false, senderNetId, false, userChampionUID);

            championData.InitBaseStat(npcData.CalculateBaseStat());
            championData.maxUltimateEnergy = npcData.npcAppearance.ultimateEnergy;
            championData.isNPC = true;
            List<string> card_names = new List<string>();
            foreach (var card in npcData.npcAppearance.skills)
            {
                card_names.Add(card.KeyName);
            }

            if (championData.maxUltimateEnergy > 0 && card_names.Count > 0)
            {               
                championData.canUseEnergy = true;             
            }

            var pos = championData.transform.position;
            pos.x = 1000;
            championData.transform.position = pos;

            SpawnChampion(championData, true, isManualPosition);
            
            championData.InitAbilityCards(card_names);
        }

        [Server]
        public ChampionData CreateSummon(GameObject model, ChamStat chamStat, uint senderNetId)
        {
            var championData = _championFactory.Create(model, senderNetId == NetworkClient.connection.identity.netId, senderNetId, false);
            championData.InitBaseStat(chamStat);
            SpawnChampion(championData, true);
            championData.isIllusion = true;
            return championData;
        }

        [Server]
        private void SpawnChampion(ChampionData championData, bool isNpc, bool isManualPosition = false, Vector3 pointChampionSelect = new Vector3(), bool isSpawnRandom = true)
        {
            if (isNpc)
                isSpawnRandom = true;

            // init extension system
            InitBeforeSpawnChampion(championData, isManualPosition, pointChampionSelect, isSpawnRandom);

            NetworkServer.Spawn(championData.gameObject);
            championData.InitUltimateEnergy();
            // init ultimate energy for champion
            if (isNpc == false)
            {
                // Init all Abilities cards on a champion
                championData.InitAbilityCards();
                _championSpecialStat.InitSpecialStat(championData);
                _championStatSystem.InitHiddenStat(championData);
            }

            AddChampionOnSpawned(championData);

            RpcOnSpawnChampion(championData, isNpc);
        }

        [Server]
        private void AddChampionOnSpawned(ChampionData championData)
        {
            if (listChampions.TryGetValue(championData.netId, out _) == false)
            {
                listChampions.Add(championData.netId, championData);
            }
            else
                listChampions[championData.netId] = championData;

            agents.Add(championData.agent);
            championData.handles.OnDeads.Add(new ChampionOnDeadHandle(championData.netId, championData, this));

            // non target
            if (championData.isIllusion)
            {
                listIllusionNonTarget.Add(championData);
                return;
            }

            if (championData.ownedByHost)
                listChampionsInHost.Add(championData);
            else
                listChampionsOnClient.Add(championData);

        }

        [Server]
        public void CreateChampionFromName(string championName, uint senderNetId, string userChampionUID, Vector3 pointChampionSelect, bool isSpawnRandom = true)
        {
            if (listChampionsInHost.Count >= maxChampionInGame)
            {
                Logs.Error($"cant Create New Champion '{championName}' when reach to max champion.");
                return;
            }

            if (GameData.chamBaseDB.listChampionBasesInTool.TryGetValue(championName, out var championBase))
                CreateChampionFromBaseData(championBase, userChampionUID, senderNetId, true, pointChampionSelect, isSpawnRandom);
        }

        [ClientRpc]
        public void RpcOnSpawnChampion(ChampionData championData, bool isNpc)
        {
            championData.tag = championData.ownedByHost ? GameTags.Player : GameTags.Enemy;
            // illusion
            if (championData.isIllusion)
            {
                InitOnSpawnChampion(championData);
                return;
            }

            if (championData.IsMine && isNpc == false)
            {
                CreateCardSkill(championData);

                var cardUltimateController = CardUltimateController.Instance;
                // var baseCharacter = championData.GetComponent<BaseCharacterCardController>();
                //
                // if (baseCharacter)
                // {
                // 	cardUltimateController.GenerateCardUltimate(baseCharacter);
                // } else
                // {
                cardUltimateController.GenerateCardUltimate(championData);
                // }
            }

            InitOnSpawnChampion(championData);
            // Add champion to tracking list
            GameObserver.GetInstance().AddChampionToObserver(championData);
        }

        public override void OnStartClient()
        {
            base.OnStartClient();

            SetupCameraRotate();

            FindObjectOfType<TopBarMenu>()?.Show(false);
            FindObjectOfType<BotBarMenu>()?.Show(false);
            // if (GameStateManager.instance.isPvp || GameStateManager.instance.isPvpBot)
            // {
            // 	EnergyManager.Instance?.UseEnergy();
            // }
            //
            // var champions = GameData.chamBaseDB.listChampionBasesInTool.Values;
            // foreach (var champion in champions)
            // {
            // 	//_objectPool.CreatPool(champion.Model);
            // 	//NetworkClient.RegisterPrefab(champion.Model);
            // 	
            // 	//NetworkClient.RegisterPrefab();
            // }

            Map.GeneratePlacerCells();

            agents.Clear();

            _cellHighlightController.Init(listPlayerCells, listEnemyCells);

            RegisterChampionPrefabs();
        }

        public async void RegisterChampionPrefab(string championUid)
        {
            var modelRef = GameData.modelAssetRefDB.championModelsRef;
            if (false == modelRef.TryGetValue(championUid, out var champ))
            {

                Logs.Error($"Cant Find User Champion In GameData: {championUid}");
                return;
            }

            var model = await champ.championVariantRef.LoadAsync<GameObject>();

            if (model == null)
            {
                Logs.Error($"Load Model ref key: {championUid} fail!!!");
                return;
            }

            _objectPool.CreatPool(model);
            NetworkClient.RegisterPrefab(model);
        }

        public void RegisterPrefab(GameObject prefab)
        {
            _objectPool.CreatPool(prefab);
            NetworkClient.RegisterPrefab(prefab);
        }
        private void RegisterChampionPrefabs()
        {
            Logs.Info("Register Champion In Team...");
            var userChampions = UserData.GetTeamBattle();
            foreach (var champion in userChampions)
            {
                RegisterChampionPrefab(champion.championUID);
            }
        }

        public void RegisterOpponentPrefab()
        {
            Logs.Info("Register Champion In Opponent...");

            foreach (var opponent in PlayerNetwork.Instance.listOpponentChampions)
                RegisterChampionPrefab(opponent.championBaseUID);
        }

        [ClientRpc]
        public void RpcRegisterHostPrefabOnClient(List<string> championUidOnHost)
        {
            if (isServer)
                return;

            Logs.Info("Register Champion Host On Client...");
            foreach (var champion in championUidOnHost)
            {
                RegisterChampionPrefab(champion);
            }
        }

        // public void PreloadChampionModels()
        // {
        // 	UserTeam.GetDefaultTeam().GetUserTeamInBattle();
        // }


        // [Server]
        // private Coordinate GetRandomCell(bool isMine)
        // {
        // 	var list = isMine ? listPlayerCells : listEnemyCells;
        //
        // 	if (list.Count == 0)
        // 	{
        // 		return new Coordinate(0, 0);
        // 	}
        //
        // 	var index = Random.Range(0, list.Count);
        // 	var coord = list[index];
        //
        // 	list.RemoveAt(index);
        //
        // 	return coord;
        // }

        public bool CheckCoord(Coordinate coord)
        {
            bool _isMine = isServer;
            var list = _isMine ? listPlayerCells : listEnemyCells;
            if (list.Contains(coord)) return false;
            return true;
        }

        public void CmdCheckAndSetPosChampionDrag(ChampionData _champion, Coordinate coord, Vector3 pos, Vector3 OldPosChampion)
        {
            bool _isMine = isServer;
            var list = _isMine ? listPlayerCells : listEnemyCells;
            var mapField = _isMine ? Map.mapData.playerPlacer : Map.mapData.enemyPlacer;
            //	var coord = mapField.ConvertWorldPositionToOffsetCoord(vtCheckCoord);
            // neu champion da o do roi thi ko gui cmd nua, tinh ca chinh champion dang di chuyen
            if (list.Contains(coord))
            {
                _champion.transform.position = OldPosChampion;
                // Hight light cell at character
                if (mapField.GetHexCellFromWorldPosition(OldPosChampion, out var hxCell))
                {
                    hxCell.GetComponent<CellStateController>().OnCharacterDragIn();
                }

                return;
            }
            _champion.transform.position = pos;

            CmdSendPosChampionDrag(_champion, new Vector2(coord.column, coord.row), pos, OldPosChampion);
        }

        [Command(requiresAuthority = false)]
        public void CmdSendPosChampionDrag(ChampionData _champion, Vector2 coord, Vector3 vtCheckCoord, Vector3 OldPosChampion)
        {
            // server check lai tranh client gui khong dung vi tri
            bool _isMine = _champion.IsMine;
            var list = _isMine ? listPlayerCells : listEnemyCells;
            var mapField = _isMine ? Map.mapData.playerPlacer : Map.mapData.enemyPlacer;
            var coordinate = Coordinate.New((int)coord.x, (int)coord.y); // mapField.ConvertWorldPositionToOffsetCoord(vtCheckCoord);

            if (list.Contains(coordinate))
                return;

            var offset = OldPosChampion;
            offset.y = 0;
            var coordChampion = mapField.ConvertWorldPositionToOffsetCoord(offset);
            list.Remove(coordChampion);
            list.Add(coordinate);

            _champion.transform.position = vtCheckCoord;

            ChampionActiveSyncPosition(_champion);
        }

        [ClientRpc]
        void ChampionActiveSyncPosition(ChampionData _champion)
        {
            _champion.GetComponent<NetworkTransform>().syncPosition = true;

        }

        public void InitOnSpawnChampion(ChampionData championData)
        {
            championData.allies = championData.ownedByHost ? listChampionsInHost : listChampionsOnClient;
            championData.enemies = championData.ownedByHost ? listChampionsOnClient : listChampionsInHost;
            int championLevel = 0;

            if (championData.IsMine)
            {
                var champions = UserData.GetTeamBattle();
                int index = champions.FindIndex(champion => championData.userChampionUID == champion.UserChampionUID);
                if (index >= 0)
                {
                    championLevel = (int)champions[index].level;
                }
            }
            else
            {
                int index = PlayerNetwork.Instance.listOpponentChampions.FindIndex(stat =>
                    stat.userChampionUID == championData.userChampionUID);
                if (index >= 0)
                {
                    championLevel = (int)PlayerNetwork.Instance.listOpponentChampions[index].level;
                }
            }


            Logs.Info($"Champion {championData.name} level: {championLevel}");

            var obj = championData.gameObject;

            // set health bar UI
            _championHealthBar.SetHealthBar(championData);
            bool isChampion = championData.isIllusion || championData.isNPC;

            championData.canUseEnergy = !isChampion || championData.canUseEnergy;
            
            // Inject Extend Systems
            obj.InjectInstance(_championDamageText);
            obj.InjectInstance(championData);
            obj.InjectInstance(_bulletSystem);
            obj.InjectInstance(_championHealthBar);
            obj.InjectInstance(_championVfxSystem);

        }


        /// <summary>
        /// Create all cards
        /// </summary>
        /// <param name="champion"></param>
        private void CreateCardSkill(ChampionData champion)
        {
            //  var cardManager = FindObjectOfType<CardPoolManager>();
            var championSkillData = champion.GetComponent<ChampionSkillCard>();
            if (championSkillData == null)
                return;

            _cardPoolManager.AddCardFromChampion(championSkillData.baseData, champion);
            // champion.GetComponent<BaseCharacterCardController>()?.RegisterPlayerEneryEvent();
        }

        [Server]
        public void InitBeforeSpawnChampion(ChampionData championData, bool isManualPosition, Vector3 pointChampionSelect, bool isSpawnRandom = true)
        {
            try
            {
                GameObject champion = championData.gameObject;
                var isMine = championData.IsMine;

                champion.tag = isMine ? GameTags.Player : GameTags.Enemy;

                var excludeList = isMine ? listPlayerCells.ToList() : listEnemyCells.ToList();

                var pos = Map.GetRandomPlacerPosition(isMine, excludeList);

                if (!isSpawnRandom)
                {
                    pos = pointChampionSelect;
                }

                if (isManualPosition == false)
                {
                    var c = Map.ConvertWorldPositionToPlacerCoordinate(isMine, pos);

                    if (isMine)
                        listPlayerCells.Add(c);
                    else
                        listEnemyCells.Add(c);

                    pos.y = 0;
                    champion.transform.position = pos;
                }

                champion.transform.rotation = Quaternion.Euler(0, championData.ownedByHost ? Map.mapData.playerRotateAngle : -Map.mapData.playerRotateAngle, 0);
            }
            catch (Exception e)
            {
                throw new Exception($"Unexpected exception: {e.Message} at {e.Source}");
            }
            // championData.GetComponent<ChampionAgent>().bodyRadius = mapField.InnerRadius - 0.1f;
            // re-convert attack range to new range on map
            //championData.attackData.range = championData.attackData.range;
            //championData.attackData.baseRange = championData.attackData.baseRange;
        }

        public bool SetPositionOpponentChampion( /*ChampionData championData,*/ Vector3 posChampionSelect, out Vector3 posOut)
        {
            posOut = Vector3.zero;
            //bool isMine = championData.IsMine;
            var mapField = /*isMine ? _mapSystem.mapData.playerPlacer :*/ Map.mapData.enemyPlacer;
            if (false == mapField.GetHexCellFromWorldPosition(posChampionSelect, out var cell))
            {
                return false;
            }
            posOut = cell.transform.position;
            return true;
        }

        public void RemoveChampion(uint championNetId)
        {
            for (int i = 0; i < listChampionsInHost.Count; i++)
            {
                if (listChampionsInHost[i].netId == championNetId)
                {
                    RemoveAgent(listChampionsInHost[i].agent);
                    listChampionsInHost.RemoveAt(i);
                    break;
                }
            }

            for (int i = 0; i < listChampionsOnClient.Count; i++)
            {
                if (listChampionsOnClient[i].netId == championNetId)
                {
                    RemoveAgent(listChampionsOnClient[i].agent);
                    listChampionsOnClient.RemoveAt(i);

                    break;
                }
            }

            listChampions.Remove(championNetId);
        }

        private void RemoveAgent(ChampionAgent agent)
        {
            for (int i = agents.Count - 1; i > -1; i--)
            {
                if (agents[i].GetInstanceID() != agent.GetInstanceID())
                    continue;

                //Logs.Info($"REMOVE AGENT: {i}");
                agents.RemoveAt(i);
            }
        }

        [Server]
        public void StartAliveAllChampions()
        {
            foreach (var champion in listChampions)
            {
                // Logs.Info($"Apply Trait For champion: {champion.Value.name}");
                // champion.Value.ApplyTrait();
                GeneralEffectSystem.Instance.OnChampionAlive(champion.Value);
                champion.Value.controller.StartAlive();

            }

        }

        // [Server]
        // private void ApplyTrait(ChampionData championData)
        // {
        // 	if (string.IsNullOrEmpty(championData.userChampionUID))
        // 		return;
        //
        // 	// server
        // 	if (championData.ownedByHost)
        // 	{
        // 		var userCham = Data.UserData.listChampions.Find((cham) => cham.UserChampionUID == championData.userChampionUID);
        // 		championData.ApplyTraits(userCham.traitUIDs);
        //
        // 		return;
        // 	}
        //
        // 	// opponents
        // 	var index = PlayerNetwork.Instance.listOpponentChampions.FindIndex(c => c.userChampionUID == championData.userChampionUID);
        // 	if (index < 0)
        // 	{
        // 		Logs.Error($"Cant Find Trait of Opponent Champion UID: {championData.userChampionUID}");
        // 		return;
        // 	}
        //
        // 	championData.ApplyTraits(PlayerNetwork.Instance.listOpponentChampions[index].listTraits);
        // }

        public void ResetAllChampions()
        {
            if (isServer)
            {
                ResetChampions(listChampionsInHost);
                ResetChampions(listChampionsOnClient);

                listChampions.Clear();
            }

            agents.Clear();
        }

        public void ResetChampions(SyncList<ChampionData> champions)
        {
            // reset list champions here by let's die
            foreach (var champion in champions)
            {
                // champion.healthData = new HealthData(0, champion.healthData.maxHealth); //.health = 0;
                champion.statModifier.ApplyModify(new StatTypeData(StatTypes.Health, 0));
                //champion.gameObject.Recycle();
                // champion.controller.SetDeath();
            }

            champions.Clear();
        }

        public float GetTeamPowerOnHost()
        {
            var listChampionsInTeam = UserTeam.GetDefaultTeam().championsTeam;
            var listChampionInTeamCreated = new List<UserChampionInTeam>();

            foreach (var champion in listChampionsInHost)
            {
                var index = listChampionsInTeam.FindIndex(c => c.championUserUID == champion.userChampionUID);

                if (index < 0)
                {

                    Logs.Info($"Cant Find Champion: {champion?.name} with champion user uid: {champion?.userChampionUID} in Default team");
                    continue;
                }

                listChampionInTeamCreated.Add(listChampionsInTeam[index]);
            }

            var power = 0f;
            foreach (UserChampionInTeam championInTeam in listChampionInTeamCreated)
            {
                power += championInTeam.GetPower();
            }

            return power;
        }

        public float GetTeamPowerClient()
        {
            var listOpponentChampions = PlayerNetwork.Instance.listOpponentChampions;
            var power = 0f;

            foreach (var champion in listChampionsOnClient)
            {
                var index = listOpponentChampions.FindIndex(c => c.userChampionUID == champion.userChampionUID);

                if (index < 0)
                {

                    // Logs.Info($"Cant Find Champion: {champion.name} with champion user uid: {champion.userChampionUID} in Default team");
                    continue;
                }

                // listChampionInTeamCreated.Add(listOpponentChampions[index]);

                power += listOpponentChampions[index].power;
            }

            return power;
        }

        public void SetPlayerWinLose(bool playerWin)
        {
            if (playerWin)
            {
                listChampionsOnClient.Clear();
            }
            else
            {
                listChampionsInHost.Clear();
            }

            CampainUIDemo.Instance.curWave = 99;
            GameStateManager.instance.StartCheckResultGame();
        }
    }

    class ChampionOnDeadHandle : IOnDead
    {
        public void OnDead()
        {
            _championManager.RemoveChampion(_id);
            _championData.handles.OnDeads.Remove(this);
        }

        public ChampionOnDeadHandle(uint id, ChampionData championData, ChampionManager championManager)
        {
            _id = id;
            _championManager = championManager;
            _championData = championData;
        }

        private readonly uint _id;
        private readonly ChampionManager _championManager;
        private readonly ChampionData _championData;
    }
}