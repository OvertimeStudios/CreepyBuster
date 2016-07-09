using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Prime31;


// Advanced player attributes.
public enum PlayerAttributes : uint
{
	Alien = 0xFFFF0000,
	Predator = 0x0000FFFF
}



namespace Prime31
{
	public partial class GameCenterMultiplayerGUIManagerTwo : MonoBehaviourGUI
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
		}
	
	
		void OnDisable()
		{
			GameCenterMultiplayerManager.playerConnectedEvent -= playerConnected;
			GameCenterMultiplayerManager.playerDisconnectedEvent -= playerDisconnected;
		}
	
	
		#region Event listeners
	
		public void playerConnected( string playerId )
		{
			statusText.text = "player connected: " + playerId;
		}
	
	
		void playerDisconnected( string playerId )
		{
			statusText.text = "player disconnected: " + playerId;
		}
	
		#endregion;
	
	
		void OnGUI()
		{
			beginColumn();
	
			if( GUILayout.Button( "Matchmaker for Game One" ) )
			{
				GameCenterMultiplayerBinding.showMatchmakerWithFilters( 2, 4, 1, 0, null );
			}
	
	
			if( GUILayout.Button( "Matchmaker for Game Two" ) )
			{
				GameCenterMultiplayerBinding.showMatchmakerWithFilters( 2, 4, 2, 0, null );
			}
	
	
			if( GUILayout.Button( "Send To All" ) )
			{
				GameCenterMultiplayerBinding.sendMessageToAllPeers( "GameCenterMultiplayerGUIManagerTwo", "receiveTime", Time.timeSinceLevelLoad.ToString(), false );
			}
	
	
			if( GUILayout.Button( "Disconnect" ) )
			{
				GameCenterMultiplayerBinding.disconnectFromMatch();
			}
	
	
			if( GUILayout.Button( "Get Connected PlayerIds" ) )
			{
				string[] playersIds = GameCenterMultiplayerBinding.getAllConnectedPlayerIds();
				foreach( string p in playersIds )
					Debug.Log( "connected to: " + p );
			}
	
	
			endColumn( true );
	
	
			if( GUILayout.Button( "Find Match as Alien" ) )
			{
				GameCenterMultiplayerBinding.findMatchProgrammaticallyWithFilters( 2, 4, 0, (uint)PlayerAttributes.Alien );
			}
	
	
			if( GUILayout.Button( "Find Match as Predator" ) )
			{
				GameCenterMultiplayerBinding.findMatchProgrammaticallyWithFilters( 2, 4, 0, (uint)PlayerAttributes.Predator );
			}
	
	
	
			if( GUILayout.Button( "Cancel Find Match" ) )
			{
				GameCenterMultiplayerBinding.cancelProgrammaticMatchRequest();
			}
	
			endColumn();
	
	
	
			if( bottomLeftButton( "Raw Data Send Scene" ) )
			{
				GameCenterMultiplayerBinding.disconnectFromMatch();
				loadLevel( "GameCenterMultiplayerTestSceneThree" );
			}
		}
	
	
		public void receiveTime( string data )
		{
			messageText.text = data;
		}
	
#endif
	}

}
