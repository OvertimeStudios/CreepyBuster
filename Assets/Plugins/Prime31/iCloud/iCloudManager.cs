using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Prime31;



#if UNITY_IOS || UNITY_TVOS
namespace Prime31
{
	public class iCloudManager : AbstractManager
	{
		// Fired when a change is synced from iCloud.  Includes a list of all the keys that were changed.
		public static event Action<List<object>> keyValueStoreDidChangeEvent;

		// Fired when the ubiquity identity changes. You can use iCloudBinding.getUbiquityIdentityToken to get the current token
		public static event Action ubiquityIdentityDidChangeEvent;

		// Fired when a synchronize fails and the console has a missing entitlements message in it
		public static event Action entitlementsMissingEvent;

		// Fired when the document store changes. Includes a list of all files and if they are downloaded.
		public static event Action<List<iCloudDocument>> documentStoreUpdatedEvent;


		public class iCloudDocument
		{
			public string filename;
			public bool isDownloaded;
			public DateTime contentChangedDate;
			public string downloadStatus;
			public string path;


			public iCloudDocument( Dictionary<string,object> dict )
			{
				if( dict.ContainsKey( "filename" ) )
					filename = dict["filename"].ToString();

				if( dict.ContainsKey( "isDownloaded" ) )
					isDownloaded = bool.Parse( dict["isDownloaded"].ToString() );

				var intermediate = new System.DateTime( 1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc );

				if( dict.ContainsKey( "contentChangedDate" ) )
				{
					var timeSinceEpoch = double.Parse( dict["contentChangedDate"].ToString() );
					contentChangedDate = intermediate.AddSeconds( timeSinceEpoch );
				}

				if( dict.ContainsKey( "downloadStatus" ) )
					downloadStatus = dict["downloadStatus"].ToString();

				if( dict.ContainsKey( "path" ) )
					path = dict["path"].ToString();
			}


			public static List<iCloudDocument> fromJSON( string json )
			{
				var list = json.listFromJson();
				var files = new List<iCloudDocument>( list.Count );

				foreach( Dictionary<string,object> dict in list )
					files.Add( new iCloudDocument( dict ) );

				return files;
			}


			public override string ToString()
			{
				 return string.Format( "<iCloudDocument> filename: {0}, isDownloaded: {1}, contentChangedDate: {2}", filename, isDownloaded, contentChangedDate );
			}
		}


		static iCloudManager()
		{
			AbstractManager.initialize( typeof( iCloudManager ) );
		}


		private void ubiquityIdentityDidChange( string param )
		{
			ubiquityIdentityDidChangeEvent.fire();
		}


		private void keyValueStoreDidChange( string param )
		{
			if( keyValueStoreDidChangeEvent != null )
			{
				var obj = param.listFromJson();
				keyValueStoreDidChangeEvent( obj );
			}
		}


		private void entitlementsMissing( string empty )
		{
			if( entitlementsMissingEvent != null )
				entitlementsMissingEvent();
		}


		private void documentStoreUpdated( string json )
		{
			if( documentStoreUpdatedEvent != null )
			{
				var files = iCloudDocument.fromJSON( json );
				documentStoreUpdatedEvent( files );
			}
		}

	}
}
#endif
