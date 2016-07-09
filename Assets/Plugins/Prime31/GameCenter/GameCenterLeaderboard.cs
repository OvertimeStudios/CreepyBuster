using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Prime31;


#if UNITY_IOS || UNITY_TVOS || UNITY_STANDALONE_OSX

namespace Prime31
{
	public class GameCenterLeaderboard
	{
		public string leaderboardId;
		public string title;
	
	
		public static List<GameCenterLeaderboard> fromJSON( string json )
		{
			List<GameCenterLeaderboard> leaderboardList = new List<GameCenterLeaderboard>();
	
			// decode the json
			var dict = json.dictionaryFromJson();
	
			// create DTO's from the Dictionary
			foreach( var keyValuePair in dict )
				leaderboardList.Add( new GameCenterLeaderboard( keyValuePair.Key as string, keyValuePair.Value as string ) );
	
			return leaderboardList;
		}
	
	
		public GameCenterLeaderboard( string leaderboardId, string title )
		{
			this.leaderboardId = leaderboardId;
			this.title = title;
		}
	
	
		public override string ToString()
		{
			 return string.Format( "<Leaderboard> leaderboardId: {0}, title: {1}", leaderboardId, title );
		}
	
	}

}
#endif
