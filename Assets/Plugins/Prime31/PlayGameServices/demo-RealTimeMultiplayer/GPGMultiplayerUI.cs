using UnityEngine;
using Prime31;



/// <summary>
/// This demo scene is setup to show how to get a real-time multiplayer game started in a variety of ways (via received invitation, programmatically or via the player selector).
///
/// First, you must have an authenticated player (Authenticate button). Once you do you can pick how you want to start the room (Room Creation section). The demo scene will
/// automatically show the Google Play waiting room UI (showWaitingRoom) when a room is created or joined (note that this is an optional step for when you make your game).
/// Once all players are connected it gets dismissed and we are in a real-time game ready to start sending data.
///
/// For demonstration purposes, you can send data (via the Send Unreliable Message to All button) or leave the room (via the Leave Room button) once connected.
/// </summary>

namespace Prime31
{
	public class GPGMultiplayerUI : MonoBehaviourGUI
	{
#if UNITY_ANDROID
	
		private bool _isGameInProgress;
		private string _lastReceivedMessage = string.Empty;
	
	
		void Start()
		{
			PlayGameServices.enableDebugLog( true );
		}
	
	
		void OnGUI()
		{
			GUI.skin.label.alignment = TextAnchor.MiddleCenter;
			beginColumn();
	
	
	
			// if there is no game in progress we show buttons to setup a room
			if( !_isGameInProgress )
			{
				if( GUILayout.Button( "Authenticate" ) )
				{
					PlayGameServices.authenticate();
				}
	
	
				GUILayout.Label( "Room Creation" );
	
				if( GUILayout.Button( "Show Invitation Inbox" ) )
				{
					GPGMultiplayer.showInvitationInbox();
				}
	
	
				if( GUILayout.Button( "Start Quick Match" ) )
				{
					GPGMultiplayer.startQuickMatch( 1, 1 );
				}
	
	
				if( GUILayout.Button( "Create Room Programmatically" ) )
				{
					GPGMultiplayer.createRoomProgrammatically( 1, 1 );
				}
	
	
				if( GUILayout.Button( "Show Player Selector" ) )
				{
					GPGMultiplayer.showPlayerSelector( 1, 1 );
				}
			}
			else
			{
				GUILayout.Label( "In Real-time Match" );
	
				if( GUILayout.Button( "Send Unreliable Message to All" ) )
				{
					var bytes = System.Text.Encoding.UTF8.GetBytes( "howdy. current time: " + System.DateTime.Now );
					GPGMultiplayer.sendUnreliableRealtimeMessageToAll( bytes );
				}
	
	
	
				if( GUILayout.Button( "Get Room Participants" ) )
				{
					var participants = GPGMultiplayer.getParticipants();
					Utils.logObject( participants );
				}
	
	
				if( GUILayout.Button( "Leave Room" ) )
				{
					GPGMultiplayer.leaveRoom();
				}
	
	
				GUILayout.Space( 40 );
	
				GUILayout.Label( _lastReceivedMessage );
			}
	
			endColumn( false );
	
	
			if( bottomLeftButton( "Back to Standard Demo Scene" ) )
				loadLevel( "PlayGameServicesDemoScene" );
		}
	
	
		#region Event Listeners
	
		void OnEnable()
		{
			// once the room is connected we are clear to start playing
			GPGMultiplayerManager.onRoomConnectedEvent += onRoomConnectedEvent;
	
			// this event will fire whenever a real-time message is received
			GPGMultiplayerManager.onRealTimeMessageReceivedEvent += onRealTimeMessageReceivedEvent;
	
			// finally, when one of these events fires we are done with the game
			GPGMultiplayerManager.onDisconnectedFromRoomEvent += onDisconnectedOrLeftRoom;
			GPGMultiplayerManager.onLeftRoomEvent += onDisconnectedOrLeftRoom;
		}
	
	
		void OnDisable()
		{
			GPGMultiplayerManager.onRoomConnectedEvent -= onRoomConnectedEvent;
			GPGMultiplayerManager.onRealTimeMessageReceivedEvent -= onRealTimeMessageReceivedEvent;
			GPGMultiplayerManager.onDisconnectedFromRoomEvent -= onDisconnectedOrLeftRoom;
			GPGMultiplayerManager.onLeftRoomEvent -= onDisconnectedOrLeftRoom;
		}
	
	
		private void onRoomConnectedEvent( GPGRoom room, GPGRoomUpdateStatus status )
		{
			_isGameInProgress = true;
		}
	
	
		private void onWaitingRoomCompletedEvent( bool didSucceed )
		{
			_isGameInProgress = didSucceed;
		}
	
	
		private void onDisconnectedOrLeftRoom()
		{
			_isGameInProgress = false;
		}
	
	
		private void onRealTimeMessageReceivedEvent( string senderParticipantId, byte[] message )
		{
			// we will store off the message that we received and process it later since this can be called
			// multiple times per frame or on any thread
			var messageString = System.Text.Encoding.UTF8.GetString( message );
			_lastReceivedMessage = string.Format( "Last Message: " + messageString );
		}
	
		#endregion
	
#endif
	}

}
