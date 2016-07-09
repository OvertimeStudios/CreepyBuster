using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using Prime31;



#if UNITY_IOS || UNITY_TVOS

namespace Prime31
{
	public class GameCenterManager : AbstractManager
	{
		// Player events
		// Fired when retrieving player data (friends) fails
		public static event Action<string> loadPlayerDataFailedEvent;
	
		// Fired when player data is loaded after requesting friends
		public static event Action<List<GameCenterPlayer>> playerDataLoadedEvent;
	
		// Fired when a player is logged in
		public static event Action playerAuthenticatedEvent;
	
		// Fired when a player fails to login
		public static event Action<string> playerFailedToAuthenticateEvent;
	
		// Fired when the login view controller is available to show. Call authenticateLocalPlayer again to display it.
		public static event Action playerAuthenticationRequiredEvent;
		
		// Fired when a call to authenticate neither succeeds nor fails. This is how Apple conveys that Game Center is disabled.
		public static event Action gameCenterDisabledEvent;
	
		// Fired when a player logs out
		public static event Action playerLoggedOutEvent;
	
		// Fired when the profile image is loaded for the player and includes the full path to the image
		public static event Action<string> profilePhotoLoadedEvent;
	
		// Fired when the profile image fails to load
		public static event Action<string> profilePhotoFailedEvent;
	
		// Fired when a call to generateIdentityVerificationSignature succeeds. Includes the publicKeyUrl, timestamp, signature and salt (base64 encoded).
		public static event Action<Dictionary<string,string>> generateIdentityVerificationSignatureSucceededEvent;
	
		// Fired when a call to generateIdentityVerificationSignature fails.
		public static event Action<string> generateIdentityVerificationSignatureFailedEvent;
	
	
		// Leaderboard events
		// Fired when loading leaderboard category data fails
		public static event Action<string> loadCategoryTitlesFailedEvent;
	
		// Fired when loading leaderboard category data completes
		public static event Action<List<GameCenterLeaderboard>> categoriesLoadedEvent;
	
		// Fired when reporting a score fails
		public static event Action<string> reportScoreFailedEvent;
	
		// Fired when reporting a score finishes successfully
		public static event Action<string> reportScoreFinishedEvent;
	
		// Fired when retrieving scores fails
		public static event Action<string> retrieveScoresFailedEvent;
	
		// Fired when retrieving scores completes successfully
		public static event Action<GameCenterRetrieveScoresResult> scoresLoadedEvent;
	
		// Fired when retrieving scores for a playerId fails
		public static event Action<string> retrieveScoresForPlayerIdFailedEvent;
	
		// Fired when retrieving scores for a playerId completes successfully
		public static event Action<GameCenterRetrieveScoresResult> scoresForPlayerIdLoadedEvent;
	
		// Achievement events
		// Fired when reporting an achievement fails
		public static event Action<string> reportAchievementFailedEvent;
	
		// Fired when reporting an achievement completes successfully
		public static event Action<string> reportAchievementFinishedEvent;
	
		// Fired when loading achievements fails
		public static event Action<string> loadAchievementsFailedEvent;
	
		// Fired when loading achievements completes successfully
		public static event Action<List<GameCenterAchievement>> achievementsLoadedEvent;
	
		// Fired when resetting achievements fails
		public static event Action<string> resetAchievementsFailedEvent;
	
		// Fired when resetting achievements completes successfully
		public static event Action resetAchievementsFinishedEvent;
	
		// Fired when loading achievement metadata fails
		public static event Action<string> retrieveAchievementMetadataFailedEvent;
	
		// Fired when loading achievement metadata completes successfully
		public static event Action<List<GameCenterAchievementMetadata>> achievementMetadataLoadedEvent;
	
	
		// Challenge events
		// Fired when a call to selectChallengeablePlayerIDsForAchievement fails
		public static event Action<string> selectChallengeablePlayerIDsDidFailEvent;
	
		// Fired when a call to selectChallengeablePlayerIDsForAchievement completes
		public static event Action<List<object>> selectChallengeablePlayerIDsDidFinishEvent;
		
		// Fired when a player has received a challenge, triggered by a push notification from the server
		public static event Action<GameCenterChallenge> localPlayerDidReceiveChallengeEvent;
	
		// Fired when the user taps a challenge notification banner or the "Play Now" button for a challenge inside Game Center
		public static event Action<GameCenterChallenge> localPlayerDidSelectChallengeEvent;
	
