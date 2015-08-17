using System;
using Android.Content;
using Android.Database.Sqlite;
using Android.Provider;
using SQLite;
using System.Collections.Generic;
using Android.Graphics;

namespace TouriDroid
{
	public class ChatUser
	{
		public int UserId { get; set ; }
		public Bitmap profileImage { get; set; }
		public String UserName { get; set; }
		public String CommonName { get; set; }
		public int NumberOfUnreadMessages { get; set; }
	}

	public class ChatMessage
	{
		[PrimaryKey, AutoIncrement]
		public long ID { get; set; }
		public String FromUser { get; set; }
		public String FromName { get; set; }
		public String ToUser { get; set; }
		public String Message { get; set; }
		public String Msgtimestamp { get; set; }
		public int MyResponse { get; set; }  //set this to 1 (TRUE) if its a response from the current user
		public String Delivered { get; set; } 
		public String MsgRead { get; set; } 
		public int FromUserId { get; set; }
		public int ToUserId { get; set; }
		public string ToName { get; set; }
		//public char downloaded { get; set; }
		//public string lastDownloaded { get; set; }

		public override string ToString()
		{
			return string.Format("[ChatMessageEntry: ID={0}, FromUser={1}, ToUser={2}, Message={3}, Msgtimestamp={4}, delivered={5}, MsgRead={6}, FromUserId={7}, ToUserId={8}, FromName={9}, ToName={10}]", 
				ID, FromUser, ToUser, Message, Msgtimestamp, Delivered, MsgRead, FromUserId, ToUserId, FromName, ToName);
		}
	}

	public class ChatMessageEntry 
	{
		public const String _ID = "ID";
		public const String TABLE_NAME = "chatmessage";
		//public const String COLUMN_NAME_ENTRY_ID = "entryid";
		public const String COLUMN_FROMUSER_TITLE = "FromUser";
		public const String COLUMN_TOUSER_SUBTITLE = "ToUser";
		public const String COLUMN_MYRESPONSE_SUBTITLE = "MyResponse";
		public const String COLUMN_MESSAGE = "Message";
		public const String COLUMN_TIMESTAMP = "Msgtimestamp";
		public const String COLUMN_DELIVERED = "Delivered";
		public const String COLUMN_MSGREAD = "MsgRead";
		public const String COLUMN_FROMUSERID = "FromUserId";
		public const String COLUMN_TOUSERID = "ToUserId";
		public const String COLUMN_FROMNAME = "FromName";
		public const String COLUMN_TONAME = "ToName";
	}

	public static class ChatMessageContract {
		public const String TEXT_TYPE = " TEXT";
		public const String INT_TYPE = " INTEGER";
		private const String COMMA_SEP = ",";

		public const String SQL_CREATE_ENTRIES =
			"CREATE TABLE " + ChatMessageEntry.TABLE_NAME + " (" +
			ChatMessageEntry._ID + " INTEGER PRIMARY KEY," +
			ChatMessageEntry.COLUMN_FROMUSER_TITLE + TEXT_TYPE + COMMA_SEP +
			ChatMessageEntry.COLUMN_TOUSER_SUBTITLE + TEXT_TYPE + COMMA_SEP +
			ChatMessageEntry.COLUMN_MYRESPONSE_SUBTITLE + INT_TYPE + COMMA_SEP +
			ChatMessageEntry.COLUMN_MESSAGE + TEXT_TYPE + COMMA_SEP +
			ChatMessageEntry.COLUMN_TIMESTAMP +	TEXT_TYPE + COMMA_SEP +
			ChatMessageEntry.COLUMN_DELIVERED +	TEXT_TYPE + COMMA_SEP +
			ChatMessageEntry.COLUMN_MSGREAD + TEXT_TYPE + COMMA_SEP +
			ChatMessageEntry.COLUMN_FROMUSERID + TEXT_TYPE + COMMA_SEP +
			ChatMessageEntry.COLUMN_TOUSERID + TEXT_TYPE + COMMA_SEP + 
			ChatMessageEntry.COLUMN_FROMNAME + TEXT_TYPE + COMMA_SEP +
			ChatMessageEntry.COLUMN_TONAME + TEXT_TYPE +
			" )";

		public const String SQL_DELETE_ENTRIES = "DROP TABLE IF EXISTS " + ChatMessageEntry.TABLE_NAME;

	}

	//The above method needs to be invoked before any database calls are made. 
	//Therefore, you might want to consider adding the call to this database function from your application's OnCreate() event.
	public class DataManager
	{
		private ChatMessageReaderDbHelper _helper;

		public void SetContext(Context context)
		{
			if (context != null)
			{
				_helper = new ChatMessageReaderDbHelper(context);
			}
		}

		public long AddMessage(ChatMessage addMsg)
		{
			using (var db = new SQLiteConnection(_helper.WritableDatabase.Path))
			{
				try
				{
					return db.Insert(addMsg);
				}
				catch (Exception ex)
				{
					//exception handling code to go here
					return Constants.Uninitialized;
				}
			}
		}

		public long DeleteMessage(ChatMessage deleteMsg)
		{
			using (var db = new SQLiteConnection(_helper.WritableDatabase.Path))
			{
				try
				{					
					return db.Delete(deleteMsg);
				}
				catch (Exception ex)
				{
					//exception handling code to go here
					return Constants.Uninitialized;
				}
			}
		}

