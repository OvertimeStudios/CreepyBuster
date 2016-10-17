using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;


#if UNITY_IOS || UNITY_TVOS
public enum GameCenterLeaderboardTimeScope
{
	Today = 0,
	Week,
	AllTime
};

public enum GameCenterViewControllerState
{
	Default = -1,
	Leaderboards,
	Achievements,
	Challenges
}


namespace Prime31
{
	public class GameCenterBinding
	{
		#region Player and General methods

		[DllImport("__Internal")]
	    private static extern void _gameCenterAuthenticateLocalPlayer( bool shouldShowLoginScreenIfNotAuthenticated );

		// Authenticates the player. This needs to be called before using anything in GameCenter and should
		// preferalbly be called shortly after application launch. If shouldShowLoginScreenIfNotAuthenticated is set to false,
		// the playerAuthenticationRequiredEvent will fire when the login view is available to show. Call authenticateLocalPlayer
		// at that time to show it.
	    public static void authenticateLocalPlayer( bool shouldShowLoginScreenIfNotAuthenticated = true )
	    {
			if( Application.platform == RuntimePlatform.IPhonePlayer || (int)Application.platform == 31 )
				_gameCenterAuthenticateLocalPlayer( shouldShowLoginScreenIfNotAuthenticated );
	    }


		[DllImport("__Internal")]
	    private static extern bool _gameCenterIsPlayerAuthenticated();

		// Checks to see if the current player is authenticated.
		public static bool isPlayerAuthenticated()
	    {
			if( Application.platform == RuntimePlatform.IPhonePlayer || (int)Application.platform == 31 )
				return _gameCenterIsPlayerAuthenticated();
			return false;
	    }


		[DllImport("__Internal")]
	    private static extern string _gameCenterPlayerAlias();

		// Gets the alias of the current player.
	    public static string playerAlias()
	    {
			if( Application.platform == RuntimePlatform.IPhonePlayer || (int)Application.platform == 31 )
				return _gameCenterPlayerAlias();
			return string.Empty;
	    }


		[DllImport("__Internal")]
	    private static extern string _gameCenterPlayerIdentifier();

		// Gets the playerIdentifier of the current player.
	    public static string playerIdentifier()
	    {
			if( Application.platform == RuntimePlatform.IPhonePlayer || (int)Application.platform == 31 )
				return _gameCenterPlayerIdentifier();
			return string.Empty;
	    }


		[DllImport("__Internal")]
	    private static extern bool _gameCenterIsUnderage();

		// Checks to see if the current player is underage.
	    public static bool isUnderage()
	    {
			if( Application.platform == RuntimePlatform.IPhonePlayer || (int)Application.platform == 31 )
				return _gameCenterIsUnderage();
			return false;
	    }


		[DllImport("__Internal")]
	    private static extern void _gameCenterRetrieveFriends( bool loadProfileImages, bool loadLargeProfileImages );

		// Sends off a request to get the current users friend list and optionally loads profile images asynchronously
	    public static void retrieveFriends( bool loadProfileImages, bool loadLargeProfileImages = true )
	    {
			if( Application.platform == RuntimePlatform.IPhonePlayer || (int)Application.platform == 31 )
				_gameCenterRetrieveFriends( loadProfileImages, loadLargeProfileImages );
	    }


		[DllImport("__Internal")]
		private static extern void _gameCenterRetrieveRecentPlayers( bool loadProfileImages, bool loadLargeProfileImages );

