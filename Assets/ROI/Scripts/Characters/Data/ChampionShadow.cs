using System.Runtime.CompilerServices;
using UnityEngine;


namespace ROI
{
	public class ChampionShadow : MonoBehaviour, IOnDead, IOnStartAlive
	{

		private float offset = 0.1f;
		public float shadowScale = 0.8f;

		// Start is called before the first frame update
		public float height_level;

		private Transform _transform;

		private float _timoutUpdate = 1f;

		void Start()
		{
			_transform = transform;
			Vector3 scale = gameObject.transform.localScale;
			scale *= shadowScale;
			_transform.localScale = scale;

			GetComponentInParent<ChampionData>().handles.OnDeads.Add(this);
			GetComponentInParent<ChampionData>().handles.OnStartAlive.Add(this);

			UpdateShadowOffset();
		}

		// Update is called once per frame
		void LateUpdate()
		{
			if (_timoutUpdate > 0)
			{
				_timoutUpdate -= Time.deltaTime;
				return;
			}

			_timoutUpdate -= 1f;

			UpdateShadowOffset();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void UpdateShadowOffset()
		{
			Vector3 p = _transform.position;
			p.y = height_level + offset;
			_transform.position = p;
		}

		public void OnDead()
		{
			this.gameObject.SetActive(false);
		}

		public void OnStartAlive()
		{
			this.gameObject.SetActive(true);
		}
	}
}