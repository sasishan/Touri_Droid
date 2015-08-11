
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
using Android.Graphics;
using Java.IO;
using System.IO;


namespace TouriDroid
{
	[Activity (Label = "Chat", ConfigurationChanges=Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]
	public class ActiveChat : Activity
	{
		ChatClient 						mClient = null;	
		DataManager 					dm;
		public string 					mTargetUsername="";
		private string 					mTargetGuideId;
		private string 					mMyUsername = "";
		List<ChatItem> 					mMyMessages;
		ChatMessageAdapter			    mAdapter;
		public ChatServiceBinder 		binder;
		SessionManager 					mSm;
		int 							mFromUserId=Constants.Uninitialized;
		int 							mMyUserId=Constants.Uninitialized;
		string 							mUserProfileImage;

		//Messages need to be cleared cos OnStart is recalled each time, while onCreate is only called on startup
		protected override void OnStart ()
		{

			base.OnStart ();
		}			

		protected override async void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
	//		RequestWindowFeature (WindowFeatures.CustomTitle);
	//		Window.SetFeatureInt(WindowFeatures.CustomTitle, Resource.Layout.ChatTitle_Layout);

			SetContentView (Resource.Layout.ActiveChat_Activity);

			//addPreferencesFromResource(R.xml.settings);

			//get the information of who we want to chat WITH
			mTargetGuideId = 	Intent.GetStringExtra ("TargetGuideId") ?? "-1";
			mTargetUsername = 	Intent.GetStringExtra ("TargetUserName") ?? "";
			string fName = 		Intent.GetStringExtra ("TargetFirstName") ?? "";
			string lName = 		Intent.GetStringExtra ("TargetLastName") ?? "";
			string topic =      Intent.GetStringExtra ("Category") ?? "General";
			mUserProfileImage =  Intent.GetStringExtra ("ImageName") ?? "";

			mFromUserId = Convert.ToInt32 (mTargetGuideId);
			ActionBar.SetDisplayHomeAsUpEnabled (true);
			ActionBar.SetHomeButtonEnabled (true);

			SupportFunctions sf = new SupportFunctions();
			if (!mUserProfileImage.Equals ("")) {
				//FileInputStream fo = new FileInputStream(imagePath);
				Bitmap image = sf.GetImageFromStorage(mUserProfileImage);

				if (image != null) {
					ActionBar.SetDisplayOptions (ActionBar.DisplayOptions | ActionBarDisplayOptions.ShowCustom, 0);
					ImageView imageView = new ImageView(ActionBar.ThemedContext);
					imageView.SetScaleType(ImageView.ScaleType.Center);
					imageView.SetImageBitmap (image);

					ActionBar.LayoutParams layoutParams = new ActionBar.LayoutParams(
						ActionBar.LayoutParams.WrapContent,
						ActionBar.LayoutParams.WrapContent, 
						GravityFlags.Left | GravityFlags.CenterVertical);
					//	layoutParams.RightMargin = 40;

					LayoutInflater inflator = (LayoutInflater) this.GetSystemService(Context.LayoutInflaterService);
					View v = inflator.Inflate(Resource.Layout.ChatTitle_Layout, null);

					ImageView profileImage = v.FindViewById<ImageView> (Resource.Id.userImage);
					TextView chat_user = v.FindViewById<TextView> (Resource.Id.chat_user);
					chat_user.Text = fName + lName;
					profileImage.SetImageBitmap (image);
					imageView.LayoutParameters =layoutParams;
					ActionBar.CustomView = v;
					ActionBar.SetDisplayShowTitleEnabled(false);
					ActionBar.SetDisplayShowCustomEnabled(true);
				}
			}

			mSm = new SessionManager (this);
			if (mSm.isLoggedIn() == true) {
				mMyUsername = mSm.getEmail ();
				mMyUserId = mSm.getGuideId();
			}

	//		this.Title = fName + " " + lName + " | " + topic;

			HandleClicks ();


		}

		protected override void OnResume ()
		{
			LoadScreen ();
			base.OnResume ();
		}

		public void LoadScreen()
		{
			var messages = FindViewById<ListView> (Resource.Id.Messages);
			messages.TranscriptMode = TranscriptMode.AlwaysScroll;

			var inputManager = (InputMethodManager)GetSystemService (InputMethodService);

			dm = new DataManager ();
			dm.SetContext (this);

			List<ChatMessage> myChatMessages = dm.GetMessagesFromUser(mMyUsername, mTargetUsername);
			if (myChatMessages == null) {
				Log.Debug (Constants.TOURI_TAG, "failed to get DB messages!");
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
				oneChatItem.deliveredToServer = m.Delivered;

				if (m.MyResponse == Constants.MyResponseYes) {
					oneChatItem.myMessage = true;
					oneChatItem.user = "Me";
				}
				mMyMessages.Add (oneChatItem);//
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
				//mClient.disconnect ();
				Finish ();
				return true;
			default:
				return base.OnOptionsItemSelected (item);
			}			
		}

		protected override void OnStop ()
		{
			removeMessageReceivedEvent ();
			base.OnStop ();
		}


		private void removeMessageReceivedEvent()
		{
			try
			{
				Log.Debug (Constants.TOURI_TAG, "removeMessageReceivedEvent");
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
				mClient = new ChatClient (mMyUsername, mTargetUsername, mMyUserId);
			}

			//	mClient.disconnect ();
			await mClient.Connect ();

			if (mClient.isConnected == false) {
				Toast.MakeText(this, "Could not connect to chat server. Check your internet connection.", ToastLength.Short).Show();
				Finish ();
				return;
			}

			Button button = FindViewById<Button>(Resource.Id.sendMessageButton);
			var input = FindViewById<EditText> (Resource.Id.inputChat);

			//get chat cient waits till the service starts
			//await GetChatClient ();
			string newMessage;
			button.Click -= async (o, e) => {};
			button.Click += async (o, e) => {

				newMessage = input.Text;
				input.Text="";
				if (string.IsNullOrEmpty(newMessage))
				{
					return;
				}

				Log.Debug (Constants.TOURI_TAG, "button.Click - Sending private message");

				ChatItem oneNewChatItem = new ChatItem();
				oneNewChatItem.message = newMessage;
				oneNewChatItem.user = "Me";
				oneNewChatItem.messageTimestamp=DateTime.Now.ToString();
				oneNewChatItem.myMessage= true;
				oneNewChatItem.deliveredToServer="sending...";

				//add this to the listview to show it on screen
				Log.Debug ("ActiveChat", "button.Click- Add item to message list");
				mMyMessages.Add(oneNewChatItem);
				mAdapter.NotifyDataSetChanged();

				int messageId = await mClient.SendPrivateMessage(newMessage, mTargetUsername, mMyUserId);

				//@todo more efficient way?
				//add it to the Database as well
				ChatMessage cm = new ChatMessage();
				if (messageId>0)
				{
					oneNewChatItem.deliveredToServer="delivered";
					mSm.SetLastMessageId(messageId);

					cm.Message = newMessage;
					cm.FromUser = mTargetUsername;
					cm.FromUserId = mFromUserId;
					cm.ToUser =	mMyUsername;
					cm.Msgtimestamp = oneNewChatItem.messageTimestamp;
					cm.MyResponse=Constants.MyResponseYes; // this is my response
					cm.Delivered = oneNewChatItem.deliveredToServer;
					cm.MsgRead = Constants.MessageIsRead;

					Log.Debug ("ActiveChat", "button.Click - Add item to DB");

					dm.AddMessage(cm);
				}
				else
				{
					oneNewChatItem.deliveredToServer="not delivered";
				}
				mAdapter.NotifyDataSetChanged();

				Log.Debug ("ActiveChat", "button.Click - Notifydatasetchanged");
			};


			removeMessageReceivedEvent ();
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

