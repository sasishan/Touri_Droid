
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace TouriDroid
{
	public class ChatListFragment : Fragment
	{
		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			// Create your fragment here
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			// Use this to return your custom view for this Fragment
			// return inflater.Inflate(Resource.Layout.YourFragment, container, false);
			var view = inflater.Inflate(Resource.Layout.ChatList, container, false);

			SessionManager sm = new SessionManager (view.Context);

			string myUsername = "";
			if (sm.isLoggedIn ()) {
				myUsername = sm.getEmail ();
			} 

			DataManager dm = new DataManager ();
			dm.SetContext (view.Context);
			List<string> users = dm.GetUsersWhoSentMeMessages (myUsername);

			var messages = view.FindViewById<ListView> (Resource.Id.Messages);

			var adapter = new ArrayAdapter<string> (Activity, Android.Resource.Layout.SimpleListItem1, users);
			messages.Adapter = adapter;

			messages.ItemClick += (object sender, AdapterView.ItemClickEventArgs e) => 
			{
				var chatActivity = new Intent (view.Context, typeof(ActiveChat));

				chatActivity.PutExtra ("TargetUserName", users.ElementAt(e.Position));
				chatActivity.PutExtra ("TargetFirstName", "");
				chatActivity.PutExtra ("TargetLastName", "");
				this.StartActivity(chatActivity);
			};
			return view;
		}
	}
}

