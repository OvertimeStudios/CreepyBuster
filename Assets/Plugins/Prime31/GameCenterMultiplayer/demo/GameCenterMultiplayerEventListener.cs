using UnityEngine;
using System.Collections;
using System.Collections.Generic;



namespace Prime31
{
	public partial class GameCenterMultiplayerEventListener : MonoBehaviour
	{
#if UNITY_IOS
		void Start()
		{
			GameCenterMultiplayerManager.matchmakerCancelledEvent += matchmakerCancelled;
			GameCenterMultiplayerManager.matchmakerFoundMatchEvent += matchmakerFoundMatch;
			GameCenterMultiplayerManager.matchmakerFailedEvent += matchmakerFailedEvent;
			GameCenterMultiplayerManager.friendRequestControllerFinishedEvent += friendRequestControllerFinished;
			GameCenterMultiplayerManager.playerConnectedEvent += playerConnected;
			GameCenterMultiplayerManager.playerDisconnectedEvent += playerDisconnected;
			GameCenterMultiplayerManager.playerConnectionFailedEvent += playerConnectionFailed;
			GameCenterMultiplayerManager.inviteRequestWasReceivedEvent += inviteRequestWasReceived;
			GameCenterMultiplayerManager.receivedRawDataEvent += receivedRawDataEvent;
	
			GameCenterMultiplayerManager.playerBeganSpeakingEvent += playerBeganSpeaking;
			GameCenterMultiplayerManager.playerStoppedSpeakingEvent += playerStoppedSpeaking;
	
			GameCenterMultiplayerManager.findMatchFailedEvent += findMatchFailed;
			GameCenterMultiplayerManager.findMatchFinishedEvent += findMatchFinished;
			GameCenterMultiplayerManager.addPlayersToMatchFailedEvent += addPlayersToMatchFailed;
			GameCenterMultiplayerManager.findActivityFailedEvent += findActivityFailed;
			GameCenterMultiplayerManager.findActivityFinishedEvent += findActivityFinished;
			GameCenterMultiplayerManager.findActivityForGroupFailedEvent += findActivityForGroupFailed;
			GameCenterMultiplayerManager.findActivityForGroupFinishedEvent += findActivityForGroupFinished;
			GameCenterMultiplayerManager.retrieveMatchesBestScoresFailedEvent += retrieveMatchesBestScoresFailed;
			GameCenterMultiplayerManager.retrieveMathesBestScoresFinishedEvent += retrieveMatchesBestScoresFinished;
			GameCenterMultiplayerManager.chooseBestHostPlayerCompletedEvent += chooseBestHostPlayerCompletedEvent;
		}
	
	
		void OnDisable()
		{
			// Remove all the event handlers
			GameCenterMultiplayerManager.matchmakerCancelledEvent -= matchmakerCancelled;
			GameCenterMultiplayerManager.matchmakerFoundMatchEvent -= matchmakerFoundMatch;
			GameCenterMultiplayerManager.matchmakerFailedEvent -= matchmakerFailedEvent;
			GameCenterMultiplayerManager.friendRequestControllerFinishedEvent -= friendRequestControllerFinished;
			GameCenterMultiplayerManager.playerConnectedEvent -= playerConnected;
			GameCenterMultiplayerManager.playerDisconnectedEvent -= playerDisconnected;
			GameCenterMultiplayerManager.playerConnectionFailedEvent -= playerConnectionFailed;
			GameCenterMultiplayerManager.inviteRequestWasReceivedEvent -= inviteRequestWasReceived;
			GameCenterMultiplayerManager.receivedRawDataEvent -= receivedRawDataEvent;
	
			GameCenterMultiplayerManager.playerBeganSpeakingEvent -= playerBeganSpeaking;
			GameCenterMultiplayerManager.playerStoppedSpeakingEvent -= playerStoppedSpeaking;
	
			GameCenterMultiplayerManager.findMatchFailedEvent -= findMatchFailed;
			GameCenterMultiplayerManager.findMatchFinishedEvent -= findMatchFinished;
			GameCenterMultiplayerManager.addPlayersToMatchFailedEvent -= addPlayersToMatchFailed;
			GameCenterMultiplayerManager.findActivityFailedEvent -= findActivityFailed;
			GameCenterMultiplayerManager.findActivityFinishedEvent -= findActivityFinished;
			GameCenterMultiplayerManager.findActivityForGroupFailedEvent -= findActivityForGroupFailed;
			GameCenterMultiplayerManager.findActivityForGroupFinishedEvent -= findActivityForGroupFinished;
			GameCenterMultiplayerManager.retrieveMatchesBestScoresFailedEvent -= retrieveMatchesBestScoresFailed;
			GameCenterMultiplayerManager.retrieveMathesBestScoresFinishedEvent -= retrieveMatchesBestScoresFinished;
			GameCenterMultiplayerManager.chooseBestHostPlayerCompletedEvent -= chooseBestHostPlayerCompletedEvent;
		}
	
	
	
