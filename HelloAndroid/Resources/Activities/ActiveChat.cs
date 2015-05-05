
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
	[Activity (Label = "Chat")]			
	public class ActiveChat : Activity
	{
		
		protected override async void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.ActiveChat_Activity);

			ActionBar.SetDisplayHomeAsUpEnabled (true);
			ActionBar.SetHomeButtonEnabled (true);

			//get the information of who we want to chat WITH
			string targetGuideId = Intent.GetStringExtra ("TargetGuideId") ?? "Data not available";
			string fName = Intent.GetStringExtra ("TargetFirstName") ?? "Data not available";
			string lName = Intent.GetStringExtra ("TargetLastName") ?? "Data not available";

			SessionManager sm = new SessionManager (this);

			string myUsername = "";
			if (sm.isLoggedIn() == true) {
				myUsername = sm.getEmail ();
			}

			ChatClient client = new ChatClient (myUsername, targetGuideId);


			var input = FindViewById<EditText> (Resource.Id.Input);
			var messages = FindViewById<ListView> (Resource.Id.Messages);

			var inputManager = (InputMethodManager)GetSystemService (InputMethodService);
			var adapter = new ArrayAdapter<string> (this, Android.Resource.Layout.SimpleListItem1, new List<string> ());
			messages.Adapter = adapter;

			await client.Connect();

			if (sm.isLoggedIn () == false) {
				await client.SendMyUsername();
			}
				
			Button button = FindViewById<Button>(Resource.Id.sendMessageButton);

			button.Click += (o, e) => {
				if (string.IsNullOrEmpty(input.Text))
				{
					return;
				}

				//client.Send(input.Text);
				client.SendPrivateMessage(input.Text);
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
			client.ReceiveMyUserName+=(sender, message) => RunOnUiThread( () =>
				{client._myUsername=message;}
			);
			//client._myUsername=message
			client.OnMessageReceived+=(sender, message) => RunOnUiThread( () =>
				adapter.Add(message.message)
			);
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
	}
}

