using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mirror;
using ROI.Data;
using ROI.Scripts.UI.Champion;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ROI
{

	[Serializable]
	public class ChampionPick
	{
		public string championUid;
		public string championBaseUid;
		public bool ownedByHost;
	}

	public class ChampionPickPhase : NetworkBehaviour
	{

		[SerializeField] private PanelCardChampion panelCardChampion;
		[SerializeField] public int numCardPerPhase = 3;
		[SerializeField] private int timePerPhase = 20;
		[SerializeField] private int timePhaseDrag = 3;
		[SerializeField] public int numPhase = 5;
		[SyncVar] public int curPhase = 1;
		public bool paused = false;

		[SerializeField] private CampainUIDemo campainUIDemo;

		public readonly SyncList<ChampionPick> lstChampionsOnPick = new SyncList<ChampionPick>();

		private readonly List<int> _listIndexes = new List<int>();

		[SyncVar] public int timeTurn;

		private ChampionManager _championManager;


		private ChampionView _championView;

		private bool _cancelCheck = false;
		private bool timeOut = false;

		private Coroutine _countTime;
		public List<UserChampion> preplaced_champions;

		private void OnDisable()
		{
			_cancelCheck = true;
		}

		private void Awake()
		{
			_championView = FindObjectOfType<ChampionView>(true);
			preplaced_champions = new List<UserChampion>();
		}

		private void Start()
		{
			_championManager = FindObjectOfType<ChampionManager>();
			var btnRandom = panelCardChampion.btnRandom;
			if (btnRandom)
			{
				btnRandom.interactable = true;
				btnRandom.gameObject.SetActive(false);
				btnRandom.onClick.RemoveAllListeners();
				btnRandom.onClick.AddListener(() =>
				{
					var lstCardChampion = panelCardChampion._listCard;
					var count = lstCardChampion.Count;
					var card = lstCardChampion[Random.Range(0, count)];
					card.PickRandom();
					SoundManager.Instance?.PlaySoundSummonChampion(GetIndexRarity(card.baseDataChampion.Rarity));
					btnRandom.interactable = false;
					LockCard();
				});
			}
		}

		public override void OnStartServer()
		{
			base.OnStartServer();

			lstChampionsOnPick.Clear();
			_listIndexes.Clear();

			initListChampionDone = false;
		}


		[Server]
		private async void CheckEnd()
		{
			while (curPhase <= numPhase && !_cancelCheck)
			{
				await Task.Delay(1000);
				CheckEndTurn();
			}
		}


		public static void Shuffle<T>(IList<T> list)
		{
			int n = list.Count;
			while (n > 1)
			{
				n--;
				var rng = new System.Random();
				var k = rng.Next(n + 1);
				(list[k], list[n]) = (list[n], list[k]);
			}
		}

		// public bool IsPicked(string championUid) // , SyncList<ChampionPick> lstChampionPick)
		// {
		// 	for (int i = 0; i < lstChampionsOnPick.Count; i++)
		// 	{
		// 		if (lstChampionsOnPick[i].championUid.Equals(championUid))
		// 			return lstChampionsOnPick[i].isPicked;
		// 	}
		//
		// 	return false;
		// }

		[SyncVar]
		public bool initListChampionDone;

		public async Task InitListChampionsOnPick()
		{
			initListChampionDone = false;

			var lists = new List<ChampionPick>();
			var lstChampionOpponent = PlayerNetwork.Instance.listOpponentChampions;
			foreach (var championTeamStat in lstChampionOpponent.Where(championTeamStat => championTeamStat.inBattle))
			{
				var championPick = new ChampionPick();
				championPick.championUid = championTeamStat.userChampionUID;
				championPick.championBaseUid = championTeamStat.championBaseUID;
				championPick.ownedByHost = false;

				lstChampionsOnPick.Add(championPick);
				_listIndexes.Add(_listIndexes.Count);

				await Task.Delay(100);
			}

			await Task.Delay(100);

			var lstChampionHost = UserData.listChampions;
			foreach (var champion in lstChampionHost)
			{
				var championPick = new ChampionPick();
				championPick.championUid = champion.UserChampionUID;
				championPick.championBaseUid = champion.championUID;
				championPick.ownedByHost = true;

				lstChampionsOnPick.Add(championPick);
				_listIndexes.Add(_listIndexes.Count);
				await Task.Delay(100);
			}

			await Task.Delay(100);

			Logs.Info($"_listIndexes:{_listIndexes.Count},lstChampionsOnPick: {lstChampionsOnPick.Count} ");
			initListChampionDone = true;
		}

		//
		// [Server]
		// public void RandomListChampionPick()
		// {
		// 	
		// }
		[Server]
		public void PreplacedChampion(UserChampion champion)
		{
            var card = panelCardChampion._listCard.Find(card => card._userChampion.championUID == champion.championUID);
            if(card != null )
			{
                card.PickRandom();
                SoundManager.Instance.PlaySoundSummonChampion(GetIndexRarity(card.baseDataChampion.Rarity));
                panelCardChampion.btnRandom.interactable = false;
                LockCard();
			}
			else
			{
                var championBaseData = GameData.chamBaseDB.listChampionBasesInTool[champion.championUID];
				var championCardHandleSpawn = FindObjectOfType<ChampionCardHandleSpawn>(true);
                championCardHandleSpawn.reset();
                _championManager.CreateChampion(championBaseData.KeyName, champion.UserChampionUID, Vector3.zero, true);
                SoundManager.Instance.PlaySoundSummonChampion(GetIndexRarity(championBaseData.Rarity));
                panelCardChampion.btnRandom.interactable = false;
                LockCard();
            }


        }

		

		[Server]
		public void StartPick()
		{

			var popup = PopupManager.Instance.ShowPopUp<PopupLoading>( PopUpName.PopUpLoading );
			popup.LoadingDone();
			
			//check campaign
			if (campainUIDemo)
			{
				numPhase = GetNumberChampion() < 5 ? GetNumberChampion() : 5;
			}

			curPhase = 1;
			// ClearPick();

			_cancelCheck = false;
			panelCardChampion.txtTittle.text = "Drag one champion on the arena to choose it.";
			panelCardChampion.txtTittle.AddKeyLocalizeToLocalizeEvent("localize_hud_putChampion_text");

			Shuffle(_listIndexes);

			StartPickPhase(_listIndexes);

			// Logs.Info($"{lstChampionsOnPick.Count}");

			if (_countTime != null)
				StopCoroutine(_countTime);

			_championView?.gameObject.SetActive(true);
			_countTime = StartCoroutine(CountTime());

			CheckEnd();

			if(preplaced_champions.Count > 0)
			{
                foreach(var champion in preplaced_champions)
				{
                    PreplacedChampion(champion);
				}
            }
		}

		// private void ClearPick()
		// {
		// 	for (int i = 0; i < lstChampionsOnPick.Count; i++)
		// 	{
		// 		lstChampionsOnPick[i].isPicked = false;
		// 	}
		// }


		int GetNumberChampion()
		{
			int count = 0;
			var lstChampionHost = Data.UserData.listChampions;
			foreach (var champion in lstChampionHost)
			{
				count++;
			}

			return count;
		}


		[Server]
		public async void CheckEndTurn()
		{

			// Logs.Info("CheckEndTurn 0");
			var countHost = _championManager.listChampionsInHost.Count;
			var countClient = _championManager.listChampionsOnClient.Count;

			if (campainUIDemo)
				countClient = countHost;

			if (countHost == curPhase && countClient == curPhase)
			{
				// Logs.Info("CheckEndTurn 1");

				timeTurn = timePerPhase + 1;
				timeOut = true;

				SetPicked(_championManager.listChampionsInHost[countHost - 1].userChampionUID);
				if (!campainUIDemo)
					SetPicked(_championManager.listChampionsOnClient[countClient - 1].userChampionUID);

				// var championPick = new ChampionPick();
				// championPick.championUid = _championManager.listChampionsInHost[countHost - 1].userChampionUID;
				// championPick.championBaseUid = lstChampionsOnPick.Find(champion => champion.championUid == championPick.championUid && champion.ownedByHost).championBaseUid;
				//
				// lstChampionsOnPick.Add(championPick);
				// if (!campainUIDemo)
				// {
				//
				// 	var championPickClient = new ChampionPick();
				// 	championPickClient.championUid = _championManager.listChampionsOnClient[countClient - 1].userChampionUID;
				// 	championPickClient.championBaseUid = lstChampionsOnPick.Find(
				// 		champion => champion.championUid == championPickClient.championUid && champion.ownedByHost == false).championBaseUid;
				//
				// 	lstChampionsOnPick.Add(championPickClient);
				//
				// }

				curPhase++;
				if (curPhase <= numPhase)
				{
					// ClearPick();

					await Task.Delay(100);

					Shuffle(_listIndexes);

					StartPickPhase(_listIndexes);

					if (_countTime != null)
						StopCoroutine(_countTime);

					_countTime = StartCoroutine(CountTime());
				}
				else
				{
					// Logs.Info("CheckEndTurn 5");
					timeOut = true;
					StartDrag();
				}
			}
			else
			{
				if (timeTurn < timePerPhase)
				{
					// Logs.Info("CheckEndTurn 4");
					return;
				}
				if (timeOut)
				{
					// Logs.Info("CheckEndTurn 5");
					return;
				}
				Logs.Info("Pick Random");
				timeOut = true;
				PickRandom();
			}
		}

		public void SetPicked(string championUId)
		{
			for (int i = _listIndexes.Count - 1; i >= 0; i--)
			{
				var champion = lstChampionsOnPick[_listIndexes[i]];

				if (champion.championUid.Equals(championUId))
				{
					_listIndexes.RemoveAt(i);
					return;
				}
			}
		}


		[Server]
		private IEnumerator CountTime()
		{
			var timePhase = timePerPhase;
			if (curPhase == numPhase + 1)
			{
				timePhase = timePhaseDrag;

				RpcCleanData();
			}

			timeTurn = 0;
			timeOut = false;
			while (timeTurn <= timePhase && !timeOut)
			{
				
				if(!paused)
				timeTurn++;
                var popup = PopupManager.Instance.ShowPopUp<PopupLoading>(PopUpName.PopUpLoading);
                popup.LoadingDone();
                yield return new WaitForSeconds(1);
			}

			if (curPhase == numPhase + 1)
			{
				StartBattle();
				_cancelCheck = true;
				timeOut = true;
				this.gameObject.SetActive(false);

			}           
        }

		[ClientRpc]
		private void RpcCleanData()
		{
			Logs.Info("clear up and close Champion View");
			_championView.CleanChampionBase();
			_championView?.gameObject.SetActive(false);

		}


		[ClientRpc]
		private async void StartPickPhase(List<int> randomIndexes)
		{
			panelCardChampion.btnRandom.gameObject.SetActive(true);

			Logs.Info($"randomIndexes: {randomIndexes.Count}");

			var lstChampions = new List<string>();

			while (lstChampionsOnPick.Count < 1)
			{
				await Task.Delay(100);

				Logs.Info("Wait lstChampionsOnPick On Pick Sync ");

				if (!this)
					return;
			}

			for (int i = 0; i < randomIndexes.Count; i++)
			{
				var champion = lstChampionsOnPick[randomIndexes[i]];

				if (champion.ownedByHost != isServer)
					continue;
				lstChampions.Add(champion.championUid);
			}


			panelCardChampion.txtPhase.text = curPhase + "/" + (numPhase + 1);
			numCardPerPhase = lstChampions.Count > 3 ? 3 : lstChampions.Count;
			if (campainUIDemo)
			{
				if (!GameSettings.IsCampaignTutorialComplete)
					numCardPerPhase = 1;
			}
			panelCardChampion.RandomChampionIgnore(numCardPerPhase, lstChampions, isServer); //, lstChampion);
			panelCardChampion.btnRandom.interactable = true;

			while (_cancelCheck == false && timeTurn <= timePerPhase && !timeOut)
			{
				var time = timePerPhase - timeTurn;
				var sec = time < 10 ? "0" + time : time.ToString();
				panelCardChampion.txtTime.text = "00:" + sec;
				await Task.Delay(1000);
				CountPick();
			}

			if (panelCardChampion && panelCardChampion.btnRandom)
				panelCardChampion.btnRandom.interactable = false;
			LockCard();
		}


		[Server]
		public void StartBattle()
		{
			if (!campainUIDemo)
			{
				PlayerNetwork.Instance.RpcStartMatch();
				// PlayerNetwork.Instance.RpcUpdateStatus(string.Empty);
			}
			else
			{
				campainUIDemo.OnPlay();
			}
		}

		[Server]
		public void StartDrag()
		{

			if (!GameSettings.IsCampaignTutorialComplete)
			{
				Tutorial.Instance.StagePickEnd();
			}

			StartDragPhase();

			if (_countTime != null)
				StopCoroutine(_countTime);

			_countTime = StartCoroutine(CountTime());

			//CheckEnd();
		}

		[ClientRpc]
		private async void StartDragPhase()
		{
			panelCardChampion.txtPhase.text = curPhase + "/" + (numPhase + 1);
			panelCardChampion.txtTittle.text = "Drag and drop champion to change your formation.";
			panelCardChampion.txtTittle.AddKeyLocalizeToLocalizeEvent("localize_hud_changeChampionPosition_text");
			_cancelCheck = true;
			panelCardChampion.btnRandom.gameObject.SetActive(false);
			foreach (var card in panelCardChampion._listCard)
			{
				card.gameObject.SetActive(false);
			}
			while (timeTurn <= timePhaseDrag && !timeOut)
			{
				var time = timePhaseDrag - timeTurn;
				var sec = time < 10 ? "0" + time : time.ToString();
				panelCardChampion.txtTime.text = "00:" + sec;
				await Task.Delay(1000);

			}
			if (!isServer) this.gameObject.SetActive(false);

		}


		public void CountPick()
		{
			var lstAlly = isServer ? _championManager.listChampionsInHost : _championManager.listChampionsOnClient;
			var lstEnemy = isServer ? _championManager.listChampionsOnClient : _championManager.listChampionsInHost;

			//var lstAllyPick = isServer ? lstChampionHostPicked : lstChampionClientPicked;
			//var lstEnemyPick = isServer ? lstChampionClientPicked : lstChampionHostPicked;


			panelCardChampion.CountChampion(lstAlly, lstChampionsOnPick, isServer, false);
			if (GameStateManager.instance.isPvp || GameStateManager.instance.isPvpBot)
			{
				panelCardChampion.CountChampion(lstEnemy, lstChampionsOnPick, !isServer, true);
			}
		}


		public void LockCard()
		{
			var lstCardChampion = panelCardChampion._listCard;
			foreach (var card in lstCardChampion)
			{
				if (card)
					card.Lock();
			}

		}


		[ClientRpc]
		public void PickRandom()
		{
			panelCardChampion.btnRandom.interactable = false;
			var lstChampionPicked = isServer ? _championManager.listChampionsInHost : _championManager.listChampionsOnClient;
			LockCard();
			if (lstChampionPicked.Count < curPhase)
			{
				var lstCardChampion = panelCardChampion._listCard;
				var count = lstCardChampion.Count;
				var card = lstCardChampion[Random.Range(0, count)];
				card.PickRandom();
				SoundManager.Instance?.PlaySoundSummonChampion(GetIndexRarity(card.baseDataChampion.Rarity));
			}
		}


		int GetIndexRarity(ROI.DataEntity.ChampionRarity rarity)
		{
			int index = 0;
			switch (rarity.name.ToLower())
			{
				case "common":
					index = 0;
					break;
				case "rare":
					index = 1;
					break;
				case "epic":
					index = 2;
					break;
				case "legendary":
					index = 3;
					break;
			}

			return index;
		}

	}
}