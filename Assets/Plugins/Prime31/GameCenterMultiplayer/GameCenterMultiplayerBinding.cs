using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;


#if UNITY_IOS

namespace Prime31
{
	public class GameCenterMultiplayerBinding : MonoBehaviour
	{
	    [DllImport("__Internal")]
	    private static extern void _gameCenterMultiplayerSetShouldReinvitePlayerOnDisconnect( bool shouldReinvite );
	
		// Sets whether players that are disconnected should be reinvited. Note that this only works with 2 player matches.
	    public static void setShouldReinvitePlayerOnDisconnect( bool shouldReinvite )
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer )
				_gameCenterMultiplayerSetShouldReinvitePlayerOnDisconnect( shouldReinvite );
	    }
	
	
	    [DllImport("__Internal")]
	    private static extern void _gameCenterMultiplayerShowMatchmakerWithMinMaxPlayers( int minPlayers, int maxPlayers );
	
		// Shows the GC matchmaker UI. minPlayers and maxPlayers must be between 2 and 4.
	    public static void showMatchmakerWithMinMaxPlayers( int minPlayers, int maxPlayers )
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer )
				_gameCenterMultiplayerShowMatchmakerWithMinMaxPlayers( minPlayers, maxPlayers );
	    }
	
	
	    [DllImport("__Internal")]
	    private static extern void _gameCenterMultiplayerShowMatchmakerWithFilters( int minPlayers, int maxPlayers, uint playerGroup, uint playerAttributes, string playersToInvite );
	
		// Shows the GC matchmaker UI. minPlayers and maxPlayers must be between 2 and 4.  playerGroup and playerAttributes are advanced filters.
	    public static void showMatchmakerWithFilters( int minPlayers, int maxPlayers, uint playerGroup, uint playerAttributes, string[] playersToInvite )
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer )
			{
				var playerIdString = playersToInvite != null ? string.Join( ",", playersToInvite ) : null;
				_gameCenterMultiplayerShowMatchmakerWithFilters( minPlayers, maxPlayers, playerGroup, playerAttributes, playerIdString );
			}
	    }
	
	
		[DllImport("__Internal")]
		private static extern void _gameCenterMultiplayerShowMatchmakerWithPlayersToInvite( string participants );
	
		// Shows the GC matchmaker UI with the specified players
	    public static void showMatchmakerWithPlayersToInvite( string[] participants )
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer )
				_gameCenterMultiplayerShowMatchmakerWithPlayersToInvite( string.Join( ",", participants ) );
	    }
	
	
		[DllImport("__Internal")]
	    private static extern void _gameCenterMultiplayerAddPlayersToCurrentMatchWithAppleInterface();
	
		// Shows the matchmaker view controller and attempts to add players to the current match. Note: only
		// call this method on one device!
	    public static void addPlayersToCurrentMatchWithAppleInterface()
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer )
				_gameCenterMultiplayerAddPlayersToCurrentMatchWithAppleInterface();
	    }
	
	
		[DllImport("__Internal")]
		private static extern int _gameCenterMultiplayerExpectedPlayerCount();
	
		// Gets the expected player count of the current match. Returns -1 if there is no match currently in progress
		public static int expectedPlayerCount()
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer )
				return _gameCenterMultiplayerExpectedPlayerCount();
			return -1;
		}
	
	
		[DllImport("__Internal")]
	    private static extern void _gameCenterMultiplayerShowFriendRequestController();
	
		// Shows the friend request controller for adding additional friends to the match (iOS 4.2+ only)
	    public static void showFriendRequestController()
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer )
				_gameCenterMultiplayerShowFriendRequestController();
	    }
	
	
		[DllImport("__Internal")]
	    private static extern void _gameCenterMultiplayerShowFriendRequestControllerWithOptions( string message, string participants );
	
		// Shows the friend request controller for adding additional friends to the match (iOS 4.2+ only) with optional message and recipients.
		// Recipients can be any combination of email addresses and playerIds
	    public static void showFriendRequestController( string message, string[] recipients )
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer )
				_gameCenterMultiplayerShowFriendRequestControllerWithOptions( message, string.Join( ",", recipients ) );
	    }
	
	
		[DllImport("__Internal")]
	    private static extern string _gameCenterMultiplayerSendMessageToAllPeers( string gameObject, string method, string param, bool reliably );
	
		// Sends a message to all connected devices forwarding through to the specified remote gameObject and method.
	    public static string sendMessageToAllPeers( string gameObject, string method, string param, bool reliably )
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer )
				return _gameCenterMultiplayerSendMessageToAllPeers( gameObject, method, param, reliably );
			return string.Empty;
	    }
	
	
		[DllImport("__Internal")]
	    private static extern string _gameCenterMultiplayerSendMessageToPeers( string playerIds, string gameObject, string method, string param, bool reliably );
	
		// Sends a message only to the specified players forwarding through to the specified remote gameObject and method.
	    public static string sendMessageToPeers( string[] playerIds, string gameObject, string method, string param, bool reliably )
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer )
				return _gameCenterMultiplayerSendMessageToPeers( string.Join( ",", playerIds ), gameObject, method, param, reliably );
			return string.Empty;
	    }
	
	
		[DllImport("__Internal")]
		private static extern string _gameCenterMultiplayerSendRawMessageToAllPeers( byte[] param, int length, bool reliably );
	
		// Sends a raw byte array message to all connected devices
		public static string sendRawMessageToAllPeers( byte[] bytes, bool reliably )
	    {
			return sendRawMessageToAllPeers( bytes, bytes.Length, reliably );
	    }
	
	
		public static string sendRawMessageToAllPeers( byte[] bytes, int length, bool reliably )
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer )
				return _gameCenterMultiplayerSendRawMessageToAllPeers( bytes, length, reliably );
			return string.Empty;
	    }
	
	
		[DllImport("__Internal")]
		private static extern string _gameCenterMultiplayerSendRawMessageToPeers( string playerIds, byte[] param, int length, bool reliably );
	
		// Sends a raw byte array message to the specified players
		public static string sendRawMessageToPeers( string[] playerIds, byte[] bytes, bool reliably )
	    {
			return sendRawMessageToPeers( playerIds, bytes, bytes.Length, reliably );
	    }
	
	
		public static string sendRawMessageToPeers( string[] playerIds, byte[] bytes, int length, bool reliably )
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer )
				return _gameCenterMultiplayerSendRawMessageToPeers( string.Join( ",", playerIds ), bytes, length, reliably );
			return string.Empty;
	    }
	
	
		[DllImport("__Internal")]
	    private static extern string _gameCenterGetAllConnectedPlayerIds();
	
		// Gets an array of all the connected playerIds
	    public static string[] getAllConnectedPlayerIds()
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer )
			{
				string playerIdString = _gameCenterGetAllConnectedPlayerIds();
				return playerIdString.Split( new string[] { "," }, StringSplitOptions.None );
			}
	
			return new string[]{};
	    }
	
	
		[DllImport("__Internal")]
	    private static extern void _gameCenterMultiplayerDisconnectFromMatch();
	
		// Disconnects and cleans up the current match.
	    public static void disconnectFromMatch()
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer )
				_gameCenterMultiplayerDisconnectFromMatch();
	    }
	
	
		[DllImport("__Internal")]
		private static extern void _gameCenterChooseBestHostPlayer();
	
		// Chooses the best host player. Note that this must be called on all devices simultaneously and they all must be on iOS 6+ for it to do anything.
	    public static void chooseBestHostPlayer()
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer )
				_gameCenterChooseBestHostPlayer();
	    }
	
	
		#region Voice Chat
	
	    [DllImport("__Internal")]
	    private static extern bool _gameCenterMultiplayerIsVOIPAllowed();
	
		// Checks to see if the current device and network connection support VOIP.
	    public static bool isVOIPAllowed()
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer )
				return _gameCenterMultiplayerIsVOIPAllowed();
			return false;
	    }
	
	
	    [DllImport("__Internal")]
	    private static extern void _gameCenterMultiplayerEnableVoiceChat( bool isEnabled );
	
		// Enables voice chat.  This must be called before attempting to add voice chat channels.
	    public static void enableVoiceChat( bool isEnabled )
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer )
				_gameCenterMultiplayerEnableVoiceChat( isEnabled );
	    }
	
	
	    [DllImport("__Internal")]
	    private static extern void _gameCenterMultiplayerCloseAllOpenVoiceChats();
	
		// Closes all open voice chats.
	    public static void closeAllOpenVoiceChats()
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer )
				_gameCenterMultiplayerCloseAllOpenVoiceChats();
	    }
	
	
	    [DllImport("__Internal")]
	    private static extern void _gameCenterMultiplayerAddAndStartVoiceChatChannel( string channelName );
	
		// Adds a voice channel with the given name and starts it up.
	    public static void addAndStartVoiceChatChannel( string channelName )
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer )
				_gameCenterMultiplayerAddAndStartVoiceChatChannel( channelName );
	    }
	
	
	    [DllImport("__Internal")]
	    private static extern void _gameCenterMultiplayerStartVoiceChat( string channelName, bool shouldEnable );
	
		// Enables/disables the voice chat.  This is used to turn voice chat on/off.
	    public static void startVoiceChat( string channelName, bool shouldEnable )
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer )
				_gameCenterMultiplayerStartVoiceChat( channelName, shouldEnable );
	    }
	
	
	    [DllImport("__Internal")]
	    private static extern void _gameCenterMultiplayerEnableMicrophone( string channelName, bool shouldEnable );
	
		// Enables/disables the microphone for the given channel
	    public static void enableMicrophone( string channelName, bool shouldEnable )
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer )
				_gameCenterMultiplayerEnableMicrophone( channelName, shouldEnable );
	    }
	
	
	    [DllImport("__Internal")]
	    private static extern void _gameCenterMultiplayerSetVolume( string channelName, float volume );
	
		// Sets the volume for the given channel. Volume should be between 0 and 1.
	    public static void setVolume( string channelName, float volume )
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer )
				_gameCenterMultiplayerSetVolume( channelName, volume );
	    }
	
	
	    [DllImport("__Internal")]
	    private static extern void _gameCenterMultiplayerSetMute( string channelName, string playerId, bool shouldMute );
	
		// Mutes/unmutes the given playerId on the given channel.
	    public static void setMute( string channelName, string playerId, bool shouldMute )
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer )
				_gameCenterMultiplayerSetMute( channelName, playerId, shouldMute );
	    }
	
	
	    [DllImport("__Internal")]
	    private static extern void _gameCenterMultiplayerReceiveUpdates( string channelName, bool shouldReceiveUpdates );
	
		// Turns on updates for the channel.  This will fire the playerBeganSpeaking and playerStoppedSpeaking events.
	    public static void receiveUpdates( string channelName, bool shouldReceiveUpdates )
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer )
				_gameCenterMultiplayerReceiveUpdates( channelName, shouldReceiveUpdates );
	    }
	
		#endregion;
	
	
		#region Manual Matchmaking
	
	    [DllImport("__Internal")]
	    private static extern void _gameCenterMultiplayerFindMatchProgrammaticallyWithMinMaxPlayers( int minPlayers, int maxPlayers );
	
		// Finds a match programmatically with no GC UI shown.
	    public static void findMatchProgrammaticallyWithMinMaxPlayers( int minPlayers, int maxPlayers )
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer )
				_gameCenterMultiplayerFindMatchProgrammaticallyWithMinMaxPlayers( minPlayers, maxPlayers );
	    }
	
	
	    [DllImport("__Internal")]
	    private static extern void _gameCenterMultiplayerFindMatchProgrammaticallyWithFilters( int minPlayers, int maxPlayers, uint playerGroup, uint playerAttributes );
	
		// Finds a match with advanced filters programmatically with no GC UI shown.
	    public static void findMatchProgrammaticallyWithFilters( int minPlayers, int maxPlayers, uint playerGroup, uint playerAttributes )
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer )
				_gameCenterMultiplayerFindMatchProgrammaticallyWithFilters( minPlayers, maxPlayers, playerGroup, playerAttributes );
	    }
	
	
	    [DllImport("__Internal")]
	    private static extern void _gameCenterMultiplayerCancelProgrammaticMatchRequest();
	
		// Cancels the find match programatically request.
	    public static void cancelProgrammaticMatchRequest()
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer )
				_gameCenterMultiplayerCancelProgrammaticMatchRequest();
	    }
	
	
	    [DllImport("__Internal")]
	    private static extern void _gameCenterMultiplayerAddPlayersToCurrentMatch( string peerIds );
	
	    // Adds the specified players to the current match
	    public static void addPlayersToCurrentMatch( string[] playerIds )
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer )
	            _gameCenterMultiplayerAddPlayersToCurrentMatch( string.Join( ",", playerIds ) );
	    }
	
	
	
	    [DllImport("__Internal")]
	    private static extern void _gameCenterMultiplayerFindAllActivity();
	
		// Finds all the activity for your game.  Only finds programmatically started activity.
	    public static void findAllActivity()
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer )
				_gameCenterMultiplayerFindAllActivity();
	    }
	
	
	    [DllImport("__Internal")]
	    private static extern void _gameCenterMultiplayerFindAllActivityForPlayerGroup( int playerGroup );
	
		// Finds all the activity for a playerGroup for your game.  Only finds programmatically started activity.
	    public static void findAllActivityForPlayerGroup( int playerGroup )
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer )
				_gameCenterMultiplayerFindAllActivityForPlayerGroup( playerGroup );
	    }
	
		#endregion;
	
	}

}
#endif
