using System;
using UnityEngine;
namespace ROI
{
	[System.Serializable]
	public readonly struct CardManaData : IEquatable<CardManaData>
	{
		public readonly uint ChampionNetId;
		public readonly string CardKey;
		public readonly float BonusCost;
		public readonly float OriginCost;
		public readonly int MinimumManaCost;
		public float CurrentCost =>  Mathf.Max( MinimumManaCost,OriginCost + BonusCost) ;

		public CardManaData(uint championNetId, string cardKeyData, float originCost, float bonusCost, int minimumCost)
		{
			ChampionNetId = championNetId;
			CardKey = cardKeyData;
			BonusCost = bonusCost;
			MinimumManaCost = minimumCost;
			OriginCost = originCost;
		}

		public CardManaData Clone(float bonusCost)
		{
			return new CardManaData(ChampionNetId, CardKey, OriginCost, bonusCost, MinimumManaCost);
		}
		public bool Equals(CardManaData other)
		{
			return ChampionNetId == other.ChampionNetId && CardKey == other.CardKey;// && BonusCost.Equals(other.BonusCost) && OriginCost.Equals(other.OriginCost) && MinimumManaCost == other.MinimumManaCost;
		}
		public override bool Equals(object obj)
		{
			return obj is CardManaData other && Equals(other);
		}
		public override int GetHashCode()
		{
			return HashCode.Combine(ChampionNetId, CardKey, BonusCost, OriginCost, MinimumManaCost);
		}
	}
}