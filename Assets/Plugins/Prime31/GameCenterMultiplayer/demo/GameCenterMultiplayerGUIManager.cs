using UnityEngine;
using System.Collections;
using Prime31;



namespace Prime31
{
	public partial class GameCenterMultiplayerGUIManager : MonoBehaviourGUI
	{
		public GUIText statusText;
		public GUIText messageText;
	
	
#if UNITY_IOS
		void Start()
		{
			GameCenterManager.playerAuthenticatedEvent += () => { Debug.Log( "Player authenticated" ); };
	
			// always authenticate at every launch
			GameCenterBinding.authenticateLocalPlayer();
	
			// Listen to a few events to help update the GUIText
			GameCenterMultiplayerManager.playerConnectedEvent += playerConnected;
			GameCenterMultiplayerManager.playerDisconnectedEvent += playerDisconnected;
		}
	
	
		void OnDisable()
		{
			GameCenterMultiplayerManager.playerConnectedEvent -= playerConnected;
			GameCenterMultiplayerManager.playerDisconnectedEvent -= playerDisconnected;
		}
	
	
	
		#region Event listeners and message handler
	
		public void playerConnected( string playerId )
		{
			statusText.text = "player connected: " + playerId;
		}
	
	
		void playerDisconnected( string playerId )
		{
			statusText.text = "player disconnected: " + playerId;
		}
	
	
		public void receiveTime( string data )
		{
			messageText.text = data;
		}
	
		#endregion;
	
	
		void OnGUI()
		{
			beginColumn();
	
			if( GUILayout.Button( "Show Matchmaker" ) )
			{
				GameCenterMultiplayerBinding.showMatchmakerWithMinMaxPlayers( 2, 4 );
			}
	
	
			if( GUILayout.Button( "Send To All" ) )
			{
				var error = GameCenterMultiplayerBinding.sendMessageToAllPeers( "GameCenterMultiplayerGUIManager", "receiveTime", Time.timeSinceLevelLoad.ToString(), false );
				if( error != null )
					Debug.LogError( "error with sendMessageToAllPeers: " + error );
			}
	
	
			if( GUILayout.Button( "Disconnect" ) )
			{
				GameCenterMultiplayerBinding.disconnectFromMatch();
			}
	
	
			if( GUILayout.Button( "Show Friend Request (iOS 4.2+)" ) )
			{
				GameCenterMultiplayerBinding.showFriendRequestController();
			}
	
	
			endColumn( true );
	
	
			if( GUILayout.Button( "Find Match (no GUI)" ) )
			{
				GameCenterMultiplayerBinding.findMatchProgrammaticallyWithMinMaxPlayers( 2, 4 );
			}
	
	
	
			if( GUILayout.Button( "Cancel Find Match" ) )
			{
				GameCenterMultiplayerBinding.cancelProgrammaticMatchRequest();
			}
	
	
			if( GUILayout.Button( "Find Activity" ) )
			{
				GameCenterMultiplayerBinding.findAllActivity();
			}
	
	
			if( GUILayout.Button( "Enable Voicechat" ) )
			{
				GameCenterMultiplayerBinding.enableVoiceChat( true );
			}
	
	
			if( GUILayout.Button( "Start Voicechat" ) )
			{
				GameCenterMultiplayerBinding.addAndStartVoiceChatChannel( "testChannel" );
			}
	
	
			if( GUILayout.Button( "Receive VC Updates" ) )
			{
				GameCenterMultiplayerBinding.receiveUpdates( "testChannel", true );
			}
	
			endColumn();
	
	
			if( bottomLeftButton( "Advanced Multiplayer" ) )
			{
				GameCenterMultiplayerBinding.disconnectFromMatch();
				loadLevel( "GameCenterMultiplayerTestSceneTwo" );
			}
		}
#endif
	}

}
