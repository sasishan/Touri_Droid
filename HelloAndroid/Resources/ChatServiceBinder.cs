﻿using System;
using Android.OS;
using Android.App;
using Android.Content;
using Android.Widget;	

namespace TouriDroid
{
	[Service]public class ChatService : Service
	{
		/** indicates how to behave if the service is killed */
		int mStartMode;

		/** interface for clients that bind */
		IBinder mBinder;     

		/** indicates whether onRebind should be used */
		bool mAllowRebind;

		DataManager dm;
		ChatClient client ;

		/** Called when the service is being created. */
		public override void OnCreate() {
		}

		/** The service is starting, due to a call to startService() */
		public int OnStartCommand(Intent intent, int flags, int startId) {
			return mStartMode;
		}

		public override IBinder OnBind (Intent intent)
		{
			return mBinder;
		}

		/** Called when all clients have unbound with unbindService() */
		public override Boolean OnUnbind(Intent intent) {
			Toast.MakeText(this, "Chat server disconnect", ToastLength.Long).Show();
			return mAllowRebind;
		}

		/** Called when a client is binding to the service with bindService()*/
		public override void OnRebind(Intent intent) {

		}

		/** Called when The service is no longer used and is being destroyed */
		public override void OnDestroy() {
			Toast.MakeText(this, "Chat server disconnect", ToastLength.Long).Show();
			client.disconnect ();
		}

		public override StartCommandResult OnStartCommand (Intent intent, StartCommandFlags flags, int startId)
		{
			// Let it continue running until it is stopped.
			Toast.MakeText(this, "Service Started", ToastLength.Long).Show();
			StartChatConnection ();

			return StartCommandResult.Sticky;
		}	

		public async void StartChatConnection () {
			SessionManager sm = new SessionManager (this);
			string myUsername = sm.getEmail ();
			client= new ChatClient (myUsername, "0");
			await client.Connect ();
			Toast.MakeText(this, "Connected to Chat Server", ToastLength.Long).Show();

			dm = new DataManager ();
			dm.SetContext (this);

			//assume only guides get messages for now so there will be a ToUser name
			client.OnMessageReceived += (sender, message) => { 
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

			};						
		}


		public event EventHandler<string> messageChanged = delegate { };
	}
}

