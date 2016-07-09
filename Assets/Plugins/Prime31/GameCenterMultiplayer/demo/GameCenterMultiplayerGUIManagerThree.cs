using UnityEngine;
using System.Collections;
using Prime31;



namespace Prime31
{
	public partial class GameCenterMultiplayerGUIManagerThree : MonoBehaviourGUI
	{
		public GUIText statusText;
		public GUIText messageText;
	
	
#if UNITY_IOS
		void Start()
		{
			// always authenticate at every launch
			GameCenterBinding.authenticateLocalPlayer();
	
			// Listen to a few events to help update the GUIText
			GameCenterMultiplayerManager.playerConnectedEvent += playerConnected;
			GameCenterMultiplayerManager.playerDisconnectedEvent += playerDisconnected;
			GameCenterMultiplayerManager.receivedRawDataEvent += receivedRawDataEvent;
		}
	
	
		void OnDisable()
		{
			GameCenterMultiplayerManager.playerConnectedEvent -= playerConnected;
			GameCenterMultiplayerManager.playerDisconnectedEvent -= playerDisconnected;
			GameCenterMultiplayerManager.receivedRawDataEvent -= receivedRawDataEvent;
		}
	
	
		#region Event listeners and message handlers
	
		public void playerConnected( string playerId )
		{
			statusText.text = "player connected: " + playerId;
		}
	
	
		void playerDisconnected( string playerId )
		{
			statusText.text = "player disconnected: " + playerId;
		}
	
	
		void receivedRawDataEvent( string playerId, byte[] data )
		{
			var theStr = System.Text.UTF8Encoding.UTF8.GetString( data );
			Debug.Log( "receivedRawDataEvent converted back to string: " + theStr );
	
			messageText.text = theStr;
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
				// we will just send some text across the wire encoded into a byte array for demonstration purposes
				var theStr = "im a string that will be encoded to a byte array and sent across the wire using GameCenter";
				var bytes = System.Text.UTF8Encoding.UTF8.GetBytes( theStr );
	
				GameCenterMultiplayerBinding.sendRawMessageToAllPeers( bytes, true );
			}
	
	
			endColumn( true );
	
	
			if( GUILayout.Button( "Find Match (no GUI)" ) )
			{
				GameCenterMultiplayerBinding.findMatchProgrammaticallyWithMinMaxPlayers( 2, 4 );
			}
	
			endColumn();
	
	
			if( bottomLeftButton( "Leaderboard Scene" ) )
			{
				GameCenterMultiplayerBinding.disconnectFromMatch();
				loadLevel( "GameCenterTestScene" );
			}
		}
#endif
	}

}
