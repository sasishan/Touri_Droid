
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
		DataManager mDm;
		Converter mConverter;
		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			mConverter = new Converter ();

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
				string token = sm.getAuthorizedToken ();
				GetMyMessages(token);
			} 

			mDm = new DataManager ();
			mDm.SetContext (view.Context);

			List<string> users = mDm.GetUsersWhoSentMeMessages (myUsername);

			var messages = view.FindViewById<ListView> (Resource.Id.Messages);

			var adapter = new ArrayAdapter<string> (Activity, Android.Resource.Layout.SimpleListItem1, users);
			messages.Adapter = adapter;

			TextView noMsgs = view.FindViewById<TextView> (Resource.Id.nomsgs);
			if (messages.Count > 0) {
				noMsgs.Visibility = ViewStates.Gone;
			}

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


		public async void GetMyMessages(string accessToken)
		{
			string url = Constants.DEBUG_BASE_URL + Constants.URL_MyMessages;
			Comms comms = new Comms();

			var json = await comms.getWebApiData(url, accessToken);
			if (json==null)
			{
				//no need to do anything more
				return;
			}

			for (int i = 0; i < json.Count; i++) {
				ChatMessage cm = mConverter.parseOneChatMessage (json[i]);

				if (cm == null) {
					continue;
				}

				//this is not a response from the current user
				cm.MyResponse=Constants.MyResponseNo;
				//add it straight to the DB
				mDm.AddMessage(cm);
			}

		}
	}
}

