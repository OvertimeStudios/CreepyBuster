using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Prime31;


#if UNITY_IOS
public enum GKTurnBasedParticipantStatus
{
    Unknown     = 0,
    Invited     = 1,    // a participant has been invited but not yet responded
    Declined    = 2,    // a participant that has declined an invite to this match
    Matching    = 3,    // a participant that is waiting to be matched
    Active      = 4,    // a participant that is active in this match
    Done        = 5,    // a participant is done with this session
}


public enum GKTurnBasedMatchOutcome
{
    None         = 0,        // Participants who are not done with a match have this state
    Quit         = 1,        // Participant quit
    Won          = 2,        // Participant won
    Lost         = 3,        // Participant lost
    Tied         = 4,        // Participant tied
    TimeExpired  = 5,        // Game ended due to time running out
    First        = 6,
    Second       = 7,
    Third        = 8,
    Fourth       = 9,

    CustomRange = 0x00FF0000	// game result range available for custom app use
}



namespace Prime31
{
	public class GKTurnBasedParticipant
	{
		public string playerId;
		public GKTurnBasedParticipantStatus status;
		public DateTime lastTurnDate;
		public GKTurnBasedMatchOutcome matchOutcome;
		/// <summary>
		/// this will be null on iOS </summary> 8
		/// </summary>
		public GKPlayer player;
	
	
		public static List<GKTurnBasedParticipant> fromJSON( string json )
		{
			var participantList = new List<GKTurnBasedParticipant>();
	
			// decode the json
			var list = json.listFromJson();
	
			// create DTO's from the Hashtables
			foreach( Dictionary<string,object> dict in list )
				participantList.Add( new GKTurnBasedParticipant( dict ) );
	
			return participantList;
		}
	
	
		public GKTurnBasedParticipant()
		{}
	
	
		public GKTurnBasedParticipant( Dictionary<string,object> dict )
		{
			if( dict == null )
				return;
	
			if( dict.ContainsKey( "playerId" ) )
				playerId = dict["playerId"].ToString();
	
			if( dict.ContainsKey( "status" ) )
				status = (GKTurnBasedParticipantStatus)int.Parse( dict["status"].ToString() );
	
			if( dict.ContainsKey( "matchOutcome" ) )
				matchOutcome = (GKTurnBasedMatchOutcome)int.Parse( dict["matchOutcome"].ToString() );
	
			// grab and convert the date
			if( dict.ContainsKey( "lastTurnDate" ) )
			{
				var timeSinceEpoch = double.Parse( dict["lastTurnDate"].ToString() );
				var intermediate = new DateTime( 1970, 1, 1, 0, 0, 0, DateTimeKind.Utc );
				lastTurnDate = intermediate.AddSeconds( timeSinceEpoch );
			}
	
			if( dict.ContainsKey( "player" ) )
			{
				var playerDict = dict["player"] as Dictionary<string,object>;
				player = new GKPlayer( playerDict );
			}
		}
	
	
		public override string ToString()
		{
			 return string.Format( "<Participant> playerId: {0}, status: {1}, matchOutcome: {2}, lastTurnDate: {3}",
				playerId, status, matchOutcome, lastTurnDate );
		}
	}

}
#endif
