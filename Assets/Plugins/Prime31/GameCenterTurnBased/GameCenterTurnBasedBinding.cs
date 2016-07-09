using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using Prime31;


#if UNITY_IOS

namespace Prime31
{
	public class GameCenterTurnBasedBinding
	{
		// Checks to see if turn based multiplayer is available
		[System.Obsolete( "Deprecated. We only support iOS 5+ so this is a moot call." )]
		public static bool isTurnBasedMultiplayerAvailable()
		{
			return true;
		}


		[DllImport("__Internal")]
		private static extern void _gcTurnBasedFindMatch( int minPlayers, int maxPlayers, bool showExistingMatches );

		// find a match using the matchmaker optionally showing all existing matches
		public static void findMatch( int minPlayers, int maxPlayers, bool showExistingMatches )
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer )
				_gcTurnBasedFindMatch( minPlayers, maxPlayers, showExistingMatches );
		}


		[DllImport("__Internal")]
		private static extern void _gcTurnBasedFindMatchWithFilters( int minPlayers, int maxPlayers, string playersToInvite, uint playerGroup, uint playerAttributes, bool showExistingMatches );

		// find a match using the matchmaker with advanced filtering options
		public static void findMatchWithFilters( int minPlayers, int maxPlayers, string[] playersToInvite, uint playerGroup, uint playerAttributes, bool showExistingMatches )
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer )
			{
				string playerIdString = null;
				if( playersToInvite != null && playersToInvite.Length > 0 )
					playerIdString = string.Join( ",", playersToInvite );

				_gcTurnBasedFindMatchWithFilters( minPlayers, maxPlayers, playerIdString, playerGroup, playerAttributes, showExistingMatches );
			}
		}


		[DllImport("__Internal")]
		private static extern void _gcSaveCurrentTurnWithMatchData( byte[] data, int length );

		// iOS 6+ only! Saves the current match data without ending the turn or match
		public static void saveCurrentTurnWithMatchData( byte[] data )
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer )
				_gcSaveCurrentTurnWithMatchData(data, data.Length );
		}


		[DllImport("__Internal")]
		private static extern void _gcTurnBasedEndTurnWithNextParticipant( string playerId, byte[] data, int length );

		// Ends the current participant's turn. You may update the matchOutcome for any GKTurnBasedParticipants that you wish to before ending the turn.
		// null is a valid playerId.  If null is used then the next unassigned player will be set as the next participant
		public static void endTurnWithNextParticipant( string playerId, byte[] data )
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer )
				_gcTurnBasedEndTurnWithNextParticipant( playerId, data, data.Length );
		}


		[DllImport("__Internal")]
		private static extern void _gcTurnBasedEndTurnWithNextParticipants( string playerIds, double timeout, byte[] data, int length );

		// iOS 6+ only! Ends the current participant's turn. The playerIds array must contain only valid playerIds! You cannot use this method to pass
		// the turn to a ghost player. Use endTurnWithNextParticipant if there is no next participant available yet.
		public static void endTurnWithNextParticipants( string[] playerIds, double timeout, byte[] data )
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer )
			{
				if( playerIds == null || playerIds.Length == 0 )
				{
					Debug.LogError( "aborting. endTurnWithNextParticipants was called with either a null or empty array." );
					return;
				}

				_gcTurnBasedEndTurnWithNextParticipants( string.Join( ",", playerIds ), timeout, data, data.Length );
			}
		}


		[DllImport("__Internal")]
		private static extern void _gcTurnBasedEndMatchInTurnWithMatchData( byte[] data, int length );

		// Calling this method ends the match for all players. This method may only be called by the current participant.
		// Before your game calls this method, the matchOutcome property on each participant object stored in the
		// participants property must have been set to a value other than GKTurnBasedMatchOutcomeNone.
		public static void endMatchInTurnWithMatchData( byte[] data )
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer )
				_gcTurnBasedEndMatchInTurnWithMatchData( data, data.Length );
		}


		[DllImport("__Internal")]
		private static extern void _gcTurnBasedEndMatchInTurnWithMatchDataAndExtras( byte[] data, int length, string scoreJSON, string achievementsJSON );

		// iOS 7+ only. Calling this method ends the match for all players. This method may only be called by the current participant.
		// Before your game calls this method, the matchOutcome property on each participant object stored in the
		// participants property must have been set to a value other than GKTurnBasedMatchOutcomeNone.
		public static void endMatchInTurnWithMatchDataAndExtras( byte[] data, GKTurnBasedScore[] scores, GKTurnBasedAchievement[] achievements )
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer )
				_gcTurnBasedEndMatchInTurnWithMatchDataAndExtras( data, data.Length, Json.encode( scores ), Json.encode( achievements ) );
		}


		[DllImport("__Internal")]
		private static extern void _gcTurnBasedLoadMatchData();

		// Loads the game-specific data associated with a match.
		public static void loadMatchData()
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer )
				_gcTurnBasedLoadMatchData();
		}


		[DllImport("__Internal")]
		private static extern void _gcTurnBasedParticipantQuitInTurnWithOutcome( int outcome, string nextParticipantPlayerId, byte[] data, int length );

		// Resigns the current player from the match without ending the match
		public static void participantQuitInTurnWithOutcome( GKTurnBasedMatchOutcome outcome, string nextParticipantPlayerId, byte[] data )
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer )
				_gcTurnBasedParticipantQuitInTurnWithOutcome( (int)outcome, nextParticipantPlayerId, data, data.Length );
		}


		[DllImport("__Internal")]
		private static extern void _gcTurnBasedParticipantQuitInTurnWithOutcomeAndTimeout( int outcome, string nextParticipantPlayerIds, double timeout, byte[] data, int length );

		// iOS 6+ only! Resigns the current player from the match without ending the match
		public static void participantQuitInTurnWithOutcome( GKTurnBasedMatchOutcome outcome, string[] nextParticipantPlayerIds, double timeout, string nextParticipantPlayerId, byte[] data )
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer )
			{
				if( nextParticipantPlayerIds == null )
					nextParticipantPlayerIds = new string[0];
				_gcTurnBasedParticipantQuitInTurnWithOutcomeAndTimeout( (int)outcome, string.Join( ",", nextParticipantPlayerIds ), timeout, data, data.Length );
			}
		}


		[DllImport("__Internal")]
		private static extern void _gcTurnBasedParticipantQuitOutOfTurnWithOutcome( int outcome );

		// Resigns the player from the match when that player is not the current player. This action does not end the match.
		public static void participantQuitOutOfTurnWithOutcome( GKTurnBasedMatchOutcome outcome )
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer )
				_gcTurnBasedParticipantQuitOutOfTurnWithOutcome( (int)outcome );
		}


		[DllImport("__Internal")]
		private static extern void _gcTurnBasedSetMatchMessage( string message );

		// Sets the message for the match. Only the current player can set the message.
		public static void setMatchMessage( string message )
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer )
				_gcTurnBasedSetMatchMessage( message );
		}
		
		
		[DllImport("__Internal")]
		private static extern void _gcTurnBasedSetLocalizableMessage( string key, string arguments );

		// Sets the message for the match. Only the current player can set the message. Uses the Xcode localization
		// system for the key/arguments.
		public static void setLocalizableMessage( string key, string[] arguments = null )
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer )
				_gcTurnBasedSetLocalizableMessage( key, Json.encode( arguments ) );
		}
		
		
		[DllImport("__Internal")]
		private static extern void _gcTurnBasedSendReminder( string matchId, string participants, string key, string arguments );

		// iOS 7+ only. Sends a push notification remender to all the specified participants. Uses the Xcode localization
		// system for the key/arguments.
		public static void sendReminder( string matchId, string[] participants, string key, string[] arguments = null )
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer )
				_gcTurnBasedSendReminder( matchId, Json.encode( participants ), key, Json.encode( arguments ) );
		}


		[DllImport("__Internal")]
		private static extern bool _gcTurnBasedIsCurrentPlayersTurn();

		// Convenience method to see if it is the current players turn
		public static bool isCurrentPlayersTurn()
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer )
				return _gcTurnBasedIsCurrentPlayersTurn();

			return false;
		}


		[DllImport("__Internal")]
		private static extern void _gcTurnBasedSetMatchOutcomeForParticipant( int outcome, string participantPlayerId );

		// Sets the match outcome for the given participant
		public static void setMatchOutcomeForParticipant( GKTurnBasedMatchOutcome outcome, string participantPlayerId )
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer )
				_gcTurnBasedSetMatchOutcomeForParticipant( (int)outcome, participantPlayerId );
		}


		[DllImport("__Internal")]
		private static extern void _gcTurnBasedFindMatchProgramatically( int minPlayers, int maxPlayers );

		// Programatic match Methods
		// finds a match programatically
		public static void findMatchProgrammatically( int minPlayers, int maxPlayers )
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer )
				_gcTurnBasedFindMatchProgramatically( minPlayers, maxPlayers );
		}


		[DllImport("__Internal")]
		private static extern void _gcTurnBasedFindMatchProgramaticallyWithFilters( int minPlayers, int maxPlayers, string playersToInvite, uint playerGroup, uint playerAttributes );

		// Find a match programatically with advanced filtering options
		public static void findMatchProgrammaticallyWithFilters( int minPlayers, int maxPlayers, string[] playersToInvite, uint playerGroup, uint playerAttributes )
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer )
			{
				string playerIdString = null;
				if( playersToInvite != null && playersToInvite.Length > 0 )
					playerIdString = string.Join( ",", playersToInvite );

				_gcTurnBasedFindMatchProgramaticallyWithFilters( minPlayers, maxPlayers, playerIdString, playerGroup, playerAttributes );
			}
		}


		[DllImport("__Internal")]
		private static extern void _gcTurnBasedLoadMatches();

		// loads all matches the player is currently involved with
		public static void loadMatches()
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer )
				_gcTurnBasedLoadMatches();
		}


		[DllImport("__Internal")]
		private static extern void _gcTurnBasedRemoveCurrentMatch();

		// Programmatically removes the current match from Game Center. The developer should not do this without user input.
		public static void removeCurrentMatch()
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer )
				_gcTurnBasedRemoveCurrentMatch();
		}


		[DllImport("__Internal")]
		private static extern void _gcTurnBasedRemoveMatch( string matchId );

		// Programmatically removes a match from Game Center. The developer should not do this without user input.
		public static void removeMatch( string matchId )
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer )
				_gcTurnBasedRemoveMatch( matchId );
		}


		[DllImport("__Internal")]
		private static extern void _gcTurnBasedAcceptInvite( string matchId );

		// iOS 6+ only! Programmatically accept an invitation to a turn-based match. Results in the acceptInviteForMatchFailed/SuccessfulEvent firing.
		// If successful, it will set the returned match as current and load the match data automatically.
		public static void acceptInvite( string matchId )
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer )
				_gcTurnBasedAcceptInvite( matchId );
		}


		[DllImport("__Internal")]
		private static extern void _gcTurnBasedDeclineInvite( string matchId );

		// iOS 6+ only! Programmatically decline an invitation to a turn-based match.
		public static void declineInviteForMatch( string matchId )
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer )
				_gcTurnBasedDeclineInvite( matchId );
		}


		[DllImport("__Internal")]
		private static extern void _gcTurnBasedRematchCurrentMatch();

		// iOS 6+ only! Sets up a rematch of the current match. This works identically to a programmatic match and will fire the findMatchProgramaticallyFailedEvent
		// or the findMatchProgramaticallyFinishedEvent after setting the current match to the newly created rematch. The match data will also be loaded automatically.
		// Do not call this method on matches that do not have actual participents and have not been properly ended!
		public static void rematchCurrentMatch()
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer )
				_gcTurnBasedRematchCurrentMatch();
		}


		[DllImport("__Internal")]
		private static extern void _gcTurnBasedRematchMatch( string matchId );

		// iOS 6+ only! Sets up a rematch of the current match. This works identically to a programmatic match and will fire the findMatchProgramaticallyFailedEvent
		// or the findMatchProgramaticallyFinishedEvent after setting the current match to the newly created rematch. The match data will also be loaded automatically.
		// Do not call this method on matches that do not have actual participents and have not been properly ended!
		public static void rematchMatch( string matchId )
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer )
				_gcTurnBasedRematchMatch( matchId );
		}


		[DllImport("__Internal")]
		private static extern void _gcTurnBasedChangeCurrentMatch( string matchId, bool shouldLoadMatchData );

		// Sets the currently active match optionally loading the match data after changing the match
		public static void changeCurrentMatch( string matchId, bool shouldLoadMatchData = true )
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer )
				_gcTurnBasedChangeCurrentMatch( matchId, shouldLoadMatchData );
		}

	}

}
#endif