		#region Standard Matchmaking
	
		void matchmakerCancelled()
		{
			Debug.Log( "matchmakerCancelled" );
		}
	
	
		void playerConnectionFailed( string error )
		{
			Debug.Log( "playerConnectionFailed: " + error );
		}
	
	
		void matchmakerFailedEvent( string error )
		{
			Debug.Log( "matchmakerFailedEvent: " + error );
		}
	
	
		void inviteRequestWasReceived()
		{
			Debug.Log( "inviteRequestWasReceived. we will not show the matchmaker to start up the match" );
			GameCenterMultiplayerBinding.showMatchmakerWithMinMaxPlayers( 2, 4 );
		}
	
	
		void playerDisconnected( string playerId )
		{
			Debug.Log( "playerDisconnected: " + playerId );
		}
	
	
		void playerConnected( string playerId )
		{
			Debug.Log( "playerConnected: " + playerId );
		}
	
	
		void matchmakerFoundMatch( int count )
		{
			Debug.Log( "matchmakerFoundMatch with expected player count: " + count );
		}
	
	
		void friendRequestControllerFinished()
		{
			Debug.Log( "friendRequestControllerFinished" );
		}
	
		#endregion;
	
	
		void receivedRawDataEvent( string playerId, byte[] data )
		{
			Debug.Log( "receivedRawDataEvent from: " + playerId + ", data.Length: " + data.Length );
		}
	
	
		#region Voice Chat
	
		void playerBeganSpeaking( string playerId )
		{
			Debug.Log( "playerBeganSpeaking: " + playerId );
		}
	
	
		void playerStoppedSpeaking( string playerId )
		{
			Debug.Log( "playerStoppedSpeaking: " + playerId );
		}
	
		#endregion;
	
	
		#region Programmatic Matchmaking
	
		void findMatchFailed( string error )
		{
			Debug.Log( "findMatchFailed: " + error );
		}
	
	
		void findMatchFinished( int count )
		{
			Debug.Log( "findMatchFinished with expected count: " + count );
		}
	
	
		void addPlayersToMatchFailed( string error )
		{
			Debug.Log( "addPlayersToMatchFailed: " + error );
		}
	
	
		void findActivityFailed( string error )
		{
			Debug.Log( "findActivityFailed: " + error );
		}
	
	
		void findActivityFinished( int count )
		{
			Debug.Log( "findActivityFinished with count: " + count );
		}
	
	
		void findActivityForGroupFailed( string error )
		{
			Debug.Log( "findActivityForGroupFailed: " + error );
		}
	
	
		void findActivityForGroupFinished( int count )
		{
			Debug.Log( "findActivityForGroupFinished with count: " + count );
		}
	
	
		void retrieveMatchesBestScoresFailed( string error )
		{
			Debug.Log( "retrieveMatchesBestScoresFailed: " + error );
		}
	
	
		void retrieveMatchesBestScoresFinished( GameCenterRetrieveScoresResult retrieveScoresResult )
		{
			Debug.Log( "retrieveMatchesBestScoresFinished" );
			Prime31.Utils.logObject( retrieveScoresResult );
		}
	
	
		void chooseBestHostPlayerCompletedEvent( string playerId )
		{
			Debug.Log( "chooseBestHostPlayerCompletedEvent: " + playerId );
		}
	
		#endregion;
	
#endif
	}

}
