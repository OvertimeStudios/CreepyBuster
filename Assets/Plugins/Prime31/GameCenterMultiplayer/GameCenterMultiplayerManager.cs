using UnityEngine;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Prime31;


#if UNITY_IOS

namespace Prime31
{
	public class GameCenterMultiplayerManager : AbstractManager
	{
		// Fired when the matchmaker is cancelled by the user
		public static event Action matchmakerCancelledEvent;
	
		// Fired when the matchmaker fails to find a match
		public static event Action<string> matchmakerFailedEvent;
	
		// Fired when the matchmaker finds a match
		public static event Action<int> matchmakerFoundMatchEvent;
	
		// Fired when the friend request controller is dismissed
		public static event Action friendRequestControllerFinishedEvent;
	
		// Fired when a player connects to a match
		public static event Action<string> playerConnectedEvent;
	
		// Fired when a player disconnects to a match
		public static event Action<string> playerDisconnectedEvent;
	
		// Fired when connected to a player fails
		public static event Action<string> playerConnectionFailedEvent;
	
		// Fired when either an invite request is received either from another player or initiated by the current player via GameCenter.app
		public static event Action inviteRequestWasReceivedEvent;
	
		// Fired when a player is speaking if receiveUpdates is true for the channel
		public static event Action<string> playerBeganSpeakingEvent;
	
		// Fired when a player goes silent if receiveUpdates is true for the channel
		public static event Action<string> playerStoppedSpeakingEvent;
	
		// Fired when a programmatic match fails to connect
		public static event Action<string> findMatchFailedEvent;
	
		// Fired when a programmatic find match connects
		public static event Action<int> findMatchFinishedEvent;
	
		// Fired when adding players to a current match fails
		public static event Action<string> addPlayersToMatchFailedEvent;
	
		// Fired when finding current activity for your game fails
		public static event Action<string> findActivityFailedEvent;
	
		// Fired when finding activity returns successfully
		public static event Action<int> findActivityFinishedEvent;
	
		// Fired when finding current activity for a group fails
		public static event Action<string> findActivityForGroupFailedEvent;
	
		// Fired when finding activity for a group returns successfully
		public static event Action<int> findActivityForGroupFinishedEvent;
	
		// Fired when retrieving scores fails
		public static event Action<string> retrieveMatchesBestScoresFailedEvent;
	
		// Fired when retrieving scores finishes successfully
		public static event Action<GameCenterRetrieveScoresResult> retrieveMathesBestScoresFinishedEvent;
	
		// Fired after calling chooseBestHostPlayer when Apple chooses the best host player. Includes the playerId.
		public static event Action<string> chooseBestHostPlayerCompletedEvent;
	
		// Fired when a raw message is received. Includes the playerId and the raw data
		public static event Action<string,byte[]> receivedRawDataEvent;
	
	
	
		private delegate void receivedDataCallback( string playerId, IntPtr dataBuf, int dataSize );
	
		[DllImport("__Internal")]
		private static extern void _gameCenterMultiplayerSetGameKitReceivedDataCallback( receivedDataCallback callback );
	
	
	    static GameCenterMultiplayerManager()
	    {
			AbstractManager.initialize( typeof( GameCenterMultiplayerManager ) );
	
			if( Application.platform == RuntimePlatform.IPhonePlayer )
				_gameCenterMultiplayerSetGameKitReceivedDataCallback( didReceivedData );
	    }
	
	
		[AOT.MonoPInvokeCallback( typeof( receivedDataCallback ) )]
		private static void didReceivedData( string playerId, IntPtr dataBuf, int dataSize )
		{
			var data = new byte[dataSize];
			Marshal.Copy( dataBuf, data, 0, dataSize );
	
			if( receivedRawDataEvent != null )
				receivedRawDataEvent( playerId, data );
		}
	
	
	
		#region Standard Matchmaking
	
