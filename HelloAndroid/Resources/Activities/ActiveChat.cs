
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


namespace TouriDroid
{
	[Activity (Label = "Chat", ConfigurationChanges=Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]
	public class ActiveChat : Activity
	{
		ChatClient mClient = null;	
		protected override async void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.ActiveChat_Activity);

			ActionBar.SetDisplayHomeAsUpEnabled (true);
			ActionBar.SetHomeButtonEnabled (true);

			//get the information of who we want to chat WITH
			string targetGuideId = Intent.GetStringExtra ("TargetGuideId") ?? "Data not available";
			string targetUserName = Intent.GetStringExtra ("TargetUserName") ?? "Data not available";
			string fName = Intent.GetStringExtra ("TargetFirstName") ?? "Data not available";
			string lName = Intent.GetStringExtra ("TargetLastName") ?? "Data not available";

			SessionManager sm = new SessionManager (this);

			string myUsername = "";
			if (sm.isLoggedIn() == true) {
				myUsername = sm.getEmail ();
			}

			mClient = new ChatClient (myUsername, targetUserName);

			var input = FindViewById<EditText> (Resource.Id.inputChat);
			var messages = FindViewById<ListView> (Resource.Id.Messages);
			messages.TranscriptMode = TranscriptMode.AlwaysScroll;

			var inputManager = (InputMethodManager)GetSystemService (InputMethodService);

			DataManager dm = new DataManager ();
			dm.SetContext (this);

			List<ChatMessage> myChatMessages = dm.GetMessagesFromUser(myUsername, targetUserName);
			List<ChatItem> myMessages = new List<ChatItem>();

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
				myMessages.Add (oneChatItem);
				//myMessages.Add (targetUserName + " ["+m.Msgtimestamp+"]: "+ m.Message);
			}

			//var adapter = new ArrayAdapter<string> (this, Android.Resource.Layout.SimpleListItem1, myMessages);

			var adapter = new ChatMessageAdapter(this, myMessages);
			messages.Adapter = adapter;

			await mClient.Connect();


			// if i'm not logged in, get a username from the chat hub
			if (sm.isLoggedIn () == false) {
				await mClient.SendMyUsername();
			}
				
			Button button = FindViewById<Button>(Resource.Id.sendMessageButton);

			button.Click += (o, e) => {
				if (string.IsNullOrEmpty(input.Text))
				{
					return;
				}

				//client.Send(input.Text);
				//Send the message and reflect it back on the screen
				mClient.SendPrivateMessage(input.Text);
			
				ChatItem oneNewChatItem = new ChatItem();
				oneNewChatItem.message = input.Text;
				oneNewChatItem.user = "Me";
				oneNewChatItem.messageTimestamp=DateTime.Now.ToString();
				oneNewChatItem.myMessage= true;

				//add this to the listview to show it on screen
				myMessages.Add(oneNewChatItem);

				//add it to the Database as well
				ChatMessage cm = new ChatMessage();
				cm.Message = input.Text;
				cm.FromUser=targetUserName;
				cm.ToUser=myUsername;
				cm.Msgtimestamp = oneNewChatItem.messageTimestamp;
				cm.MyResponse=Constants.MyResponseYes; // this is my response
				dm.AddMessage(cm);

				adapter.NotifyDataSetChanged();
				input.Text ="";
			};

	
	//		input.EditorAction+= delegate {
	//				inputManager.HideSoftInputFromWindow(input.WindowToken, HideSoftInputFlags.None);
	//			if (string.IsNullOrEmpty(input.Text))
	//			{
	//				return;
		//		}
		//	
		//		client.Send(input.Text);
		//		input.Text ="";
		//	};*/	
			mClient.ReceiveMyUserName+=(sender, message) => RunOnUiThread( () =>
				{mClient._myUsername=message;}
			);
			//client._myUsername=message
			mClient.OnMessageReceived+=(sender, message) => RunOnUiThread( () =>
				{	ChatItem oneNewChatItem = new ChatItem();
					oneNewChatItem.message = message.message;
					oneNewChatItem.user = message.fromUser;
					oneNewChatItem.messageTimestamp=DateTime.Now.ToString();
					myMessages.Add(oneNewChatItem);
					//@todo check if my message
					oneNewChatItem.myMessage= false;
					adapter.NotifyDataSetChanged();
				}
				//adapter.Add(message.message)
			);
		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			switch (item.ItemId) {
			case Android.Resource.Id.Home:
				if (mClient != null) {
					mClient.disconnect ();
				}
				Finish ();
				return true;
			default:
				return base.OnOptionsItemSelected (item);
			}			
		}
	}
}

