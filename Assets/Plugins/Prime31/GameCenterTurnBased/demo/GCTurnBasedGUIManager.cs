using UnityEngine;
using System.Collections.Generic;
using Prime31;



namespace Prime31
{
	public partial class GCTurnBasedGUIManager : MonoBehaviourGUI
	{
		public GUIText textLabel;
	
#if UNITY_IOS
		private string _matchDataString = string.Empty;
		private GKTurnBasedMatch _currentMatch;
	
	
		void Start()
		{
			// always authenticate at every launch
			GameCenterBinding.authenticateLocalPlayer();
	
			// listen for when the match data gets loaded. Note that in a game that is more than one scene you should be adding and removing your event handlers as needed!
			GameCenterTurnBasedManager.matchDataLoadedEvent += ( bytes ) =>
			{
				_matchDataString = System.Text.UTF8Encoding.UTF8.GetString( bytes );
				textLabel.text = "match data: " + _matchDataString;
			};
	
			GameCenterTurnBasedManager.turnBasedMatchmakerViewControllerDidFindMatchEvent += currentMatchChanged;
			GameCenterTurnBasedManager.handleTurnEventEvent += currentMatchChanged;
			GameCenterTurnBasedManager.handleMatchEndedEvent += currentMatchChanged;
		}
	
	
		// for illustration purposes and to keep things simple we will set the match whenever any event
		// that returns it fires
		private void currentMatchChanged( GKTurnBasedMatch match )
		{
			// probably not the most user friendly thing to do but we will just swap to whatever match just had an event
			_currentMatch = match;
			GameCenterTurnBasedBinding.changeCurrentMatch( match.matchId );
			Debug.Log( "loaded match: " + match.matchId + ". The match data will be automatically fetched for us." );
		}
	
	
		private byte[] matchDataStringAsBytes()
		{
			// first, we append something so that we can mimic a player taking a turn and doing something
			var r = new System.Random().Next( 0, 26 );
			char letter = (char)( 'a' + r );
			_matchDataString = _matchDataString + letter.ToString();
			textLabel.text = "match data: " + _matchDataString;
	
			return System.Text.UTF8Encoding.UTF8.GetBytes( _matchDataString );
		}
	
	
		void OnGUI()
		{
			beginColumn();
	
			if( GUILayout.Button( "Find Match" ) )
			{
				matchDataStringAsBytes();
				GameCenterTurnBasedBinding.findMatch( 2, 8, true );
			}
	
	
			if( GUILayout.Button( "End Turn For Match" ) )
			{
				// we will just find any player that is not us to use as the next player
				var ourPlayerId = GameCenterBinding.playerIdentifier();
				var nextPlayerId = string.Empty;
	
				foreach( var p in _currentMatch.participants )
				{
					if( p.playerId != ourPlayerId )
					{
						nextPlayerId = p.playerId;
						break;
					}
				}
	
				GameCenterTurnBasedBinding.endTurnWithNextParticipant( nextPlayerId, matchDataStringAsBytes() );
			}
	
	
			if( GUILayout.Button( "Is it my Turn?" ) )
			{
				Debug.Log( "is it my turn? " + GameCenterTurnBasedBinding.isCurrentPlayersTurn() );
			}
	
	
			GUILayout.Space( 50 );
			if( GUILayout.Button( "End Match In Turn" ) )
			{
				// you cannot end a match without first setting the matchOutcome for all players in the match
				foreach( var p in _currentMatch.participants )
					GameCenterTurnBasedBinding.setMatchOutcomeForParticipant( GKTurnBasedMatchOutcome.Tied, p.playerId );
	
				GameCenterTurnBasedBinding.endMatchInTurnWithMatchData( matchDataStringAsBytes() );
			}
	
	
			if( GUILayout.Button( "End Match In Turn With Extras" ) )
			{
				// you cannot end a match without first setting the matchOutcome for all players in the match
				foreach( var p in _currentMatch.participants )
					GameCenterTurnBasedBinding.setMatchOutcomeForParticipant( GKTurnBasedMatchOutcome.Tied, p.playerId );
	
				var scores = new List<GKTurnBasedScore>();
				scores.Add( new GKTurnBasedScore( _currentMatch.participants[0].playerId, "grp.leaderboard_01", 1000 ) );
				scores.Add( new GKTurnBasedScore( _currentMatch.participants[1].playerId, "grp.leaderboard_01", 2000 ) );
	
				GameCenterTurnBasedBinding.endMatchInTurnWithMatchDataAndExtras( matchDataStringAsBytes(), scores.ToArray(), null );
			}
	
	
			if( GUILayout.Button( "Quit Out of Turn" ) )
			{
				GameCenterTurnBasedBinding.participantQuitOutOfTurnWithOutcome( GKTurnBasedMatchOutcome.Quit );
			}
	
	
			endColumn( true );
	
	
			if( GUILayout.Button( "Load All Matches" ) )
			{
				GameCenterTurnBasedBinding.loadMatches();
			}
	
	
			if( GUILayout.Button( "Remove Current Match" ) )
			{
				GameCenterTurnBasedBinding.removeCurrentMatch();
			}
	
	
			if( GUILayout.Button( "Rematch Current Match" ) )
			{
				// this will only work for a completed match on iOS 6+
				GameCenterTurnBasedBinding.rematchCurrentMatch();
			}
	
	
			if( GUILayout.Button( "Set Match Message" ) )
			{
				// this can only be set if it is the current player's turn
				GameCenterTurnBasedBinding.setMatchMessage( "The match's message!" );
			}
	
			endColumn();
	
	
			if( bottomLeftButton( "Back to 1st Scene" ) )
			{
				loadLevel( 0 );
			}
		}
#endif
	}

}