		// Fired when the local player has completed one of their challenges, triggered by a push notification from the server
		public static event Action<GameCenterChallenge> localPlayerDidCompleteChallengeEvent;
	
		// Fired when a non-local player has completed a challenge issued by the local player. Triggered by a push notification from the server.
		public static event Action<GameCenterChallenge> remotePlayerDidCompleteChallengeEvent;
	
		// Fired when challenges load
		public static event Action<List<GameCenterChallenge>> challengesLoadedEvent;
	
		// Fired when challenges fail to laod
		public static event Action<string> challengesFailedToLoadEvent;
	
		// iOS 7+. Fired when a challenge is successfully sent. Includes an array of all the playerIds that the challenge was sent to.
		public static event Action<List<object>> challengeIssuedSuccessfullyEvent;
	
		// iOS 7+. Fired when the challenge composer completes and the user did not send the challenge to anyone.
		public static event Action challengeNotIssuedEvent;
	
	
	
	
	    static GameCenterManager()
	    {
			AbstractManager.initialize( typeof( GameCenterManager ) );
	    }
	
	
		#region Player callbacks
	
		void loadPlayerDataDidFail( string error )
		{
			if( loadPlayerDataFailedEvent != null )
				loadPlayerDataFailedEvent( error );
		}
	
	
		void loadPlayerDataDidLoad( string jsonFriendList )
		{
			List<GameCenterPlayer> list = GameCenterPlayer.fromJSON( jsonFriendList );
	
			if( playerDataLoadedEvent != null )
				playerDataLoadedEvent( list );
		}
	
	
		void playerDidLogOut()
		{
			if( playerLoggedOutEvent != null )
				playerLoggedOutEvent();
		}
	
	
		void playerDidAuthenticate( string playerId )
		{
			if( playerAuthenticatedEvent != null )
				playerAuthenticatedEvent();
		}
	
	
		void playerAuthenticationFailed( string error )
		{
			if( playerFailedToAuthenticateEvent != null )
				playerFailedToAuthenticateEvent( error );
		}
		
		
		void playerAuthenticationRequired( string empty )
		{
			if( playerAuthenticationRequiredEvent != null )
				playerAuthenticationRequiredEvent();
		}
		
		
		void gameCenterDisabled( string empty )
		{
			if( gameCenterDisabledEvent != null )
				gameCenterDisabledEvent();
		}
	
	
		void loadProfilePhotoDidLoad( string path )
		{
			if( profilePhotoLoadedEvent != null )
				profilePhotoLoadedEvent( path );
		}
	
	
		void loadProfilePhotoDidFail( string error )
		{
			if( profilePhotoFailedEvent != null )
				profilePhotoFailedEvent( error );
		}
	
	
		void generateIdentityVerificationSignatureSucceeded( string json )
		{
			if( generateIdentityVerificationSignatureSucceededEvent != null )
				generateIdentityVerificationSignatureSucceededEvent( Json.decode<Dictionary<string,string>>( json ) );
		}
	
	
		void generateIdentityVerificationSignatureFailed( string error )
		{
			if( generateIdentityVerificationSignatureFailedEvent != null )
				generateIdentityVerificationSignatureFailedEvent( error );
		}
	
		#endregion;
	
	
		#region Leaderboard callbacks
	
		void loadCategoryTitlesDidFail( string error )
		{
			if( loadCategoryTitlesFailedEvent != null )
				loadCategoryTitlesFailedEvent( error );
		}
	
	
		void categoriesDidLoad( string jsonCategoryList )
		{
			List<GameCenterLeaderboard> list = GameCenterLeaderboard.fromJSON( jsonCategoryList );
	
			if( categoriesLoadedEvent != null )
				categoriesLoadedEvent( list );
		}
	
	
		void reportScoreDidFail( string error )
		{
			if( reportScoreFailedEvent != null )
				reportScoreFailedEvent( error );
		}
	
	
		void reportScoreDidFinish( string category )
		{
			if( reportScoreFinishedEvent != null )
				reportScoreFinishedEvent( category );
		}
	
	
		void retrieveScoresDidFail( string category )
		{
			if( retrieveScoresFailedEvent != null )
				retrieveScoresFailedEvent( category );
		}
	
	
		void retrieveScoresDidLoad( string json )
		{
			if( scoresLoadedEvent != null )
				scoresLoadedEvent( Json.decode<GameCenterRetrieveScoresResult>( json ) );
		}
	
	
		void retrieveScoresForPlayerIdDidFail( string error )
		{
			if( retrieveScoresForPlayerIdFailedEvent != null )
				retrieveScoresForPlayerIdFailedEvent( error );
		}
	
	
		void retrieveScoresForPlayerIdDidLoad( string json )
		{
			if( scoresForPlayerIdLoadedEvent != null )
				scoresForPlayerIdLoadedEvent( Json.decode<GameCenterRetrieveScoresResult>( json ) );
		}
	
