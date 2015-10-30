using UnityEngine;
using System;
using System.Data;
using System.Collections;
using MySql.Data;
using MySql.Data.MySqlClient;

public class DBHandler : MonoBehaviour 
{
	#if DB_IMPLEMENTED
	public string server;
	public string database;
	public string userID;
	public string password;
	public bool pooling;

	private static string source;
	private static MySqlConnection connection;

	private static DBUser dbUser;

	#region get/set
	public static bool IsConnected
	{
		get {	return connection != null && connection.State.Equals(ConnectionState.Open);	}
	}

	public static DBUser User
	{
		get { return dbUser; }
	}
	#endregion
	
	#region singleton
	private static DBHandler instance;
	public static DBHandler Instance
	{
		get 
		{
			if(instance == null)
				instance = GameObject.FindObjectOfType<DBHandler>();
			
			return instance;
		}
	}
	#endregion

	// Use this for initialization
	void Start () 
	{
		source = 	"Server = " + server + ";" +
					"Database = " + database + ";" +
					"User ID = " + userID + ";" + 
					"Password = " + password + ";" +
					"Pooling = " + pooling.ToString() + ";";
	}
	
	
	public static void EnsureConnection()
	{
		if (!IsConnected)
			Connect ();
	}
	
	public static void Connect()
	{
		DBHandler.Instance.ConnectDB ();
	}
	
	private void ConnectDB()
	{
		Debug.Log(source);
		try
		{
			connection = new MySqlConnection (source);
			Debug.Log("Connection: " + connection);
			Debug.Log("connection.ConnectionString: " + connection.ConnectionString);
			Debug.Log("Connection State: " + connection.State);
			Debug.Log("connection.Ping(): " + connection.Ping());
			connection.Open ();
			//Debug.Log("Connection State: " + connection.State);
		}
		catch(Exception e)
		{
			Debug.LogError("ConnectDB() Error: " + e.ToString());
		}
	}
	
	void OnApplicationQuit()
	{
		Debug.Log("killing connection"); 
		if (connection != null)
		{
			if (!connection.State.Equals(ConnectionState.Closed)) 
				connection.Close();
			
			connection.Dispose(); 
		}
	}
	
	public static DBUser GetUser(string facebookID)
	{
		Debug.Log("Getting User");
		DBUser user = null;

		EnsureConnection ();
		
		try
		{
			string query = "GetUser";

			MySqlCommand command = new MySqlCommand(query, connection);
			command.CommandType = CommandType.StoredProcedure;
			command.Parameters.AddWithValue("@$facebookID", facebookID);

			MySqlDataReader rdr = command.ExecuteReader();
			while (rdr.Read())
			{
				user = new DBUser();
				user.id = int.Parse(rdr[0].ToString());
				user.firstname = rdr[1].ToString();
				user.lastname = rdr[2].ToString();
				user.email = rdr[3].ToString();
				user.gender = rdr[4].ToString();
			}
			rdr.Close();
		}
		
		catch (Exception e)
		{
			Debug.LogError(e.ToString());
		}

		dbUser = user;

		return user;
	}
	
	public static DBUser CreateUser(string facebookID, string firstName, string lastName, string email, string gender)
	{
		Debug.Log("No user found... Creating new user");
		
		EnsureConnection ();
		
		DBUser user = new DBUser ();
		try
		{
			string query = "CreateUser";
			MySqlCommand command = new MySqlCommand(query, connection);
			command.CommandType = CommandType.StoredProcedure;
			command.Parameters.AddWithValue("@$firstname", firstName);
			command.Parameters.AddWithValue("@$lastname", lastName);
			command.Parameters.AddWithValue("@$email", email);
			command.Parameters.AddWithValue("@$gender", gender);
			command.Parameters.AddWithValue("@$facebookID", facebookID);
			
			bool success = command.ExecuteNonQuery() == 1;
			
			if(success)
			{
				Debug.Log("Created User Successfully");
				return GetUser(facebookID);
			}
		}
		catch(Exception e)
		{
			Debug.LogError(e.ToString());
		}

		return user;
	}

	public static int GetGameID(string gameName)
	{
		int gameID = -1;

		Debug.Log(string.Format("GetGameID({0})", gameName));
		EnsureConnection ();

		Debug.Log("connection: " + connection);

		try
		{
			string query = "GetGameID";
			
			MySqlCommand command = new MySqlCommand(query, connection);
			command.CommandType = CommandType.StoredProcedure;
			command.Parameters.AddWithValue("@$gameName", gameName);

			MySqlDataReader rdr = command.ExecuteReader();
			while (rdr.Read())
			{
				gameID = int.Parse(rdr[0].ToString());
			}
			rdr.Close();
		}
		
		catch (Exception e)
		{
			Debug.LogError("GetGameID() Error: " + e.ToString());
		}
		
		return gameID;
	}

	public static int GetUserRanking(int userID, int gameID)
	{
		Debug.Log("Getting user ranking");

		int position = -1;

		EnsureConnection ();
		
		try
		{
			string query = "GetUserRankingPosition";
			MySqlCommand command = new MySqlCommand(query, connection);
			command.CommandType = CommandType.StoredProcedure;
			command.Parameters.AddWithValue("@$userID", userID);
			command.Parameters.AddWithValue("@$gameID", gameID);
			
			MySqlDataReader rdr = command.ExecuteReader();
			while (rdr.Read())
			{
				position = int.Parse(rdr[0].ToString());
			}
			rdr.Close();
		}
		catch(Exception e)
		{
			Debug.LogError(e.ToString());
		}

		return position;
	}

	public static void CreateUserScore(int userID, int gameID)
	{
		Debug.Log("Creating user score");
		
		EnsureConnection ();

		try
		{
			string query = "CreateUserScore";
			MySqlCommand command = new MySqlCommand(query, connection);
			command.CommandType = CommandType.StoredProcedure;
			command.Parameters.AddWithValue("@$userID", userID);
			command.Parameters.AddWithValue("@$gameID", gameID);
			
			bool success = command.ExecuteNonQuery() == 1;
			
			if(success)
			{
				Debug.Log("Created User Score Successfully");
			}
		}
		catch(Exception e)
		{
			Debug.LogError(e.ToString());
		}
	}

	public static void UpdateUserScore(int userID, int gameID, float score)
	{
		Debug.Log("Updating user score");
		
		EnsureConnection ();
		
		try
		{
			string query = "UpdateUserScore";
			MySqlCommand command = new MySqlCommand(query, connection);
			command.CommandType = CommandType.StoredProcedure;
			command.Parameters.AddWithValue("@$userID", userID);
			command.Parameters.AddWithValue("@$gameID", gameID);
			command.Parameters.AddWithValue("@$score", score);
			
			bool success = command.ExecuteNonQuery() == 1;
			
			if(success)
			{
				Debug.Log("Updated User Score Successfully");
			}
		}
		catch(Exception e)
		{
			Debug.LogError(e.ToString());
		}
	}

	#endif
}

public class DBUser 
{
	public int id;
	public string firstname;
	public string lastname;
	public string email;
	public string gender;
	public string facebookID;
	
	public override string ToString()
	{
		return 	"{id: " + id + "; " +
				"facebookID: " + facebookID + "; " +
				"First Name: " + firstname + "; " +
				"Last Name: " + lastname + "; " +
				"E-mail: " + email + "; " +
				"Gender: " + gender + "}";
	}
}