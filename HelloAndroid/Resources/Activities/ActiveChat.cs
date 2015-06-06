
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Views.InputMethods;
using Android.Util;
using System.Threading.Tasks;


namespace TouriDroid
{
	[Activity (Label = "Chat", ConfigurationChanges=Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]
	public class ActiveChat : Activity
	{
		private string 					TAG = "ActiveChat";
		ChatClient 						mClient = null;	
		DataManager 					dm;
		public string 					mTargetUsername="";
		private string 					mTargetGuideId;
		private string 					mMyUsername = "";
		List<ChatItem> 					mMyMessages;
		ChatMessageAdapter			    mAdapter;
		public ChatServiceBinder 		binder;

		//Messages need to be cleared cos OnStart is recalled each time, while onCreate is only called on startup
		protected override void OnStart ()
		{
			base.OnStart ();

			HandleClicks ();
		}			

		protected override async void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.ActiveChat_Activity);

			ActionBar.SetDisplayHomeAsUpEnabled (true);
			ActionBar.SetHomeButtonEnabled (true);

			//get the information of who we want to chat WITH
			mTargetGuideId = 	Intent.GetStringExtra ("TargetGuideId") ?? "";
			mTargetUsername = 	Intent.GetStringExtra ("TargetUserName") ?? "";
			string fName = 		Intent.GetStringExtra ("TargetFirstName") ?? "";
			string lName = 		Intent.GetStringExtra ("TargetLastName") ?? "";

			SessionManager sm = new SessionManager (this);
			if (sm.isLoggedIn() == true) {
				mMyUsername = sm.getEmail ();
			}

			var messages = FindViewById<ListView> (Resource.Id.Messages);
			messages.TranscriptMode = TranscriptMode.AlwaysScroll;

			var inputManager = (InputMethodManager)GetSystemService (InputMethodService);

			dm = new DataManager ();
			dm.SetContext (this);

			List<ChatMessage> myChatMessages = dm.GetMessagesFromUser(mMyUsername, mTargetUsername);
			if (myChatMessages == null) {
				Log.Debug (TAG, "failed to get DB messages!");
				Toast.MakeText(this, "Failed to open DB", ToastLength.Short).Show();
				Finish ();
				return;
			}
			mMyMessages = new List<ChatItem>();

			foreach (ChatMessage m in myChatMessages) {
				ChatItem oneChatItem = new ChatItem ();
				oneChatItem.message = m.Message;
				oneChatItem.messageTimestamp = m.Msgtimestamp;
				oneChatItem.user = m.FromUser;
				oneChatItem.myMessage = false;

				if (m.MyResponse == Constants.MyResponseYes) {
					oneChatItem.myMessage = true;
					oneChatItem.user = "Me";
				}
				mMyMessages.Add (oneChatItem);
			}

			mAdapter = new ChatMessageAdapter(this, mMyMessages);
			messages.Adapter = mAdapter;
		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			switch (item.ItemId) {
			case Android.Resource.Id.Home:
				removeMessageReceivedEvent ();
				//@todo - disconnecting explictly ends our online status
				mClient.disconnect ();
				Finish ();
				return true;
			default:
				return base.OnOptionsItemSelected (item);
			}			
		}

		protected override void OnDestroy ()
		{
			removeMessageReceivedEvent ();
			base.OnDestroy ();
		}

		private void removeMessageReceivedEvent()
		{
			try
			{
				Log.Debug (TAG, "removeMessageReceivedEvent");
				mClient.OnMessageReceived -= (sender, message) => RunOnUiThread (() => {
				});
			}
			catch(Exception e)
			{
				//do nothing
			}
		}

		public async void HandleClicks()
		{
			if (mClient == null) {
				mClient = new ChatClient (mMyUsername, mTargetUsername);
			}

		//	mClient.disconnect ();
			await mClient.Connect ();

			Button button = FindViewById<Button>(Resource.Id.sendMessageButton);
			var input = FindViewById<EditText> (Resource.Id.inputChat);

			//get chat cient waits till the service starts
			//await GetChatClient ();
			button.Click += (o, e) => {
				if (string.IsNullOrEmpty(input.Text))
				{
					return;
				}

				Log.Debug (TAG, "button.Click - Sending private message");

				mClient.SendPrivateMessage(input.Text, mTargetUsername);

				ChatItem oneNewChatItem = new ChatItem();
				oneNewChatItem.message = input.Text;
				oneNewChatItem.user = "Me";
				oneNewChatItem.messageTimestamp=DateTime.Now.ToString();
				oneNewChatItem.myMessage= true;

				//add this to the listview to show it on screen
				Log.Debug ("ActiveChat", "button.Click- Add item to message list");
				mMyMessages.Add(oneNewChatItem);

				//add it to the Database as well
				ChatMessage cm = new ChatMessage();
				cm.Message = input.Text;
				cm.FromUser = mTargetUsername;
				cm.ToUser =	mMyUsername;
				cm.Msgtimestamp = oneNewChatItem.messageTimestamp;
				cm.MyResponse=Constants.MyResponseYes; // this is my response

				Log.Debug ("ActiveChat", "button.Click - Add item to DB");
				dm.AddMessage(cm);

				Log.Debug ("ActiveChat", "button.Click - Notifydatasetchanged");
				mAdapter.NotifyDataSetChanged();
				input.Text ="";
			};

			mClient.OnMessageReceived+=(sender, message) => RunOnUiThread( () =>
				{	
					Log.Debug ("ActiveChat", "In OnMessageReceived");
					if (message.fromUser.Equals(mTargetUsername))
					{
						ChatItem oneNewChatItem = new ChatItem();
						oneNewChatItem.message = message.message;
						oneNewChatItem.user = message.fromUser;
						oneNewChatItem.messageTimestamp=DateTime.Now.ToString();

						Log.Debug ("ActiveChat", "OnMessageReceived - add item to message list");
						mMyMessages.Add(oneNewChatItem);
						//@todo check if my message
						oneNewChatItem.myMessage= false;
						mAdapter.NotifyDataSetChanged();
					}
				}
			);
		}
	}
}

