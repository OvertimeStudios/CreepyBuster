using UnityEngine;
using System.Collections;



namespace Prime31
{
	public class GPGSnapshotMetadata
	{
		public double lastModifiedTimestamp;
		public string description;
		public string name;
		public string deviceName;
		public long playedTime;
		public string title;
		public long progressValue;
	
	
		public override string ToString()
		{
			return Prime31.JsonFormatter.prettyPrint( Prime31.Json.encode( this ) );
		}
	}

}