		public long DeleteAllMessages()
		{
			using (var db = new SQLiteConnection(_helper.WritableDatabase.Path))
			{
				try
				{					
					return db.DeleteAll<ChatMessage>();
				}
				catch (Exception ex)
				{
					//exception handling code to go here
					return Constants.Uninitialized;
				}
			}
		}

		public long UpdateMessage(ChatMessage updMsg)
		{
			using (var db = new SQLiteConnection(_helper.WritableDatabase.Path))
			{
				try
				{
					return db.Update(updMsg);
				}
				catch (Exception ex)
				{
					//exception handling code to go here
					return Constants.Uninitialized;
				}
			}
		}

		//retrieve a specific user by querying against their first name
		public List<ChatMessage> GetMessagesFromUser(string myUsername, string fromUsername)
		{
			const string getMessagesQuery = "SELECT * FROM "+ ChatMessageEntry.TABLE_NAME + " WHERE (ToUser = ? AND FromUser= ?) OR (ToUser = ? AND FromUser = ?)";
			using (var database = new SQLiteConnection(_helper.ReadableDatabase.Path))
			{
				try
				{					
					//var messages =  database.Table<ChatMessage>();
					//var query2 = database.Table<ChatMessage>().Where (q => q.ToUser==myUsername);
					var query = database.Query<ChatMessage>(getMessagesQuery, myUsername, fromUsername, fromUsername, myUsername);

					List<ChatMessage> messages = new List<ChatMessage>();
					foreach (ChatMessage m in query)
					{
						if (m.MsgRead.Equals(Constants.MessageUnread))
						{
							m.MsgRead=Constants.MessageIsRead;
							UpdateMessage(m);
						}
						messages.Add(m);
					}
					return messages;
				}
				catch (Exception ex)
				{
					//exception handling code to go here
					return null;
				}
			}
		}

		//retrieve a specific user by querying against their first name
		public List<ChatUser> GetUsersWhoSentMeMessages(string myUsername)
		{
			const string getUsersQuery = "SELECT DISTINCT FromUser, FromUserId, FromName FROM "+ ChatMessageEntry.TABLE_NAME + " WHERE ToUser = ?";
			const string getUserMsgsUnreadQuery = "SELECT COUNT(*) FROM "+ ChatMessageEntry.TABLE_NAME + " WHERE FromUser = ? AND ToUser = ? AND MsgRead = 'N' AND MyResponse=0";
			using (var database = new SQLiteConnection(_helper.ReadableDatabase.Path))
			{
				try
				{					
					//var messages =  database.Table<ChatMessage>();
					//var query2 = database.Table<ChatMessage>().Where (q => q.ToUser==myUsername);
					var query = database.Query<ChatMessage>(getUsersQuery, myUsername);

					List<ChatUser> fromUsers = new List<ChatUser>();
					foreach (ChatMessage m in query)
					{
						ChatUser cu = new ChatUser();
						cu.UserName= m.FromUser;
						cu.CommonName = m.FromName;
						cu.UserId = m.FromUserId;
						int msgCount = database.ExecuteScalar<int>(getUserMsgsUnreadQuery, m.FromUser, myUsername);
						cu.NumberOfUnreadMessages =msgCount;
						fromUsers.Add(cu);
					}
					return fromUsers;
				}
				catch (Exception ex)
				{
					//exception handling code to go here
					return null;
				}
			}
		}

		//retrieve a specific user by querying against their first name
		public List<ChatMessage> GetAllMyMessages(string myUsername)
		{
			using (var database = new SQLiteConnection(_helper.ReadableDatabase.Path))
			{
				try
				{					
					//var messages =  database.Table<ChatMessage>();
					var query = database.Table<ChatMessage>().Where (v => v.ToUser==myUsername);
					List<ChatMessage> messages = new List<ChatMessage>();
					foreach (ChatMessage m in query)
					{
						messages.Add(m);
					}
					return messages;
				}
				catch (Exception ex)
				{
					//exception handling code to go here
					return null;
				}
			}
		}

	}


	public class ChatMessageReaderDbHelper: SQLiteOpenHelper 
	{
		// If you change the database schema, you must increment the database version.
		public const int DATABASE_VERSION = 1;
		public const String DATABASE_NAME = "ChatMessage.db";

		public ChatMessageReaderDbHelper(Context context):base(context, DATABASE_NAME, null, DATABASE_VERSION) 
		{			
		}

		public override void OnCreate(SQLiteDatabase db) 
		{
			db.ExecSQL(ChatMessageContract.SQL_CREATE_ENTRIES);
		}

		public override void OnUpgrade(SQLiteDatabase db, int oldVersion, int newVersion) 
		{
			// This database is only a cache for online data, so its upgrade policy is
			// to simply to discard the data and start over
			db.ExecSQL(ChatMessageContract.SQL_DELETE_ENTRIES);
			OnCreate(db);
		}

		public override void OnDowngrade(SQLiteDatabase db, int oldVersion, int newVersion) 
		{
			OnUpgrade(db, oldVersion, newVersion);
		}
	}

}

