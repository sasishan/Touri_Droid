
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
		ChatClient 						mClient = null;	
		public ChatServiceBinder 		binder;
		private ChatServiceConnection 	mChatServiceConnection;
		DataManager 					dm;
		public string 					mTargetUsername;
		private string 					mTargetGuideId;
		private string 					mMyUsername = "";
		List<ChatItem> 					mMyMessages;
		ChatMessageAdapter			    mAdapter;


		protected override void OnStart ()
		{
			base.OnStart ();

			//bind to the chat service
			//this way we don't have to invoke multiple connections
			var chatServiceIntent = new Intent (this, typeof(ChatService));
			mChatServiceConnection = new ChatServiceConnection (this);

			bool connected = BindService (chatServiceIntent, mChatServiceConnection, Bind.AutoCreate);

			if (connected == true) {
				Log.Debug ("ActiveChat", "Connected to service");
			} else {
				Log.Debug ("ActiveChat", "Error connecting to service");
				Finish ();
			}

			HandleClicks ();
		}

		public async void HandleClicks()
		{
			Button button = FindViewById<Button>(Resource.Id.sendMessageButton);
			var input = FindViewById<EditText> (Resource.Id.inputChat);

			//get chat cient waits till the service starts
			await GetChatClient ();

			button.Click += (o, e) => {
				if (string.IsNullOrEmpty(input.Text))
				{
					return;
				}

				mClient.SendPrivateMessage(input.Text, mTargetUsername);

				ChatItem oneNewChatItem = new ChatItem();
				oneNewChatItem.message = input.Text;
				oneNewChatItem.user = "Me";
				oneNewChatItem.messageTimestamp=DateTime.Now.ToString();
				oneNewChatItem.myMessage= true;

				//add this to the listview to show it on screen
				mMyMessages.Add(oneNewChatItem);

				//add it to the Database as well
				ChatMessage cm = new ChatMessage();
				cm.Message = input.Text;
				cm.FromUser = mTargetUsername;
				cm.ToUser=mMyUsername;
				cm.Msgtimestamp = oneNewChatItem.messageTimestamp;
				cm.MyResponse=Constants.MyResponseYes; // this is my response
				dm.AddMessage(cm);

				mAdapter.NotifyDataSetChanged();
				input.Text ="";
			};

			mClient.OnMessageReceived+=(sender, message) => RunOnUiThread( () =>
				{	
					ChatItem oneNewChatItem = new ChatItem();
					oneNewChatItem.message = message.message;
					oneNewChatItem.user = message.fromUser;
					oneNewChatItem.messageTimestamp=DateTime.Now.ToString();
					mMyMessages.Add(oneNewChatItem);
					//@todo check if my message
					oneNewChatItem.myMessage= false;
					mAdapter.NotifyDataSetChanged();
				}
			);
		}

		protected override async void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.ActiveChat_Activity);

			ActionBar.SetDisplayHomeAsUpEnabled (true);
			ActionBar.SetHomeButtonEnabled (true);

			// restore from connection there was a configuration change, such as a device rotation
			#pragma warning disable 618
			mChatServiceConnection = LastNonConfigurationInstance as ChatServiceConnection;
			#pragma warning restore 618

			if (mChatServiceConnection != null) {
				binder = mChatServiceConnection.Binder;
			}

			//get the information of who we want to chat WITH
			mTargetGuideId = Intent.GetStringExtra ("TargetGuideId") ?? "Data not available";
			mTargetUsername = Intent.GetStringExtra ("TargetUserName") ?? "Data not available";
			string fName = Intent.GetStringExtra ("TargetFirstName") ?? "Data not available";
			string lName = Intent.GetStringExtra ("TargetLastName") ?? "Data not available";

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

		private async Task GetChatClient()
		{
			int count = 0;
			while (binder == null) {
				if (count++ > 10) {
					Finish ();
					return;
				}
				await Task.Delay (1000);
			}
			mClient = await binder.GetService.GetChatClient();
		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			switch (item.ItemId) {
			case Android.Resource.Id.Home:
				Finish ();
				return true;
			default:
				return base.OnOptionsItemSelected (item);
			}			
		}

		protected override void OnDestroy ()
		{
			base.OnDestroy ();

			if (!isConfigurationChange) {
				if (binder.IsBound) {
					UnbindService (mChatServiceConnection);
					binder.IsBound = false;
				}
			}	
		}

		bool isConfigurationChange = false;
		// return the service connection if there is a configuration change
		public override Java.Lang.Object OnRetainNonConfigurationInstance ()
		{
			#pragma warning disable 618
			base.OnRetainNonConfigurationInstance ();
			#pragma warning restore 618

			isConfigurationChange = true;

			return mChatServiceConnection;
		}

	}
}

