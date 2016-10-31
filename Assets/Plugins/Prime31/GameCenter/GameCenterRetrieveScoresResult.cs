using UnityEngine;
using System.Collections.Generic;


#if UNITY_IOS || UNITY_TVOS || UNITY_STANDALONE_OSX

namespace Prime31
{
	public class GameCenterRetrieveScoresResult
	{
		public List<GameCenterScore> scores;
		public string category;
	}

}
#endif
