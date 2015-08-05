using System;
using Android.OS;
using Android.App;
using Android.Content;
using Android.Widget;
using System.Threading.Tasks;
using Android.Support.V4.App;
using Android.Util;	

namespace TouriDroid
{
	[Service]public class ChatService : Service
	{
		private static readonly int MessageReceivedId = 1000;
		private bool 				mShowNotifications=true;
		SessionManager 				mSm = null;
		string						mMyUsername;
		private bool				mKeepPinging=true;

		/** interface for clients that bind */
		IBinder mBinder;     

		/** indicates whether onRebind should be used */
		bool mAllowRebind=true;

		DataManager mDm;
		ChatClient mClient ;

		/** Called when the service is being created. */
		public override void OnCreate() {
			mDm = new DataManager ();
			mDm.SetContext (this);
			mSm = new SessionManager (this);
			mMyUsername = mSm.getEmail ();


		}

		public override IBinder OnBind (Intent intent)
		{
			mBinder = new ChatServiceBinder (this);
			mShowNotifications = false;
			Log.Debug (Constants.TOURI_TAG, "In OnBind");
			return mBinder;
		}

		/** Called when all clients have unbound with unbindService() */
		public override Boolean OnUnbind(Intent intent) {
			mShowNotifications = true;
			Log.Debug (Constants.TOURI_TAG, "In Unbind");
			return mAllowRebind;
		}

		/** Called when a client is binding to the service with bindService()*/
		public override void OnRebind(Intent intent) {

		}

		/** Called when The service is no longer used and is being destroyed */
		public override void OnDestroy() {
			Toast.MakeText(this, "Chat server disconnect", ToastLength.Long).Show();
			Log.Debug (Constants.TOURI_TAG, "OnDestroy");
			mKeepPinging = false;
			mClient.OnMessageReceived -= (sender, message) => { };
			mClient.PingMe -= (sender, timestamp) => {};
			mClient.disconnect ();
			mClient = null;
			base.OnDestroy ();
		}

		public override StartCommandResult OnStartCommand (Intent intent, StartCommandFlags flags, int startId)
		{
			// Let it continue running until it is stopped.
			//Toast.MakeText(this, "Service Started", ToastLength.Long).Show();
			Log.Debug (Constants.TOURI_TAG, "OnStartCommand - Chat Service Started");

			/*	var ongoing = new Notification (Resource.Drawable.ic_touri_logo_trans, "Touri");
			// Create the PendingIntent with the back stack
			// When the user clicks the notification, SecondActivity will start up.
			Intent resultIntent = new Intent(this, typeof(MainActivity));
			resultIntent.PutExtra("CallChat", true);  //tell mainactivity to start chat fragment
			Android.Support.V4.App.TaskStackBuilder stackBuilder = Android.Support.V4.App.TaskStackBuilder.Create(this);
			stackBuilder.AddNextIntent(resultIntent);
			PendingIntent resultPendingIntent = stackBuilder.GetPendingIntent(0, (int) PendingIntentFlags.UpdateCurrent);

			ongoing.SetLatestEventInfo (this, "Touri", "You are online", resultPendingIntent);
			StartForeground ((int)NotificationFlags.ForegroundService, ongoing);*/
			StartChatConnection (300);
			LoadMyMessages ();

			//PollMyMessages (120);


			//A Sticky Service will get restarted with a null intent if the OS ever shuts it down due to memory pressure:
			return StartCommandResult.Sticky;
		}	

		private async void LoadMyMessages ()
		{
			Log.Debug (Constants.TOURI_TAG, "In LoadMyMessages");
			string token = mSm.getAuthorizedToken ();
			SupportFunctions sf = new SupportFunctions ();

			// Create the PendingIntent with the back stack
			// When the user clicks the notification, SecondActivity will start up.
			Intent resultIntent = new Intent(this, typeof(MainActivity));
			resultIntent.PutExtra("CallChat", true);  //tell mainactivity to start chat fragment

			Android.Support.V4.App.TaskStackBuilder stackBuilder = Android.Support.V4.App.TaskStackBuilder.Create(this);
			stackBuilder.AddNextIntent(resultIntent);

			PendingIntent resultPendingIntent = stackBuilder.GetPendingIntent(0, (int) PendingIntentFlags.UpdateCurrent);

			Log.Debug (Constants.TOURI_TAG, "Calling GetMyMessages");

			int count = await sf.GetMyMessages (token, mDm, mSm);
			if (count > 0) {
				ShowNewMessagesNotification (resultPendingIntent);
			}

			Log.Debug (Constants.TOURI_TAG, "Exiting LoadMyMessages");
		}

		protected async Task initalizeClient(string userName)
		{
			Log.Debug (Constants.TOURI_TAG, "initalizeClient");
			mClient = new ChatClient (userName, "0");

			await mClient.Connect ();
			Toast.MakeText(this, "Connected to Chat Server", ToastLength.Long).Show();
		}

		public async void PollMyMessages(int intervalInSeconds)
		{
			Log.Debug (Constants.TOURI_TAG, "StartChatConnection");
			SupportFunctions sf = new SupportFunctions ();
			string token = mSm.getAuthorizedToken ();

			// Create the PendingIntent with the back stack
			// When the user clicks the notification, SecondActivity will start up.
			Intent resultIntent = new Intent(this, typeof(MainActivity));
			resultIntent.PutExtra("CallChat", true);  //tell mainactivity to start chat fragment

			Android.Support.V4.App.TaskStackBuilder stackBuilder = Android.Support.V4.App.TaskStackBuilder.Create(this);
			stackBuilder.AddNextIntent(resultIntent);

			PendingIntent resultPendingIntent = stackBuilder.GetPendingIntent(0, (int) PendingIntentFlags.UpdateCurrent);

			//assume only guides get messages for now so there will be a ToUser name
			int lastMessageId=0;
			while (true) {
				int count = await sf.GetMyMessages (token, mDm, mSm);
				if (count > 0) {
					ShowNewMessagesNotification (resultPendingIntent);
				}
				await Task.Delay (intervalInSeconds * 1000);
			}
		}

		public async void StartChatConnection (int intervalInSeconds) {

			Log.Debug (Constants.TOURI_TAG, "StartChatConnection");
			await initalizeClient (mMyUsername);

			// Create the PendingIntent with the back stack
			// When the user clicks the notification, SecondActivity will start up.
			Intent resultIntent = new Intent(this, typeof(MainActivity));
			resultIntent.PutExtra("CallChat", true);  //tell mainactivity to start chat fragment

			Android.Support.V4.App.TaskStackBuilder stackBuilder = Android.Support.V4.App.TaskStackBuilder.Create(this);
			stackBuilder.AddNextIntent(resultIntent);

			PendingIntent resultPendingIntent = stackBuilder.GetPendingIntent(0, (int) PendingIntentFlags.UpdateCurrent);

			mClient.PingMe -= (sender, timestamp) => {};
			mClient.PingMe += (sender, timestamp) => {
				Log.Debug(Constants.TOURI_TAG, "Pinged, we are connected at " + timestamp);

				if (mClient==null)
				{
					Log.Debug(Constants.TOURI_TAG, "mClient is null!!");
				}
				else
				{
					mClient.isConnected=true;
				}
			};

			//assume only guides get messages for now so there will be a ToUser name
			mClient.OnMessageReceived -= (sender, message) => { };
			mClient.OnMessageReceived += (sender, message) => { 
				Log.Debug (Constants.TOURI_TAG, "OnMessageReceived");
				Log.Debug (Constants.TOURI_TAG, "mShowNotifications "+ mShowNotifications);

				LogNewMessage(message);

				if (mShowNotifications)
				{
					ShowNotification(message, resultPendingIntent);
				}
			};			

			while (mKeepPinging) {
				if (mClient == null) {
					Log.Debug (Constants.TOURI_TAG, "Fatal: mClient is null!");
				} 
				else if (mClient.isConnected == false) {
					try
					{
						Log.Debug (Constants.TOURI_TAG, "isConnect is false" );
						await mClient._connection.Start ();
						LoadMyMessages();
					}
					catch (Exception e) {
						Log.Debug (Constants.TOURI_TAG, "Exception on Pinging caught but keep moving ");
					}
					//mClient.isConnected = true;
				}

				if (mClient == null) {
					Log.Debug (Constants.TOURI_TAG, "Fatal: mClient is null!");
				} else {
					Log.Debug (Constants.TOURI_TAG, "Going to ping server");
					int result = await mClient.PingServer ();

					if (result != Constants.SUCCESS) {
						Log.Debug (Constants.TOURI_TAG, "Could not ping server, so calling LoadMyMessages");
						LoadMyMessages ();
					}
				}
				await Task.Delay (intervalInSeconds * 1000);
			}

			return;
		}

		private int ShowNewMessagesNotification(PendingIntent pendingIntent)
		{
			int result = Constants.FAIL;
			if (pendingIntent != null ) {

				NotificationCompat.Builder builder = new NotificationCompat.Builder (this)
					.SetAutoCancel (true) // dismiss the notification from the notification area when the user clicks on it
					.SetContentIntent (pendingIntent) // start up this activity when the user clicks the intent.
					.SetContentTitle ("Touri") // Set the title
					.SetSmallIcon (Resource.Drawable.ic_stat_touri_t_logo_trans) // This is the icon to display
					.SetContentText ("New message received"); // the message to display.

				// Finally publish the notification
				NotificationManager notificationManager = (NotificationManager)GetSystemService (NotificationService);
				notificationManager.Notify (MessageReceivedId, builder.Build ());

				result = Constants.SUCCESS;
			}

			return result;
		}

		private int ShowNotification(Message message, PendingIntent pendingIntent)
		{
			int result = Constants.FAIL;
			if (pendingIntent != null && message != null) {

				NotificationCompat.Builder builder = new NotificationCompat.Builder (this)
					.SetAutoCancel (true) // dismiss the notification from the notification area when the user clicks on it
					.SetContentIntent (pendingIntent) // start up this activity when the user clicks the intent.
					.SetContentTitle (message.fromUser) // Set the title
					.SetSmallIcon (Resource.Drawable.ic_stat_touri_t_logo_trans) // This is the icon to display
					.SetContentText (message.message); // the message to display.

				// Finally publish the notification
				NotificationManager notificationManager = (NotificationManager)GetSystemService (NotificationService);
				notificationManager.Notify (MessageReceivedId, builder.Build ());

				result = Constants.SUCCESS;
			}

			return result;
		}

		//Log the message in the database
		private int LogNewMessage(Message message)
		{
			int result = Constants.SUCCESS;

			ChatMessage cm = new ChatMessage ();
			cm.FromUser = message.fromUser;
			cm.ToUser = mMyUsername;
			cm.Message = message.message;
			cm.MyResponse=Constants.MyResponseNo;
			cm.Msgtimestamp = DateTime.Now.ToString ();
			cm.MsgRead = Constants.MessageUnread; //message not read as the service is reading it
			Log.Debug (Constants.TOURI_TAG, "Logging a message from " + cm.FromUser);

			// dont insert messages from myself back (eg. could not deliver a message is returned)
			//if (!cm.FromUser.Equals(mMyUsername))
			//{
				long id = mDm.AddMessage (cm);
			//}

			return result;
		}

		public async Task<ChatClient> GetChatClient()
		{
			Log.Debug (Constants.TOURI_TAG, "GetChatClient");

			if (mClient == null) {
				SessionManager sm = new SessionManager (this);
				string myUsername = sm.getEmail ();
				Log.Debug (Constants.TOURI_TAG, "GetChatClient - mClient is null");
				await initalizeClient (myUsername);
			}

			return mClient;
		}


		public event EventHandler<string> messageChanged = delegate { };
	}

