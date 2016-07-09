using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Prime31;


#if UNITY_IOS
public enum GKTurnBasedMatchStatus
{
    Unknown   = 0,
    Open      = 1,
    Ended     = 2,
    Matching  = 3
}



namespace Prime31
{
	public class GKTurnBasedMatch
	{
		public string matchId;
		public GKTurnBasedMatchStatus status;
		public List<GKTurnBasedParticipant> participants;
		public DateTime creationDate;
		public GKTurnBasedParticipant currentParticipant;
		public string message;
		
		
		public static List<GKTurnBasedMatch> fromJSON( string json )
		{
			var matchList = new List<GKTurnBasedMatch>();
			
			// decode the json
			var list = json.listFromJson();
			
			// create DTO's from the Hashtables
			foreach( Dictionary<string,object> ht in list )
				matchList.Add( new GKTurnBasedMatch( ht ) );
			
			return matchList;
		}
		
		
		public GKTurnBasedMatch( Dictionary<string,object> dict )
		{
			if( dict.ContainsKey( "matchId" ) )
				matchId = dict["matchId"].ToString();
			
			if( dict.ContainsKey( "status" ) )
				status = (GKTurnBasedMatchStatus)int.Parse( dict["status"].ToString() );
			
			if( dict.ContainsKey( "message" ) )
				message = dict["message"].ToString();
			
			// grab and convert the date
			if( dict.ContainsKey( "creationDate" ) )
			{
				var timeSinceEpoch = double.Parse( dict["creationDate"].ToString() );
				var intermediate = new DateTime( 1970, 1, 1, 0, 0, 0, DateTimeKind.Utc );
				creationDate = intermediate.AddSeconds( timeSinceEpoch );
			}
			
			// get the participants
			if( dict.ContainsKey( "participants" ) )
			{
				var participantsList = dict["participants"] as List<object>;
				participants = new List<GKTurnBasedParticipant>( participantsList.Count );
				
				foreach( Dictionary<string,object> particpantDict in participantsList )
				{
					if( particpantDict != null )
						participants.Add( new GKTurnBasedParticipant( particpantDict ) );
					else
						participants.Add( new GKTurnBasedParticipant() );
				}
			}
			
			// get the current participant
			if( dict.ContainsKey( "currentParticipant" ) )
				currentParticipant = new GKTurnBasedParticipant( dict["currentParticipant"] as Dictionary<string,object> );
		}
		
		
		public override string ToString()
		{
			 return string.Format( "<GKTurnBasedMatch> matchId: {0}, status: {1}, participants: {2}, creationDate: {3}, currentParticipant: {4}, message: {5}",
				matchId, status, participants, creationDate, currentParticipant, message );
		}
	}

}
#endif
