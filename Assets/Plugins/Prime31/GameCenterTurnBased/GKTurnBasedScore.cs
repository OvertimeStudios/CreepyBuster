using UnityEngine;
using System.Collections;


#if UNITY_IOS
namespace Prime31
{
	/// <summary>
	/// This class is used only when calling the GameCenterTurnBasedBinding.endMatchInTurnWithMatchDataAndExtras method
	/// </summary>
	public class GKTurnBasedScore
	{
		#pragma warning disable 0414
		private string playerId;
		private string leaderboardId;
		private System.Int64 score;
		#pragma warning restore 0414


		public GKTurnBasedScore( string playerId, string leaderboardId, System.Int64 score )
		{
			this.playerId = playerId;
			this.leaderboardId = leaderboardId;
			this.score = score;
		}
	}
}
#endif