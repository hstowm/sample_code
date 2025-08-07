using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Mirror;

namespace ROI
{
	/// <summary>
	/// Champion Pause Handle
	/// </summary>
	readonly struct ChampionPauseHandle
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining), Server]
		public void AddPauseHandle(IPauseHandle pauseHandle)
		{
			// if (_listPauseHandles.Count == 0)
			// 	_championController.PauseWithHandle();
			Logs.Info($"Champion {_championData.name} has been paused with handle: {pauseHandle.GetType().FullName}");
			pauseHandle.IsPaused = true;
			_listPauseHandles.Add(pauseHandle);
		}

		[Server, MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool UpdatePauseState()
		{
			var index = _listPauseHandles.Count - 1;
			if (index < 0)
				return false;
			
			for (int i = index; i >= 0; i--)
			{
				if (_listPauseHandles[i].IsPaused)
					continue;

				_listPauseHandles.RemoveAt(i);
			}

			return true;
		}

		[Server, MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Clear()
		{
			_listPauseHandles.Clear();
		}


		public ChampionPauseHandle(ChampionData championData, ChampionController controller)
		{
			_championData = championData;
			_listPauseHandles = new List<IPauseHandle>(64);
			_championController = controller;
		}

		private readonly ChampionData _championData;
		private readonly ChampionController _championController;
		private readonly List<IPauseHandle> _listPauseHandles;
	}
}