		// iOS 10+ only! Sends off a request to get any recent players (the closest thing to friends in iOS 10) and optionally loads profile images asynchronously
		public static void retrieveRecentPlayers( bool loadProfileImages, bool loadLargeProfileImages = true )
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer || (int)Application.platform == 31 )
				_gameCenterRetrieveRecentPlayers( loadProfileImages, loadLargeProfileImages );
		}


		[DllImport("__Internal")]
	    private static extern void _gameCenterLoadPlayerData( string playerIds, bool loadProfileImages, bool loadLargeProfileImages );

		// Gets GameCenterPlayer objects for all the given playerIds and optionally loads the profile images asynchronously
	    public static void loadPlayerData( string[] playerIdArray, bool loadProfileImages, bool loadLargeProfileImages = true )
	    {
			if( Application.platform == RuntimePlatform.IPhonePlayer || (int)Application.platform == 31 )
				_gameCenterLoadPlayerData( string.Join( ",", playerIdArray ), loadProfileImages, loadLargeProfileImages );
	    }


		[DllImport("__Internal")]
	    private static extern void _gameCenterLoadProfilePhotoForLocalPlayer();

		// Starts the loading of the profile image for the currently logged in player
	    public static void loadProfilePhotoForLocalPlayer()
	    {
			if( Application.platform == RuntimePlatform.IPhonePlayer || (int)Application.platform == 31 )
				_gameCenterLoadProfilePhotoForLocalPlayer();
	    }


		[DllImport("__Internal")]
		private static extern void _gameCenterShowGameCenterViewController( int viewState, int timeScope, string leaderboardId );

		// Shows a specific Game Center view controller. timeScope and leaderboardId are only valid for GameCenterViewControllerState.Leaderboards
		public static void showGameCenterViewController( GameCenterViewControllerState viewState )
		{
			showGameCenterViewController( viewState, GameCenterLeaderboardTimeScope.AllTime, null );
		}

	    public static void showGameCenterViewController( GameCenterViewControllerState viewState, GameCenterLeaderboardTimeScope timeScope, string leaderboardId )
	    {
			if( Application.platform == RuntimePlatform.IPhonePlayer || (int)Application.platform == 31 )
				_gameCenterShowGameCenterViewController( (int)viewState, (int)timeScope, leaderboardId );
	    }



		[DllImport("__Internal")]
		private static extern void _gameCenterGenerateIdentityVerificationSignature();

		// Generates a signature that allows a third party server to authenticate the local player. Results in the generateIdentityVerificationSignatureSucceeded/FailedEvent firing
		public static void generateIdentityVerificationSignature()
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer || (int)Application.platform == 31 )
				_gameCenterGenerateIdentityVerificationSignature();
		}


		[DllImport("__Internal")]
		private static extern void _gameCenterShowCustomNotificationBanner( string title, string message, float duraction );

		// Shows a custom notification banner
		public static void showCustomNotificationBanner( string title, string message, float duraction )
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer || (int)Application.platform == 31 )
				_gameCenterShowCustomNotificationBanner( title, message, duraction );
		}

		#endregion;


		#region Leaderboard methods

		[DllImport("__Internal")]
	    private static extern void _gameCenterLoadLeaderboardLeaderboardTitles();

		// Sends off a request to get all the currently live leaderboards including leaderboardId and title.
	    public static void loadLeaderboardTitles()
	    {
			if( Application.platform == RuntimePlatform.IPhonePlayer || (int)Application.platform == 31 )
				_gameCenterLoadLeaderboardLeaderboardTitles();
	    }


		[DllImport("__Internal")]
	    private static extern void _gameCenterReportScore( System.Int64 score, System.UInt64 context, string leaderboardId );

		// Reports a score for the given leaderboardId.
	    public static void reportScore( System.Int64 score, string leaderboardId )
	    {
			if( Application.platform == RuntimePlatform.IPhonePlayer || (int)Application.platform == 31 )
				_gameCenterReportScore( score, 0L, leaderboardId );
	    }

		public static void reportScore( System.Int64 score, System.UInt64 context, string leaderboardId )
	    {
			if( Application.platform == RuntimePlatform.IPhonePlayer || (int)Application.platform == 31 )
				_gameCenterReportScore( score, context, leaderboardId );
	    }


		[DllImport("__Internal")]
	    private static extern void _gameCenterShowLeaderboardWithTimeScope( int timeScope );

		// Shows the standard GameCenter leaderboard with the given time scope.
	    public static void showLeaderboardWithTimeScope( GameCenterLeaderboardTimeScope timeScope )
	    {
			if( Application.platform == RuntimePlatform.IPhonePlayer || (int)Application.platform == 31 )
				_gameCenterShowLeaderboardWithTimeScope( (int)timeScope );
	    }


		[DllImport("__Internal")]
	    private static extern void _gameCenterShowLeaderboardWithTimeScopeAndLeaderboardId( int timeScope, string leaderboardId );

		// Shows the standard GameCenter leaderboard for the given leaderboardId with the given time scope.
	    public static void showLeaderboardWithTimeScopeAndLeaderboard( GameCenterLeaderboardTimeScope timeScope, string leaderboardId )
	    {
			if( Application.platform == RuntimePlatform.IPhonePlayer || (int)Application.platform == 31 )
				_gameCenterShowLeaderboardWithTimeScopeAndLeaderboardId( (int)timeScope, leaderboardId );
	    }


		[DllImport("__Internal")]
	    private static extern void _gameCenterRetrieveScores( bool friendsOnly, int timeScope, int start, int end );

		// Sends a request to get the current scores with the given criteria. End MUST be between 1 and 100 inclusive.
		// Results in the retrieveScoresFailedEvent/scoresLoadedEvent firing and if the local players scores gets loaded scoresForPlayerIdLoadedEvent will fire.
	    public static void retrieveScores( bool friendsOnly, GameCenterLeaderboardTimeScope timeScope, int start, int end )
	    {
			if( Application.platform == RuntimePlatform.IPhonePlayer || (int)Application.platform == 31 )
				_gameCenterRetrieveScores( friendsOnly, (int)timeScope, start, end );
	    }


		[DllImport("__Internal")]
	    private static extern void _gameCenterRetrieveScoresForLeaderboard( bool friendsOnly, int timeScope, int start, int end, string leaderboardId );

		// Sends a request to get the current scores with the given criteria. End MUST be between 1 and 100 inclusive.
		// Results in the retrieveScoresFailedEvent/scoresLoadedEvent firing and if the local players scores gets loaded scoresForPlayerIdLoadedEvent will fire.
	    public static void retrieveScores( bool friendsOnly, GameCenterLeaderboardTimeScope timeScope, int start, int end, string leaderboardId )
	    {
			if( Application.platform == RuntimePlatform.IPhonePlayer || (int)Application.platform == 31 )
				_gameCenterRetrieveScoresForLeaderboard( friendsOnly, (int)timeScope, start, end, leaderboardId );
	    }


		[System.Obsolete( "Use retrieveScoresForPlayerIds" )]
	    public static void retrieveScoresForPlayerId( string playerId )
	    {
			retrieveScoresForPlayerIds( new string[] { playerId }, null );
	    }


		[System.Obsolete( "Use retrieveScoresForPlayerIds" )]
	    public static void retrieveScoresForPlayerId( string playerId, string leaderboardId )
	    {
			retrieveScoresForPlayerIds( new string[] { playerId }, leaderboardId );
	    }


		[DllImport("__Internal")]
	    private static extern void _gameCenterRetrieveScoresForPlayerIds( string playerIds, string leaderboardId, bool friendsOnly );

		// Sends a request to get the current scores for the given playerIds and leaderboardId. scoresForPlayerIdsLoadedEvent/retrieveScoresForPlayerIdsFailedEvent will fire with the results.
	    public static void retrieveScoresForPlayerIds( string[] playerIdArray, string leaderboardId, bool friendsOnly = false )
	    {
			if( Application.platform == RuntimePlatform.IPhonePlayer || (int)Application.platform == 31 )
				_gameCenterRetrieveScoresForPlayerIds( string.Join( ",", playerIdArray ), leaderboardId, friendsOnly );
	    }

		#endregion;


		#region Achievement methods

		[DllImport("__Internal")]
	    private static extern void _gameCenterReportAchievement( string identifier, float percent );

		// Reports an achievement with the given identifier and percent complete
	    public static void reportAchievement( string identifier, float percent )
	    {
			if( Application.platform == RuntimePlatform.IPhonePlayer || (int)Application.platform == 31 )
				_gameCenterReportAchievement( identifier, percent );
	    }


		[DllImport("__Internal")]
	    private static extern void _gameCenterGetAchievements();

		// Sends a request to get a list of all the current achievements for the authenticated player.
	    public static void getAchievements()
	    {
			if( Application.platform == RuntimePlatform.IPhonePlayer || (int)Application.platform == 31 )
				_gameCenterGetAchievements();
	    }


		[DllImport("__Internal")]
	    private static extern void _gameCenterResetAchievements();

		// Resets all the achievements for the authenticated player.
	    public static void resetAchievements()
	    {
			if( Application.platform == RuntimePlatform.IPhonePlayer || (int)Application.platform == 31 )
				_gameCenterResetAchievements();
	    }


		[DllImport("__Internal")]
	    private static extern void _gameCenterShowAchievements();

		// Shows the standard, GameCenter achievement list
	    public static void showAchievements()
	    {
			if( Application.platform == RuntimePlatform.IPhonePlayer || (int)Application.platform == 31 )
				_gameCenterShowAchievements();
		}


		[DllImport("__Internal")]
	    private static extern void _gameCenterRetrieveAchievementMetadata();

		// Sends a request to get the achievements for the current game.
	    public static void retrieveAchievementMetadata()
	    {
			if( Application.platform == RuntimePlatform.IPhonePlayer || (int)Application.platform == 31 )
				_gameCenterRetrieveAchievementMetadata();
	    }


		[DllImport("__Internal")]
		private static extern void _gameCenterShowCompletionBannerForAchievements();

		// Shows a completion banner for achievements if when reported they are at 100%
	    public static void showCompletionBannerForAchievements()
	    {
			if( Application.platform == RuntimePlatform.IPhonePlayer || (int)Application.platform == 31 )
				_gameCenterShowCompletionBannerForAchievements();
	    }

		#endregion;


		#region Challenge methods

		[DllImport("__Internal")]
		private static extern void _gameCenterConfigureChallengeBanners( bool showBannerForLocallyCompletedChallenge, bool showBannerForLocallyReceivedChallenge, bool showBannerForRemotelyCompletedChallenge );

		// Configures when challenge banners will be shown
		public static void configureChallengeBanners( bool showBannerForLocallyCompletedChallenge, bool showBannerForLocallyReceivedChallenge, bool showBannerForRemotelyCompletedChallenge )
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer || (int)Application.platform == 31 )
				_gameCenterConfigureChallengeBanners( showBannerForLocallyCompletedChallenge, showBannerForLocallyReceivedChallenge, showBannerForRemotelyCompletedChallenge );
		}


		[DllImport("__Internal")]
		private static extern void _gameCenterLoadReceivedChallenges();

		// Sends a request to load all received challenges
	    public static void loadReceivedChallenges()
	    {
			if( Application.platform == RuntimePlatform.IPhonePlayer || (int)Application.platform == 31 )
				_gameCenterLoadReceivedChallenges();
	    }


		[DllImport("__Internal")]
		private static extern void _gameCenterIssueScoreChallenge( System.Int64 score, System.Int64 context, string leaderboardId, string playerIds, string message );

		// Issues a score challenge to the given players for the leaderboard
	    public static void issueScoreChallenge( System.Int64 score, System.Int64 context, string leaderboardId, string[] playerIds, string message )
	    {
			if( Application.platform == RuntimePlatform.IPhonePlayer || (int)Application.platform == 31 )
				_gameCenterIssueScoreChallenge( score, context, leaderboardId, string.Join( ",", playerIds ), message );
	    }


		[DllImport("__Internal")]
		private static extern void _gameCenterSelectChallengeablePlayerIDsForAchievement( string identifier, string playerIds );

		// Checks the given playerIds to see if any are eligible for the achievement challenge
	    public static void selectChallengeablePlayerIDsForAchievement( string identifier, string[] playerIds)
	    {
			if( Application.platform == RuntimePlatform.IPhonePlayer || (int)Application.platform == 31 )
				_gameCenterSelectChallengeablePlayerIDsForAchievement( identifier, string.Join( ",", playerIds ) );
	    }


		[DllImport("__Internal")]
		private static extern void _gameCenterIssueAchievementChallenge( string identifier, string playerIds, string message );

		// Issues an achievement challenge to the players for the given identifier
	    public static void issueAchievementChallenge( string identifier, string[] playerIds, string message )
	    {
			if( Application.platform == RuntimePlatform.IPhonePlayer || (int)Application.platform == 31 )
				_gameCenterIssueAchievementChallenge( identifier, string.Join( ",", playerIds ), message );
	    }

		#endregion

	}

}
#endif