	public class ChatServiceBinder : Binder
	{
		protected ChatService service;

		public ChatServiceBinder (ChatService service) 
		{ 
			this.service = service; 
		}

		public ChatService GetService
		{
			get { return this.service; }
		} 

		public bool IsBound { get; set; }  

	}

	class MainChatServiceConnection : Java.Lang.Object, IServiceConnection
	{
		MainActivity activity;
		ChatServiceBinder binder;

		public MainChatServiceConnection (MainActivity activity)
		{
			this.activity = activity;
		}

		public void OnServiceConnected (ComponentName name, IBinder service)
		{
			var chatServiceBinder = service as ChatServiceBinder;
			if (chatServiceBinder != null) {
				binder = (ChatServiceBinder)chatServiceBinder;
				binder.IsBound = true;

				Log.Debug ("ChatServiceConnection", "OnServiceConnected");
			}
		}

		public void OnServiceDisconnected (ComponentName name)
		{
			binder.IsBound = false;
		}
	}

	class ChatServiceConnection : Java.Lang.Object, IServiceConnection
	{
		ActiveChat activity;
		ChatServiceBinder binder;

		public ChatServiceBinder Binder {
			get {
				return binder;
			}
		}

		public ChatServiceConnection (ActiveChat activity)
		{
			this.activity = activity;
		}

		public void OnServiceConnected (ComponentName name, IBinder service)
		{
			var chatServiceBinder = service as ChatServiceBinder;
			if (chatServiceBinder != null) {
				activity.binder = chatServiceBinder;
				activity.binder.IsBound = true;

				Log.Debug ("ChatServiceConnection", "OnServiceConnected");
				// keep instance for preservation across configuration changes
				this.binder = (ChatServiceBinder) service;
			}
		}

		public void OnServiceDisconnected (ComponentName name)
		{
			activity.binder.IsBound = false;
		}
	}
}