		#endregion;
	
	
		#region Achievements
	
		void reportAchievementDidFail( string error )
		{
			if( reportAchievementFailedEvent != null )
				reportAchievementFailedEvent( error );
		}
	
	
		void reportAchievementDidFinish( string identifier )
		{
			if( reportAchievementFinishedEvent != null )
				reportAchievementFinishedEvent( identifier );
		}
	
	
		void loadAchievementsDidFail( string error )
		{
			if( loadAchievementsFailedEvent != null )
				loadAchievementsFailedEvent( error );
		}
	
	
		void achievementsDidLoad( string jsonAchievmentList )
		{
			List<GameCenterAchievement> list = GameCenterAchievement.fromJSON( jsonAchievmentList );
	
			if( achievementsLoadedEvent != null )
				achievementsLoadedEvent( list );
		}
	
	
		void resetAchievementsDidFail( string error )
		{
			if( resetAchievementsFailedEvent != null )
				resetAchievementsFailedEvent( error );
		}
	
	
		void resetAchievementsDidFinish( string emptyString )
		{
			if( resetAchievementsFinishedEvent != null )
				resetAchievementsFinishedEvent();
		}
	
	
		void retrieveAchievementsMetaDataDidFail( string error )
		{
			if( retrieveAchievementMetadataFailedEvent != null )
				retrieveAchievementMetadataFailedEvent( error );
		}
	
	
		void achievementMetadataDidLoad( string jsonAchievementDescriptionList )
		{
			List<GameCenterAchievementMetadata> list = GameCenterAchievementMetadata.fromJSON( jsonAchievementDescriptionList );
	
			if( achievementMetadataLoadedEvent != null )
				achievementMetadataLoadedEvent( list );
		}
	
		#endregion;
	
	
		#region Challenges
	
		void selectChallengeablePlayerIDsDidFail( string error )
		{
			if( selectChallengeablePlayerIDsDidFailEvent != null )
				selectChallengeablePlayerIDsDidFailEvent( error );
		}
	
	
		void selectChallengeablePlayerIDsDidFinish( string json )
		{
			if( selectChallengeablePlayerIDsDidFinishEvent != null )
				selectChallengeablePlayerIDsDidFinishEvent( json.listFromJson() );
		}
		
		
		void localPlayerDidReceiveChallenge( string json )
		{
			if( localPlayerDidReceiveChallengeEvent != null )
				localPlayerDidReceiveChallengeEvent( new GameCenterChallenge( json.dictionaryFromJson() ) );
		}
	
	
		void localPlayerDidSelectChallenge( string json )
		{
			if( localPlayerDidSelectChallengeEvent != null )
				localPlayerDidSelectChallengeEvent( new GameCenterChallenge( json.dictionaryFromJson() ) );
		}
	
	
		void localPlayerDidCompleteChallenge( string json )
		{
			if( localPlayerDidCompleteChallengeEvent != null )
				localPlayerDidCompleteChallengeEvent( new GameCenterChallenge( json.dictionaryFromJson() ) );
		}
	
	
		void remotePlayerDidCompleteChallenge( string json )
		{
			if( remotePlayerDidCompleteChallengeEvent != null )
				remotePlayerDidCompleteChallengeEvent( new GameCenterChallenge( json.dictionaryFromJson() ) );
		}
	
	
		void challengesLoaded( string json )
		{
			if( challengesLoadedEvent != null )
				challengesLoadedEvent( GameCenterChallenge.fromJson( json ) );
		}
	
	
		void challengesFailedToLoad( string error )
		{
			if( challengesFailedToLoadEvent != null )
				challengesFailedToLoadEvent( error );
		}
	
	
		void challengeIssuedSuccessfully( string json )
		{
			if( challengeIssuedSuccessfullyEvent != null )
				challengeIssuedSuccessfullyEvent( json.listFromJson() );
		}
	
	
		void challengeNotIssued( string empty )
		{
			if( challengeNotIssuedEvent != null )
				challengeNotIssuedEvent();
		}
	
		#endregion
	}

}
#endif
