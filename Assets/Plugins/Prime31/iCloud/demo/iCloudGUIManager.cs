using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Prime31;



namespace Prime31
{
	public class iCloudGUIManager : MonoBehaviourGUI
	{
	#if UNITY_IOS || UNITY_TVOS
		private string _filename = "myCloudFile.txt";

		void OnGUI()
		{
			beginColumn();


			if( GUILayout.Button( "Synchronize" ) )
			{
				// Note: synchronize is called automatically for you when iCloud initializes and your app closes so in practice you will not need to call it
				var didSync = P31Prefs.synchronize();
				Debug.Log( "did synchronize: " + didSync );
			}


			if( GUILayout.Button( "Get the ubiquityIdentityToken" ) )
			{
				var token = iCloudBinding.getUbiquityIdentityToken();
				Debug.Log( "ubiquityIdentityToken: " + token );
			}


			if( GUILayout.Button( "Get Custom iCloud Container" ) )
			{
				var container = iCloudBinding.documentsDirectoryForContainerId( "iCloud.com.prime31.shared-container" );
				Debug.Log( "custom iCloud container: " + container );
			}


			if( GUILayout.Button( "Set Int 29" ) )
			{
				P31Prefs.setInt( "theInt", 29 );
				Debug.Log( "int: " + P31Prefs.getInt( "theInt" ) );
			}


			if( GUILayout.Button( "Set string 'word'" ) )
			{
				P31Prefs.setString( "theString", "word" );
				Debug.Log( "string: " + P31Prefs.getString( "theString" ) );
			}


			if( GUILayout.Button( "Set Bool" ) )
			{
				P31Prefs.setBool( "theBool", true );
				Debug.Log( "bool: " + P31Prefs.getBool( "theBool" ) );
			}


			if( GUILayout.Button( "Set Float 13.68" ) )
			{
				P31Prefs.setFloat( "theFloat", 13.68f );
				Debug.Log( "float: " + P31Prefs.getFloat( "theFloat" ) );
			}


			if( GUILayout.Button( "Set Dictionary" ) )
			{
				var dict = new Dictionary<string,object>();
				dict.Add( "aFloat", 25.5f );
				dict.Add( "aString", "dogma" );
				dict.Add( "anInt", 16 );
				P31Prefs.setDictionary( "theDict", dict );
				Prime31.Utils.logObject( P31Prefs.getDictionary( "theDict" ) );
			}


			if( GUILayout.Button( "Get All" ) )
			{
				Debug.Log( "int: " + P31Prefs.getInt( "theInt" ) );
				Debug.Log( "string: " + P31Prefs.getString( "theString" ) );
				Debug.Log( "bool: " + P31Prefs.getBool( "theBool" ) );
				Prime31.Utils.logObject( P31Prefs.getDictionary( "theDict" ) );
				Debug.Log( "float: " + P31Prefs.getFloat( "theFloat" ) );
			}


			if( GUILayout.Button( "Remove All Values" ) )
			{
				P31Prefs.removeAll();
			}


			endColumn( true );


			if( GUILayout.Button( "Is Document Store Available" ) )
			{
				Debug.Log( "Is document store available: " + P31Prefs.iCloudDocumentStoreAvailable );
			}


			if( GUILayout.Button( "Is File in iCloud?" ) )
			{
				Debug.Log( "Is file in iCloud: " + iCloudBinding.isFileInCloud( _filename ) );
			}


			if( GUILayout.Button( "Is File Downloaded?" ) )
			{
				Debug.Log( "Is file downloaded: " + iCloudBinding.isFileDownloaded( _filename ) );
			}


			if( GUILayout.Button( "Save File to iCloud" ) )
			{
				// make up a file name
				_filename = string.Format( "myCloudFile{0}.txt", Random.Range( 0, 10000 ) );

				var didSave = P31CloudFile.writeAllText( _filename, "going to write some text" );
				Debug.Log( "Did write file to iCloud: " + didSave );
			}


			if( GUILayout.Button( "Modify File" ) )
			{
				var didSave = P31CloudFile.writeAllText( _filename, "changed up the text and now its this" );
				Debug.Log( "Did write file to iCloud: " + didSave );
			}


			if( GUILayout.Button( "Get Contents of File" ) )
			{
				var lines = P31CloudFile.readAllLines( _filename );
				Debug.Log( "File contents: " + string.Join( "", lines ) );
			}


			if( GUILayout.Button( "Evict File" ) )
			{
				iCloudBinding.evictFile( _filename );
			}

			endColumn();
		}

#endif
	}

}
