using UnityEngine;
using System.Collections;


#if UNITY_IOS
namespace Prime31
{
	/// <summary>
	/// This class is used only when calling the GameCenterTurnBasedBinding.endMatchInTurnWithMatchDataAndExtras method
	/// </summary>
	public class GKTurnBasedAchievement
	{
		#pragma warning disable 0414
		private string playerId;
		private string identifier;
		private float percent;
		#pragma warning restore 0414


		public GKTurnBasedAchievement( string playerId, string identifier, float percent )
		{
			this.playerId = playerId;
			this.identifier = identifier;
			this.percent = percent;
		}
	}
}
#endif