		public void matchmakerWasCancelled( string empty )
		{
			if( matchmakerCancelledEvent != null )
				matchmakerCancelledEvent();
		}
	
	
		public void matchmakerFailed( string error )
		{
			if( matchmakerFailedEvent != null )
				matchmakerFailedEvent( error );
		}
	
	
		public void matchmakerFoundMatchWithExpectedPlayerCount( string playerCount )
		{
			int expectedPlayerCount = int.Parse( playerCount );
			if( matchmakerFoundMatchEvent != null )
				matchmakerFoundMatchEvent( expectedPlayerCount );
		}
	
	
		public void friendRequestComposeViewControllerDidFinish( string empty )
		{
			if( friendRequestControllerFinishedEvent != null )
				friendRequestControllerFinishedEvent();
		}
	
	
		public void playerDidConnectToMatch( string playerId )
		{
			if( playerConnectedEvent != null )
				playerConnectedEvent( playerId );
		}
	
	
		public void playerDidDisconnectFromMatch( string playerId )
		{
			if( playerDisconnectedEvent != null )
				playerDisconnectedEvent( playerId );
		}
	
	
		public void connectionWithPlayerFailed( string playerId )
		{
			if( playerConnectionFailedEvent != null )
				playerConnectionFailedEvent( playerId );
		}
	
	
		public void inviteRequestReceived( string emtpy )
		{
			if( inviteRequestWasReceivedEvent != null )
				inviteRequestWasReceivedEvent();
		}
	
		#endregion;
	
	
		#region Voice Chat Events
	
		public void playerIsSpeaking( string playerId )
		{
			if( playerBeganSpeakingEvent != null )
				playerBeganSpeakingEvent( playerId );
		}
	
	
		public void playerIsSilent( string playerId )
		{
			if( playerStoppedSpeakingEvent != null )
				playerStoppedSpeakingEvent( playerId );
		}
	
		#endregion;
	
	
		#region Programmatic Matchmaking Events
	
		public void findMatchProgramaticallyFailed( string error )
		{
			if( findMatchFailedEvent != null )
				findMatchFailedEvent( error );
		}
	
	
		public void findMatchProgramaticallyFinishedWithExpectedPlayerCount( string playerCount )
		{
			int expectedPlayerCount = int.Parse( playerCount );
			if( findMatchFinishedEvent != null )
				findMatchFinishedEvent( expectedPlayerCount );
		}
	
	
		public void addPlayersToCurrentMatchFailed( string error )
		{
			if( addPlayersToMatchFailedEvent != null )
				addPlayersToMatchFailedEvent( error );
		}
	
	
		public void findAllActivityFailed( string error )
		{
			if( findActivityFailedEvent != null )
				findActivityFailedEvent( error );
		}
	
	
		public void findAllActivityFinished( string activity )
		{
			int activityCount = int.Parse( activity );
			if( findActivityFinishedEvent != null )
				findActivityFinishedEvent( activityCount );
		}
	
	
		public void findAllActivityForPlayerGroupFailed( string error )
		{
			if( findActivityForGroupFailedEvent != null )
				findActivityForGroupFailedEvent( error );
		}
	
	
		public void findAllActivityForPlayerGroupFinished( string activity )
		{
			int activityCount = int.Parse( activity );
			if( findActivityForGroupFinishedEvent != null )
				findActivityForGroupFinishedEvent( activityCount );
		}
	
	
		public void retrieveMatchesBestScoresDidFail( string error )
		{
			if( retrieveMatchesBestScoresFailedEvent != null )
				retrieveMatchesBestScoresFailedEvent( error );
		}
	
	
		public void retrieveMatchesBestScoresDidFinish( string json )
		{
			if( retrieveMathesBestScoresFinishedEvent != null )
				retrieveMathesBestScoresFinishedEvent( Json.decode<GameCenterRetrieveScoresResult>( json ) );
		}
	
	
		public void chooseBestHostPlayerCompleted( string playerId )
		{
			if( chooseBestHostPlayerCompletedEvent != null )
				chooseBestHostPlayerCompletedEvent( playerId );
		}
	
		#endregion;
	
	}

}
#endif
