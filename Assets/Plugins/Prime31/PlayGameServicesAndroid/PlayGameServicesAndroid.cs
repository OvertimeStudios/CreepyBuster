using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Prime31;


#if UNITY_ANDROID

namespace Prime31
{
	public class PlayGameServices
	{
		private static AndroidJavaObject _plugin;


		static PlayGameServices()
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			using( var pluginClass = new AndroidJavaClass( "com.prime31.PlayGameServicesPlugin" ) )
				_plugin = pluginClass.CallStatic<AndroidJavaObject>( "instance" );
		}


		#region Settings, Auth and Sharing

		/// <summary>
		/// Returns null on error. Refer to Google's documentation (https:////developers.google.com/identity/sign-in/web/server-side-flow) for the details
		/// on what this does.
		/// </summary>
		public static string getGamesServerAuthCode( string serverClientId )
		{
			if( Application.platform != RuntimePlatform.Android )
				return null;

			return _plugin.Call<string>( "getGamesServerAuthCode", serverClientId );
		}


		/// <summary>
		/// Refer to Google's documentation (https:////developers.google.com/identity/sign-in/web/server-side-flow) for the details
		/// on what this does. Results in the authTokenFetchResultEvent firing.
		/// </summary>
		public static void getGamesServerAuthCode2( string serverClientId, string oauthScope = null )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			_plugin.Call( "getGamesServerAuthCode2", serverClientId, oauthScope );
		}


		/// <summary>
		/// Sets the max number of conflict resolution retries
		/// </summary>
		public static void setMaxSnapshotConflictResolveRetries( int maxRetries )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			_plugin.Call( "setMaxSnapshotConflictResolveRetries", maxRetries );
		}


		/// <summary>
		/// Enables high detail logs
		/// </summary>
		public static void enableDebugLog( bool shouldEnable )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			_plugin.Call( "enableDebugLog", shouldEnable );
		}


		/// <summary>
		/// Sets the placement of the all toasts
		/// </summary>
		public static void setToastSettings( GPGToastPlacement placement )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			_plugin.Call( "setToastSettings", (int)placement );
		}


		/// <summary>
		/// Returns null if not invitation is present or the invitationId if there was one
		/// </summary>
		public static string getLaunchInvitation()
		{
			if( Application.platform != RuntimePlatform.Android )
				return null;

			return _plugin.Call<string>( "getLaunchInvitation" );
		}


		/// <summary>
		/// This will attempt to sign in the user with no UI.
		/// </summary>
		public static void attemptSilentAuthentication()
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			_plugin.Call( "attemptSilentAuthentication" );
		}


		/// <summary>
		/// This will attempt to sign in the user in a separate, proxy Activity that differs from the main Unity Activity.
		/// </summary>
		public static void authenticateInProxyActivity()
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			_plugin.Call( "authenticateInProxyActivity" );
		}
		

		/// <summary>
		/// Starts the authentication process which will happen either in the Google+ app, Chrome or Mobile Safari
		/// </summary>
		public static void authenticate()
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			_plugin.Call( "authenticate" );
		}


		/// <summary>
		/// Logs the user out
		/// </summary>
		public static void signOut()
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			_plugin.Call( "signOut" );
		}


		/// <summary>
		/// Checks to see if there is a currently signed in user. Utilizes a terrible hack due to a bug with Play Game Services connection status.
		/// </summary>
		public static bool isSignedIn()
		{
			if( Application.platform != RuntimePlatform.Android )
				return false;

			return _plugin.Call<bool>( "isSignedIn" );
		}


		/// <summary>
		/// Gets the logged in players details
		/// </summary>
		public static GPGPlayerInfo getLocalPlayerInfo()
		{
			var player = new GPGPlayerInfo();

			if( Application.platform != RuntimePlatform.Android )
				return player;

			var json = _plugin.Call<string>( "getLocalPlayerInfo" );
			return Json.decode<GPGPlayerInfo>( json );
		}


		/// <summary>
		/// Loads player details for the given playerId. Results in the loadPlayerCompletedEvent firing when the operation completes.
		/// </summary>
		public static void loadPlayer( string playerId )
		{
			if( Application.platform == RuntimePlatform.Android )
				_plugin.Call( "loadPlayer", playerId );
		}


		/// <summary>
		/// Loads the player status. Results in the loadPlayerStatsSucceeded/FailedEvent firing.
		/// </summary>
		public static void loadPlayerStats( bool forceReload = false )
		{
			if( Application.platform == RuntimePlatform.Android )
				_plugin.Call( "loadPlayerStats", forceReload );
		}


		/// <summary>
		/// Reloads all Play Game Services related metadata
		/// </summary>
		public static void reloadAchievementAndLeaderboardData()
		{
			if( Application.platform == RuntimePlatform.Android )
				_plugin.Call( "loadBasicModelData" );
		}


		/// <summary>
		/// Loads a profile image from a Uri. Once loaded the profileImageLoadedAtPathEvent will fire.
		/// </summary>
		public static void loadProfileImageForUri( string uri )
		{
			if( Application.platform == RuntimePlatform.Android )
				_plugin.Call( "loadProfileImageForUri", uri );
		}


		/// <summary>
		/// Shows a native Google+ share dialog with optional prefilled message and optional url to share. Note that the Plus permission is required to
		/// use Google+ share dialogs!
		/// </summary>
		public static void showShareDialog( string prefillText = null, string urlToShare = null )
		{
			if( Application.platform == RuntimePlatform.Android )
				_plugin.Call( "showShareDialog", prefillText, urlToShare );
		}


		/// <summary>
		/// Shows the video capture overlay for in-game recording.
		/// </summary>
		public static void showVideoCaptureOverlay()
		{
			if( Application.platform == RuntimePlatform.Android )
				_plugin.Call( "showVideoCaptureOverlay" );
		}

		#endregion


		#region Achievements

		/// <summary>
		/// Shows the achievements screen
		/// </summary>
		public static void showAchievements()
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			_plugin.Call( "showAchievements" );
		}


		/// <summary>
		/// Reveals the achievement if it was previously hidden
		/// </summary>
		public static void revealAchievement( string achievementId )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			_plugin.Call( "revealAchievement", achievementId );
		}


		/// <summary>
		/// Unlocks the achievement. Note that showsCompletionNotification does nothing on Android.
		/// </summary>
		public static void unlockAchievement( string achievementId, bool showsCompletionNotification = true )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			_plugin.Call( "unlockAchievement", achievementId, showsCompletionNotification );
		}


		/// <summary>
		/// Increments the achievement. Only works on achievements setup as incremental in the Google Developer Console.
		/// Fires the incrementAchievementFailed/Succeeded event when complete.
		/// </summary>
		public static void incrementAchievement( string achievementId, int numSteps )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			_plugin.Call( "incrementAchievement", achievementId, numSteps );
		}


		/// <summary>
		/// Directly sets the step count for the incremental achievement. Fires the incrementAchievementFailed/Succeeded event when complete.
		/// </summary>
		public static void setAchievementSteps( string achievementId, int numStep )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			_plugin.Call( "setAchievementSteps", achievementId, numStep );
		}


		/// <summary>
		/// Gets the achievement metadata
		/// </summary>
		public static List<GPGAchievementMetadata> getAllAchievementMetadata()
		{
			if( Application.platform != RuntimePlatform.Android )
				return new List<GPGAchievementMetadata>();

			var json = _plugin.Call<string>( "getAllAchievementMetadata" );
			return Json.decode<List<GPGAchievementMetadata>>( json );
		}

		#endregion


		#region Leaderboards

		/// <summary>
		/// Shows the requested leaderboard
		/// </summary>
		public static void showLeaderboard( string leaderboardId )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			_plugin.Call( "showLeaderboard", leaderboardId );
		}


		/// <summary>
		/// Shows the requested leaderboard for the specified time scope
		/// </summary>
		public static void showLeaderboard( string leaderboardId, GPGLeaderboardTimeScope timeScope )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			_plugin.Call( "showLeaderboardWithTimeScope", leaderboardId, (int)timeScope );
		}


		/// <summary>
		/// Shows a list of all learderboards
		/// </summary>
		public static void showLeaderboards()
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			_plugin.Call( "showLeaderboards" );
		}


		/// <summary>
		/// Submits a score for the given leaderboard with optional scoreTag. Fires the submitScoreFailed/Succeeded event when complete.
		/// </summary>
		public static void submitScore( string leaderboardId, long score, string scoreTag = "" )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			_plugin.Call( "submitScore", leaderboardId, score, scoreTag );
		}


		/// <summary>
		/// Loads scores for the given leaderboard. Fires the loadScoresFailed/Succeeded event when complete.
		/// </summary>
		public static void loadScoresForLeaderboard( string leaderboardId, GPGLeaderboardTimeScope timeScope, bool isSocial, bool personalWindow )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			_plugin.Call( "loadScoresForLeaderboard", leaderboardId, (int)timeScope, isSocial, personalWindow );
		}


		/// <summary>
		/// Loads more scores for the given leaderboard. Note that loadScoresForLeaderboard must first be called before you can load more scores for a leaderboard.
		/// Fires the loadScoresFailed/Succeeded event when complete.
		/// </summary>
		public static void loadMoreScoresForLeaderboard( string leaderboardId )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			_plugin.Call( "loadMoreScoresForLeaderboard", leaderboardId );
		}


		/// <summary>
		/// Loads the current players score for the given leaderboard. Fires the loadCurrentPlayerLeaderboardScoreSucceeded/FailedEvent when complete.
		/// </summary>
		public static void loadCurrentPlayerLeaderboardScore( string leaderboardId, GPGLeaderboardTimeScope timeScope, bool isSocial )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			_plugin.Call( "loadCurrentPlayerLeaderboardScore", leaderboardId, (int)timeScope, isSocial );
		}


		/// <summary>
		/// Gets all the leaderboards metadata
		/// </summary>
		public static List<GPGLeaderboardMetadata> getAllLeaderboardMetadata()
		{
			if( Application.platform != RuntimePlatform.Android )
				return new List<GPGLeaderboardMetadata>();

			var json = _plugin.Call<string>( "getAllLeaderboardMetadata" );
			return Json.decode<List<GPGLeaderboardMetadata>>( json );
		}

		#endregion


		#region Events and Quests

		/// <summary>
		/// Sends a request to load all events for this app. The allEventsLoadedEvent is fired when the request completes.
		/// </summary>
		public static void loadAllEvents()
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			_plugin.Call( "loadAllEvents" );
		}


		/// <summary>
		/// Increments an event 1 or more steps
		/// </summary>
		public static void incrementEvent( string eventId, int steps )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			if( steps <= 0 )
				return;

			_plugin.Call( "incrementEvent", eventId, steps );
		}


		// Show the default popup for certain quest states. Popups are only supported for quest in either the STATE_ACCEPTED or STATE_COMPLETED state.
		// If the quest is in another state, no popup will be shown
		[System.Obsolete( "Google has deprecated Quests. http://bit.ly/2nPCO8d" )]
		public static void showStateChangedPopup( string questId )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			_plugin.Call( "showStateChangedPopup", questId );
		}


		// Shows the quest list with all the quests currently available to this app
		[System.Obsolete( "Google has deprecated Quests. http://bit.ly/2nPCO8d" )]
		public static void showQuestList()
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			_plugin.Call( "showQuestList" );
		}


		// Claims a completed quest milestone
		[System.Obsolete( "Google has deprecated Quests. http://bit.ly/2nPCO8d" )]
		public static void claimQuestMilestone( string questId, string questMilestoneId )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			_plugin.Call( "claimQuest", questId, questMilestoneId );
		}

		#endregion


		#region Snapshots

		/// <summary>
		/// Shows the snapshot list view. Results in one of the following events firing: snapshotListUserSelectedSnapshotEvent,
		/// snapshotListUserRequestedNewSnapshotEvent or snapshotListCanceledEvent.
		/// </summary>
		public static void showSnapshotList( int maxSavedGamesToShow, string title, bool allowAddButton, bool allowDelete )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			_plugin.Call( "showSnapshotList", maxSavedGamesToShow, title, allowAddButton, allowDelete );
		}


		/// <summary>
		/// Saves a snapshot optionally creating a new one if createIfMissing is true. Results in the saveSnapshotSucceeded/FailedEvent firing.
		/// </summary>
		public static void saveSnapshot( string snapshotName, bool createIfMissing, byte[] data, string description, GPGSnapshotConflictPolicy conflictPolicy = GPGSnapshotConflictPolicy.MostRecentlyModified, long playedTimeMilliseconds = 0, long progressValue = -1 )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			_plugin.Call( "saveSnapshot", snapshotName, createIfMissing, data, description, (int)conflictPolicy, playedTimeMilliseconds, progressValue );
		}


		/// <summary>
		/// Loads a snapshot. Results in the loadSnapshotSucceeded/FailedEvent firing.
		/// </summary>
		public static void loadSnapshot( string snapshotName )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			_plugin.Call( "loadSnapshot", snapshotName );
		}


		/// <summary>
		/// Deletes a snapshot
		/// </summary>
		public static void deleteSnapshot( string snapshotName )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			_plugin.Call( "deleteSnapshot", snapshotName );
		}

		#endregion

	}

}
#endif
