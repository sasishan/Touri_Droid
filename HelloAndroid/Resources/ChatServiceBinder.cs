using System;
using Android.OS;
using Android.App;
using Android.Content;
using Android.Widget;
using System.Threading.Tasks;
using Android.Support.V4.App;	

namespace TouriDroid
{
	[Service]public class ChatService : Service
	{
		private static readonly int MessageReceivedId = 1000;
		private bool mShowNotifications=false;

		int mStartMode=0;

		/** interface for clients that bind */
		IBinder mBinder;     

		/** indicates whether onRebind should be used */
		bool mAllowRebind=true;

		DataManager dm;
		ChatClient mClient ;

		/** Called when the service is being created. */
		public override void OnCreate() {
		}

		/** The service is starting, due to a call to startService() */
		public int OnStartCommand(Intent intent, int flags, int startId) {
			return mStartMode;
		}

		public override IBinder OnBind (Intent intent)
		{
			mBinder = new ChatServiceBinder (this);
			mShowNotifications = false;
			return mBinder;
		}

		/** Called when all clients have unbound with unbindService() */
		public override Boolean OnUnbind(Intent intent) {
			mShowNotifications = true;
			return mAllowRebind;
		}

		/** Called when a client is binding to the service with bindService()*/
		public override void OnRebind(Intent intent) {

		}

		/** Called when The service is no longer used and is being destroyed */
		public override async void OnDestroy() {
			Toast.MakeText(this, "Chat server disconnect", ToastLength.Long).Show();
			await mClient.disconnect ();
		}

		public override StartCommandResult OnStartCommand (Intent intent, StartCommandFlags flags, int startId)
		{
			// Let it continue running until it is stopped.
			Toast.MakeText(this, "Service Started", ToastLength.Long).Show();
			StartChatConnection ();

			//A Sticky Service will get restarted with a null intent if the OS ever shuts it down due to memory pressure:
			return StartCommandResult.Sticky;
		}	

		protected async Task initalizeClient(string userName)
		{
			mClient = new ChatClient (userName, "0");

			await mClient.Connect ();
			Toast.MakeText(this, "Connected to Chat Server", ToastLength.Long).Show();
		}

		public async void StartChatConnection () {

			SessionManager sm = new SessionManager (this);
			string myUsername = sm.getEmail ();
			int _count = 0;

			await initalizeClient (myUsername);

			dm = new DataManager ();
			dm.SetContext (this);

			// Create the PendingIntent with the back stack
			// When the user clicks the notification, SecondActivity will start up.
			Intent resultIntent = new Intent(this, typeof(MainActivity));

			Android.Support.V4.App.TaskStackBuilder stackBuilder = Android.Support.V4.App.TaskStackBuilder.Create(this);
		//	stackBuilder.AddParentStack(Class.FromType(typeof(MainActivity)));
			stackBuilder.AddNextIntent(resultIntent);

			PendingIntent resultPendingIntent = stackBuilder.GetPendingIntent(0, (int) PendingIntentFlags.UpdateCurrent);

			//assume only guides get messages for now so there will be a ToUser name
			mClient.OnMessageReceived += (sender, message) => { 
				ChatMessage cm = new ChatMessage ();
				cm.FromUser = message.fromUser;
				cm.ToUser = myUsername;
				cm.Message = message.message;
				cm.MyResponse=Constants.MyResponseNo;
				cm.Msgtimestamp = DateTime.Now.ToString ();

				// dont record messages from myself back (eg. could not deliver a message is returned)
				if (!cm.FromUser.Equals(myUsername))
				{
					long id = dm.AddMessage (cm);
				}

				// Build the notification
				if (mShowNotifications)
				{
					NotificationCompat.Builder builder = new NotificationCompat.Builder(this)
						.SetAutoCancel(true) // dismiss the notification from the notification area when the user clicks on it
						.SetContentIntent(resultPendingIntent) // start up this activity when the user clicks the intent.
						.SetContentTitle("Touri Message Received") // Set the title
						.SetSmallIcon(Resource.Drawable.touri_logo) // This is the icon to display
						.SetContentText(String.Format("Message received from {0}", message.fromUser)); // the message to display.

					// Finally publish the notification
					NotificationManager notificationManager = (NotificationManager)GetSystemService(NotificationService);
					notificationManager.Notify(MessageReceivedId, builder.Build());
				}
			};						
		}

		public async Task<ChatClient> GetChatClient()
		{
			if (mClient == null) {
				SessionManager sm = new SessionManager (this);
				string myUsername = sm.getEmail ();
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

